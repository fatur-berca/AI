using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.Repositories
{
    public interface IMasterCostRepo : IGenericRepository<MasterCost>
    {
        List<MasterCost> SelectAllCostByListCostType(List<string> costType);
        List<MasterCostDTO> ListMasterCostWithLatestEffectiveStartDate(string type);
        int SaveData(MasterCost input, bool status);
        void SaveDataUpload(MasterCost input, bool status);
        List<MasterCost> GetAllMasterCost(MasterCostInput input);
        int CommitSave();
    }
}
