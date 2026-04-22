using System;
using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;

namespace hms_tom_dev.Models.Masters
{
    public class MasterRoleFunctionViewModel
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