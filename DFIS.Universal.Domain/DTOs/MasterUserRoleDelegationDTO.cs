using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Universal.Domain.DTOs
{
    public class MasterUserRoleDelegationDTO
    {
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
        public string CurrentUser { get; set; }
        //public MasterRoleModel MasterRoleModel { get; set; }
        //public MasterUserModel MasterUserModel1 { get; set; }
        //public MasterUserModel MasterUserModel2 { get; set; }
        //public List<UserLocationDelegationModel> UserLocationDelegationModel { get; set; }
    }
    //public class MasterRoleModel
    //{
    //    public int IDRole { get; set; }
    //    public string RoleName { get; set; }
    //    public bool IsActive { get; set; }
    //    public string CreatedBy { get; set; }
    //    public System.DateTime CreatedDate { get; set; }
    //    public string UpdatedBy { get; set; }
    //    public System.DateTime UpdatedDate { get; set; }
    //    public string Remarks { get; set; }
    //}
    //public class MasterUserModel
    //{
    //    public string IDUser { get; set; }
    //    public string FullName { get; set; }
    //    public string Email { get; set; }
    //    public string Address { get; set; }
    //    public string Phone { get; set; }
    //    public bool IsActive { get; set; }
    //    public string CreatedBy { get; set; }
    //    public System.DateTime CreatedDate { get; set; }
    //    public string UpdatedBy { get; set; }
    //    public System.DateTime UpdatedDate { get; set; }
    //    public string Remarks { get; set; }
    //}
    //public class UserLocationDelegationModel
    //{
    //    public int IDUserLocationDelegation { get; set; }
    //    public int IDUserDelegation { get; set; }
    //    public string DelegationIDLocation { get; set; }
    //    public string CreatedBy { get; set; }
    //    public System.DateTime CreatedDate { get; set; }
    //    public string UpdatedBy { get; set; }
    //    public System.DateTime UpdatedDate { get; set; }
    //}
}
