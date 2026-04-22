using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DFIS.Universal.Domain.DTOs;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Transport
{
    public class TransportUnitFrequentViewModel : ViewModelBase
    {
        public string policenumber { get; set; }
        public string VehicleType { get; set; }
        public string Vendor { get; set; }
        public Nullable<int> C1 { get; set; }
        public Nullable<int> C2 { get; set; }
        public Nullable<int> C3 { get; set; }
        public Nullable<int> C4 { get; set; }
        public Nullable<int> C5 { get; set; }
        public Nullable<int> C6 { get; set; }
        public Nullable<int> C7 { get; set; }
        public string IDPoliceRegNumber { get; set; }
    }
}