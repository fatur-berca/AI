using DFIS.Universal.Domain.DTOs;
using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterCostCenterAccountBLL: IInsertOrUpdateBLL<MasterCostCenterAccountDTO>
    {
        List<MasterListDTO> GetList();
        List<MasterCostCenterAccountDTO> GetALLMasterCostCenterAccount(MasterCostCenterAccountInput input);
        MasterCostCenterAccountDTO GetMasterCostCenterAccountBySenderReceiverMaterial(MasterCostCenterAccountInput input);
        //void SaveData(MasterCostCenterAccount input, bool status);
        int SaveData(MasterCostCenterAccountDTO input, bool status);
        bool DelData(int key);
    }
}
