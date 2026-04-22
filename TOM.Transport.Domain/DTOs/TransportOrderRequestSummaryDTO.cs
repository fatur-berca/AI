using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportOrderRequestSummaryDTO
    {
        public int IDRequest { get; set; }
        public string RequestNo { get; set; }
        public string Zone { get; set; }
        public DateTime ShipmentDate { get; set; }
        public bool IsActive { get; set; }
        public int IDTransportOrder { get; set; }
        public int IDTransportOrderDetail { get; set; }
        public string SenderIDLocation { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string OrderStatus { get; set; }
        public string OrderType { get; set; }
        public string VehicleType { get; set; }
        public string STONo { get; set; }
        public string MaterialType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool OrderIsActive { get; set; }

        public int TotalUnit { get; set; }
        public DateTime RequestDate { get { return CreatedDate; } }
        public string OrderCreator { get { return CreatedBy; } }
        public bool InProcess { get { return OrderStatus != "Draft"; } }
    }
}
