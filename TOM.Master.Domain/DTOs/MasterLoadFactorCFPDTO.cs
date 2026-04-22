using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.Domain.DTOs
{
    public class MasterLoadFactorCFPDTO
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
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
    }
}
