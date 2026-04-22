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
    public class MasterMappingBLL : IMasterMappingBLL
    {
        private readonly IMasterMappingRepo _masterMappingRepo;
        private readonly IGenericRepository<MasterMapping> _generalRepo;

        public MasterMappingBLL(IMasterMappingRepo masterMappingRepo, IGenericRepository<MasterMapping> generalRepo)
        {
            _masterMappingRepo = masterMappingRepo;
            _generalRepo = generalRepo;
        }

        public List<MasterMappingDTO> GetMasterMappings(MasterMappingInput input)
        {
            var queryFilter = PredicateHelper.True<MasterMapping>();

            //queryFilter = queryFilter.And(p => p.IsActive == true);

            if (!string.IsNullOrEmpty(input.MapFrom))
            {
                queryFilter = queryFilter.And(m => m.MapFrom == input.MapFrom);
            }
            if (!string.IsNullOrEmpty(input.MapTo))
            {
                queryFilter = queryFilter.And(m => m.MapTo == input.MapTo);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterMapping>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();

            return Mapper.Map<List<MasterMappingDTO>>(dbResult);
        }

        public MasterMappingDTO SaveData(MasterMappingDTO input)
        {
            var validateInput = new MasterMappingInput()
            {
                MapFrom = input.MapFrom,
                MapTo = input.MapTo,               
                IsActive = input.IsActive
            };
            var prevData = ValidateMasterMappings(validateInput);
            var dbMstFABrand = Mapper.Map<MasterMapping>(input);
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
            return Mapper.Map<MasterMappingDTO>(dbMstFABrand);
        }

        public List<MasterMappingDTO> ValidateMasterMappings(MasterMappingInput input)
        {
            var queryFilter = PredicateHelper.True<MasterMapping>();

            if (!string.IsNullOrEmpty(input.MapFrom))
                queryFilter = queryFilter.And(m => m.MapFrom == input.MapFrom);
            if(!string.IsNullOrEmpty(input.MapTo))
                queryFilter = queryFilter.And(m => m.MapTo == input.MapTo);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterMapping>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterMappingDTO>>(dbResult);
        }

        public MasterMappingDTO EditData(MasterMappingDTO input)
        {
            //var dbMstFABrand = Mapper.Map<MasterMapping>(input);
            //dbMstFABrand.CreatedDate = DateTime.Now;
            //dbMstFABrand.UpdatedDate = DateTime.Now;
            //_generalRepo.Update(dbMstFABrand);
            //_generalRepo.Save();
            //return Mapper.Map<MasterMappingDTO>(dbMstFABrand);

            var validateInput = new MasterMappingInput()
            {
                MapFrom = input.MapFrom,
                MapTo = input.MapTo,
                IsActive = input.IsActive
            };
            var prevData = ValidateMasterMappings(validateInput);
            var dbMstFABrand = Mapper.Map<MasterMapping>(input);
            //var check = Mapper.Map<MasterMapping>(GetById(input.IDMasterMapping));
            if (prevData == null || (prevData != null && prevData.Count == 0))
            {
                dbMstFABrand.CreatedDate = DateTime.Now;
                dbMstFABrand.UpdatedDate = DateTime.Now;
                _generalRepo.Update(dbMstFABrand);
                _generalRepo.Save();
            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterMappingDTO>(dbMstFABrand);
        }

        public MasterMappingDTO GetById(int id)
        {
            var result = _generalRepo.GetByID(id);

            // return result and Mapping result from entity to DTO
            // because the return type is DTO
            return Mapper.Map<MasterMappingDTO>(result);
        }

        public MasterMappingDTO Import(MasterMappingDTO input)
        {
            var masterMapping = Mapper.Map<MasterMapping>(input);

            try
            {
                _generalRepo.Insert(masterMapping);
                _generalRepo.Save();
            }
            catch
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }

            return Mapper.Map<MasterMappingDTO>(masterMapping);
        }
    }
}
