using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Outputs;

namespace DFIS.Universal.BusinessLogics
{
    public interface IUtilitiesBLL
    {
        List<MasterUserRoleMappingDTO> GetListRole(string username);
        //UserSession GetResponsibilityPage(string iduser, List<MasterUserLocationMapping> listLocationMap);
        UserSession GetResponsibilityPage(string iduser, int idRole);
        List<String> GetResponsibilityButton(string iduser, int parentID);
        int SaveMasterUserRoleMapping(MasterUserRoleMappingDTO Input);
        int SaveMasterUserLocationMapping(MasterUserLocationMappingDTO Input);
    }
}
