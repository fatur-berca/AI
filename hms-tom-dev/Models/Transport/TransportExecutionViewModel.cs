using System;
using System.Collections.Generic;
using System.Web.Mvc;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;

namespace hms_tom_dev.Models.Transport
{
    public class TransportExecutionViewModel : ViewModelBase
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

        public string UserRole { get; set; }

        public virtual MasterVendorTOMViewModel MasterVendor { get; set; }

        #region bagiandefaultvalue
        public virtual List<SelectListItem> orderCategoryList { get; set; }
        public virtual List<SelectListItem> siStatusList { get; set; }
        public virtual List<SelectListItem> orderTypeList { get; set; }
        public virtual List<SelectListItem> zoneList { get; set; }
        public virtual List<SelectListItem> materialTypeList { get; set; }
        public virtual List<SelectListItem> locationList { get; set; }
        public virtual List<SelectListItem> executionTypeList { get; set; }
        public virtual List<SelectListItem> transportStatusList { get; set; }
        public virtual List<SelectListItem> transportModeList { get; set; }
        #endregion
        #region filterTransactionExecutionAddNew
        public int? weekNow { get; set; }
        public int? yearNow { get; set; }
        public virtual List<SelectListItem> vehicleTypeList { get; set; }
        public virtual List<SelectListItem> transportationModeList { get; set; }
        public virtual List<SelectListItem> vendorList { get; set; }
        #endregion
        #region tambahan untuk dropdown di transport execution edit
        public virtual List<SelectListItem> transportCategoryList { get; set; }
        public virtual List<SelectListItem> siTypeList { get; set; }
        public virtual List<SelectListItem> costCenterList { get; set; }
        public virtual List<SelectListItem> policeRegNumberList { get; set; }
        public virtual List<SelectListItem> vesselNameList { get; set; }
        public virtual List<SelectListItem> driverList { get; set; }
        public virtual List<SelectListItem> codriverList { get; set; }        
        public virtual List<SelectListItem> viaList { get; set; }
        #endregion

        #region Bagian buat editor monitoring
        public List<TransportOrderViewModel> TransportOrders { get; set; }
        public DateTime GIDate { get; set; }
        public DateTime GRDate { get; set; }
        #endregion
    }
}