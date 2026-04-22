using System;
using System.Collections.Generic;

namespace TOM.Master.Domain.DTOs
{
    public class MasterVendorTOMDTO
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

        public virtual ICollection<MasterVendorTOMDTO> MasterVendorTOM1 { get; set; }
        public virtual MasterVendorTOMDTO MasterVendorTOM2 { get; set; }
    }
}
