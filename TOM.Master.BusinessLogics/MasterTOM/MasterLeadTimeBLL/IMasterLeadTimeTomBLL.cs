using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.EntitiesDAL.EDMX;
using System.Web;
using TOM.Master.BusinessLogics;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterLeadTimeTomBLL: IInsertOrUpdateBLL<MasterLeadTimeTomDTO>, IImporterBLL<MasterLeadTimeTomDTO>
    {
        List<MasterLeadTimeTomDTO> GetMasterLeadTimeToms(MasterLeadTimeTomInput input);                
        List<MasterLeadTimeTomDTO> GetAllMasterLeadTimeTOM(MasterLeadTimeTomInput input);
        List<MasterList> GetTmList();
        List<MasterList> GetViaList();
        List<MasterList> GetTypeList();
        //MasterLeadTimeTomDTO SaveData(MasterLeadTimeTomDTO input);
        //MasterLeadTimeTomDTO EditData(MasterLeadTimeTomDTO input);
        int SaveData(MasterLeadTimeTomDTO input);        
        int EditData(MasterLeadTimeTomDTO input);
        MasterLeadTimeTomDTO GetById(int id);
        //MasterLeadTimeTomDTO UploadDataLeadTimeTom(MasterLeadTimeTomDTO input, string userid, string sheetName);
        int UploadDataLeadTimeTom(MasterLeadTimeTomDTO input, string userid, string sheetName);
        List<MasterVendorTOMDTO> GetDropDownVendor();
        List<MasterVendorTOMDTO> GetMasterVendorTom(MasterVendorTOMInput input);
        bool Delete(int keyId);
    }
}
