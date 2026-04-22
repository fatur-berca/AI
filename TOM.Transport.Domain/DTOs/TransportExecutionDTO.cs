using System;
using System.Collections.Generic;
using TOM.Master.Domain.DTOs;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportExecutionDTO
    {
        public TransportExecutionDTO()
        {
            TransportVesselCustomReport = new TransportVesselCustomReport();
        }

        public TransportExecutionDTO(TransportExecutionDTO dto)
        {
            if (dto == null)
                TransportVesselCustomReport = new TransportVesselCustomReport();
            else
            {
                var type = typeof(TransportExecutionDTO);
                var pis = type.GetProperties();

                foreach (var pi in pis)
                {
                    if (pi.CanRead && pi.CanWrite)
                    {
                        pi.SetValue(this, pi.GetValue(dto));
                    }
                }
            }
        }

        public int IDTransportExecution { get; set; }
        public int? IDTransportExecute { get; set; }
        public string TransportNo { get; set; }
        public string TransportStatus { get; set; }
        public DateTime TransportDate { get; set; }
        public DateTime? TransDate { get; set; }
        public string TransportCategory { get; set; }
        public string ActualVehicleType { get; set; }
        public string TransportMode { get; set; }
        public DateTime? ETD1 { get; set; }
        public DateTime? ETD2 { get; set; }
        public DateTime? ATD { get; set; }
        public DateTime? EstReceived { get; set; }
        public string ContainerNo { get; set; }
        /*public string VesselName { get; set; }        
        public DateTime? ETA1 { get; set; }
        public DateTime? ETA2 { get; set; }
        public DateTime? ATA { get; set; }
        public DateTime? ActualTimeBerthing { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerSeal { get; set; }*/
        public int? IDVendor { get; set; }
        public string CargoType { get; set; }
        public string SIType { get; set; }
        public string SIStatus { get; set; }
        public byte? SIWeek { get; set; }
        public DateTime? TargetOfArrival { get; set; }
        public DateTime? ActualArrive { get; set; }
        public string IDStartLocation { get; set; }
        public string StartLocation { get; set; }
        public string IDFinishLocation { get; set; }
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
        public string CreatedByIDUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool? IsActve { get; set; }
        public string ServiceGRNo { get; set; }
        public string Remarks { get; set; }
        public int? IDCostCenter { get; set; }

        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string VendorCity { get; set; }
        public string VendorEmail { get; set; }
        public string VendorRegion { get; set; }
        public string VendorCategory { get; set; }

        public string IDLocation { get; set; }
        public string LocationName { get; set; }
        public string STONo { get; set; }
        public decimal? KMOrder { get; set; }
        public string ZoneBased { get; set; }
        public string OrderType { get; set; }
        public string OrderCategory { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string VehicleType { get; set; }
        public DateTime? GRDate { get; set; }
        public DateTime? GIDate { get; set; }
        public string MaterialType { get; set; }
        public string Description { get; set; }
        public decimal? Qty { get; set; }
        public string UoM { get; set; }
        public string IDSender { get; set; }
        public string Sender { get; set; }
        public string IDReceiver { get; set; }
        public string Receiver { get; set; }
        public int? IDTransportOrder { get; set; }
        public int? IDTransportOrderDetail { get; set; }
        public string IsMaterialReceived { get; set; }

        public string Trip { get; set; }
        public decimal? StickPerBox { get; set; }
        public decimal? PackPerBox { get; set; }
        public decimal? StickPerPack { get; set; }
        public decimal? BasedPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        
        public decimal? AVGLoadFactor { get; set; }
        public decimal? CFPLiter { get; set; }
        public decimal? CFPTKM { get; set; }
        public decimal? KGCO2 { get; set; }

        public int? IDRoute { get; set; }
        public string Route { get; set; }

        public string MapFrom { get; set; }

        public virtual MasterVendorTOMDTO MasterVendor { get; set; }
        public virtual MasterCostCenterAccountDTO MasterCostCenter { get; set; }
        public List<TransportOrderList> TransportOrderList { get; set; }
        //public IEnumerable<TransportOrderDTO> TransportOrders { get; set; }

        //digunakan sebagai penanda pada saat generate transportation number
        public int IDCheckSave { get; set; }

        public string PoliceRegistrationNumber { get; set; }
        public string Driver1 { get; set; }
        public string Driver2 { get; set; }
        public string CoDriver { get; set; }

        //digunakan di custom report list view
        public TransportVesselCustomReport TransportVesselCustomReport { get; set; }
        public List<string> RouteCustomReport { get; set; }
        public string RouteNameCustomReport { get; set; }

        public string CurrentLocation { get; set; }
        public DateTime CurrentLocationUpdateTime { get; set; }

        public int? LeadTime { get; set; }
        public string OrderStatus { get; set; }

        public string GRBy { get; set; }
        public string GIBy { get; set; }

        public decimal? DeliveryQty { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? TotalKMRail { get; set; }
        public decimal? TotalKMBased { get; set; }
        public decimal? TotalKMSea { get; set; }

        //digunakan di custom report transport monitoring
        public virtual List<TransportPositionDetailDTO> TransportPositionDetails { get; set; }

        public DateTime PositionDate1 { get; set; }
        public DateTime PositionDate2 { get; set; }
        public DateTime PositionDate3 { get; set; }
        public string PositionName1 { get; set; }
        public string PositionName2 { get; set; }
        public string PositionName3 { get; set; }
        //digunakan di custom report transport monitoring

        //digunakan di export transport summary
        public string MappingCC { get; set; }
        public string Account { get; set; }
        //digunakan di export transport summary

        //digunakan di export custom report execution
        public string ActualCostCenterDisplay
        {
            set
            {
                
            }
            get
            {
                if (ActualCostCenter != null)
                {
                    var split = ActualCostCenter.Split('-');
                    if (split.Length > 1)
                        return split[1];
                    return split[0];
                }
                return "";
            }
        }
    }

    public class TransportExecutionExportSIDTO
    {
        public int IDTransportExecution { get; set; }

        public List<TransportOrderDTO> TransportOrders { get; set; }

        public string IDDriver1 { get; set; }
        public string IDDriver2 { get; set; }
        public string IDCoDriver { get; set; }
        public string Driver1 { get; set; }
        public string Driver2 { get; set; }
        public string CoDriver { get; set; }

        public DateTime TransportDate { get; set; }
        public string TransportNo { get; set; }
        public byte? SIWeek { get; set; }
        public string SIStatus { get; set; }
        public string SIType { get; set; }
        public DateTime? TargetOfArrival { get; set; }
        public string PoliceRegNo { get; set; }
        public string Remarks { get; set; }
        public int? IDVendor { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string VendorCity { get; set; }
        public string VendorEmail { get; set; }
        public string VendorRegion { get; set; }
        public string VehicleType { get; set; }
        public string MaterialType { get; set; }
        public string ContainerNo { get; set; }
        public string Via { get; set; }
        public bool IsChangelog { get; set; }
    }

    public class TransportVesselCustomReport
    {
        public string VesselNameCustomReport { get; set; }
        public string ContainerNoCustomReport { get; set; }
        public string ContainerSealCustomReport { get; set; }
        public DateTime? ETD1 { get; set; }
        public DateTime? ETA1 { get; set; }
    }

    public class TransportOrderList
    {
        public int IDTransportOrder { get; set; }
        public string STONo { get; set; }
        public string OrderStatus { get; set; }
        public string OrderType { get; set; }
        public string ActualSenderIDLocation { get; set; }
        public string ActualSenderLocationName { get; set; }
        public string ActualReceiverIDLocation { get; set; }
        public string ActualReceiverLocationName { get; set; }
        public string GRBy { get; set; }
        public string GIBy { get; set; }
        public int? POWeek { get; set; }
        public DateTime? GIDate { get; set; }
        public DateTime? GRDate { get; set; }
        public int? LeadTime { get; set; }
        public DateTime? EstArrivalDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class TransportExecutionDataTableDTO
    {
        public string TransportationNumber { get; set; }
        public string TransportationStatus { get; set; }
        public string CreatedBy { get; set; }
        public string VendorName { get; set; }
        public int? VendorID { get; set; }
        public byte? SIWeek { get; set; }
        public string SIType { get; set; }
        public string SIStatus { get; set; }
        public string StartLocationID { get; set; }
        public string StartLocationName { get; set; }
        public string FinishLocationID { get; set; }
        public string FinishLocationName { get; set; }
        public DateTime? TransportationDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public int IDTransportExecution { get; set; }
        public int OrderCount {get;set;}
        public List<string> MaterialType { get; set; }

        // Aliases
        public string TransportNo { get { return TransportationNumber; } }
        public string TransportStatus { get { return TransportationStatus; } }
        public DateTime? TransportDate { get { return TransportationDate; } }
        public string StartLocation { get { return StartLocationName; } }
        public string FinishLocation { get { return FinishLocationName; } }
    }

    public class TransportExecutionCustomExportDTO : TransportExecutionDataTableDTO
    {
        public TransportExecutionCustomExportDTO()
        {

        }
        public TransportExecutionCustomExportDTO(TransportExecutionDataTableDTO source)
        {
            SetValues(source);
        }
        public void SetValues(TransportExecutionDataTableDTO source)
        {
            var srctyp = typeof(TransportExecutionDataTableDTO);
            foreach(var pi in GetType().GetProperties())
            {
                var pis = srctyp.GetProperty(pi.Name);
                if (pi.CanWrite && pis != null && pis.CanRead)
                {
                    pi.SetValue(this, pis.GetValue(source));
                }
            }
        }

        public TransportVesselCustomReport TransportVesselCustomReport { get; set; }
        public string TransportCategory { get; set; }
        public string ActualVehicleType { get; set; }
        public string TransportMode { get; set; }
        public string ServicePONo { get; set; }
        public string ServiceGRNo { get; set; }
        public string ActualCostCenter { get; set; }
        public decimal? TotalKM { get; set; }
        public int? TotalBox { get; set; }
        public string RouteNameCustomReport { get; set; }
        public string Via { get; set; }
        public byte? Month { get; set; }
        public int? Year { get; set; }
        public decimal? AdditionalCost { get; set; }
        public DateTime? TargetOfArrival { get; set; }
        public DateTime? ActualArrive { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string PoliceRegistrationNumber { get; set; }
        public string Driver1 { get; set; }
        public string Driver2 { get; set; }
        public string CoDriver { get; set; }
        public decimal? DeliveryQty { get; set; }
        public decimal? TotalKMRail { get; set; }
        public decimal? TotalKMSea { get; set; }
        public decimal? TotalKMBased { get; set; }
        public string Remarks { get; set; }
    }

    public class TransportExecutionEditUpdatedDate
    {
        public string TransportNo { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
