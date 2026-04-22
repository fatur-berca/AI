using hms_tom_dev.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TOM.Transport.Domain.DTOs;

namespace hms_tom_dev.Models.Transport
{
    public class TransportSummaryViewModel : ViewModelBase
    {
        public string TransportNo { get; set; }
        public string STONo { get; set; }
        public string TransportCategory { get; set; }
        public string TransportMode { get; set; }
        public string VendorName { get; set; }
        public string VehicleType { get; set; }
        public string OrderCategory { get; set; }
        public string ZoneName { get; set; }
        public string StartLocation { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

        public virtual List<SelectListItem> transNumbers { get; set; }
        public virtual List<SelectListItem> orderNumbers { get; set; }
        public virtual List<SelectListItem> transCategorys { get; set; }
        public virtual List<SelectListItem> transModes { get; set; }
        public virtual List<SelectListItem> vendorNames { get; set; }
        public virtual List<SelectListItem> vehicleTypes { get; set; }
        public virtual List<SelectListItem> orderCategorys { get; set; }
        public virtual List<SelectListItem> zoneNames { get; set; }
        public virtual List<SelectListItem> startLocations { get; set; }
        public virtual List<SelectListItem> senderNames { get; set; }
        public virtual List<SelectListItem> receiverNames { get; set; }
    }
}