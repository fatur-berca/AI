using System;

namespace hms_tom_dev.Models.Masters
{
    public class MasterSurveyAnswerViewModel
    {
        public int IDMasterSurveyAnswer { get; set; }
        public Nullable<int> IDMasterSurveyQuestion { get; set; }
        public string Answer { get; set; }
        public string FieldType { get; set; }
        public Nullable<int> Ordering { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    }
}