using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Universal.Domain.DTOs
{
    public class MasterIMDLRoleLocationDTO
    {
        public int IDIMDLRoleLocation { get; set; }
        public string IMDLRole { get; set; }
        public string IDLocation { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string LocationName { get; set; }
        public List<String> ListIDLocation { get; set; }
        public virtual MasterLocationDTO MasterLocation { get; set; }
        public virtual MasterIMDLRoleDTO MasterIMDLRole { get; set; }

    }
}
