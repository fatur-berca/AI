using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Master.Domain.DTOs;
using DFIS.Master.Domain.Inputs;

namespace DFIS.Master.BusinessLogics
{
    public interface IMasterLocationBLL
    {
        bool SetMasterLocationMapping(string source, string IDLocation, bool value);
        List<MasterLocationDTO> GetMasterLocationsBySource(string source, MasterLocationInput input);
        List<MasterLocationDTO> GetMasterLocations(MasterLocationInput input);
        List<MasterLocationDTO> GetMasterLocationDrops(MasterLocationInput input);
        MasterLocationDTO SaveData(MasterLocationDTO input);
        MasterLocationDTO EditData(MasterLocationDTO input);
        List<MasterLocationDTO> GetDepartementForKPISSs();
        MasterLocationDTO GetById(string id);
        List<MasterLocationDTO> GetWarehouseList(string type, string region);
    }
}
