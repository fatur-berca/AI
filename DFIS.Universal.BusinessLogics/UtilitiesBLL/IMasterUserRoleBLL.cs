using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace DFIS.Universal.BusinessLogics
{
    public interface IMasterUserRoleBLL
    {

        List<MasterUserRoleMappingDTO> GetUserRoleMapping(MasterUserRoleMappingDTO input);
        bool UserHasRole(string userID, params int[] roleIDs);
        List<MasterUserDTO> GetMasterUsers();
        List<MasterRoleDTO> GetMasterRoles();
        List<MasterUserRoleMappingDTO> GetDataByIDs(MasterUserRoleMappingDTO input);
        MasterUserRoleMappingDTO EditData(MasterUserRoleMappingDTO input, string controller, string userid);

        MasterUserRoleMappingDTO SaveData(MasterUserRoleMappingDTO input, string controller, string userid);
    }
}
