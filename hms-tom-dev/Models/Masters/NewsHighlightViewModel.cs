using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hms_tom_dev.Models.Masters
{
    public class NewsHighlightViewModel
    {
        public int IDNewsHighlight { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string FileUpload { get; set; }
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    }
}