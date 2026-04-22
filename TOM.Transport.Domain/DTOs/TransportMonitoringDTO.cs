using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportMonitoringDTO
    {
        public int IDTransportVesselMonitoring { get; set; }
        public int IDTransportExecution { get; set; }
        public string VesselName { get; set; }
        public Nullable<System.DateTime> ETD1 { get; set; }
        public Nullable<System.DateTime> ETD2 { get; set; }
        public Nullable<System.DateTime> ATD { get; set; }
        public Nullable<System.DateTime> ETA1 { get; set; }
        public Nullable<System.DateTime> ETA2 { get; set; }
        public Nullable<System.DateTime> ATA { get; set; }
        public Nullable<System.DateTime> ActualTimeBerthing { get; set; }
        public Nullable<System.DateTime> EstReceived { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerSeal { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public virtual TransportExecutionDTO TransportExecution { get; set; }
    }
}
