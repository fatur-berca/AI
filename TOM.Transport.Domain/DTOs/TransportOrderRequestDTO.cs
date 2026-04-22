using System;
using System.Collections.Generic;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportOrderRequestDTO
    {
        public int IDRequest { get; set; }
        public string RequestNo { get; set; }
        public string VehicleType { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string Zone { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<TransportOrderDTO> TransportOrders { get; set; }

        public int TotalUnit { get; set; }
        public int IDTransportOrder { get; set; }
        public string STONo { get; set; }
        public string DefaultSeqNo { get; set; }
        public string SeqNo { get; set; }
        public int? IDCostCenter { get; set; }

        public object Tag { get; set; }
        public object _OldRecord { get; set; }
        
    }
}
