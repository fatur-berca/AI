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
using DFIS.Universal.Domain.Inputs;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Master.BusinessLogics
{
    public class MasterListBLL : IMasterListBLL
    {
        private readonly IMasterListRepo _masterListRepo;
        private readonly IGenericRepository<MasterList> _generalRepo;

        public MasterListBLL(IMasterListRepo masterListRepo, IGenericRepository<MasterList> generalRepo)
        {
            _masterListRepo = masterListRepo;
            _generalRepo = generalRepo;
        }

        public List<MasterListDTO> GetMasterLists(MasterListInput input)
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            if (!string.IsNullOrEmpty(input.FieldName))
                queryFilter = queryFilter.And(m => m.FieldName.Equals(input.FieldName, StringComparison.InvariantCultureIgnoreCase)).And(m => m.IsActive == true);

            if (!string.IsNullOrEmpty(input.FieldValue))
            {
                queryFilter = queryFilter.And(m => m.FieldValue.Equals(input.FieldValue, StringComparison.InvariantCultureIgnoreCase)).And(m => m.FieldName == input.FieldName);
            }
            //queryFilter = queryFilter.And(m => m.IsActive == input.IsActive);
            //  queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            //var orderByFilter = sortCriteria.GetOrderByFunc<MasterList>();

            var dbResult = _generalRepo.Get(queryFilter).OrderByDescending(x => x.UpdatedDate).ToList();

            //var FinalResult = dbResult.OrderByDescending(z => z.UpdatedDate);

            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }

        public List<MasterListDTO> GetEditMasterLists(MasterListInput input)
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            if (!string.IsNullOrEmpty(input.FieldName))
                queryFilter = queryFilter.And(m => m.FieldName.Equals(input.FieldName, StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrEmpty(input.FieldValue))
            {
                queryFilter = queryFilter.And(m => m.FieldValue.Equals(input.FieldValue, StringComparison.InvariantCultureIgnoreCase));
            }
            queryFilter = queryFilter.And(m => m.IsActive == input.IsActive);
            //queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            //var orderByFilter = sortCriteria.GetOrderByFunc<MasterList>();

            var dbResult = _generalRepo.Get(queryFilter).ToList();

            var FinalResult = dbResult.OrderByDescending(z => z.UpdatedDate);

            return Mapper.Map<List<MasterListDTO>>(FinalResult);
        }


        public List<MasterListDTO> GetMasterListValues(MasterListInput input)
        {
            var queryFilter = PredicateHelper.True<MasterList>();
            queryFilter = queryFilter.And(m => m.FieldName.Equals(input.FieldName, StringComparison.InvariantCultureIgnoreCase));

            //queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterList>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }

        public List<MasterListDTO> GetMasterListDrops(MasterListInput input)
        {

            //ist<MasterList> myList = new List<MasterList>();
            var queryFilter = PredicateHelper.True<MasterList>();
            //var result = (from c in myList
            //          select c.FieldName).Distinct();


            if (!string.IsNullOrEmpty(input.FieldName))
                queryFilter = queryFilter.And(m => m.FieldName.Equals(input.FieldName, StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrEmpty(input.FieldValue))
            {
                queryFilter = queryFilter.And(m => m.FieldValue.Equals(input.FieldValue, StringComparison.InvariantCultureIgnoreCase)).And(m => m.FieldName.Equals(input.FieldName, StringComparison.InvariantCultureIgnoreCase));
            }

            //queryFilter = queryFilter.And(p => p.IsActive == true);


            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterList>();


            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            var finalresult = dbResult.GroupBy(x => x.FieldName).Select(y => y.First()).OrderBy(y => y.FieldName);
            return Mapper.Map<List<MasterListDTO>>(finalresult);
        }

        public List<MasterListDTO> GetListForFABrands(MasterListInput input)
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName.Equals(input.FieldName, StringComparison.InvariantCultureIgnoreCase));

            //queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterList>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }

        public List<MasterListDTO> GetListForLocations(MasterListInput input)
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName.Equals("LocationType", StringComparison.InvariantCultureIgnoreCase));

            queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterList>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }

        public List<MasterListDTO> GetListForBrandCategorys(MasterListInput input)
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName.Equals("BrandCategory", StringComparison.InvariantCultureIgnoreCase));

            //queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterList>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }

        public List<MasterListDTO> GetListForEmployees(MasterListInput input)
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName.Equals("EmployeeStatus", StringComparison.InvariantCultureIgnoreCase));

            //queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterList>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }
        public List<MasterListDTO> GetInputSvListView(MasterListInput input)
        {
            var dbResult = _generalRepo.Get().Where(x => x.FieldName.ToLower() == "InputSVOvertime".ToLower()).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }


        //public void SaveData(MasterListDTO input)
        //{
        //    var dbMstList = Mapper.Map<MasterList>(input);

        //    _generalRepo.Insert(dbMstList);

        //    dbMstList.CreatedDate = DateTime.Now;
        //    dbMstList.UpdatedDate = DateTime.Now;

        //    _generalRepo.Save();
        //}

        //public MasterListDTO SaveData(MasterListDTO input)
        //{
        //    var validateInput = new MasterListInput()
        //    {
        //        FieldName = input.FieldName,
        //        FieldValue = input.FieldValue,
        //        IsActive = input.IsActive
        //    };
        //    var prevData = GetMasterLists(validateInput);
        //    var dbMstList = Mapper.Map<MasterList>(input);

        //    if (prevData == null || (prevData!= null && prevData.Count == 0))

        //    if (!prevData.Any())

        //    {
        //        dbMstList.CreatedDate = DateTime.Now;
        //        dbMstList.UpdatedDate = DateTime.Now;
        //        _generalRepo.Insert(dbMstList);
        //        _generalRepo.Save();
        //    }
        //    else
        //    {
        //        throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
        //    }
        //    return Mapper.Map<MasterListDTO>(dbMstList);
        //}

        public MasterListDTO SaveData(MasterListDTO input, string controller, string userid)
        {
            var validateInput = new MasterListInput()
            {
                FieldName = input.FieldName,
                FieldValue = input.FieldValue,
                IsActive = input.IsActive
            };
            var prevData = GetMasterLists(validateInput);
            var dbMstList = Mapper.Map<MasterList>(input);

            if (prevData == null || (prevData != null && prevData.Count == 0))

            {
                dbMstList.CreatedDate = DateTime.Now;
                dbMstList.UpdatedDate = DateTime.Now;
                _generalRepo.Insert(dbMstList);
                _generalRepo.Save(controller, userid);
            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterListDTO>(dbMstList);
        }

        //public MasterListDTO EditData(MasterListDTO input)
        //{
        //    var dbMstList = Mapper.Map<MasterList>(input);
        //    dbMstList.CreatedDate = DateTime.Now;
        //    dbMstList.UpdatedDate = DateTime.Now;
        //    _generalRepo.Update(dbMstList);
        //    _generalRepo.Save();

        //    return Mapper.Map<MasterListDTO>(dbMstList);
        //}

        public MasterListDTO EditData(MasterListDTO input, string controller, string userid)
        {
            /*var dbMstList = Mapper.Map<MasterList>(input);
            dbMstList.CreatedDate = DateTime.Now;
            dbMstList.UpdatedDate = DateTime.Now;
            _generalRepo.Update(dbMstList);
            _generalRepo.Save(controller, userid);

            return Mapper.Map<MasterListDTO>(dbMstList);*/

            //var validateInput = new MasterListInput()
            //{
            //    FieldName = input.FieldName,
            //    FieldValue = input.FieldValue,
            //    IsActive = input.IsActive
            //};
            //var prevData = GetEditMasterLists(validateInput);
            //var dbMstList = Mapper.Map<MasterList>(input);

            //if (prevData == null || (prevData != null && prevData.Count == 0))
            //{
            //    //var dbMstList = Mapper.Map<MasterList>(input);
            //    dbMstList.CreatedDate = DateTime.Now;
            //    dbMstList.UpdatedDate = DateTime.Now;
            //    _generalRepo.Update(dbMstList);
            //    _generalRepo.Save(controller, userid);
            //}
            //else
            //{
            //    throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            //}
            //return Mapper.Map<MasterListDTO>(dbMstList);

            var dbMasterList = Mapper.Map<MasterList>(input);

            var validateInput = new MasterListInput()
            {
                IDList = input.IDList,
                FieldName = input.FieldName,
                FieldValue = input.FieldValue
            };

            var prevData = GetMasterLists(validateInput);
            var filedname = _generalRepo.Get().Where(x => x.FieldName.Equals(input.FieldName, StringComparison.InvariantCultureIgnoreCase) && x.FieldValue.Equals(input.FieldValue, StringComparison.InvariantCultureIgnoreCase) && x.IDList == input.IDList).Select(x => x.FieldName).FirstOrDefault();
            var filevalue = _generalRepo.Get().Where(x => x.FieldValue.Equals(input.FieldValue, StringComparison.InvariantCultureIgnoreCase) && x.FieldName.Equals(input.FieldName, StringComparison.InvariantCultureIgnoreCase) && x.IDList == input.IDList).Select(x => x.FieldValue).FirstOrDefault();
            var idlist = _generalRepo.Get().Where(x => x.FieldValue.Equals(input.FieldValue, StringComparison.InvariantCultureIgnoreCase) && x.FieldName.Equals(input.FieldName, StringComparison.InvariantCultureIgnoreCase) && x.IDList == input.IDList).Select(x => x.IDList).FirstOrDefault();
            //var filevalue = _generalRepo.Get().Where(x => x.FieldValue == input.FieldValue && x.FieldName == input.FieldName).Select(x => x.FieldValue).FirstOrDefault();
            //var namevendor = _generalRepo.Get().Where(x => x.VendorName == input.VendorName && x.IDVendor == input.IDVendor).Select(x => x.VendorName).FirstOrDefault();


            if (prevData == null || (prevData != null && prevData.Count == 0))
            {

                TOMContextDB context = new TOMContextDB();


                MasterList c = (from x in context.MasterLists
                                where x.IDList == input.IDList
                                select x).First();
                c.FieldName = input.FieldName;
                c.FieldValue = input.FieldValue;
                c.UpdatedDate = DateTime.Now;
                c.IsActive = input.IsActive;
                c.UpdatedBy = input.UpdatedBy;
                context.SaveChanges();

            }
            else if (filedname.Equals(input.FieldName, StringComparison.InvariantCultureIgnoreCase) && filevalue.Equals(input.FieldValue, StringComparison.InvariantCultureIgnoreCase) && idlist == input.IDList)
            {
                TOMContextDB context = new TOMContextDB();


                MasterList c = (from x in context.MasterLists
                                where x.IDList == input.IDList
                                select x).First();
                c.FieldName = input.FieldName;
                c.FieldValue = input.FieldValue;
                c.UpdatedDate = DateTime.Now;
                c.IsActive = input.IsActive;
                c.UpdatedBy = input.UpdatedBy;
                context.SaveChanges();
            }

            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterListDTO>(dbMasterList);
        }


        public MasterListDTO GetById(int id)
        {
            var result = _generalRepo.GetByID(id);

            // return result and Mapping result from entity to DTO
            // because the return type is DTO
            return Mapper.Map<MasterListDTO>(result);
        }

        public List<MasterListDTO> GetMasterListByFieldName(MasterListInput input)
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName.Equals(input.FieldName, StringComparison.InvariantCultureIgnoreCase)).And(m => m.IsActive == true);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterList>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            var FinalResult = dbResult.OrderByDescending(x => x.FieldValue);

            return Mapper.Map<List<MasterListDTO>>(FinalResult);
        }

        public bool IsMasterListValid(string fieldName, string fieldValue)
        {
            return _masterListRepo.IsMasterListValid(fieldName, fieldValue);
        }

        public List<string> GetMasterListByFieldName(string fieldName)
        {
            return _masterListRepo.Get(v => v.FieldName == fieldName && v.IsActive).Select(v => v.FieldValue).ToList();
        }
    }
}
