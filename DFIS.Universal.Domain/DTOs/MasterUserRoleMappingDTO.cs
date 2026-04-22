
namespace DFIS.Universal.Domain.DTOs
{
    public class MasterUserRoleMappingDTO
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
        public virtual MasterRoleDTO MasterRole { get; set; }
        public virtual MasterUserDTO MasterUser { get; set; }
    }
}
