using System;
using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using TOM.Master.Domain.Inputs;
using DFIS.Universal.Domain.DTOs;
using TOM.Master.BusinessLogics;
using DFIS.Utils;
using System.Collections;

namespace TOM.Transport.BusinessLogics
{
    public interface ITransportOrderBLL: IInsertOrUpdateBLL<TransportOrderDTO>, IInsertOrUpdateBLL<TransportOrderRequestDTO>, IInsertOrUpdateBLL<TransportOrderDetailDTO>
    {
        bool DeleteRequest(string requestNo);
        bool DeleteRequest(string[] requestNo);
        List<TransportOrderRequestDTO> GetRequests(string reqNo = null, int? reqID = null, bool isActive = true);
        string SaveData(IEnumerable<TransportOrderRequestDTO> data, string byUser, bool handleRollback = true);
        string SaveData(TransportOrderRequestData data, string byUser, bool handleRollback = true);
        bool IsSafeToOverwrite(string stono, string updatedStatus);
        string[] SaveData(IEnumerable<TransportOrderRequestData> data, string byUser);
        List<TransportOrderDTO> SearchSTONo(string stoNo);
        List<TransportOrderDTO> Get(TransportOrderInput filter = null, string orderBy = null);
        bool IsSTONoExists(string stono, params int[] excludesTransportOrderID);
        TransportOrderDTO GetTransportOrderById(int? IDTransportOrder);
        List<TransportOrderRequestDTO> GetTransportOrderRequestById(int? IDRequest, string RequestNumber);
        int CountVehicle(string requestNumber, bool activeOnly = true);
        TransportOrderDetailDTO GetTransportOrderDetailById(int? IDTransportOrderDetail);
        List<TransportOrderDetailDTO> GetTransportOrderDetails(int? IDTransportOrder);
        List<TransportOrderDTO> GetSTONoFilterByRegion(string stono, bool isUserRoleTransport, List<string> parentLocation);
        List<MasterListDTO> getMasterList();
        List<MasterLocationDTO> getMasterLocation(bool isUserRoleTransport, List<string> parentLocation);
        List<MasterLocationDTO> GetZoneList();
        List<TransportOrderDTO> GetTransportOrder();
        List<TransportOrderDTO> GetShipmentDate();
        List<SP_GetTransportOrderRequestDTO> GetListViewTransportationOrder(TransportOrderRequestInput filter);
        dynamic GetTransportationOrderDataTable(TransportOrderRequestInput filter, DataTableModel model = null);
        void SetActiveTransportOrderRequest(List<int> idTransporReq, string userid);

        List<MasterListDTO> GetList();
        List<MasterLocationDTO> GetALLMasterLocation();
        MasterMapping GetOrderType(String MaterialType);
        MasterCostCenter GetCostCenter(MasterCostCenterAccountInput input);
        List<MasterUomDTO> GetUomByMaterial(String MaterialType);
        String GetFABrandByCode(String Code);
        List<TransportOrderRequestDTO> SaveRequestUnit(List<TransportOrderRequestInput> input, string userid);
        TransportOrder setEntityTO(TransportOrderRequestInput reqUnit, int idReq, DateTime promiseDate, string userid, string defaultseq, string STO, bool status);
        TransportOrderDetail setEntityTODetail(TransportOrderRequestInput reqUnit, int idTO, string userid);
        bool DeleteRoute(List<int> idTO);        
        List<TransportOrderDTO> GetDataDtos(string transNo, string roleName);
        List<TransportOrderDTO> GetDataDtos(string transNo, string[] senderLocation, string[] receiverLocation, DateTime? transportStartDate, DateTime? transportEndDate, string roleName);
        List<LoadingUnloadingExcelViewDTO> GetDataExcelDtos(string transNo, string[] senderLocation, string[] receiverLocation, DateTime? transportStartDate, DateTime? transportEndDate);

        //List<LoadingUnloadingExcelViewDTO> GetDataExcelDtos(string transportNo, string senderLocation, string receiverLocation, string transportDate);
        string UploadData(List<TransportOrderInput> input, List<string> OrderNumbers, string mode);

        TransportOrderDTO EditData(TransportOrderDTO input);
        void UpdateOrderStatus(TransportOrderRequestInput input, string userid);

        bool DeleteUnit(int IDTransportOrderRequest);
        bool DeleteUnit(IEnumerable<int> IDTransportOrderRequest);
        bool DeleteUnit(string ReqNo);
        bool DeleteUnit(IEnumerable<string> ReqNo);
        
        bool SendMail(string subject, string body, string[] recipients, string format = "HTML");
        List<TransportOrderDTO> GetLocatioByTn(string transportno);
        //List<TransportOrderDTO> GetLocatioByTn(string transportno, string param);

        List<TransportExecutionDTO> GetTransportationNumberFilterByRegion(string stono, bool isUserRoleTransport,List<string> parentLocation);
        List<TransportOrderDTO> GetTransportOrderByTransactionNo(string id,string state);
        List<TransportOrderDTO> GetTransportOrderByIDTransportExecution(int id);
        List<TransportOrderDTO> GetSenderLocationByTn();
        List<TransportOrderDTO> GetReceiverLocationByTn();
        IEnumerable<DateTime> GetTransportDate();

        string BaseUri { get; set; }
    }
}
