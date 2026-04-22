using System;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterIconFacilityViewModel : ViewModelBase
    {
        public int IDIconFacility { get; set; }
        public string Icon { get; set; }
        public string FieldName { get; set; }
        public bool Status { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string LinkIcon { get; set; }
        public string NamaIcon { get; set; }
    }
}