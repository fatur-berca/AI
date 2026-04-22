using TOM.EntitiesDAL.EDMX;
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
    public interface IMasterIMDLRoleLocationBLL
    {
        List<MasterIMDLRoleLocationDTO> GetMasterIMDLRoleLocations(MasterIMDLRoleLocationInput input);
        MasterIMDLRoleLocationDTO SaveData(MasterIMDLRoleLocationDTO input, string controller, string userid);
        MasterIMDLRoleLocationDTO EditData(MasterIMDLRoleLocationDTO input, string controller, string userid);
        MasterIMDLRoleLocationDTO GetById(int id);
        List<string> GetListIDLocationByIdUser(string id);
        List<MasterLocationDTO> GetDataLocation();
        List<MasterIMDLRoleDTO> GetDataIMDLRole();
        List<MasterUserLocationMapTreeListView> GetTreeList();
        List<string> GetWarehouseNameByIdUser(string id);
        List<MasterIMDLRoleLocationDTO> GetDistinctLocationMasterUserLocation();

    }
}
