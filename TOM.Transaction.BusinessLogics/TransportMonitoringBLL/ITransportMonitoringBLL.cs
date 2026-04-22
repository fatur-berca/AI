using DFIS.Universal.Domain.DTOs;
using DFIS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.Master.Domain.DTOs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.BusinessLogics.TransportMonitoringBLL
{
    public interface ITransportMonitoringBLL
    {
        bool SendMail(string subject, string body, string[] recipients, string format = "HTML");
        void SetCurrentLocation(TransportExecutionDTO dto);
        bool DeletePosition(int TPID);
        List<TransportPositionDetailDTO> GetPosition(int TEID);
        List<TransportMonitoringDTO> GetDatas(TransportMonitoringInput input);
        List<TransportVesselMonitoringDTO> getDataShip(TransportMonitoringViewInput input);
        void updateShip(TransportMonitoringViewInput input, string userid);
        bool GetSheetUpdateXls(string tn, DateTime? et, DateTime? td, DateTime? ta, DateTime? tb);
        bool GetSheetPOWeekXls(string nod, int? wk);
         bool UpdateDataTransOrd(IEnumerable<TransportOrderMonitoringInput> input, int TEID, string userID);

        bool UpdateDataTransOrd(TransportOrderMonitoringInput input, int TEID, string userID);
        IEnumerable<MasterListDTO> getMasterListForFilter(List<string> fieldName = null);
        //dynamic GetMonitoringDataTable(string mode, string fltrtn, string fltrdf, string fltrdt, string fltrts, string fltron, string fltrvn, string fltrse, string fltrre, string fltrsl, string fltrfl, bool isSuperAdmin, object req, string stOrder, int skip, int take);
        dynamic GetMonitoringDataTableTruckTrain(TransportMonitoringFilterInput input, bool isSuperAdmin, DataTableModel model = null);
        dynamic GetMonitoringDataTableShip(TransportMonitoringFilterInput input, bool isSuperAdmin, DataTableModel model = null);
        List<TransportExecutionDTO> GetListTruckTrainServerSide(string mode, string fltrtn, string fltrdf, string fltrdt, string fltrts, string fltron, string fltrvn, string fltrse, string fltrre, string fltrsl, string fltrfl, bool isSuperAdmin, object req, int skip, int take);
        int GetCountTruckTrainServerSide(string mode, string fltrtn, string fltrdf, string fltrdt, string fltrts, string fltron, string fltrvn, string fltrse, string fltrre, string fltrsl, string fltrfl, bool isSuperAdmin, object req, string stOrder);
        #region SUB TRUCK TRAIN
        List<TransportExecutionDTO> GetTransportList();
        List<MasterVendorDTO> GetVendorName();
        List<MasterLocationDTO> GetLocationName();
        List<TransportOrderDTO> GetSTONo();
        List<MasterListDTO> GetListOrderSts();
        List<TransportExecutionDTO> GetListTruckTrain(string mode, string fltrtn, string fltrdf, string fltrdt, string fltrts, string fltron, string fltrvn, string fltrse, string fltrre, string fltrsl, string fltrfl);
        List<TransportOrderDTO> GetRowListView(string idte);
        TransportExecutionDTO GetDetail(string id);
        List<TransportExecutionDTO> GetExportXls(string tab, string fltron);
        List<TransportExecutionDTO> GetExportXlsShip(string fltron);
        bool ImportXlsGI(IEnumerable<TransportMonitoringUploadInput> imported, string username);
        bool ImportXlsUP(IEnumerable<TransportMonitoringUploadInput> imported, string username);
        bool GetImportXlsGI(string sto, string ods, DateTime gin);
        bool GetImportXlsUP(string tnn, TransportPositionDetailDTO input);
        bool UpdateDataTransOrd(Int64 id, string val);
        bool InsertDataTransOrdPU(TransportPositionDetailDTO input);
        #endregion SUB TRUCK TRAIN
    }
}
