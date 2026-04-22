using DFIS.Universal.Domain.DTOs;
using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;


namespace TOM.Master.BusinessLogics
{
    public interface IMasterRoleFunctionBLL
    {
        List<MasterRoleDTO> GetRoleList();
        List<MasterRoleFunctionDTO> GetDistinctMasterRoleFunction();
        List<int> GetIDFunctionList(int idRole);
        MasterRolesFunctionMapping SaveData(MasterRoleFunctionDTO input, bool status);
        void UpdateIsActiveByIDRole(int idrole, bool status);
        List<MasterRoleFunctionTreeListView> GetMasterRoleFunctionTreeList();
    }
}
