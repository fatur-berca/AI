using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Universal.Domain.DTOs
{
    public class UserLocationDelegationDTO
    {
        public int IDUserLocationDelegation { get; set; }
        public int IDUserDelegation { get; set; }
        public string DelegationIDLocation { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }

        public virtual MasterLocationDTO MasterLocation { get; set; }
        public virtual MasterUserRoleDelegationDTO MasterUserRoleDelegation { get; set; }
    }
}
