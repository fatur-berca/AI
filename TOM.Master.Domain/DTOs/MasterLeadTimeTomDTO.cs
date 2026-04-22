using TOM.Master.Domain.DTOs;
using DFIS.Universal.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.Domain.DTOs
{
    public class MasterLeadTimeTomDTO
    {
        public int IDLeadTime { get; set; }
        public int IDVendor { get; set; }
        public string SenderIDLocation { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string ThroughIDLocation { get; set; }
        public string Via { get; set; }
        public byte Time { get; set; }
        public System.DateTime EffectiveStartDate { get; set; }
        public System.DateTime EffectiveEndDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        // Additional For Import Excel
        public string VendorCategory { get; set; }
        public string VendorName { get; set; }

        public virtual MasterLocationDTO MasterLocation { get; set; }
        public virtual MasterLocationDTO MasterLocation1 { get; set; }
        public virtual MasterLocationDTO MasterLocation2 { get; set; }
        public virtual MasterVendorDTO MasterTransportVendor { get; set; }
    }
}
