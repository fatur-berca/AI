using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hms_tom_dev.Models.Common;
using TOM.EntitiesDAL.EDMX;

namespace hms_tom_dev.Models.Masters
{
    public class MasterLeadTimeTomViewModel : ViewModelBase
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

        public virtual MasterLocation MasterLocation { get; set; }
        public virtual MasterLocation MasterLocation1 { get; set; }
        public virtual MasterLocation MasterLocation2 { get; set; }
        public virtual MasterVendor MasterTransportVendor { get; set; }
    }
}