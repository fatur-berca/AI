using DFIS.Universal.Domain.Inputs;
using System;
using System.Collections.Generic;

namespace TOM.Master.Domain.Inputs
{
    public class MasterCostInput : BaseInput
    {
        public int IDCost { get; set; }
        public string CostType { get; set; }
        public int IDVendor { get; set; }
        public string VehicleType { get; set; }
        public string OrderType { get; set; }
        public string SenderIDLocation { get; set; }
        public string ThroughIDLocation { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string Via { get; set; }
        public Nullable<decimal> MinimumKM { get; set; }
        public Nullable<int> MinimumBox { get; set; }
        public decimal BasedPrice { get; set; }
        public Nullable<decimal> DiscountPrice { get; set; }
        public Nullable<decimal> AdditionalUnitPrice { get; set; }
        public System.DateTime EffectiveStartDate { get; set; }
        public System.DateTime EffectiveEndDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public string filterDate { get; set; }
        public List<string> filterSenderLocation { get; set; }
    }
}
