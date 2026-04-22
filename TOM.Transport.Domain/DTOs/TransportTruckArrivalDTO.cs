using System;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportTruckArrivalDTO
    {
        /*
        public TransportTruckArrivalDTO(TransportTruckArrival tta, string finishAddress)
        {
            IDTransportTruckArrival = tta.IDTransportTruckArrival;
            PoliceNumber = tta.PoliceNumber;
            GPNo = tta.GPNo;
            Date = tta.Date;
            LongitudeDestination = tta.LongitudeDestination;
            LatitudeDestination = tta.LatitudeDestination;
            LongitudeTruck = tta.LongitudeTruck;
            LatitudeTruck = tta.LatitudeTruck;
            ETA = tta.ETA;
            ETACategory = tta.ETACategory;
            ETACalculate = tta.ETACalculate;
            ETACategoryCalculate = tta.ETACategoryCalculate;
            UpdatedBy = tta.UpdatedBy;
            UpdatedDate = tta.UpdatedDate;
            FinishAddress = finishAddress;
        }
        */
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
        public string FinishAddress { get; set; }
    }
}
