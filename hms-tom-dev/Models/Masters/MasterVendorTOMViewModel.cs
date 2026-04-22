using System;
using System.Collections.Generic;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterVendorTOMViewModel : ViewModelBase
    {
        public int IDVendor { get; set; }
        public string VendorName { get; set; }
        public Nullable<int> ParentVendor { get; set; }
        public string VendorAddress { get; set; }
        public string VendorRegion { get; set; }
        public string VendorEmail { get; set; }
        public string CC { get; set; }
        public string VendorCategory { get; set; }
        public string TransportationMode { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public string VendorProvince { get; set; }
        public string VendorCity { get; set; }

        //public virtual ICollection<MasterVendorTOMViewModel> MasterVendorTOM1 { get; set; }
        public virtual MasterVendorTOMViewModel MasterTransportVendor { get; set; }
    }
}