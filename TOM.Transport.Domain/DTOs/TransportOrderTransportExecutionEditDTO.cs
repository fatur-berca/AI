using System;
using System.Collections.Generic;
using DFIS.Universal.Domain.DTOs;
using TOM.Master.Domain.DTOs;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportOrderTransportExecutionEditDTO
    {
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
        public string OrderCategory { get; set; }

        //digunakan untuk mengset apakah order finish good atau raw material
        public string FGRMT { get; set; }
        //digunakan untuk perhitungan total box kalau order type finish good
        public decimal TotalBox { get; set; }
        //digunakan di TE Detail untuk pop up notifikasi, ketika add TO, tapi TO nya sudah ada di TN yang lain
        public string TransportNo { get; set; }

        public string DisplayCostCenter
        {
            get
            {
                if (MasterCostCenter != null)
                {
                    return STONo + "-" + MasterCostCenter.CostCenter;
                }
                return "";
            }
        }

        public virtual MasterLocationDTO MasterLocation { get; set; }
        public virtual MasterLocationDTO MasterLocation1 { get; set; }
        public virtual MasterLocationDTO MasterLocation2 { get; set; }
        public virtual MasterLocationDTO MasterLocation3 { get; set; }
        public virtual MasterLocationDTO MasterLocation4 { get; set; }
        public virtual MasterLocationDTO MasterLocation5 { get; set; }
        public virtual List<TransportOrderDetailDTO> TransportOrderDetails { get; set; }
        public virtual MasterCostCenterAccountDTO MasterCostCenter { get; set; }
    }
}
