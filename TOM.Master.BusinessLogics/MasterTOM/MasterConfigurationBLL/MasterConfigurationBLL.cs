using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using TOM.Master.Repositories;

namespace TOM.Master.BusinessLogics
{
    public class MasterConfigurationBLL : IMasterConfigurationBLL
    {
        private readonly IGenericRepository<MasterConfiguration> _generalRepo;
        private readonly IMasterConfigurationRepo _masterConfigurationRepo;
        private readonly IMasterFunctionRepo _masterFunctionRepo;
        private readonly IGenericRepository<MasterUser> _generalRepoUser;

        public MasterConfigurationBLL(IGenericRepository<MasterConfiguration> generalRepo, IMasterConfigurationRepo masterConfigurationRepo, IMasterFunctionRepo masterFunctionRepo,
            IGenericRepository<MasterUser> generalRepoUser)
        {
            _generalRepo = generalRepo;
            _generalRepoUser = generalRepoUser;
            _masterConfigurationRepo = masterConfigurationRepo;
            _masterFunctionRepo = masterFunctionRepo;           
        }

        public List<string> GetDistinctPageName()
        {
            return _masterConfigurationRepo.GetDistinctPageName();
        }

        public MasterConfigurationDTO GetRangeValueConfiguration(string pageName, string description)
        {
            var queryFilter = PredicateHelper.True<MasterConfiguration>();
            queryFilter = queryFilter.And(k => k.IsActive);
            if (!String.IsNullOrEmpty(pageName))
            {
                queryFilter = queryFilter.And(k => k.PageName == pageName);
            }
            if (!String.IsNullOrEmpty(description))
            {
                queryFilter = queryFilter.And(k => k.Description == description);
            }

            var dbResult = _generalRepo.Get(queryFilter).FirstOrDefault();

            return Mapper.Map<MasterConfigurationDTO>(dbResult);
        }

        public List<MasterConfiguration> GetAllMasterConfiguration()
        {
            return _masterConfigurationRepo.GetAllMasterConfiguration();
        }

        public List<MasterConfigurationDTO> GetRecords(MasterConfigurationInput input)
        {
            var queryFilter = PredicateHelper.True<MasterConfiguration>();

            queryFilter = queryFilter.And(k => k.IsActive == true);

            if (input.IDConfiguration != 0)
            {
                queryFilter = queryFilter.And(k => k.IDConfiguration == input.IDConfiguration);
            }
            if (!String.IsNullOrEmpty(input.PageName))
            {
                queryFilter = queryFilter.And(k => k.PageName == input.PageName);
            }
            if (!String.IsNullOrEmpty(input.Description))
            {
                queryFilter = queryFilter.And(k => k.Description == input.Description);
            }
            if (!String.IsNullOrEmpty(input.Value))
            {
                queryFilter = queryFilter.And(k => k.Value == input.Value);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterConfiguration>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();

            return Mapper.Map<List<MasterConfigurationDTO>>(dbResult);
        }
        public MasterUser GetUser(string userId)
        {
            var result = _generalRepoUser.GetByID(userId);

            return result;
        }

        public List<string> getValue(string PageName,string Description)
        {
            var queryFilter = PredicateHelper.True<MasterConfiguration>();
            queryFilter = queryFilter.And(m => m.Description == Description).And(p => p.PageName == PageName);
            var result = _generalRepo.Get(queryFilter).Select(x => x.Value).ToList();
            return result;
        }

        public MasterConfiguration SaveData(MasterConfiguration input, bool status)
        {
            return _masterConfigurationRepo.SaveData(input, status);
        }
    }
}