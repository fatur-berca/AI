using System;

namespace TOM.Transport.Domain.Inputs
{
    public class TransportTruckArrivalInput
    {
        public int IDTransportTruckArrival { get; set; }
        public string PoliceNumber { get; set; }
        public string GPNo { get; set; }
        public DateTime Date { get; set; }
        public string LongitudeDestination { get; set; }
        public string LatitudeDestination { get; set; }
        public string LongitudeTruck { get; set; }
        public string LatitudeTruck { get; set; }
        public DateTime? ETA { get; set; }
        public string ETACategory { get; set; }
        public DateTime? ETACalculate { get; set; }
        public string ETACategoryCalculate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
