using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterFGStackingViewModel : ViewModelBase
    {
        public int IDFGStacking { get; set; }
        public string IDLocation { get; set; }
        public string Brand { get; set; }
        public int MaxStacking { get; set; }
        public int MinStacking { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> DoubleStacking { get; set; }
    }
}