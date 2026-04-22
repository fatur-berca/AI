using System;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Transport
{
    public class TransportVesselMonitoringViewModel : ViewModelBase
    {
        public int IDTransportVesselMonitoring { get; set; }
        public int IDTransportExecution { get; set; }
        public string VesselName { get; set; }
        public DateTime? ETD1 { get; set; }
        public DateTime? ETD2 { get; set; }
        public DateTime? ATD { get; set; }
        public DateTime? ETA1 { get; set; }
        public DateTime? ETA2 { get; set; }
        public DateTime? ATA { get; set; }
        public DateTime? ActualTimeBerthing { get; set; }
        public DateTime? EstReceived { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerSeal { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        public virtual TransportExecutionViewModel TransportExecution { get; set; }
    }
}