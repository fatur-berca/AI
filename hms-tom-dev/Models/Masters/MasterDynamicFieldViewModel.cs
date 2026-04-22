using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterDynamicFieldViewModel : ViewModelBase
    {
        public int IDDynamicField { get; set; }
        public string PageName { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string GroupCollapse { get; set; }
        public int Ordering { get; set; }
        public int NotificationPeriod { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
    }
}