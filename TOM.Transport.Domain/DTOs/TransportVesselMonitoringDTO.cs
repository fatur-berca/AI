using System;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportVesselMonitoringDTO
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

        public string StartLocationName { get; set; }
        public string FinishLocationName { get; set; }

        public virtual TransportExecutionDTO TransportExecution { get; set; }
    }

    public class TransportMonitoringExportShipDTO
    {
        public string TransportationNumber { get; set; }
        public string STONumber { get; set; }
        public DateTime? StuffingDate { get; set; }
        public string StartLocation { get; set; }
        public string FinishLocation { get; set; }
        public string VendorName { get; set; }
        public string VesselName { get; set; }
        public DateTime? ETD1 { get; set; }
        public DateTime? ETD2 { get; set; }
        public DateTime? ATD { get; set; }
        public DateTime? EstimateReceived { get; set; }
        public string Remarks { get; set; }
        public string TransportationStatus { get; set; }
        public DateTime? ETA1 { get; set; }
        public DateTime? ETA2 { get; set; }
        public DateTime? ATA { get; set; }
        public DateTime? ActualTimeBerthing { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerSeal { get; set; }
        public string OrderStatus { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public DateTime? GIDate { get; set; }
        public DateTime? GRDate { get; set; }
        public string GRBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? POWeekAgent { get; set; }
    }
}
