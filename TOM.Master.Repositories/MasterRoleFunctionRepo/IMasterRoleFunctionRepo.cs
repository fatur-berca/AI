using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.Repositories
{
    public interface IMasterRoleFunctionRepo : IGenericRepository<MasterRolesFunctionMapping>
    {
        List<MasterRolesFunctionMapping> GetDistinctRoleMasterRoleFunction();
        List<MasterRolesFunctionMapping> GetAllMasterRoleFunction();
        List<MasterRolesFunctionMapping> GetAllMasterRoleFunctionByIDRole(int idrole);
        List<int> GetListIDFunctionByIdRole(int id);
        MasterRolesFunctionMapping GetMasterRoleFunctionByIDRoleIDFunction(int idrole, int idfunction);
        List<MasterRolesFunctionMapping> GetMasterRoleFunctionActiveByIDRoleParentIDFunction(int idrole, int parentid);
        List<int> GetIDRoleActiveByIDFunction(int idfunction);
        MasterRolesFunctionMapping SaveData(MasterRolesFunctionMapping input, bool status);
    }
}
