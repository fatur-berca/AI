using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hms_tom_dev.Models.Transport
{
    public class InitTransportLostClaimDamageModel
    {
        public IEnumerable<string> OriginWarehouse { get; set; }
        public IEnumerable<string> DestinationWarehouse { get; set; }
        public IEnumerable<string> STONumber { get; set; }
        public IEnumerable<string> DeliveryNoteNumber { get; set; }
        public IEnumerable<string> PoliceRegNumber { get; set; }
        public IEnumerable<string> Vendor { get; set; }
    }
}