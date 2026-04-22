using System;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportPickingListLogDTO
    {
        public int IDTransportPickingListLog { get; set; }
        public DateTime ShipmentDate { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
