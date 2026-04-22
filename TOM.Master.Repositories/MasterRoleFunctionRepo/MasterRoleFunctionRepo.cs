using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;

namespace TOM.Master.Repositories
{
    public class MasterRoleFunctionRepo : TOMGenericRepository<MasterRolesFunctionMapping>, IMasterRoleFunctionRepo
    {
        public MasterRoleFunctionRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public List<MasterRolesFunctionMapping> GetDistinctRoleMasterRoleFunction()
        {
            List<MasterRolesFunctionMapping> tempList = new List<MasterRolesFunctionMapping>();
            var distinct = from c in Get()
                group c by new
                {
                    c.IDRole
                }
                into grp
                select grp.FirstOrDefault();
            foreach (var x in distinct)
            {
                MasterRolesFunctionMapping temp = new MasterRolesFunctionMapping()
                {
                    IDRole = x.IDRole,
                    IsActive = x.IsActive,
                    CreatedDate = x.CreatedDate,
                    CreatedBy = x.CreatedBy,
                    UpdatedDate = x.UpdatedDate,
                    UpdatedBy = x.UpdatedBy,
                    MasterRole = x.MasterRole
                };
                tempList.Add(temp);
            }
            return tempList;
        }

        public List<MasterRolesFunctionMapping> GetAllMasterRoleFunction()
        {
            var queryFilter = PredicateHelper.True<MasterRolesFunctionMapping>();
            return Get(queryFilter).ToList();
        }

        public List<MasterRolesFunctionMapping> GetAllMasterRoleFunctionByIDRole(int idrole)
        {
            var queryFilter = PredicateHelper.True<MasterRolesFunctionMapping>();
            queryFilter = queryFilter.And(p => p.IDRole.Equals(idrole));
            return Get(queryFilter).ToList();
        }

        public List<int> GetListIDFunctionByIdRole(int id)
        {
            var queryFilter = PredicateHelper.True<MasterRolesFunctionMapping>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.IDRole.Equals(id));
            return Get(queryFilter).Select(x => x.IDFunction).ToList();
        }

        public MasterRolesFunctionMapping GetMasterRoleFunctionByIDRoleIDFunction(int idrole, int idfunction)
        {
            var queryFilter = PredicateHelper.True<MasterRolesFunctionMapping>();
            queryFilter = queryFilter.And(p => p.IDRole.Equals(idrole));
            queryFilter = queryFilter.And(p => p.IDFunction.Equals(idfunction));
            return Get(queryFilter).SingleOrDefault();
        }

        public List<MasterRolesFunctionMapping> GetMasterRoleFunctionActiveByIDRoleParentIDFunction(int idrole, int parentid)
        {
            var queryFilter = PredicateHelper.True<MasterRolesFunctionMapping>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.IDRole == idrole);
            queryFilter = queryFilter.And(p => p.MasterFunction.ParentIDFunction == parentid);
            return Get(queryFilter).ToList();
        }

        public List<int> GetIDRoleActiveByIDFunction(int idfunction)
        {
            var queryFilter = PredicateHelper.True<MasterRolesFunctionMapping>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.IDFunction.Equals(idfunction));
            return Get(queryFilter).Select(x => x.IDRole).ToList();
        }

        public MasterRolesFunctionMapping SaveData(MasterRolesFunctionMapping input, bool status)
        {
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
