using System;
using hms_tom_dev.Models.Masters;
using System.Collections.Generic;
using System.Collections;

namespace hms_tom_dev.Models.Transport
{
    public class TransportOrderTransportExecutionPrintViewModel
    {
        public int IDTransportOrder { get; set; }
        public int? IDTransportExecution { get; set; }
        public string STONo { get; set; }
        public string DefaultSeqNo { get; set; }
        public string SeqNo { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string SenderIDLocation { get; set; }
        public string SenderLocationName { get; set; }
        public string ReceiverIDLocation { get; set; }
        public string ReceiverLocationName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ZoneBased { get; set; }
        public string TransportNo { get; set; }
        public DateTime TransportDate { get; set; }
        public string ActualVehicleType { get; set; }
        public string PoliceRegNo { get; set; }
        public int? IDVendor { get; set; }
        public string IDDriver1 { get; set; }
        public string DriverName1 { get; set; }
        public string IDDriver2 { get; set; }
        public string DriverName2 { get; set; }
        public string IDCoDriver { get; set; }
        public string CoDriverName { get; set; }
        public string StartLocation { get; set; }
        public string VendorName { get; set; }        
        public string VesselName { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerSeal { get; set; }
        public string WarehouseAddressSender { get; set; }
        public string WarehouseAddressReceiver { get; set; }
        public string TNBarcode { get; set; }        

        public virtual MasterLocationViewModel MasterLocation { get; set; }
        public virtual MasterLocationViewModel MasterLocation1 { get; set; }
        public virtual TransportExecutionViewModel TransportExecution { get; set; }
        public virtual MasterVendorViewModel MasterVendor { get; set; }
        public virtual TransportDriverManagementViewModel TransportDriverManagement { get; set; }
        public virtual TransportDriverManagementViewModel TransportDriverManagement1 { get; set; }
        public virtual TransportDriverManagementViewModel TransportDriverManagement2 { get; set; }
        public virtual GeneralBuildingFacilityViewModel GeneralBuildingFacility { get; set; }
        public virtual GeneralBuildingFacilityViewModel GeneralBuildingFacility1 { get; set; }
    }
}