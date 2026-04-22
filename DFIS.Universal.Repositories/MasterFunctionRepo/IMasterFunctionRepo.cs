using System.Collections.Generic;
using DFIS.Contracts;

namespace DFIS.Universal.Repositories
{
    public interface IMasterFunctionRepo<TMasterFunction, TMasterRoleFunctionTreeListView> : IGenericRepository<TMasterFunction> where TMasterFunction : class
    {
        List<TMasterFunction> GetMasterFunctionByType(string type);
        TMasterFunction GetMasterFunctionByIDFunction(int idfunction);
        List<TMasterFunction> GetMasterFunctionByParentID(int parentId);
        List<TMasterRoleFunctionTreeListView> GetMasterRoleFunctionTreeList();
        List<TMasterFunction> GetAllMasterFunction();
        TMasterFunction GetMasterFunctionByFunctionName(string functionName);
        TMasterFunction GetMasterFunctionByFunctionNameParentID(string functionName, int parentID);
        TMasterFunction SaveData(TMasterFunction input, bool status);
    }
}
