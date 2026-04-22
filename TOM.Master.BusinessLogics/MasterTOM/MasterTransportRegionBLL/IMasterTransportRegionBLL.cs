using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterTransportRegionBLL
    {
        List<MasterTransportRegionDTO> GetMasterTransportRegions(MasterTransportRegionInput input);

        MasterTransportRegionDTO SaveData(MasterTransportRegionDTO input);

        MasterTransportRegionDTO EditData(MasterTransportRegionDTO input);

        MasterTransportRegionDTO GetById(int id);

    }
}
