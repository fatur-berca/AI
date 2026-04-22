using DFIS.Universal.Domain.Inputs;
using System;

namespace TOM.Transport.Domain.Inputs
{
    public class TransportUnitFrequentInput : BaseInput
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
        public string TransactionDate { get; set; }
    }
}
