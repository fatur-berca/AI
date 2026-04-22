using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using DFIS.Universal.Repositories;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using TOM.Master.Repositories;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public class MasterVendorBLL : IMasterVendorBLL
    {
        private readonly IMasterVendorRepo _masterVendorRepo;
        private readonly IGenericRepository<MasterVendor> _generalRepo;

        public MasterVendorBLL(IMasterVendorRepo masterVendorRepo, IGenericRepository<MasterVendor> generalRepo)
        {
            _masterVendorRepo = masterVendorRepo;
            _generalRepo = generalRepo;
        }

        public List<MasterVendorTOMDTO> GetMasterVendors(MasterVendorTOMInput input)
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();

            if (input.IsActive)
            {
                queryFilter = queryFilter.And(m => m.IsActive == input.IsActive);
            }
           
            if (input.IDVendor >= 0)
            {
                queryFilter = queryFilter.And(m => m.IDVendor == input.IDVendor).And(m=>m.IsActive == true);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterVendor>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterVendorTOMDTO>>(dbResult);
        }

        public MasterVendorTOMDTO SaveData(MasterVendorTOMDTO input, string controller, string userid)
        {
            var validateInput = new MasterVendorTOMInput()
            {
                IDVendor = input.IDVendor,
                VendorName = input.VendorName,
                IsActive = input.IsActive
            };

            var prevData = GetMasterVendors(validateInput);
            var check = GetById(input.IDVendor);
            var dbMstVendor = Mapper.Map<MasterVendor>(input);

            if (prevData == null || (prevData != null && prevData.Count == 0 && check == null))
            {
                dbMstVendor.CreatedDate = DateTime.Now;
                dbMstVendor.UpdatedDate = DateTime.Now;
                _generalRepo.Insert(dbMstVendor);
                _generalRepo.Save(controller, userid);
            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterVendorTOMDTO>(dbMstVendor);
        }

        public MasterVendorTOMDTO EditData(MasterVendorTOMDTO input, string controller, string userid)
        {

          
            var dbMstVendor = Mapper.Map<MasterVendor>(input);      

            var validateInput = new MasterVendorTOMInput()
            {
                IDVendor = input.IDVendor,
                VendorName = input.VendorName,
                IsActive = input.IsActive
            };

            var prevData = GetMasterVendors(validateInput);
            var vendorid = _generalRepo.Get().Where(x => x.IDVendor == input.IDVendor).Select(x => x.IDVendor).FirstOrDefault();
            //var namevendor = _generalRepo.Get().Where(x => x.VendorName == input.VendorName && x.IDVendor == input.IDVendor).Select(x => x.VendorName).FirstOrDefault();

     
            if (prevData == null || (prevData != null && prevData.Count == 0))
            {

                TOMContextDB context = new TOMContextDB();
             

                MasterVendor c = (from x in context.MasterVendors
                                    where x.IDVendor == input.IDVendor
                                    select x).First();
                c.VendorName = input.VendorName;              
                c.UpdatedDate = DateTime.Now;
                c.IsActive = input.IsActive;
                c.UpdatedBy = input.UpdatedBy;           
                context.SaveChanges();

            }
            else if (vendorid == input.IDVendor)
            {
                TOMContextDB context = new TOMContextDB();


                MasterVendor c = (from x in context.MasterVendors
                                  where x.IDVendor == input.IDVendor
                                  select x).First();
                c.VendorName = input.VendorName;
                c.UpdatedDate = DateTime.Now;
                c.IsActive = input.IsActive;
                c.UpdatedBy = input.UpdatedBy;
                context.SaveChanges();
            }

            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterVendorTOMDTO>(dbMstVendor);
        }

        public MasterVendorTOMDTO GetById(int id)
        {
            var result = _generalRepo.GetByID(id);
            return Mapper.Map<MasterVendorTOMDTO>(result);
        }

        public MasterVendorTOMDTO GetVendorByName(string name, bool mustBeAParent = true)
        {
            var res = _generalRepo.Get(v => v.IsActive == true && v.ParentVendor == null && v.VendorName == name).FirstOrDefault();
            return Mapper.Map<MasterVendorTOMDTO>(res);
        }
    }
}
