using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.Repositories
{
    public interface IMasterVendorSuggestionRepo : IGenericRepository<MasterVendorSuggestion>
    {
        List<MasterVendorSuggestion> GetAllMasterVendorSuggestion(MasterVendorSuggestionInput criteria);
        MasterVendorSuggestion GetMasterVendorSuggestionByField(string idStartLoc, string idReceiveLoc, string orderType, string transCat, string Transmode, string vechType);
        void SaveData(MasterVendorSuggestion input, bool status);
        List<MasterVendorSuggestion> GetMasterVendorSuggestionListByField(string idStartLoc, string idReceiveLoc, string orderType, string transCat, string Transmode, string vechType);

    }
}
