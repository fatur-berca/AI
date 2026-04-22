using DFIS.Universal.Domain.DTOs;
using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;

namespace DFIS.Universal.BusinessLogics
{
    public interface IMasterConfigurationEmailBLL
    {
        List<MasterFunctionDTO> GetMasterFunctionTypeMenu(string type);
        List<MasterConfigurationEmail> GetAllMasterConfigurationEmail();
        MasterConfigurationEmail SaveData(MasterConfigurationEmail input, bool status);
    }
}
