using hms_tom_dev.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hms_tom_dev.Models.Masters
{
    public class MasterUserViewModel : ViewModelBase
    {

        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}