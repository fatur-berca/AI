using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterIMDLRoleBLL
    {
        List<MasterIMDLRoleDTO> GetMasterIMDLRoles(MasterIMDLRoleInput input);
        MasterIMDLRoleDTO SaveData(MasterIMDLRoleDTO input, string controller, string userid);
        MasterIMDLRoleDTO EditData(MasterIMDLRoleDTO input, string controller, string userid);
        MasterIMDLRoleDTO GetById(string id);
        List<MasterRoleDTO> GetDataRole();
    }
}
