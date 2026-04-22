using hms_tom_dev.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hms_tom_dev.Models.Transport
{
    public class TransportMonitoringViewViewModel : ViewModelBase
    {
        public int IDTransportVesselMonitoring { get; set; }
        public int IDTransportExecution { get; set; }
        public int IDTransportOrder { get; set; }
        public string TransportNo { get; set; }
        public System.DateTime TransportDate { get; set; }
        public string StartLocation { get; set; }
        public Nullable<int> IDVendor { get; set; }
        public string VendorName { get; set; }
        public Nullable<System.DateTime> ETD1 { get; set; }
        public Nullable<System.DateTime> ETD2 { get; set; }
        public Nullable<System.DateTime> ETA1 { get; set; }
        public Nullable<System.DateTime> ETA2 { get; set; }
        public Nullable<System.DateTime> ATD { get; set; }
        public Nullable<System.DateTime> ATA { get; set; }
        public Nullable<System.DateTime> EstReceived { get; set; }
        public Nullable<System.DateTime> ActualTimeBerthing { get; set; }
        public string Remarks { get; set; }
        public string TransportStatus { get; set; }
        public string ActualVehicleType { get; set; }

        public string VesselName { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerSeal { get; set; }
        public bool IsActive { get; set; }
        public string GIDate { get; set; }
        public int CargoReceived { get; set; }
        public string STONo { get; set; }
        public string SenderIDLocation { get; set; }
        public string ReceiverIDLocation { get; set; }
        public Nullable<System.DateTime> GRDate { get; set; }
        public string OrderStatus { get; set; }
        public string VehicleType { get; set; }
        public bool DelayShipment { get; set; }
        public string DateOfStuffingFrom { get; set; }
        public string DateOfStuffingTo { get; set; }
        public string VendorID { get; set; }
    }
}