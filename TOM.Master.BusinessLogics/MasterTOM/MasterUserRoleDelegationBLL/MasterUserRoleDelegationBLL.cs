using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.Master.Repositories;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using DFIS.Universal;

namespace TOM.Master.BusinessLogics
{
    public class MasterUserRoleDelegationBLL : IMasterUserRoleDelegationBLL
    {
        private readonly IGenericRepository<MasterUserRoleDelegationView> _generalRepo;
        private readonly IGenericRepository<UserLocationDelegation> _userLocationDelegation;
        private readonly IMasterUserRoleDelegationRepo _masterUserRoleDelegationRepo;
        private readonly IGenericRepository<MasterUser> _masterUser;
        private readonly IGenericRepository<MasterRole> _masterRole;
        public MasterUserRoleDelegationBLL(IGenericRepository<MasterRole> masterRole, IGenericRepository<MasterUser> masterUser, IGenericRepository<MasterUserRoleDelegationView> generalRepo, IGenericRepository<UserLocationDelegation> userLocationDelegation, IMasterUserRoleDelegationRepo masterUserRoleDelegationRepo)
        {
            _generalRepo = generalRepo;
            _userLocationDelegation = userLocationDelegation;
            _masterUserRoleDelegationRepo = masterUserRoleDelegationRepo;
            _masterUser = masterUser;
            _masterRole = masterRole;
        }
        public List<MasterUserRoleDelegationViewDTO> GetData(MasterUserRoleDelegationViewInput input)
        {
            var queryFilter = PredicateHelper.True<MasterUserRoleDelegationView>();
            if (!String.IsNullOrEmpty(input.IDUserFrom))
            {
                queryFilter = queryFilter.And(m => m.IDUserFrom.Equals(input.IDUserFrom, StringComparison.InvariantCultureIgnoreCase));
            }

            if (!String.IsNullOrEmpty(input.IDUserTo))
            {
                queryFilter = queryFilter.And(m => m.IDUserTo.Equals(input.IDUserTo, StringComparison.InvariantCultureIgnoreCase));
            }

            //if ((input.DelegationIDRole != 0) || (input.DelegationIDRole != null))
            //{
            //    queryFilter = queryFilter.And(m => m.DelegationIDRole == input.DelegationIDRole);
            //}

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterUserRoleDelegationView>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();


            return Mapper.Map<List<MasterUserRoleDelegationViewDTO>>(dbResult);
        }

        public List<UserLocationDelegationDTO> GetDataByIdParent(int id)
        {
            var queryFilter = PredicateHelper.True<UserLocationDelegation>();
            queryFilter = queryFilter.And(m => m.IDUserDelegation == id);
            var dbResult = _userLocationDelegation.Get(queryFilter).ToList();
            return Mapper.Map<List<UserLocationDelegationDTO>>(dbResult);
        }

        public void InsertOrUpdate(MasterUserRoleDelegationDTO value, bool canUpdate = true)
        {
            // filters out the same record
            var existingRecords = _generalRepo.Get(v =>
            (value.IDUserDelegation == v.IDUserDelegation)
            ||
            (value.IDUserFrom == v.IDUserFrom
            && value.IDUserTo == v.IDUserTo
            && value.DelegationIDRole == v.DelegationIDRole)
               );

            // create date range helper
            DateRangeHelper hlp = new DateRangeHelper();
            foreach (var rec in existingRecords)
                hlp.Add(new DateRange(rec.EffectiveStartDate, rec.EffectiveEndDate, rec));

            value.UpdatedDate = DateTime.Now;

            //var val = MappingHelper.Map<MasterUserRoleDelegationDTO>(value);
            /*
            val.UserFromName = _masterUser.GetByID(val.IDUserFrom).FullName;
            val.UserToName = _masterUser.GetByID(val.IDUserTo).FullName;
            val.RoleName = _masterRole.GetByID(val.DelegationIDRole).RoleName;
            */
            var val = value;
            if (existingRecords.Count() == 0)
            {
                // new data mode (from, to or role is new)
                _masterUserRoleDelegationRepo.Save(val);
            }
            else
            {
                // update mode (from, to and role are same)
                var record = existingRecords.Where(mtl => mtl.IDUserDelegation == val.IDUserDelegation).FirstOrDefault();
                if (record == null)
                {
                    // get all intersections
                    var itx = hlp.IntersectWith(new DateRange(val.EffectiveStartDate, val.EffectiveEndDate));
                    if (itx.Count == 0)
                    {
                        // no intersection, happily insert the data :D
                        _masterUserRoleDelegationRepo.Save(val);
                    }
                    else if (itx.Count == 1)
                    {
                        // the new data can only intersects the inf ended data
                        var itr = itx.ElementAt(0);
                        if (itr.SourceRange.DateEnd.ToString("yyyy-MM-dd") != "2999-12-31")
                            throw new EffectiveDateConflictException();
                        // but only when the new data starts after the start date of the intersected data
                        if (itr.SourceRange.DateStart >= val.EffectiveStartDate)
                            throw new EffectiveDateConflictException();
                        // update the old data
                        if (itr.SourceRange.Tag is MasterUserRoleDelegationView)
                        {
                            var oldValue = itr.SourceRange.Tag as MasterUserRoleDelegationView;
                            oldValue.EffectiveEndDate = val.EffectiveStartDate - new TimeSpan(1, 0, 0, 0);
                            var oval = MappingHelper.Map<MasterUserRoleDelegationDTO>(oldValue);
                            oval.UpdatedBy = val.UpdatedBy;
                            oval.UpdatedDate = DateTime.Now;
                            oval.CurrentUser = val.UpdatedBy;
                            _masterUserRoleDelegationRepo.Save(oval);
                        }
                        // insert the new data
                        _masterUserRoleDelegationRepo.Save(val);
                    }
                    else
                        throw new EffectiveDateConflictException();

                }
                else
                {
                    // cannot intersect with anything but itself
                    var recs = hlp.IntersectWith(new DateRange(value.EffectiveStartDate, value.EffectiveEndDate));
                    if ((recs.Count == 1 && recs.ElementAt(0).SourceRange.Tag is MasterUserRoleDelegationView &&
                        (recs.ElementAt(0).SourceRange.Tag as MasterUserRoleDelegationView).DelegationIDRole != value.DelegationIDRole)
                        ||
                        recs.Count > 1)
                        throw new EffectiveDateConflictException();
                    // pure update
                    value.DelegationIDRole = record.DelegationIDRole;
                    MappingHelper.Map(value, record);
                    _masterUserRoleDelegationRepo.Save(val);
                }
            }
        }

        public int Save(MasterUserRoleDelegationDTO Input)
        {
            try
            {
                return _masterUserRoleDelegationRepo.Save(Input);
            }
            catch (ExceptionBase ex)
            {
                var message = ex;
                return 0;
            }
            
        }
    }
}
