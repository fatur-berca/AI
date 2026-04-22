using System.Collections.Generic;
using DFIS.Contracts;

namespace DFIS.Universal.Repositories
{
    public interface IMasterConfigurationRepo<T> : IGenericRepository<T> where T : class
    {
        List<T> GetAllMasterConfiguration();
        List<string> GetDistinctPageName();
        T GetMasterConfigurationByPageNameDescription(string pageName, string description);
        List<T> GetMasterConfigurationListByPageNameDescription(string pageName, List<string> description);
        List<T> GetListMasterConfigurationByPageNameDescription(string pageName, string description);
        T SaveData(T input, bool status);
    }
}
