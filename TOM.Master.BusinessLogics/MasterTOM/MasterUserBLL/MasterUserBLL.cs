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
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using TOM.EntitiesDAL;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace TOM.Master.BusinessLogics.UtilitiesBLL
{
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
            var res = Mapper.Map<List<MasterUserDTO>>(dbResult);
            _generalRepo.AllowLazyLoading = true;
            return res;
        }

        public List<MasterUserDTO> GetMasterUserViews(MasterUserInput input)
        {
            var queryFilter = PredicateHelper.True<MasterUser>();

            // queryFilter = queryFilter.And(k => k.IsActive == true);

            if (input.IDUser != null)
            {
                queryFilter = queryFilter.And(m => m.IDUser == input.IDUser);

            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterUser>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();

            return Mapper.Map<List<MasterUserDTO>>(dbResult.OrderByDescending(m => m.UpdatedDate));
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
    }
}
