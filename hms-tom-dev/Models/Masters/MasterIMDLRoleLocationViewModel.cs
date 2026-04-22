using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterIMDLRoleLocationViewModel : ViewModelBase
    {
        public int IDIMDLRoleLocation { get; set; }
        public string IMDLRole { get; set; }
        public string IDLocation { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public List<string> ListIDLocation { get; set; }
        public virtual MasterLocationViewModel MasterLocation { get; set; }
    }
}