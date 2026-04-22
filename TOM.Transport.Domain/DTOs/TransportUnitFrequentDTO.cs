using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportUnitFrequentDTO
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
