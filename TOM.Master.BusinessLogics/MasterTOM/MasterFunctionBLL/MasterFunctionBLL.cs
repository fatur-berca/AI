using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.Master.Repositories;
using DFIS.Utils;
using TOM.Master.BusinessLogics;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public class MasterFunctionBLL : IMasterFunctionBLL
    {
        private readonly IMasterFunctionRepo _masterFunctionRepo;
        private readonly IMasterListRepo _masterListRepo;

        public MasterFunctionBLL(IMasterFunctionRepo masterFunctionRepo, IMasterListRepo masterListRepo)
        {
            _masterFunctionRepo = masterFunctionRepo;
            _masterListRepo = masterListRepo;
        }

        public List<MasterList> GetTypeList()
        {
            return _masterListRepo.GetMasterListByFieldName("FunctionType");
        }

        public List<MasterFunctionDTO> GetMasterFunctionTypeMenu(string type)
        {
            if (type == "Module")
            {
                return new List<MasterFunctionDTO>();
            }
            else
            {
                var temp = Mapper.Map<List<MasterFunction>, List<MasterFunctionDTO>>(_masterFunctionRepo.GetMasterFunctionByType(type));
                return temp;
            }
        }

        public List<MasterFunction> GetAllMasterFunction()
        {
            return _masterFunctionRepo.GetAllMasterFunction();
        }

        public MasterFunction SaveData(MasterFunction input, bool status)
        {
            return _masterFunctionRepo.SaveData(input, status);
        }
        public List<MasterFunctionDTO> GetMasterFunctionTypes(MasterFunctionInput input)
        {
            var queryFilter = PredicateHelper.True<MasterFunction>();
            queryFilter = queryFilter.And(m => m.IDFunction == input.IDFunction);

            queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterFunction>();

            var dbResult = _masterFunctionRepo.Get(queryFilter, orderByFilter).ToList();
            var finalresult = dbResult.GroupBy(x => x.Type).Select(y => y.First());
            return Mapper.Map<List<MasterFunctionDTO>>(finalresult);
        }
        public List<MasterFunctionDTO> GetMasterFunctions(MasterFunctionInput input)
        {
            var queryFilter = PredicateHelper.True<MasterFunction>();
            queryFilter = queryFilter.And(m => m.Type == "Form");         

            queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterFunction>();

            var dbResult = _masterFunctionRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterFunctionDTO>>(dbResult);
        }

        public List<MasterFunctionDTO> GetMasterFunctionForDynamics(MasterFunctionInput input)
        {
            var queryFilter = PredicateHelper.True<MasterFunction>();
            string[] allowedStatus = new string[] { "Building & Facility", "Suggestion System", "Sandbag" };
            queryFilter = queryFilter.And(m => allowedStatus.Contains(m.FunctionName));

            queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterFunction>();

            var dbResult = _masterFunctionRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterFunctionDTO>>(dbResult);
        }

    }
}
