using System;
using System.Collections.Generic;
using System.Web.Mvc;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;

namespace hms_tom_dev.Models.Transport
{
    public class TransportExecutionPrintFileViewModel
    {
        public string FileName { get; set; }
        public string Uid { get; set; }
    }
}