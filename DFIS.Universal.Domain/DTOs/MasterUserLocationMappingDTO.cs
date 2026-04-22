using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Universal.Domain.DTOs
{
    public class MasterUserLocationMappingDTO
    {
        public int IDUserLocationMapping { get; set; }
        public string IDUser { get; set; }
        public string IDLocation { get; set; }
        public string LocationName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        public List<String> ListIDLocation { get; set; }
        public virtual MasterLocationDTO MasterLocation { get; set; }
        public string MasterRole { get; set; }
        public virtual MasterUserDTO MasterUser { get; set; }

        public string roleName { get; set; }
        public string functionName { get; set; }
        public string fullName { get; set; }
    }
}
