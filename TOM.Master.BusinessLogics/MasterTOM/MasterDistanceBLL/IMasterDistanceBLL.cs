using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterDistanceBLL: IImporterBLL<MasterDistanceDTO>, IInsertOrUpdateBLL<MasterDistanceDTO>
    {
        List<MasterDistanceDTO> GetAllMasterDistanceLocation(MasterDistanceInput input);
        List<MasterDistanceDTO> GetExportMasterDistance(MasterDistanceInput input); 
        List<MasterDistanceDTO> GetAllMasterDistance(MasterDistanceInput input);       
        List<MasterList> GetTmList();
        List<MasterList> GetViaList();
        List<MasterList> GetTypeList();
        //MasterDistanceDTO SaveData(MasterDistanceDTO input);
        int SaveData(MasterDistanceDTO input);
        void SaveData(MasterDistance input, bool status);
        //MasterDistanceDTO EditData(MasterDistanceDTO input);
        int EditData(MasterDistanceDTO input);
        MasterDistanceDTO GetById(int id);
        //MasterDistanceDTO UploadDataDistance(MasterDistanceDTO input, string userid, string sheetName);
        int UploadDataDistance(MasterDistanceDTO input, string userid, string sheetName);
        //int UploadDataDistance(MasterDistanceDTO input, string userid, string sheetName);
        List<MasterDistanceDTO> GetData();
        List<MasterLocationDTO> GetSenderReceiverTOM(List<string> ListLocation);
        bool DelData(int keyID);
        int RecalculateTotal();
        bool _spc_updateKMBased();
    }
}
