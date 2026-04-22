using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using TOM.EntitiesDAL.EDMX;
using OfficeOpenXml;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using DFIS.Universal.Domain.DTOs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.BusinessLogics
{
    public interface ITransportPickingListBLL
    {
        List<MasterListDTO> getMasterList(string id);
        List<TransportOrderDTO> GetSTONoFilter(string stono);
        List<TransportOrderDTO> GetALLPickingList(TransportOrderInput criteria);
        List<string> GetAllUploadedBy(TransportOrderInput criteria);
        MasterGenWeek GetWeekNow();
        List<MasterLocationDTO> GetZoneList();
        List<MasterLocationDTO> GetSenderReceiverList();
        TransportOrderDTO SelectTransportOrderByStoNo(string stono);
        int SaveData(TransportOrderDTO input, string userid);
        int SaveDetailData(TransportOrderDetailDTO input, string userid);

        List<TransportExecutionDTO> getAllTransportExecution();
        List<MasterLocationDTO> getAllMasterLocation();
        string getUserFullName(string IDUser);
        List<string> UploadPickingList(HttpPostedFileBase input, string userid);
        List<string> UploadPickingList2(HttpPostedFileBase input, string userid);
        List<string> CheckValidate(ExcelRange sheet, int rowNum, string sheetx);
        //TransportOrder setEntityTO(ExcelRange sheet, int rowNum, int idPL, int defaultSeq, string userid, string filename, string STONo = "");
        //TransportOrderDetail setEntityTODetail(ExcelRange sheet, int rowNum, int idPL, string userid);
        string setActualSenderReceiver(string idloc, string locname);
        List<TransportOrderDTO> GetTransportOrderByShipmentDate(DateTime fromDate, DateTime toDate, string uploadedBy);
        DataTable ListToDataTable(List<TransportOrderDTO> dataTO, DateTime DocDate, string userid);
        byte[] ExportToExcel(DataTable dataTable);
        List<TransportOrderDTO> GetExportXls(TransportOrderInput criteria);
        string SendEmail(string userid);

        #region LOG
        List<TransportPickingListLogDTO> GetAllBatchDate(DateTime tfrom, DateTime tto);
        List<TransportOrderDTO> GetBatchData(DateTime tbtc, string usr, DateTime tfrom, DateTime tto);
        string DeleteBatchData(DateTime tbtc, string usr, DateTime tfrom, DateTime tto, string IDUser);
        #endregion LOG
        
        #region EXPORT
        List<TransportOrderDTO> GetRawDataExport();
        List<TransportOrderDTO> GetListFormExport();
        #endregion EXPORT

        List<TransportPickingListExportDTO> GetRawTOD(TransportOrderInput criteria);

        #region new code Nicco
        List<TransportPickingListViewDTO> GetRawTPL(TransportOrderInput criteria);
        #endregion
        List<MasterListDTO> getMasterList();

        int UpdateIsActive();
    }
}
