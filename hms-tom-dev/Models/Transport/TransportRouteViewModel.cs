using System;

namespace hms_tom_dev.Models.Transport
{
    public class TransportRouteViewModel
    {
        public int IDTransportRoute { get; set; }
        public int IDTransportExecution { get; set; }
        public string IDLocation { get; set; }
        public bool IsMain { get; set; }
        public bool IsAssigned { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool? IsNonKMBased { get; set; }
    }
}