using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterVendorViewModel : ViewModelBase
    {
        public string IDVendor { get; set; }
        public string VendorName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    }
}
