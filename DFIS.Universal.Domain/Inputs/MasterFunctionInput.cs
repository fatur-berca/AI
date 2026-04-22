using System;

namespace DFIS.Universal.Domain.Inputs
{
    public class MasterFunctionInput : BaseInput
    {
        public int IDFunction { get; set; }
        public string FunctionName { get; set; }
        public int ParentIDFunction { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    }
}
