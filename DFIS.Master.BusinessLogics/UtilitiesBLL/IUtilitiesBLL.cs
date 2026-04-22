using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using DFIS.Master.Domain.Outputs;

namespace DFIS.Master.BusinessLogics.UtilitiesBLL
{
    public interface IUtilitiesBLL
    {
        List<MasterUserRoleMappingDTO> GetListRole(string username);
        //UserSession GetResponsibilityPage(string iduser, List<MasterUserLocationMapping> listLocationMap);
        UserSession GetResponsibilityPage(string iduser);
        List<String> GetResponsibilityButton(string iduser, int parentID);
        int SaveMasterUserRoleMapping(MasterUserRoleMappingDTO Input);
        int SaveMasterUserLocationMapping(MasterUserLocationMappingDTO Input);
    }
}
