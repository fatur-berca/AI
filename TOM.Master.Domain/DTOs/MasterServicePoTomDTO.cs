using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.Domain.DTOs
{
    public class MasterServicePoTomDTO
    {
        public int IDServicePO { get; set; }
        public int IDVendor { get; set; }
        public string ServicePONumber { get; set; }
        public System.DateTime EffectiveStartDate { get; set; }
        public System.DateTime EffectiveEndDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }

        public virtual MasterVendorTOMDTO MasterTransportVendor { get; set; }
    }
}
