using DFIS.Universal.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Universal.Domain.Inputs
{
    public class MasterUserRoleDelegationInput : BaseInput
    {
        //public MasterUserRoleDelegation()
        //{
        //    this.UserLocationDelegations = new HashSet<UserLocationDelegationInput>();
        //}
    
        public int IDUserDelegation { get; set; }
        public string IDUserFrom { get; set; }
        public string IDUserTo { get; set; }
        public Nullable<int> DelegationIDRole { get; set; }
        public System.DateTime EffectiveStartDate { get; set; }
        public System.DateTime EffectiveEndDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string DelegationIDLocation { get; set; }
    
        public virtual MasterRoleDTO MasterRole { get; set; }
        public virtual MasterUserDTO MasterUser { get; set; }
        public virtual MasterUserDTO MasterUser1 { get; set; }
        public virtual ICollection<UserLocationDelegationModelDTO> UserLocationDelegations { get; set; }
    }
}
