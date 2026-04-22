using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using TOM.EntitiesDAL;
using DFIS.Universal.BusinessLogics;
using DFIS.Universal;
using System.Data.Entity;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterUserBLL : IMasterUserBLL<MasterUser> { }
    public class MasterUserBLL : IMasterUserBLL
    {
        private readonly IGenericRepository<MasterUser> _generalRepo;
        private IGenericRepository<MasterUser> _genericMstUserRepo;

        public MasterUserBLL(IGenericRepository<MasterUser> generalRepo, IGenericRepository<MasterUser> genericMstUserRepo)
        {
            _generalRepo = generalRepo;
            _genericMstUserRepo = genericMstUserRepo;
        }

        public MasterUser GetLogin(string iduser)
        {
            return _genericMstUserRepo.Get(x => x.IDUser.ToLower() == iduser.ToUpper()).FirstOrDefault();
        }

        public List<MasterUserDTO> GetMasterUsers(MasterUserInput input)
        {
            var queryFilter = PredicateHelper.True<MasterUser>();

            queryFilter = queryFilter.And(k => k.IsActive);
            if (!string.IsNullOrWhiteSpace(input.IDUser))
            {
                var id = input.IDUser;
                queryFilter = queryFilter.And(k => k.IDUser == id);
            }

            _generalRepo.AllowLazyLoading = false;
            List<MasterUser> dbResult = null;
            if (input.SortExpression != null)
            {
                var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
                var orderByFilter = sortCriteria.GetOrderByFunc<MasterUser>();
                dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            }
            else
            {
                dbResult = _generalRepo.Get(queryFilter).ToList();
            }
            List<MasterUserDTO> lat = new List<MasterUserDTO>();
            /*
            foreach (var mu in dbResult)
                lat.Add(MappingHelper.Map<MasterUserDTO>(mu));
            */
            var res = Mapper.Map<List<MasterUserReductedDTO>>(dbResult);
            _generalRepo.AllowLazyLoading = true;
            return MappingHelper.Map<MasterUserDTO>(res).ToList();
        }

        public List<MasterUserDTO> GetMasterUserViews(MasterUserInput input)
        {
            var queryFilter = PredicateHelper.True<MasterUser>();
            using (var context = new TOMContextDB())
            {
                var tplv = (from x in context.MasterUsers
                            select new MasterUserDTO
                            {
                                IDUser = x.IDUser,
                                FullName = x.FullName,
                                Email = x.Email,
                                Address = x.Address,
                                Phone = x.Phone,
                                IsActive = x.IsActive,
                                CreatedDate = x.CreatedDate,
                                UpdatedDate = x.UpdatedDate
                            })
                            .OrderByDescending(x => x.UpdatedDate)
                            .OrderByDescending(x => x.CreatedDate)
                            .ToList();


                if (input.IDUser != null)
                {
                    tplv = tplv.Where(m => m.IDUser == input.IDUser).ToList();
                }


                return tplv;
            }
            // queryFilter = queryFilter.And(k => k.IsActive == true);

            //if (input.IDUser != null)
            //{
            //    queryFilter = queryFilter.And(m => m.IDUser == input.IDUser);

            //}

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            //var orderByFilter = sortCriteria.GetOrderByFunc<MasterUser>();
            //var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();

            //return Mapper.Map<List<MasterUserDTO>>(dbResult.OrderByDescending(m => m.UpdatedDate));
        }

        public MasterUserDTO SaveData(MasterUserDTO input, string controller, string userid)
        {
            var validateInput = new MasterUserInput()
            {
                IDUser = input.IDUser,
                Email = input.Email,
                Address = input.Address,
                FullName = input.FullName

            };

            var prevData = GetMasterUserViews(validateInput);
            // var check = GetById(input.IDVendor);
            var dbMstUser = Mapper.Map<MasterUser>(input);

            if (prevData == null || prevData.Count == 0)
            {
                dbMstUser.CreatedDate = DateTime.Now;
                dbMstUser.UpdatedDate = DateTime.Now;
                _generalRepo.Insert(dbMstUser);
                _generalRepo.Save(controller, userid);
            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterUserDTO>(dbMstUser);
        }

        public MasterUserDTO EditData(MasterUserDTO input, string controller, string userid)
        {


            var dbMstVendor = Mapper.Map<MasterUser>(input);

            var validateInput = new MasterUserInput()
            {
                IDUser = input.IDUser,
                Email = input.Email,
                Address = input.Address,
                FullName = input.FullName
            };

            var prevData = GetMasterUserViews(validateInput);
            // var vendorid = _generalRepo.Get().Where(x => x.IDVendor == input.IDVendor).Select(x => x.IDVendor).FirstOrDefault();
            //var namevendor = _generalRepo.Get().Where(x => x.VendorName == input.VendorName && x.IDVendor == input.IDVendor).Select(x => x.VendorName).FirstOrDefault();


            if (prevData != null || prevData.Count != 0)
            {

                TOMContextDB context = new TOMContextDB();


                MasterUser c = (from x in context.MasterUsers
                                where x.IDUser == input.IDUser
                                select x).First();
                c.FullName = input.FullName;
                c.Email = input.Email;
                c.Address = input.Address;
                c.Phone = input.Phone;
                c.UpdatedDate = DateTime.Now;
                c.IsActive = input.IsActive;
                c.UpdatedBy = input.UpdatedBy;
                context.SaveChanges();

            }

            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterUserDTO>(dbMstVendor);
        }

        public string GetFullName(string iduser)
        {
            var usr = _generalRepo.Get(u => u.IDUser == iduser).FirstOrDefault();
            if (usr != null)
                return usr.FullName;
            return null;
        }
    }
}
