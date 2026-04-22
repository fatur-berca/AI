using System;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportPositionDetailDTO
    {
        public int IDTransportPositionDetail { get; set; }
        public int IDTransportExecution { get; set; }
        public DateTime PositionDate { get; set; }
        public string PositionName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
