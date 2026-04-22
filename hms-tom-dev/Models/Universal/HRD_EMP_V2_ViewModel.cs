using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Universal
{
    public class HRD_EMP_V2_ViewModel : ViewModelBase
    {
        public int Row_ID { get; set; }
        public string FULL_NAME { get; set; }
        public string ID { get; set; }
        public string NAME { get; set; }
        public string TITLE_NAME { get; set; }
        public string EMAIL { get; set; }
        public string DIVISION_Q { get; set; }
        public string LOCATION { get; set; }
    }
}