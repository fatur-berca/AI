using System;
using AutoMapper;
using DFIS.Universal.Domain.DTOs;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportOrderTransportExecutionDTO
    {
        public TransportOrderTransportExecutionDTO()
        {}

        public TransportOrderTransportExecutionDTO(TransportOrder input, MasterLocation senderLocation, MasterLocation receiverLocation, MasterLocation actualSenderLoc, MasterLocation actualReceiverLoc, TransportExecution transexe)
        {
            IDTransportOrder = input.IDTransportOrder;
            IDRequest = input.IDRequest;
            IDTransportExecution = input.IDTransportExecution;
            IDTransportPickingListLog = input.IDTransportPickingListLog;
            STONo = input.STONo;
            DefaultSeqNo = input.DefaultSeqNo;
            SeqNo = input.SeqNo;
            ShipmentDate = input.ShipmentDate;
            KM = input.KM;
            SenderIDLocation = input.SenderIDLocation;
            ActualSenderIDLocation = input.ActualSenderIDLocation;
            ReceiverIDLocation = input.ReceiverIDLocation;
            ActualReceiverIDLocation = input.ActualReceiverIDLocation;
            LeadTime = input.LeadTime;
            VehicleType = input.VehicleType;
            OrderType = input.OrderType;
            CostCenter = input.CostCenter;
            OrderStatus = input.OrderStatus;
            MaterialReceivedTime = input.MaterialReceivedTime;
            IsMaterialReceived = input.IsMaterialReceived;
            EstArrivalDate = input.EstArrivalDate;
            GRDate = input.GRDate;
            LoadStartTime = input.LoadStartTime;
            LoadFinishTime = input.LoadFinishTime;
            UnloadStartTime = input.UnloadStartTime;
            UnloadFinishTime = input.UnloadFinishTime;
            Remarks = input.Remarks;
            FlagEmail = input.FlagEmail;
            IDLeadTime = input.IDLeadTime;
            IDCostCenter = input.IDCostCenter;
            ChangeLogFields = input.ChangeLogFields;
            IsActive = input.IsActive;
            CreatedBy = input.CreatedBy;
            CreatedDate = input.CreatedDate;
            UpdatedBy = input.UpdatedBy;
            UpdatedDate = input.UpdatedDate;
            ZoneBased = input.ZoneBased;
            OrderCategory = input.OrderCategory;
            MasterLocation = Mapper.Map<MasterLocation, MasterLocationDTO>(senderLocation);
            MasterLocation1 = Mapper.Map<MasterLocation, MasterLocationDTO>(receiverLocation);
            MasterLocation2 = Mapper.Map<MasterLocation, MasterLocationDTO>(actualSenderLoc);
            MasterLocation3 = Mapper.Map<MasterLocation, MasterLocationDTO>(actualReceiverLoc);
            TransportExecution = Mapper.Map<TransportExecution, TransportExecutionDTO> (transexe);
            //MasterVendor = Mapper.Map<MasterVendor, MasterVendorDTO>(transexe.IDVendor);
        }

        public int IDTransportOrder { get; set; }
        public int? IDRequest { get; set; }
        public int? IDTransportExecution { get; set; }
        public int? IDTransportPickingListLog { get; set; }
        public string STONo { get; set; }
        public string Supplier { get; set; }
        public string SupplierColor { get; set; }
        public string DefaultSeqNo { get; set; }
        public string SeqNo { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string ShipmentDateColor { get; set; }
        public decimal? KM { get; set; }
        public string SenderIDLocation { get; set; }
        public string SenderLocationColor { get; set; }
        public string SenderLocationName { get; set; }
        public string ActualSenderIDLocation { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string ReceiverLocationColor { get; set; }
        public string ReceiverLocationName { get; set; }
        public string ActualReceiverIDLocation { get; set; }
        public byte? LeadTime { get; set; }
        public string VehicleType { get; set; }
        public string OrderType { get; set; }
        public string CostCenter { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? MaterialReceivedTime { get; set; }
        public bool? IsMaterialReceived { get; set; }
        public DateTime? EstArrivalDate { get; set; }
        public DateTime? GRDate { get; set; }
        public DateTime? LoadStartTime { get; set; }
        public DateTime? LoadFinishTime { get; set; }
        public DateTime? UnloadStartTime { get; set; }
        public DateTime? UnloadFinishTime { get; set; }
        public string Remarks { get; set; }
        public byte? FlagEmail { get; set; }
        public int? IDLeadTime { get; set; }
        public int? IDCostCenter { get; set; }
        public string ChangeLogFields { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ZoneBased { get; set; }
        public string TransportNo { get; set; }
        public string OrderCategory { get; set; }
        public DateTime TransportDate { get; set; }
        public string ActualVehicleType { get; set; }
        public string PoliceRegNo { get; set; }
        public int? IDVendor { get; set; }
        public string IDDriver1 { get; set; }
        public string IDDriver2 { get; set; }
        public string IDCoDriver { get; set; }
        public string StartLocation { get; set; }
        public string VendorName { get; set; }

        public int TotalPage { get; set; }        

        public virtual MasterLocationDTO MasterLocation { get; set; }
        public virtual MasterLocationDTO MasterLocation1 { get; set; }
        public virtual MasterLocationDTO MasterLocation2 { get; set; }
        public virtual MasterLocationDTO MasterLocation3 { get; set; }
        public virtual TransportExecutionDTO TransportExecution { get; set; }
        public virtual MasterVendorDTO MasterVendor { get; set; }
    }
}
