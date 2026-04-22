using System;

namespace hms_tom_dev.Models.Transport
{
    public class TransportExecutionAddNewViewModel
    {
        public int? IDCheck { get; set; }
        public int? IDTransportOrder { get; set; }
        public int? IDTransportOrderDetail { get; set; }
        public string ConcatIDTransportOrder { get; set; }
        public string OrderNumber { get; set; }
        public string OrderType { get; set; }
        public string IDSenderLoc { get; set; }
        public string Sender { get; set; }
        public string IDReceiverLoc { get; set; }
        public string Receiver { get; set; }
        public string OrderedVehicleType { get; set; }
        public string CostCenter { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string MaterialType { get; set; }
        public string Description { get; set; }
        public decimal? Qty { get; set; }
        public string UoM { get; set; }
        public string OrderRemarks { get; set; }
        public string Sequence { get; set; }
        public string OrderCreator { get; set; }
        public string TransportationNumber { get; set; }
        public int? IDVendor { get; set; }
        public string VendorName { get; set; }
        public int? IDVendorSuggestion { get; set; }
        public string VendorSuggestion { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string TNCreator { get; set; }
    }
}