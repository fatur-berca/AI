using DFIS.Universal.Domain.Inputs;
using System;
using System.Collections.Generic;

namespace TOM.Master.Domain.Inputs
{
    public class MasterVendorSuggestionInput : BaseInput
    {
        public int IDVendorSuggestion { get; set; }
        public string StartLocation { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string OrderType { get; set; }
        public string TransportationCategory { get; set; }
        public string TransportationMode { get; set; }
        public string VehicleType { get; set; }
        public int SuggestedVendor { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }

        public List<string> filterSenderLocation { get; set; }
    }
}
