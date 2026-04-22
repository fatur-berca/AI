using DFIS.Universal.Domain.Inputs;
using DFIS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Inputs
{
    public class TransportMonitoringInput : UtilBaseInput
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

        #region advancesearchinput
        public string IDVendor { get; set; }
        public string TransportationNo { get; set; }
        public string OrderNumber { get; set; }
        public string StartLocation { get; set; }
        public string TransportationStatus { get; set; }
        public string SenderLocation { get; set; }
        public string ReceiverLocation { get; set; }
        public bool IsExportOrSearch { get; set; }
        public string DateOfStuffingFrom { get; set; }
        public string DateOfStuffingTo { get; set; }
        public bool DelayShipment { get; set; }
        #endregion
        public virtual TransportExecutionDTO TransportExecution { get; set; }
    }

    public class TransportOrderMonitoringInput
    {
        public string CargoReceived { get; set; }
        public string OrderStatus { get; set; }
        public string OrderType { get; set; }
        public string OrderNumber { get; set; }
        public int? ID { get; set; }
        public DateTime? GI { get; set; }
        public DateTime? GR { get; set; }
    }

    public class TransportOrderPositionInput
    {
        public string Location { get; set; }
        public DateTime? Date { get; set; }

        public int? ID { get; set; }
    }

    public class TransportMonitoringFilterInput
    {
        public List<string> transportNo { get; set; }
        public List<string> transportStatus { get; set; }
        public List<string> vendor { get; set; }
        public List<string> orderNo { get; set; }
        public List<string> sender { get; set; }
        public List<string> receiver { get; set; }
        public List<string> startLocation { get; set; }
        public List<string> finishLocation { get; set; }
        public string mode { get; set; }
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
        public bool? delayShipment { get; set; }
        public bool? includeInactive { get; set; }
        public bool isForExport { get; set; }
    }
}
