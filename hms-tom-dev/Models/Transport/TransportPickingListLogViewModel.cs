using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hms_tom_dev.Models.Transport
{
    public class TransportPickingListLogViewModel
    {
        public int IDTransportPickingListLog { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public System.DateTime ShipmentDate { get; set; }
    }

    public class TransportPickingListExport
    {
        public int Year { get; set; }
        public string Columns { get; set; }
    }
}