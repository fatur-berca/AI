using System;
using System.Collections.Generic;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.Master.Repositories;
using TOM.EntitiesDAL;
using DFIS.Contracts;
using System.Linq;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal;
using DFIS.Utils;

namespace TOM.Master.BusinessLogics
{
    public class MasterVendorTOMBLL : IMasterVendorTOMBLL
    {
        private readonly IMasterVendorTOMRepo _masterVendorTOMRepo;
        private readonly IMasterListRepo _masterListRepo;
        private readonly IGenericRepository<MasterVendor> _generalRepo;

        public MasterVendorTOMBLL(IMasterVendorTOMRepo masterVendorTOMRepo, IMasterListRepo masterListRepo, IGenericRepository<MasterVendor> generalRepo)
        {
            _masterVendorTOMRepo = masterVendorTOMRepo;
            _masterListRepo = masterListRepo;
            _generalRepo = generalRepo;
        }

        public List<MasterListDTO> GetList()
        {
            List<MasterListDTO> tempList = new List<MasterListDTO>();
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("VendorCategory")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("TransportationMode")));
            return tempList;
        }

        public List<MasterVendorTOMDTO> GetALLMasterVendors()
        {
            return Mapper.Map<List<MasterVendor>, List<MasterVendorTOMDTO>>(_masterVendorTOMRepo.GetAllMasterVendor());
        }
        public List<MasterVendorTOMDTO> GetALLMasterVendorsNoChild()
        {
            return Mapper.Map<List<MasterVendor>, List<MasterVendorTOMDTO>>(_masterVendorTOMRepo.GetAllMasterVendorActiveNoChild());
        }

        public List<MasterVendorTOMDTO> GetALLMasterVendorsEntity()
        {
            return Mapper.Map<List<MasterVendorTOMDTO>>(_masterVendorTOMRepo.GetAllMasterVendor());
        }

        public void SaveData(MasterVendorTOMDTO input, bool status)
        {
            _masterVendorTOMRepo.SaveData(Mapper.Map<MasterVendor>(input), status);
        }

        public void InsertOrUpdate(MasterVendorTOMDTO value, bool canUpdate = true)
        {
            var res = _generalRepo.Get(v =>
            (value.IDVendor != 0 && value.IDVendor == v.IDVendor)
                ||
                (v.VendorName == value.VendorName)
                );
            var exist = res.Count() > 0 ? res.First() : null;
            if (exist == null)
            {
                // new data
                _generalRepo.Insert(MappingHelper.Map<MasterVendor>(value));
            }
            else
            {
                // can only update data IF
                if (
                    (
                        // a parent/child that have unchanged parent value
                        (exist.ParentVendor == value.ParentVendor)
                        // or
                        ||
                        // a child that is not trying to be a parent
                        (exist.ParentVendor != null && value.ParentVendor != null)
                    )
                    // But of course, existing value must be non active
                    && (exist.IsActive == false
                    // or it is updating itself
                    || exist.IDVendor == value.IDVendor)
                    )
                {
                    value.IDVendor = exist.IDVendor;
                    MappingHelper.Map(value, exist);
                    _generalRepo.Update(exist);
                }
                else
                    throw new Exception("Data already exists!");
            }
            _generalRepo.Save();
        }

        public List<MasterVendorTOMDTO> Get(MasterVendorTOMInput input)
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = queryFilter.And(x => x.IsActive);
            if (input.IDVendor != 0)
            {
                queryFilter = queryFilter.And(x => x.IDVendor == input.IDVendor);
            }
            if (!string.IsNullOrEmpty(input.VendorName))
            {
                queryFilter = queryFilter.And(x => x.VendorName == input.VendorName);
            }
            var res = _generalRepo.Get(queryFilter);

            return Mapper.Map<List<MasterVendorTOMDTO>>(res);
        }

        // author : Hakim
        // date : 2019-10-01 11:25
        public List<MasterVendorTOMDTO> GetDatas(MasterVendorTOMInput input)
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();

            if (!String.IsNullOrEmpty(input.VendorName))
            {
                queryFilter = queryFilter.And(m => m.VendorName == input.VendorName);
            }

            if (!String.IsNullOrEmpty(input.VendorRegion))
            {
                queryFilter = queryFilter.And(m => m.VendorRegion == input.VendorRegion);
            }

            if (!String.IsNullOrEmpty(input.TransportationMode))
            {
                queryFilter = queryFilter.And(m => m.TransportationMode == input.TransportationMode);
            }

            queryFilter = queryFilter.And(m => m.IsActive);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression, input.SortExpression2 }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterVendor>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterVendorTOMDTO>>(dbResult);
        }

        public MasterVendorTOMDTO GetVendorByName(string name)
        {
            return Mapper.Map<MasterVendorTOMDTO>(_generalRepo.Get(c => c.VendorName == name).FirstOrDefault());
        }
    }
}
