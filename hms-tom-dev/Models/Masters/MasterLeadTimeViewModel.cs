using System;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterLeadTimeViewModel : ViewModelBase
    {
        public int IDLeadTime { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public Nullable<decimal> LeadTime { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        public virtual MasterLocationViewModel MasterLocation { get; set; }
        public virtual MasterLocationViewModel MasterLocation1 { get; set; }
    }
}