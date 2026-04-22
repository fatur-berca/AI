using System;
using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterApprovalNewsViewModel : ViewModelBase
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
        public string LinkImage { get; set; }
        public string NamaImage { get; set; }
        public string LinkFile { get; set; }
        public string NamaFile { get; set; }
        //public List<NewsHighlight> entry { get; set; }
    }
}