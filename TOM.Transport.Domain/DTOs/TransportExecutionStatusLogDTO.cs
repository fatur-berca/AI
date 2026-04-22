using System;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportExecutionStatusLogDTO
    {
        public int IDTransportExecutionStatusLog { get; set; }
        public int IDTransportExcecution { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
