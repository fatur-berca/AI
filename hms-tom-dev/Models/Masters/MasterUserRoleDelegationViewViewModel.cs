using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterUserRoleDelegationViewViewModel : ViewModelBase
    {
        public int IDUserDelegation { get; set; }
        public string IDUserTo { get; set; }
        public string UserToName { get; set; }
        public string IDUserFrom { get; set; }
        public string UserFromName { get; set; }
        public int DelegationIDRole { get; set; }
        public string RoleName { get; set; }
        public System.DateTime EffectiveStartDate { get; set; }
        public System.DateTime EffectiveEndDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
    }
}