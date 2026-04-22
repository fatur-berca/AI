using DFIS.Universal.Domain.DTOs;
using System;
using TOM.EntitiesDAL.EDMX;


namespace TOM.Transport.Domain.DTOs
{
    public class TransportPickingListViewDTO
    {
        public string STONo { get; set; }
        public string Supplier { get; set; }
        public string Item { get; set; }
        public string Brand { get; set; }
        public Nullable<long> Qty { get; set; }
        public string UoM { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public System.DateTime ShipmentDate { get; set; }
        public string Sequence { get; set; }
        public string SeqNo { get; set; }
        public string VehicleType { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public string ZoneBased { get; set; }
        public string SenderIDLocation { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string CreatedByID { get; set; }
        public string TransportNo { get; set; }
        public Boolean IsActive { get; set; }
        public Int32 IDTransportOrder { get; set; }
        public Int32 IDTransportOrderDetail { get; set; }
    }
}
