using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Universal.Domain.DTOs
{
    public class MasterUserRoleDelegationViewDTO
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
        public List<UserLocationDelegationModelDTO> UserLocationDelegationModel { get; set; }
    }

    public class UserLocationDelegationModelDTO
    {
        public int IDUserLocationDelegation { get; set; }
        public int IDUserDelegation { get; set; }
        public string DelegationIDLocation { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual MasterLocationDTO MasterLocation { get; set; }
    }
}
