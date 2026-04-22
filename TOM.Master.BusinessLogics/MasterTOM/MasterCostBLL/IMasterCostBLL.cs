using DFIS.Universal.Domain.DTOs;
using System.Collections.Generic;
using System.Web;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterCostBLL: IInsertOrUpdateBLL<MasterCostDTO>, IImporterBLL<MasterCostDTO>
    {
        List<MasterListDTO> GetList();
        List<MasterCostDTO> GetALLMasterCostByCostType(List<string> costType);
        List<MasterLocationDTO> GetALLMasterLocation();
        List<MasterVendorTOMDTO> GetALLVendor();        
        int SaveData(MasterCostDTO input, bool status);
        List<string> Upload(HttpPostedFileBase input, string userId);
        List<MasterCostDTO> GetAllMasterCost(MasterCostInput input);
        List<MasterList> GetTypeList();
        bool DelData(int keyID);
    }
}
