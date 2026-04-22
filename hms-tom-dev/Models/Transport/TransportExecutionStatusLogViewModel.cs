using System;

namespace hms_tom_dev.Models.Transport
{
    public class TransportExecutionStatusLogViewModel
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