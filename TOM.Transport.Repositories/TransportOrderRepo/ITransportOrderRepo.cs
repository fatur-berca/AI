using System;
using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Repositories
{
    public interface ITransportOrderRepo : IGenericRepository<TransportOrder>
    {
        string getNewSeqNo(DateTime shipmentDate);
        List<TransportOrderDTO> GetTOActiveByTN(string transNo, string roleName);
        List<TransportOrderDTO> GetTOActiveByTNs(string transNo, string senderLocation, string receiverLocation, string transportDate);
        List<TransportOrderDTO> GetTOActiveByTNs(string transNo, string[] senderLocation, string[] receiverLocation, string transportDate);
        List<TransportOrderDTO> GetTOActiveByTNs(string transNo, string[] senderLocation, string[] receiverLocation, DateTime? transportStartDate, DateTime? transportEndDate, string roleName);
        List<TransportOrder> GetSTONoFilter(string stono);
        List<TransportOrder> GetSTONoFilterByListRegion(string stono, List<string> region);
        List<TransportOrderDTO> GetAllTransportOrder(TransportOrderInput criteria);
        int updateOrderFromShip(TransportMonitoringViewInput input, string userid);
        List<string> GetAllUploadedBy(TransportOrderInput criteria);
        TransportOrderDTO GetTransportOrderBySTONo(string stono);
        TransportOrder GetTransportOrderBySTONo2(string stono);
        TransportOrderDTO GetTransportOrderActiveBySTONo(string stono);
        List<TransportExecution> GetTransportationNumberFilterByListRegion(string transno, List<string> region);
        //cek fungsi ini
        int SaveData(TransportOrder input);
        int InsertData(TransportOrder input);
        int SaveDataTOFromUpload(TransportOrder input);
        int getLastSequenceByShipmentDate(DateTime shipmentDate);
        string getSeqUnitByShipmentDate(DateTime shipmentDate);
        string getSequenceNo(DateTime shipmentDate, int RequestID = 0);
        string getLastSTOPreOrder(DateTime shipmentDate);
        string getLastSTONo(DateTime shipmentDate, string minSTO = null);
        TransportOrder GetTransportOrderByID(int idTO);
        void DeleteDataList(List<int> idTO);
        void DeleteData(int idTO);
        //List<TransportOrderTransportExecutionPrintDTO> GetTransportOrderFilterByidTE(List<int> idTE);
        List<TransportOrderTransportExecutionPrintDTO> GetTransportOrderFilterByidTE(int idTE);
        List<TransportOrderTransportExecutionPrintDTO> GetTransportOrderFilterByTESenderReceiver(int idTE, string sender, string receiver);
        //List<int> GetTransportOrderFilterBySenderReceiverTE(int idTE, string sender, string receiver);
        List<TransportOrder> GetTransportOrderFilterByTE(int idTE);        
        List<TransportOrder> GetIdTEFilterByTO(TransportOrderInput input);
        List<SP_GetTransportOrderRequestDTO> GetTransportOrderRequestSummary(string oNFilter = "", string zoneFilter = "", string oTFilter = "", string oVFilter = "", Nullable<System.DateTime> shipDateBeginFilter = null, Nullable<System.DateTime> shipDateEndFilter = null, string mTFilter = "", string statusFilter = "", string senderFilter = "", string receiverFilter = "", string creatorFilter = "", string locationFilter = "", bool allaccess = false);
        List<TransportPickingListExportDTO> GetRawDataTODPL(TransportOrderInput input);
        List<TransportOrder> GetTransportOrderByidTE(int idTE);
        bool SendMail(string subject, string body, IEnumerable<string> recipients, string format = "HTML", bool batch = false);
        bool CreateNewNotification(string pageName, string description, string toUser, string creatorUser, bool isSeen = false, bool isRead = false);
        bool CreateNewNotification(string pageName, string description, string[] toUser, string creatorUser, bool isSeen = false, bool isRead = false);
        IEnumerable<DateTime> GetTransportDateEnumerable();
    }
}
