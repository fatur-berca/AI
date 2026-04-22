using System;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterZoneLocationViewModel : ViewModelBase
    {
        public int IDZoneLocation { get; set; }
        public string Zone { get; set; }
        public string IDLocation { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        public virtual MasterLocationViewModel MasterLocation { get; set; }
    }
}