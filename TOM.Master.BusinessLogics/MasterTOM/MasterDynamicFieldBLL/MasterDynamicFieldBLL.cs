using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.Master.Repositories;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public class MasterDynamicFieldBLL : IMasterDynamicFieldBLL
    {
        private readonly IMasterDynamicFieldRepo _masterDynamicFieldRepo;
        private readonly IGenericRepository<MasterDynamicField> _generalRepo;

        public MasterDynamicFieldBLL(IMasterDynamicFieldRepo masterDynamicFieldRepo,IGenericRepository<MasterDynamicField> generalRepo)
        {
            _masterDynamicFieldRepo = masterDynamicFieldRepo;
            _generalRepo = generalRepo;
        }

        public List<MasterDynamicFieldDTO> GetMasterDynamicFields(MasterDynamicFieldInput input)
        {
            var queryFilter = PredicateHelper.True<MasterDynamicField>();

            if (!string.IsNullOrEmpty(input.PageName))
                queryFilter = queryFilter.And(m => m.PageName == input.PageName);
            if (!string.IsNullOrEmpty(input.FieldName))
                queryFilter = queryFilter.And(m => m.FieldName == input.FieldName);
            if (!string.IsNullOrEmpty(input.FieldType))
                queryFilter = queryFilter.And(m => m.FieldType == input.FieldType);
            if (!string.IsNullOrEmpty(input.GroupCollapse))
                queryFilter = queryFilter.And(m => m.GroupCollapse == input.GroupCollapse);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterDynamicField>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterDynamicFieldDTO>>(dbResult);
        }

        public MasterDynamicFieldDTO SaveData(MasterDynamicFieldDTO input)
        {
            var validateInput = new MasterDynamicFieldInput()
            {
                PageName = input.PageName,
                FieldName = input.FieldName,
                FieldType = input.FieldType,
                GroupCollapse = input.GroupCollapse,
                Ordering = input.Ordering,
                NotificationPeriod = input.NotificationPeriod,
                IsActive = input.IsActive
            };
            var prevData = GetMasterDynamicFields(validateInput);
            var dbMstDynamicField = Mapper.Map<MasterDynamicField>(input);
            if (prevData == null || (prevData != null && prevData.Count == 0))
            {
                if (dbMstDynamicField.GroupCollapse == null)
                    dbMstDynamicField.GroupCollapse = "";
                dbMstDynamicField.FieldName = input.FieldName.ToUpper();
                dbMstDynamicField.CreatedDate = DateTime.Now;
                dbMstDynamicField.UpdatedDate = DateTime.Now;
                _generalRepo.Insert(dbMstDynamicField);
                _generalRepo.Save();
            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterDynamicFieldDTO>(dbMstDynamicField);
        }

        public MasterDynamicFieldDTO EditData(MasterDynamicFieldDTO input)
        {

            //var dbMstDynamicField = Mapper.Map<MasterDynamicField>(input);
            //dbMstDynamicField.CreatedDate = DateTime.Now;
            //dbMstDynamicField.UpdatedDate = DateTime.Now;
            //_generalRepo.Update(dbMstDynamicField);
            //_generalRepo.Save();

            //return Mapper.Map<MasterDynamicFieldDTO>(dbMstDynamicField);

            var dbMasterList = Mapper.Map<MasterDynamicField>(input);

            var validateInput = new MasterDynamicFieldInput()
            {
                IDDynamicField = input.IDDynamicField,
                FieldName = input.FieldName,
                FieldType = input.FieldType,
                PageName = input.PageName
            };

            var prevData = GetMasterDynamicFields(validateInput);
            var filedname = _generalRepo.Get().Where(x => x.FieldName == input.FieldName && x.FieldType == input.FieldType && x.PageName == input.PageName).Select(x => x.FieldName).FirstOrDefault();
            var fieldtype = _generalRepo.Get().Where(x => x.FieldName == input.FieldName && x.FieldType == input.FieldType && x.PageName == input.PageName).Select(x => x.FieldType).FirstOrDefault();
             var iddynamic = _generalRepo.Get().Where(x => x.FieldName == input.FieldName && x.FieldType == input.FieldType && x.PageName == input.PageName).Select(x => x.IDDynamicField).FirstOrDefault();
             var pagename = _generalRepo.Get().Where(x => x.FieldName == input.FieldName && x.FieldType == input.FieldType && x.PageName == input.PageName).Select(x => x.PageName).FirstOrDefault();
               //var filevalue = _generalRepo.Get().Where(x => x.FieldValue == input.FieldValue && x.FieldName == input.FieldName).Select(x => x.FieldValue).FirstOrDefault();
            //var namevendor = _generalRepo.Get().Where(x => x.VendorName == input.VendorName && x.IDVendor == input.IDVendor).Select(x => x.VendorName).FirstOrDefault();
             if (input.GroupCollapse == null)
                 input.GroupCollapse = "";

             if (prevData == null || (prevData != null && prevData.Count == 0))
             {
                 TOMContextDB context = new TOMContextDB();
                 MasterDynamicField c = (from x in context.MasterDynamicFields
                                 where x.IDDynamicField == input.IDDynamicField
                                 select x).First();
                 c.FieldName = input.FieldName.ToUpper();
                 c.FieldType = input.FieldType;
                 c.PageName = input.PageName;
                 c.Ordering = input.Ordering;
                 c.GroupCollapse = input.GroupCollapse;
                 c.NotificationPeriod = input.NotificationPeriod;
                 c.UpdatedDate = DateTime.Now;
                 c.IsActive = input.IsActive;
                 c.UpdatedBy = input.UpdatedBy;
                 context.SaveChanges();
             }
             else if (filedname == input.FieldName && fieldtype == input.FieldType && iddynamic == input.IDDynamicField && pagename == input.PageName)
             {
                TOMContextDB context = new TOMContextDB();
                 MasterDynamicField c = (from x in context.MasterDynamicFields
                                         where x.IDDynamicField == input.IDDynamicField
                                         select x).First();
                 c.FieldName = input.FieldName.ToUpper();
                 c.FieldType = input.FieldType;
                 c.PageName = input.PageName;
                 c.Ordering = input.Ordering;
                 c.GroupCollapse = input.GroupCollapse;
                 c.NotificationPeriod = input.NotificationPeriod;
                 c.UpdatedDate = DateTime.Now;
                 c.IsActive = input.IsActive;
                 c.UpdatedBy = input.UpdatedBy;
                 context.SaveChanges();
             }
             else
             {
                 throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
             }
            return Mapper.Map<MasterDynamicFieldDTO>(dbMasterList);
        }



        public MasterDynamicFieldDTO GetById(int id)
        {
            var result = _generalRepo.GetByID(id);

            // return result and Mapping result from entity to DTO
            // because the return type is DTO
            return Mapper.Map<MasterDynamicFieldDTO>(result);
        }
    }
}
