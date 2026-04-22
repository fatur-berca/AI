using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;


namespace TOM.Master.Repositories
{
    public interface IMasterConfigurationEmailRepo : IGenericRepository<MasterConfigurationEmail>
    {
        List<MasterConfigurationEmail> GetAllMasterConfigurationEmail();
        MasterConfigurationEmail CheckAvailability(string pageName);
        MasterConfigurationEmail SaveData(MasterConfigurationEmail input, bool status);
   
    }
}
