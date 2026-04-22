using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterUserRoleDelegationViewModel : ViewModelBase
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
        public MasterRoleModel MasterRole { get; set; }
        public MasterUserModel MasterUser1 { get; set; }
        public MasterUserModel MasterUser2 { get; set; }
        public List<UserLocationDelegationModel> UserLocationDelegation { get; set; }
    }
    public class MasterRoleModel
    {
        public int IDRole { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    }
    public class MasterUserModel
    {
        public string IDUser { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    }
    public class UserLocationDelegationModel
    {
        public int IDUserLocationDelegation { get; set; }
        public int IDUserDelegation { get; set; }
        public string DelegationIDLocation { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
    }
}