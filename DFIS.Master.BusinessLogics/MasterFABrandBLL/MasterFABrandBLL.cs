using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DFIS.Contracts;
using DFIS.EntitiesDAL;
using DFIS.EntitiesDAL.EDMX;
using DFIS.Master.Domain.DTOs;
using DFIS.Master.Domain.Inputs;
using DFIS.Master.Repositories;
using DFIS.Utils;
using DFIS.Utils.Exceptions;


namespace DFIS.Master.BusinessLogics
{
    public class MasterFABrandBLL : IMasterFABrandBLL
    {
        private readonly IMasterFABrandRepo _masterFABrandRepo;
        private readonly IGenericRepository<MasterFABrand> _generalRepo;

        public MasterFABrandBLL(IMasterFABrandRepo masterFABrandRepo, IGenericRepository<MasterFABrand> generalRepo)
        {
            _masterFABrandRepo = masterFABrandRepo;
            _generalRepo = generalRepo;
        }

        public List<MasterFABrandDTO> GetMasterFABrands(MasterFABrandInput input)
        {
            var queryFilter = PredicateHelper.True<MasterFABrand>();

            if (!string.IsNullOrEmpty(input.SpeakingCode))
                queryFilter = queryFilter.And(m => m.SpeakingCode == input.SpeakingCode);

            if (!string.IsNullOrEmpty(input.Type))
            {
                queryFilter = queryFilter.And(m => m.Type == input.Type);
            }

            //queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterFABrand>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterFABrandDTO>>(dbResult);
        }

        public List<MasterFABrandDTO> GetAllMasterFABrands()
        {
            var result = _generalRepo.GetAll().ToList();
            return Mapper.Map<List<MasterFABrandDTO>>(result);
        }

        public MasterFABrandDTO SaveData(MasterFABrandDTO input)
        {
            var validateInput = new MasterFABrandInput()
            {
                FACode = input.FACode,
                SpeakingCode = input.SpeakingCode,
                Type = input.Type,
                StickPerBox = input.StickPerBox,
                StickPerPack = input.StickPerPack,
                PackPerBox  = input.PackPerBox,                
                IsActive = input.IsActive
            };
            var prevData = GetMasterFABrands(validateInput);
            var dbMstFABrand = Mapper.Map<MasterFABrand>(input);
            if (prevData == null || (prevData != null && prevData.Count == 0))
            {
                dbMstFABrand.CreatedDate = DateTime.Now;
                dbMstFABrand.UpdatedDate = DateTime.Now;
                _generalRepo.Insert(dbMstFABrand);
                _generalRepo.Save();
            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterFABrandDTO>(dbMstFABrand);
        }

        public MasterFABrandDTO EditData(MasterFABrandDTO input)
        {
            var dbMstFABrand = Mapper.Map<MasterFABrand>(input);
            dbMstFABrand.CreatedDate = DateTime.Now;
            dbMstFABrand.UpdatedDate = DateTime.Now;
            _generalRepo.Update(dbMstFABrand);
            _generalRepo.Save();

            return Mapper.Map<MasterFABrandDTO>(dbMstFABrand);
        }


        public MasterFABrandDTO GetById(string id)
        {
            var result = _generalRepo.GetByID(id);

            // return result and Mapping result from entity to DTO
            // because the return type is DTO
            return Mapper.Map<MasterFABrandDTO>(result);
        }

    }
}
