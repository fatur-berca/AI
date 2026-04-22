using DFIS.Universal.Domain.DTOs;
using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterVendorTOMBLL : IInsertOrUpdateBLL<MasterVendorTOMDTO>
    {
        List<MasterListDTO> GetList();
        List<MasterVendorTOMDTO> Get(MasterVendorTOMInput input);
        List<MasterVendorTOMDTO> GetDatas(MasterVendorTOMInput input);
        List<MasterVendorTOMDTO> GetALLMasterVendors();
        void SaveData(MasterVendorTOMDTO input, bool status);
        List<MasterVendorTOMDTO> GetALLMasterVendorsEntity();
        List<MasterVendorTOMDTO> GetALLMasterVendorsNoChild();
        MasterVendorTOMDTO GetVendorByName(string name);
    }
}
