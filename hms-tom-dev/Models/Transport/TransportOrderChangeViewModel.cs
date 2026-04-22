using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hms_tom_dev.Models.Transport
{
    public class TransportOrderChangeViewModel
    {
        public int IDTransportOrderChangeLog { get; set; }
        public int TransportOrderID { get; set; }
        public int Version { get; set; }
        public string OldValue { get; set; }
        public string ModifiedField { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual TransportOrderViewModel TransportOrder { get; set; }
    }
}