using hms_tom_dev.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hms_tom_dev.Models.Masters
{
    public class MappingSigaRoleViewModel : ViewModelBase
    {

        public int id { get; set; }
        public string nama { get; set; }
        public int role_id { get; set; }
        public bool flag { get; set; }
    }
}