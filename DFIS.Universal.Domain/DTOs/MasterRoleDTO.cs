using System;

namespace DFIS.Universal.Domain.DTOs
{
    public class MasterRoleDTO
    {
        public int IDRole { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    }
}
