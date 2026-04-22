using DFIS.Utils;

namespace TOM.Transport.Domain.Inputs
{
    public class TransportLostClaimDamageInput : UtilBaseInput
    {
        public string DateReceiptFrom { get; set; }
        public string DateReceiptTo { get; set; }
        public string OriginWarehouse { get; set; }
        public string DestinationWarehouse { get; set; }
        public string STONumber { get; set; }
        public string DeliveryNoteNumber { get; set; }
        public string PoliceRegNumber { get; set; }
        public string Vendor { get; set; }
        public bool IsActive { get; set; }
    }
}
