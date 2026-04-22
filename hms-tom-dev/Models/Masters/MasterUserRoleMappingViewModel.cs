using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterUserRoleMappingViewModel : ViewModelBase
    {
        public int IDUserRoleMapping { get; set; }
        public int IDRole { get; set; }
        public string IDUser { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public string Name { get; set; }
        public string RoleName { get; set; }
      
    }
}