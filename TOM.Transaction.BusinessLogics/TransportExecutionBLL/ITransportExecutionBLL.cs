using System;
using System.Collections.Generic;
using DFIS.Universal.Domain.DTOs;
using TOM.Master.Domain.DTOs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using System.Web;
using DFIS.Universal.Domain.Outputs;
using OfficeOpenXml;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;

namespace TOM.Transport.BusinessLogics.TransportExecutionBLL
{
    public interface ITransportExecutionBLL
    {
        TransportVesselMonitoringDTO GetTransportVessel(int IDTransportExecution);
        TransportExecutionDTO GetTransportNo(string TN);
        int UpdateVessel(TransportMonitoringViewInput input, string userid);
        TransportExecutionEditDTO GetTransportExecutionByID(int id);
        TransportOrderTransportExecutionEditDTO GetTransportOrderActiveByStoNO(string stono);
        //int UpdateOrder(TransportMonitoringViewInput input, string userid);
        List<TransportExecutionDTO> GetTransportNo(TransportationExecutionInput input);
        List<TransportOrderDTO> GetOrderNo(TransportOrderInput input);
        List<TransportMonitoringViewDTO> GetTransportExecutionDTOs(TransportMonitoringViewInput input);
        List<TransportOrderDTO> GetSTONoFilterByRegion(string stono, bool isUserRoleTransport, List<string> parentLocation);
        List<TransportExecutionDTO> GetTransportationNumberFilterByRegion(string stono, bool isUserRoleTransport, List<string> parentLocation);
        List<TransportExecutionDTO> GetTransportationNumberFilter(string stono, bool isUserRoleTransport, string UserId);
        List<MasterListDTO> getMasterList(List<string> fieldName = null);
        List<MasterLocationDTO> getMasterLocation(bool isUserRoleTransport, List<string> parentLocation);
        List<MasterListDTO> GetZoneList(List<UserRole> userRole);
        List<MasterVendorTOMDTO> GetVendorList(List<UserRole> userRole);
        List<MasterVendorTOMDTO> GetVendorSuggestionList(List<UserRole> userRole);
        List<string> GetCostCenterList();
        List<TransportVehicleDataDTO> GetVehicleDataList(List<UserRole> userRole);
        List<TransportVehicleDataDTO> GetVehicleByCriteriaActive(TransportVehicleDataInput criteria);
        List<TransportDriverManagementDTO> GetDriverList(List<UserRole> userRole);
        List<TransportDriverManagementFilterDTO> GetDriverByCriteriaActive(TransportDriverManagementInput criteria);
        MasterServicePoTomDTO GetServicePO(int idvendor, DateTime date);
        dynamic GetExecutionDataTable(TransportOrderInput input, string userid, DataTableModel model = null);
        List<TransportExecutionDTO> GetExecutionDataTableExportCustomReport(TransportOrderInput input, string userid);
        List<TransportExecutionDTO> GetALLTransportationExecution(TransportOrderInput input, string userid);
        List<TransportExecutionAddNewDTO> GetTransportationExecutionAddNew(TransportOrderInput input);
        List<string> GenerateTransportationNumber(string idStartLocation, string transportMode, string userid, List<TransportExecutionAddNewDTO> toList);
        void SetActiveTransportExecution(List<int> idTransporExe, string userid);
        List<String> CalculateTransportExecutionDistance(string vendorCategory, DateTime transDate, string transportMode, List<TransportRouteDTO> transRoute, string via);
        List<string> CheckSaveData(TransportExecutionDTO data, List<TransportRouteDTO> saveTransRoute, List<TransportOrderDTO> saveTransOrder, TransportVesselMonitoringDTO saveTransVessel, TransportVendorChangeLogDTO saveVendorLog, string userid, DateTime latestDateEdit);
        List<string> SaveData(TransportExecutionDTO data, List<TransportRouteDTO> saveTransRoute, List<TransportOrderDTO> saveTransOrder, TransportVesselMonitoringDTO saveTransVessel, TransportVendorChangeLogDTO saveVendorLog, string userid, DateTime latestDateEdit);

        #region SUB EXPORT
        List<TransportExecutionDTO> GetExportXls(string fltrtn, string fltron, string fltroc, string fltrsi, string fltrot, string fltrzo, string fltrdf, string fltrdt, string fltrsl, string fltrmt, string fltrse, string fltrre);
        #endregion SUB EXPORT

        List<string> UploadExecution(HttpPostedFileBase input, string userid);
        void UploadExecution(IEnumerable<TransportExecutionImportInput> input, string userid);
        void UploadExecution(IEnumerable<IEnumerable<TransportExecutionImportInput>> input, string userid);
        List<string> CheckValidate(ExcelWorksheet sheet);
        string getTypeUpload(int totalColumns);

        //TransportExecutionPrintDTO PrintDocument(List<int> idTE, string username);
        TransportExecutionPrintDTO PrintDocument(int idTE, string username);
        TransportExecutionPrintDTO PrintDocumentIPB(int idTE, string username);

        MasterGenWeek GetWeekNow();

        #region SUB SI
        List<TransportExecutionDTO> GetVendorSI(List<UserRole> userRole);
        MasterVendor GetVendor(int idVendor);
        MasterVendor GetVendorByZone(int idVendor, string StartLocation = "");
        List<MasterGenWeekDTO> GetWeekMstGen(int year);
        MasterGenWeek GetMasterGenWeek(int week, int year);
        List<TransportExecutionDTO> GetExportSI(int fltrvn, int fltrwk, int fltryr);
        List<TransportExecutionDTO> GetDataTEByVendorTransportDate(DateTime transportDate, int idVendor, bool IsRoleTransport, List<string> RegionList, string StartLocation);
        List<TransportExecutionExportSIDTO> GetExportSI(DateTime transportDate, int vendorID, bool isTransport, List<string> regionList, List<string> tncreator);
        List<TransportExecutionDTO> GetTEChangelogForExportSI(DateTime transDate, int idVendor, bool isTransport, List<string> regionList, string startLoc);
        bool SendEmailSI(string mailto, string vendor, string reg, string week, string tglawal, string tglakhir, string user, string files);
        #endregion SUB SI

        //List<MasterVendor> GetVendorBySuggestionList(int idTE, List<UserRole> userRole);
        List<MasterVendor> GetVendorBySuggestionList(int idTE);
        void SaveVendorTE(List<TransportationExecutionInput> input, string userid);
        List<TransportExecutionVendorDTO> GetTotalVendorByTN(TransportOrderInput input);
        List<TransportExecutionAddNewDTO> SaveDataTemp(List<TransportExecutionAddNewDTO> input, string userid);
        List<TransportExecutionAddNewDTO> GetDataTemp(string userid);
        IEnumerable<object> GetTNCreatorShipping(string tncreator);
        decimal CalculateDistance(string distanceType, DateTime transDate, string transportMode, List<TransportRouteDTO> routes = null);
        List<String> CalculateExecutionDistance(string distanceType, DateTime transDate, string transportMode, List<TransportRouteDTO> routes = null);
    }
}
