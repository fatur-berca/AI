using DFIS.Universal.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.Domain.Inputs
{
    public class MasterLoadFactorCFPInput : BaseInput
    {
        public int IDLoadFactorCFP { get; set; }
        public string VehicleType { get; set; }
        public string Mode { get; set; }
        public Nullable<decimal> KMperLiter { get; set; }
        public Nullable<double> KgCO2perLiter { get; set; }
        public string BrandCategory { get; set; }
        public int MaxQty { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public Nullable<double> WeightperStick { get; set; }
        public string _WeightperStick { get; set; }
        public string _KMperLiter { get; set; }
        public string _KgCO2perLiter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> VehicleTypeListFilter { get; set; }
    }
}
