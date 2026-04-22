using System.Collections.Generic;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace DFIS.Universal.BusinessLogics
{
    public interface IMasterConfigurationBLL<TMasterConfiguration, TMasterUser>
    {
        List<MasterFunctionDTO> GetMasterFunctionTypeMenu(string type);
        List<TMasterConfiguration> GetAllMasterConfiguration();
        List<MasterConfigurationDTO> GetRecords(MasterConfigurationInput input);
        TMasterConfiguration SaveData(TMasterConfiguration input, bool status);
        TMasterUser GetUser(string userId);
        List<string> GetValue(string PageName, string Descriptionn);
    }
}
