using hms_tom_dev.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TOM.Transport.Domain.DTOs;

namespace hms_tom_dev.Models.Transport
{
    public class TransportMonitoringViewModel : ViewModelBase
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

        public string TransportNo { get; set; }
        public string TransportStatus { get; set; }
        public string VendorName { get; set; }
        public string LocationName { get; set; }
        public string STONo { get; set; }

        public string UserRole { get; set; }

        public virtual List<SelectListItem> transNumbers { get; set; }
        public virtual List<SelectListItem> transStatus { get; set; }
        public virtual List<SelectListItem> vendorNames { get; set; }
        public virtual List<SelectListItem> locationNames { get; set; }
        public virtual List<SelectListItem> orderNumbers { get; set; }

        public virtual TransportExecutionDTO TransportExecution { get; set; }
    }
}