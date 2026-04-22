using System;
using System.Collections.Generic;

namespace DFIS.Universal.Domain.DTOs
{
    public class MasterRoleFunctionDTO
    {
        public int IDRolesFunctionMapping { get; set; }
        public int IDRole { get; set; }
        public int IDFunction { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        public List<int> ListIDFunction { get; set; }

        public virtual MasterFunctionDTO MasterFunction { get; set; }
        public virtual MasterRoleDTO MasterRole { get; set; }
    }
}
