using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hms_tom_dev.Models.Common
{
    public class ViewModelBase
    {
        public string ResponseType { get; set; }
        public string Message { get; set; }
        public string State { get; set; }
        public List<string> ListMonths { get; set; }
        public List<int> ListWeeks { get; set; }
        public List<int> ListYears { get; set; }
        public bool ClosingPage { get; set; }
    }
}