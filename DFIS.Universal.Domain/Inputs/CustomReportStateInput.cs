using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Utils;

namespace DFIS.Universal.Domain.Inputs
{
    public class CustomReportStateInput //: BaseInput
    {
        public int IDCustomReportLayout { get; set; }
        public string IDUser { get; set; }
        public string PageName { get; set; }
        public string FieldName { get; set; }
        public string LayoutName { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        //public virtual MasterUser MasterUser { get; set; }
    }
}
