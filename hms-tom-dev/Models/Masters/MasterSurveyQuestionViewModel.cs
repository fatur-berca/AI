using System;
using System.Collections.Generic;

namespace hms_tom_dev.Models.Masters
{
    public class MasterSurveyQuestionViewModel
    {
        public int IDMasterSurveyQuestion { get; set; }
        public string Question { get; set; }
        public string Section { get; set; }
        public string Group { get; set; }
        public string OldGroup { get; set; }
        public Nullable<int> Ordering { get; set; }
        public Nullable<int> OldOrdering { get; set; }
        public bool IsActive { get; set; }
        public bool OldIsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        public virtual ICollection<MasterSurveyAnswerViewModel> MasterSurveyAnswers { get; set; }
    }
}