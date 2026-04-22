using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Universal.Domain.DTOs
{
    public class MasterIMDLRoleDTO
    {
        public string IMDLRole { get; set; }
        public int IDRole { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string RoleName { get; set; }
        public virtual MasterRoleDTO MasterRole { get; set; }
    }
}
