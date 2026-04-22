using DFIS.Universal.Domain.DTOs;
using System;
using System.Collections.Generic;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportOrderDTO
    {
        public TransportOrderDTO()
        {}

        public TransportOrderDTO(TransportOrder input, List<TransportOrderDetail> detail, TransportExecution execution, MasterLocation senderLocation, MasterLocation receiverLocation, MasterLocation actualSenderLoc, MasterLocation actualReceiverLoc)
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
            LoadBoxperWorkingTime = input.LoadBoxperWorkingTime;
            UnloadBoxperWorkingTime = input.UnloadBoxperWorkingTime;
            MasterLocation = Mapper.Map<MasterLocation, MasterLocationDTO>(senderLocation);
            MasterLocation1 = Mapper.Map<MasterLocation, MasterLocationDTO>(receiverLocation);
            MasterLocation2 = Mapper.Map<MasterLocation, MasterLocationDTO>(actualSenderLoc);
            MasterLocation3 = Mapper.Map<MasterLocation, MasterLocationDTO>(actualReceiverLoc);
            TransportOrderDetails = Mapper.Map<List<TransportOrderDetail>, List<TransportOrderDetailDTO>>(detail);
            TransportExecution = Mapper.Map<TransportExecution, TransportExecutionDTO>(execution);
        }

        public int IDTransportOrder { get; set; }
        public int? IDRequest { get; set; }
        public int? IDTransportExecution { get; set; }
        public int? IDTransportPickingListLog { get; set; }
        public string STONo { get; set; }
        //public string Supplier { get; set; }
        //public string SupplierColor { get; set; }
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
        public int? POWeek { get; set; }
        public DateTime? MaterialReceivedTime { get; set; }
        public bool? IsMaterialReceived { get; set; }
        public DateTime? EstArrivalDate { get; set; }
        public DateTime? GIDate { get; set; }
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
        public string OrderCategory { get; set; }
        //A New Role
        public string TransportMode { get; set; }

        public DateTime DocDate { get; set; }
        public string Material { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal? Qty { get; set; }
        public string UoM { get; set; }
        public string Supplier { get; set; }

        public decimal? PackPerBox { get; set; }

        public string LocNameSender { get; set; }
        public string LocNameReceiver { get; set; }
        public string ModifChangeLog { get; set; }
        public string ModifChangeLogD { get; set; }
        
        public virtual MasterLocationDTO MasterLocation { get; set; }
        public virtual MasterLocationDTO MasterLocation1 { get; set; }
        public virtual MasterLocationDTO MasterLocation2 { get; set; }
        public virtual MasterLocationDTO MasterLocation3 { get; set; }
        public virtual MasterLocationDTO MasterLocation4 { get; set; }
        public virtual MasterLocationDTO MasterLocation5 { get; set; }
        public virtual TransportExecutionDTO TransportExecution { get; set; }
        public virtual TransportPickingListLogDTO TransportPickingListLog { get; set; }
        public virtual TransportOrderRequestDTO TransportOrderRequest { get; set; }
        
        public virtual List<TransportOrderDetailDTO> TransportOrderDetails { get; set; }
        public virtual ICollection<TransportOrderChangeLogDTO> TransportOrderChangeLogs { get; set; }

        public virtual ICollection<MasterListDTO> MasterList { get; set; }

        public object Tag { get; set; }
        public object _OldRecord { get; set; }

        public bool Checked { get; set; }
        public string IdLocation { get; set; }
        public string LocationName { get; set; }
        public Nullable<decimal> LoadBoxperWorkingTime { get; set; }
        public Nullable<decimal> UnloadBoxperWorkingTime { get; set; }
        public int ExistingState { get; set; }

        public string GIBy { get; set; }
        public string GRBy { get; set; }

        public decimal TotalBox { get; set; }

        public bool StatusChanged { get; set; }
        public bool IsNewData { get; set; }

        public string CreatedByFullName { get; set; }
        public string UpdatedByFullName { get; set; }
        public string TransportNo { get; set; }
    }

    public class LoadingUnloadingHistoryDataGridDTO
    {
        public bool IsActive { get; set; }
        public string TransportNo { get; set; }
        public DateTime? TransportDate { get; set; }
        public string STONo { get; set; }
        public string SenderLocationName { get; set; }
        public string ReceiverLocationName { get; set; }
        public decimal? LoadBoxperWorkingTime { get; set; }
        public decimal? UnloadBoxperWorkingTime { get; set; }
    }
}
