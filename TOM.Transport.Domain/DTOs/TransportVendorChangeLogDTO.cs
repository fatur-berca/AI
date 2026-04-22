using System;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportVendorChangeLogDTO
    {
        public int IDVendorChangeLog { get; set; }
        public int IDTransportExecution { get; set; }
        public DateTime TransportDate { get; set; }
        public int IDVendor { get; set; }
        public string SIType { get; set; }
        public string SIStatus { get; set; }
        public DateTime? TargetOfArrival { get; set; }
        public DateTime? ActualArrive { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
