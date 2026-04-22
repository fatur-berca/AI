
using DFIS.Universal.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.Master.Domain.DTOs;

namespace TOM.Master.Domain.Inputs
{
    public class MasterLeadTimeTomInput : BaseInput
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

        public virtual MasterVendorDTO MasterTransportVendor { get; set; }

        public string filterDate { get; set; }
        public List<string> filterSenderLocation { get; set; }
    }
}
