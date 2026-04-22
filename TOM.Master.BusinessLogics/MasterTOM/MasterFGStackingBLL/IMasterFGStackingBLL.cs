using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterFGStackingBLL
    {
        List<MasterFGStackingDTO> GetMasterFGStackings(MasterFGStackingInput input, string iduser);

        MasterFGStackingDTO SaveData(MasterFGStackingDTO input);

        MasterFGStackingDTO EditData(MasterFGStackingDTO input);

        List<MasterFGStackingDTO> GetById(int id);

        List<MasterLocationDTO> GetMasterLocations(string iduser);
        List<string> ListAvailableSpeakingCode();
    }
}
