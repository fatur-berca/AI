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
    public class MasterGuidelineBLL : IMasterGuidelineBLL
    {
        private readonly IMasterGuidelineRepo _masterGuidelineRepo;
        private readonly IGenericRepository<MasterFunction> _generalFunctionRepo;

        private readonly IGenericRepository<MasterGuideline> _generalRepo;


        public MasterGuidelineBLL(IMasterGuidelineRepo masterGuidelineRepo, IGenericRepository<MasterGuideline> generalRepo, IGenericRepository<MasterFunction> generalFunctionRepo)
        {
            _masterGuidelineRepo = masterGuidelineRepo;
            _generalRepo = generalRepo;

           
            _generalFunctionRepo = generalFunctionRepo;
        }

        public List<MasterGuidelineDTO> GetMasterGuidelines(MasterGuidelineInput input)
        {
            var queryFilter = PredicateHelper.True<MasterGuideline>();

            if (!string.IsNullOrEmpty(input.PageName))
                queryFilter = queryFilter.And(m => m.PageName == input.PageName);

            if (!string.IsNullOrEmpty(input.Keywords))
            {
                queryFilter = queryFilter.And(m => m.Keywords == input.Keywords);
            }

            //queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterGuideline>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterGuidelineDTO>>(dbResult);
        }

        public List<MasterFunctionDTO> GetListForGuidelines(MasterFunctionInput input)
        {
            var queryFilter = PredicateHelper.True<MasterFunction>();
            queryFilter = queryFilter.And(m => m.Type == "Form");


            queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterFunction>();

            var dbResult = _generalFunctionRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterFunctionDTO>>(dbResult);
        }

        public MasterGuidelineDTO SaveData(MasterGuidelineDTO input)
        {
            var validateInput = new MasterGuidelineInput()
            {
                PageName = input.PageName,
                Keywords = input.Keywords,
                Description = input.Description,
                Url = input.Url,
                IsActive = input.IsActive
            };
            var prevData = GetMasterGuidelines(validateInput);
            var dbMstGuideline = Mapper.Map<MasterGuideline>(input);
            if (prevData == null || (prevData != null && prevData.Count == 0))
            {
                dbMstGuideline.CreatedDate = DateTime.Now;
                dbMstGuideline.UpdatedDate = DateTime.Now;
                _generalRepo.Insert(dbMstGuideline);
                _generalRepo.Save();
            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterGuidelineDTO>(dbMstGuideline);
        }

        public MasterGuidelineDTO EditData(MasterGuidelineDTO input)
        {
            var dbMstGuideline = Mapper.Map<MasterGuideline>(input);
            dbMstGuideline.CreatedDate = DateTime.Now;
            dbMstGuideline.UpdatedDate = DateTime.Now;
            _generalRepo.Update(dbMstGuideline);
            _generalRepo.Save();

            return Mapper.Map<MasterGuidelineDTO>(dbMstGuideline);
        }

        public MasterGuidelineDTO GetById(int id)
        {
            var result = _generalRepo.GetByID(id);

            return Mapper.Map<MasterGuidelineDTO>(result);
        }

        
        public List<MasterGuidelineDTO> GetListForGuidelinesByDescAndKeyword(MasterGuidelineInput input)
        {
            var queryFilter = PredicateHelper.True<MasterGuideline>();

            if (!string.IsNullOrEmpty(input.Keywords))
            {
                queryFilter = queryFilter.And(m => m.Description.Contains(input.Keywords) || m.Keywords.Contains(input.Keywords));
            }

            queryFilter = queryFilter.And(m => m.IsActive);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterGuideline>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();

            return Mapper.Map<List<MasterGuidelineDTO>>(dbResult);
        }       
        
    }
}
