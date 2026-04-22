using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterConfigurationBLL //: IMasterConfigurationBLL<MasterConfiguration, MasterUser>
    {
        List<string> GetDistinctPageName();
        List<MasterConfiguration> GetAllMasterConfiguration();
        List<MasterConfigurationDTO> GetRecords(MasterConfigurationInput input);
        MasterConfiguration SaveData(MasterConfiguration input, bool status);
        MasterUser GetUser(string userId);
        List<string> getValue(string PageName, string Descriptionn);
    }
}
