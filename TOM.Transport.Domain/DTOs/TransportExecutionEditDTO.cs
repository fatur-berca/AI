using System;
using System.Collections.Generic;
using TOM.Master.Domain.DTOs;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportExecutionEditDTO
    {
        public int IDTransportExecution { get; set; }
        public string TransportNo { get; set; }
        public string TransportStatus { get; set; }
        public DateTime TransportDate { get; set; }
        public string TransportCategory { get; set; }
        public string ActualVehicleType { get; set; }
        public string TransportMode { get; set; }
        /*public string VesselName { get; set; }
        public DateTime? ETD1 { get; set; }
        public DateTime? ETD2 { get; set; }
        public DateTime? ATD { get; set; }
        public DateTime? ETA1 { get; set; }
        public DateTime? ETA2 { get; set; }
        public DateTime? ATA { get; set; }
        public DateTime? ActualTimeBerthing { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerSeal { get; set; }*/
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
        public bool IsActive { get; set; }
        public string ServiceGRNo { get; set; }
        public string Remarks { get; set; }
        public int? DeliveryQty { get; set; }
        public decimal? CFPLiter { get; set; }
        public decimal? CFPTKM { get; set; }
        public decimal? KGCO2 { get; set; }
        public decimal? AVGLoadFactor { get; set; }
        public decimal? TotalKMRail { get; set; }
        public decimal? TotalKMBased { get; set; }
        public decimal? TotalKMSea { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? BasedCost { get; set; }
        public decimal? DiscountCost { get; set; }
        public int? IDCostCenter { get; set; }

        public string VendorName { get; set; }
        public string IDLocation { get; set; }
        public string LocationName { get; set; }
        public string STONo { get; set; }
        public string ZoneBased { get; set; }
        public string OrderType { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string VehicleType { get; set; }
        public string MaterialType { get; set; }
        public string Description { get; set; }
        public decimal? Qty { get; set; }
        public string UoM { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public int IDTransportOrderDetail { get; set; }

        //dipakai untuk memunculkan baris baru vendor
        public bool IsUnfullfillOrRecall { get; set; }

        public virtual MasterVendorTOMDTO MasterVendor { get; set; }
        public virtual TransportDriverManagementDTO TransportDriverManagement { get; set; }
        public virtual TransportDriverManagementDTO TransportDriverManagement1 { get; set; }
        public virtual TransportDriverManagementDTO TransportDriverManagement2 { get; set; }
        public virtual TransportVehicleDataDTO TransportVehicleData { get; set; }
        public virtual List<TransportOrderTransportExecutionEditDTO> TransportOrders { get; set; }
        public virtual List<TransportExecutionStatusLogDTO> TransportExecutionStatusLogs { get; set; }
        public virtual List<TransportVendorChangeLogDTO> TransportVendorChangeLogs { get; set; }
        public virtual List<TransportRouteDTO> TransportRoutes { get; set; }
        public virtual List<TransportVesselMonitoringDTO> TransportVesselMonitorings { get; set; }
        public virtual MasterCostCenterAccountDTO MasterCostCenter { get; set; }
    }
}
