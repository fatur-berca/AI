using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterLoadFactorCFPViewModel : ViewModelBase
    {
        public int IDLoadFactorCFP { get; set; }
        public string VehicleType { get; set; }
        public string Mode { get; set; }
        public Nullable<decimal> KMperLiter { get; set; }
        public Nullable<decimal> KgCO2perLiter { get; set; }
        public string BrandCategory { get; set; }
        public int MaxQty { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public Nullable<decimal> WeightperStick { get; set; }
        public string _WeightperStick { get; set; }
        public string _KMperLiter { get; set; }
        public string _KgCO2perLiter { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }


    }
}