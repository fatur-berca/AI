using System;

namespace DFIS.Universal.Domain.Inputs
{
    public class MasterConfigurationInput : BaseInput
    {
        public int IDConfiguration { get; set; }
        public string PageName { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    }
}
