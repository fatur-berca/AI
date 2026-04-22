using DFIS.Universal.Domain.DTOs;
using System.Collections.Generic;
using System.Web;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterVendorSuggestionBLL: IImporterBLL<MasterVendorSuggestionDTO>, IInsertOrUpdateBLL<MasterVendorSuggestionDTO>
    {
        MasterVendorSuggestionDTO GetByID(int id);
        List<MasterListDTO> GetList();
        List<MasterLocationDTO> GetLocation();
        List<MasterVendorSuggestionDTO> GetALLMasterVendorSuggestion(MasterVendorSuggestionInput criteria);
        void SaveData(MasterVendorSuggestion input, bool status);
        List<string> Upload(HttpPostedFileBase input, string userId);
        List<MasterVendorTOMDTO> GetVendorByCriteria(MasterVendorSuggestionDTO Criteria);
        MasterVendorSuggestion ValidateData(MasterVendorSuggestion Input);
    }
}
