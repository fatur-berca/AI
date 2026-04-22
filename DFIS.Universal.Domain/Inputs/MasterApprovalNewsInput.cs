using System;

namespace DFIS.Universal.Domain.Inputs
{
    public class MasterApprovalNewsInput : BaseInput
    {
        public int IDNewsHighlight { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string FileUpload { get; set; }
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string Remarks { get; set; }
        public int Click { get; set; }
    }
}
