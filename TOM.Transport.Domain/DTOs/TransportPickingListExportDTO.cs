using DFIS.Universal.Domain.DTOs;
using System;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportPickingListExportDTO
    {        

        public TransportPickingListExportDTO()
        { }

        public TransportPickingListExportDTO(TransportOrder to, TransportOrderDetail tod, MasterLocation ml1, MasterLocation ml2, MasterLocation ml3, MasterLocation ml4)
        {
            IDTransportExecution = to.IDTransportExecution;
            STONo = to.STONo;
            Supplier = tod.Supplier;
            Item = tod.Code;
            Brand = tod.Description;
            Qty = tod.Qty;
            UoM = tod.UoM;
            ShipmentDate = to.ShipmentDate;
            SenderIDLocation = to.SenderIDLocation;
            SenderLocationName = ml1.LocationName;
            ActualSenderIDLocation = to.SenderIDLocation;
            ActualSenderName = "";
            //ActualSenderName = ml3.LocationName;
            ReceiverIDLocation = to.ReceiverIDLocation;
            ReceiverLocationName = ml2.LocationName;
            ActualReceiverIDLocation = to.ActualReceiverIDLocation;
            ActualReceiverName = "";
            //ActualReceiverName = ml4.LocationName;
            DefaultSeqNo = to.DefaultSeqNo;
            SeqNo = to.SeqNo == null ? to.DefaultSeqNo : to.SeqNo;
            VehicleType = to.VehicleType;
            Remarks = to.Remarks;
            CreatedBy = to.CreatedBy;
            UpdatedBy = to.UpdatedBy;
        }

        public string STONo { get; set; }
        public string Supplier { get; set; }
        public string Item { get; set; }
        public string Brand { get; set; }
        public decimal? Qty { get; set; }
        public string UoM { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string SenderIDLocation { get; set; }
        public string SenderLocationName { get; set; }
        public string ActualSenderIDLocation { get; set; }
        public string ActualSenderName { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string ReceiverLocationName { get; set; }
        public string ActualReceiverIDLocation { get; set; }
        public string ActualReceiverName { get; set; }
        public string DefaultSeqNo { get; set; }
        public string VehicleType { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }        
        public string UpdatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public string UpdatedByFullName { get; set; }
        public int? IDTransportExecution { get; set; }
        public string TN { get; set; }
        public string SeqNo { get; set; }

        public virtual TransportOrderDetail TransportOrderDetail { get; set; }
        public virtual MasterLocationDTO MasterLocation { get; set; }
        public virtual MasterLocationDTO MasterLocation1 { get; set; }
        public virtual MasterLocationDTO MasterLocation2 { get; set; }
        public virtual MasterLocationDTO MasterLocation3 { get; set; }
    }
}
