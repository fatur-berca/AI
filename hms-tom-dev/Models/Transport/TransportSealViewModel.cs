using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;

namespace hms_tom_dev.Models.Transport
{
    public class TransportSealViewModel : ViewModelBase
    {
        public int IDTransportSeal { get; set; }
        public int IDTransportExecution { get; set; }
        public string SealNumber { get; set; }
        public string SealActivity { get; set; }
        public string IDLocation { get; set; }
        public string LocationName { get; set; }
        public System.DateTime SealTime { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByFullName { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        public virtual MasterLocationViewModel MasterLocation { get; set; }
        public virtual TransportExecutionViewModel TransportExecution { get; set; }
    }
}