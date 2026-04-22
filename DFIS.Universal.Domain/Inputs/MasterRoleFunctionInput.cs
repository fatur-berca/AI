using System;
using DFIS.Universal.Domain.DTOs;

namespace DFIS.Universal.Domain.Inputs
{
    public class MasterRoleFunctionInput : BaseInput
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

        public virtual MasterFunctionDTO MasterFunction { get; set; }
        public virtual MasterRoleDTO MasterRole { get; set; }
    }
}
