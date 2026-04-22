using System;
using System.Collections.Generic;
using System.Linq;
using DFIS.Contracts;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using DFIS.Universal.Repositories;

namespace TOM.Master.Repositories
{
    public interface IMasterFunctionRepo : IMasterFunctionRepo<MasterFunction, MasterRoleFunctionTreeListView> { }
    public class MasterFunctionRepo : TOMGenericRepository<MasterFunction>, IMasterFunctionRepo
    {
        private readonly ITOMGenericRepository<MasterRoleFunctionTreeListView> _generalRepo;
        
        public MasterFunctionRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
            _generalRepo = new TOMGenericRepository<MasterRoleFunctionTreeListView>(contextEntities);
        }

        /// <summary>
        /// Mengambil isian dropdown Parent dari tabel master function dengan where param type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>List MasterFunction</returns>
        public List<MasterFunction> GetMasterFunctionByType(string type)
        {
            var queryFilter = PredicateHelper.True<MasterFunction>();
            queryFilter = queryFilter.And(p => p.IsActive).And(p => p.Type == type);
            return Get(queryFilter).ToList();
        }

        public MasterFunction GetMasterFunctionByIDFunction(int idfunction)
        {
            var queryFilter = PredicateHelper.True<MasterFunction>();
            queryFilter = queryFilter.And(p => p.IDFunction == idfunction);
            return Get(queryFilter).First();
        }

        public List<MasterFunction> GetMasterFunctionByParentID(int parentId)
        {
            var queryFilter = PredicateHelper.True<MasterFunction>();
            queryFilter = queryFilter.And(p => p.IsActive).And(p => p.ParentIDFunction == parentId);
            return Get(queryFilter).ToList();
        }

        public List<MasterRoleFunctionTreeListView> GetMasterRoleFunctionTreeList()
        {
            var queryFilter = PredicateHelper.True<MasterRoleFunctionTreeListView>();
            return _generalRepo.Get(queryFilter).OrderBy(x => x.NameModule).ThenBy(x => x.NameForm).ThenBy(x => x.NameButton).ToList();
        }

        public List<MasterFunction> GetAllMasterFunction()
        {
            var queryFilter = PredicateHelper.True<MasterFunction>();
            return Get(queryFilter).ToList();
        }

        public MasterFunction GetMasterFunctionByFunctionName(string functionName)
        {
            var queryFilter = PredicateHelper.True<MasterFunction>();
            queryFilter = queryFilter.And(p => p.IsActive).And(p => p.FunctionName == functionName);
            return Get(queryFilter).FirstOrDefault();
        }

        public MasterFunction GetMasterFunctionByFunctionNameParentID(string functionName, int parentID)
        {
            var queryFilter = PredicateHelper.True<MasterFunction>();
            queryFilter = queryFilter.And(p => p.IsActive).And(p => p.FunctionName == functionName).And(p => p.ParentIDFunction == parentID);
            return Get(queryFilter).FirstOrDefault();
        }

        public MasterFunction SaveData(MasterFunction input, bool status)
        {
            if (input.ParentIDFunction == 0)
                input.ParentIDFunction = null;
            if (status)
            {
                input.CreatedDate = DateTime.Now;
                input.UpdatedDate = DateTime.Now;
                Insert(input);
            }
            else
            {
                input.UpdatedDate = DateTime.Now;
                Update(input);
            }
            Save();
            return input;
        }
    }
}
