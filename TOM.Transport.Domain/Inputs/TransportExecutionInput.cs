using System;
using DFIS.Utils;

namespace TOM.Transport.Domain.Inputs
{
    public class TransporExecutionInput : UtilBaseInput
    {
        public int IDTransportExecution { get; set; }
        public string TransportNo { get; set; }
        public string TransportStatus { get; set; }
        public DateTime TransportDate { get; set; }
        public string TransportCategory { get; set; }
        public string ActualVehicleType { get; set; }
        public string TransportMode { get; set; }
        public string VesselName { get; set; }
        public DateTime? ETD1 { get; set; }
        public DateTime? ETD2 { get; set; }
        public DateTime? ATD { get; set; }
        public DateTime? ETA1 { get; set; }
        public DateTime? ETA2 { get; set; }
        public DateTime? ATA { get; set; }
        public DateTime? ActualTimeBerthing { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerSeal { get; set; }
        public int? IDVendor { get; set; }
        public string SIType { get; set; }
        public string SIStatus { get; set; }
        public byte? SIWeek { get; set; }
        public DateTime? TargetOfArrival { get; set; }
        public DateTime? ActualArrive { get; set; }
        public string StartLocation { get; set; }
        public string FinishLocation { get; set; }
        public string ServicePONo { get; set; }
        public string SerivceGRNo { get; set; }
        public string ActualCostCenter { get; set; }
        public string PoliceRegNo { get; set; }
        public string IDDriver1 { get; set; }
        public string IDDriver2 { get; set; }
        public string IDCoDriver { get; set; }
        public int? IDDistanceKMBased { get; set; }
        public int? IDDistanceBoxTripBased { get; set; }
        public int? IDCost { get; set; }
        public decimal? TotalKM { get; set; }
        public decimal? KMRail { get; set; }
        public decimal? KMBased { get; set; }
        public decimal? KMSea { get; set; }
        public int? TotalBox { get; set; }
        public decimal? ASDPCost { get; set; }
        public decimal? SPSICost { get; set; }
        public decimal? AdditionalCost { get; set; }
        public string Via { get; set; }
        public byte? Month { get; set; }
        public int? Year { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }        
    }
}
