using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterLocationBLL: IInsertOrUpdateBLL<MasterLocationDTO>
    {
        List<MasterLocationDTO> GetMasterLocations(MasterLocationInput input);
        List<MasterLocationDTO> GetMasterLocationDrops(MasterLocationInput input);
        MasterLocationDTO SaveData(MasterLocationDTO input);
        MasterLocationDTO EditData(MasterLocationDTO input);
        List<MasterLocationDTO> GetDepartementForKPISSs();
        MasterLocationDTO GetById(string id);
        string GetNameById(string id);
        List<MasterLocationDTO> GetWarehouseList(string type, string region);

        List<MasterLocationDTO> GetMasterLocationMapping(MasterLocationDTO criteria);
        bool SetMasterLocationMapping(MasterLocationDTO input, bool value);
        List<MasterLocationDTO> Get(Expression<Func<MasterLocation, bool>> filter);
        List<MasterLocationDTO> GetSenderReceiverTOM(List<string> ListLocation);
        List<MasterLocationDTO> GetByConfig(string configName, Expression<Func<MasterLocation, bool>> filter = null);
    }
}
