using System;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Transport
{
    public class TransportTruckArrivalViewModel : ViewModelBase
    {
        public int IDTransportTruckArrival { get; set; }
        public string PoliceNumber { get; set; }
        public string GPNo { get; set; }
        public DateTime Date { get; set; }
        public string LongitudeDestination { get; set; }
        public string LatitudeDestination { get; set; }
        public string LongitudeTruck { get; set; }
        public string LatitudeTruck { get; set; }
        public Nullable<DateTime> ETA { get; set; }
        public string ETACategory { get; set; }
        public Nullable<DateTime> ETACalculate { get; set; }
        public string ETACategoryCalculate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
    }
}