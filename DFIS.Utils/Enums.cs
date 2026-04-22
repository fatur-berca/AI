using System.ComponentModel;

namespace DFIS.Utils
{
    public class Enums
    {
        public enum SealStatus
        {
            [Description("CLOSE")]
            close,
            [Description("LOCKED")]
            locked,
            [Description("OPEN")]
            open,
            [Description("UNLOCKED")]
            unlocked
        }
        public enum ResponseType
        {
            NoLayoutName,
            Success,
            Error,
            Overwrite,//digunakan di menu Continuous Improve SS, kalau data sudah ada di db
            Status,//digunakan di menu Continuous Improve SS, kalau status sudah approve atau reject tidak bisa save 
            Invalid,
            Unsequence,
            DoubleData,
            NotExist
        }
        public enum UserRole
        {
            [Description("SUPER ADMIN")]
            SuperAdmin,
        }

        public enum RoleUserList
        {
            [Description("SUPER ADMIN")]
            SA,
            [Description("TRANSPORT")]
            TRANSPORT,
            [Description("ADMIN WAREHOUSE")]
            AWR,
            [Description("ADMIN TRANSPORT")]
            ATR,
            [Description("CUSTOMER")]
            CUSTOMER
        }

        public enum PageName
        {
            #region Master
            [Description("CustomReportState")]
            CustomReportState,
            [Description("MstList")]
            MasterList,
            [Description("MstFGStacking")]
            MstFgStacking,
            [Description("MstTruckSealStock")]
            MstTruckSeal,
            [Description("MasterBrand")]
            MasterBrand,
            [Description("MstLocation")]
            MasterLocation,
            [Description("MstIconFacility")]
            MasterIconFacility,
            [Description("MstEquipmentType")]
            MasterEquipmentType,
            [Description("MstTransportRegion")]
            MstTransportRegion,
            [Description("MstVendor")]
            MstVendor,
            [Description("MstDataLoadFactorCFP")]
            MstDataLoadFactorCFP,
            [Description("MstConfiguration")]
            MstConfiguration,
            [Description("MstConfigurationEmail")]
            MstConfigurationEmail,
            [Description("MstConvertionPointSS")]
            MstConvertionPointSS,
            [Description("MstHoliday")]
            MstHoliday,
            [Description("MstMapping")]
            MstMapping,
            [Description("MstColorKPISummary")]
            MstColorKPISummary,
            [Description("MstRoleFunction")]
            MstRoleFunction,
            [Description("MstDynamicField")]
            MstDynamicField,
            [Description("MstGuideline")]
            MstGuideline,
            [Description("MstStockMonitoring")]
            MstStockMonitoring,
            [Description("MstUserLocationMap")]
            MstUserLocationMap,
            [Description("MstApprovalNews")]
            MstApprovalNews,
            [Description("MstKilometer")]
            MstKilometer,
            [Description("MstSurveyQuestion")]
            MstSurveyQuestion,
            [Description("MstVendorTOM")]
            MstVendorTOM,
            [Description("MstCostKMBoxSPSI")]
            MstCostKMBoxSPSI,
            [Description("MstCostTripASDP")]
            MstCostTripASDP,
	        [Description("MstDistance")]
            MstDistance,
	        [Description("MstLeadTimeTom")]
            MstLeadTimeTom,
            [Description("MstVendorSuggestion")]
            MstVendorSuggestion,
            [Description("MstUom")]
            MstUom,
            [Description("MstServicePoTom")]
            MstServicePoTom,
            [Description("MstCostCenterAccount")]
            MstCostCenterAccount,
            [Description("MstCost")]
            MstCost,
            [Description("MstUserRoleDelegation")]
            MstUserRoleDelegate,
            #endregion

            #region GENERAL
            [Description("Building")]
            BuildingAndFacility,
            [Description("BudgetMonitoring")]
            BudgetMonitoringList,
            [Description("FiveSMonitoring")]
            FiveSMonitoringList,
            [Description("ContinuousImprovementSandBag")]
            ContinousImprovementSandBagList,
            [Description("ContinuousImprovementSugestionSystem")]
            ContinuousImprovementSugestionSystemList,
            [Description("RecognitionHistory")]
            RecognitionHistory,
            [Description("SafeIntegration")]
            SafeIntegration,
            [Description("UserGuideline")]
            UserGuideline,
            [Description("BestPracticeToolBox")]
            BestPracticeToolBox,
            #endregion

            #region WAREHOUSE
            [Description("StockOnHand")]
            WarehouseStockMonitoring,
            [Description("BeetleTrapMonitoring")]
            WarehouseBeetleTrapMonitoring,
            [Description("TemperatureHumidity")]
            WarehouseTemperatureHumidity,
            [Description("SealManagement")]
            WarehouseSealManagement,
            [Description("SealStock")]
            WarehouseSealStock,
            [Description("stockcount")]
            WarehouseStockCount,
            [Description("ViewAnnualWarehouseUtilization")]
            ViewAnnualWarehouseUtilization,
            [Description("AuditChecklist")]
            AuditChecklist,
            #endregion

            #region Transport
            [Description("TransportLostDamageClaim")]
            TransportLostDamageClaim,
            [Description("TicketNcr")]
            TransportTicketNcr,
            [Description("DriverManagement")]
            TransportDriverManagement,
            [Description("VesselMonitoring")]
            TransportVesselMonitoring,
            [Description("LoadUnloading")]
            TransportLoadUnloading,
            [Description("VehicleData")]
            TransportVehicleData,
            [Description("GenerateETransportCard")]
            TransportGenerateETransportCard,
            [Description("TruckArrival")]
            TransportTruckArrival,
            [Description("TransportPickingList")]
            TransportPickingList,
            [Description("TransportationSeal")]
            TransportationSeal,
            [Description("TransportationOrder")]
            TransportationOrder,
            [Description("TransportationExecution")]
            TransportationExecution,
            [Description("TransportationMonitoring")]
            TransportationMonitoring,
            [Description("TransportationSummary")]
            TransportationSummary,
            [Description("LoadingUnloadingHistory")]
            LoadingUnloadingHistory,
            #endregion

            #region KPI
            [Description("KPIProductivity")]
            KPIProductivity,
            [Description("KPIInventorySearching")]
            KPIInventorySearching,
            [Description("KPIDistributionCost")]
            KPIDistributionCost,
            [Description("LoadFactor")]
            KPILoadFactor,
            [Description("Suggestion")]
            KPISuggestion,
            [Description("CarbonFootprint")]
            KPICarbonFootprint,
            [Description("KPIOvertime")]
            KPIOvertime,
            [Description("TotalRecordable")]
            KPITotalRecordable,
            [Description("KPIVehicleDeliveryProcess")]
            KPIVehicleDeliveryProcess,
            [Description("KPISummaryReport")]
            KPISummaryReport,
            [Description("KPIICAuditFinding")]
            KPIICAuditFinding,
            [Description("KPIInternalLogisticAudit")]
            KPIInternalLogisticAudit,
			[Description("KPIDCLaborProductivity")]
            KPIDCLaborProductivity,
            #endregion

            #region BEETLETRAP
            [Description("BeetleTrapCBITarget")]
            BeetleTrapCBITarget,
            [Description("BeetleTrapSericoNumber")]
            BeetleTrapSericoNumber,
            [Description("BeetleTrapGenerateQRCode")]
            BeetleTrapGenerateQRCode,
            [Description("BeetleTrapInteractiveMap")]
            BeetleTrapInteractiveMap,
            [Description("BeetleTrapLocationDeactivate")]
            BeetleTrapLocationDeactivate,
            [Description("BeetleTrapMapDashboard")]
            BeetleTrapMapDashboard,
            [Description("BeetleTrapChartDashboard")]
            BeetleTrapChartDashboard,
            #endregion

            #region Report
            [Description("TransportationSummary")]
            TrSummary,
            #endregion
        }

        

        public enum ButtonName
        {
            AddNew,
            SaveChanges,
            Upload,
            Close,
            Export,
            CustomReport,
            CustomExport,
            Email,
            GenerateSAP,
            Log,
            Delete,
            AddNewUnit,
            SaveAsDraft,
            Submit,
            PrintDocument,
            ShippingInstruction,
            GenerateTN,
            Save,
            Edit,
            Recovery,
            Validation,
            UpdateStatus,
            Reset,
            Search,
            Add,
            Cancel,
            Refresh,
            OB,
            Back,
            No,
            Recalculate,
            PrintProfile,
            SetInactive,
            [Description("+Add")]
            plusAdd,
            OpenApps,
            Download,
            UploadFile,
            UploadSaleForecast,
            SealStock,
            SealStockList,
            [Description("+AddNewRow")]
            plusAddNewRow,
            Revoke,
            ShipLocation,
            InputSV,
            DeclareZero,
            RecordZero,
            [Description("Regional Map View")]
            RegionalMapView,
            [Description("ScoreApproval")]
            ScoreApproval,
            [Description("EditApprove")]
            EditApprove,
            UpdateVia
        }

        public enum MasterConfigurationValue
        {
            [Description("Closing Page")]
            ClosingPage,
            [Description("RoleZoneMapping")]
            RoleZoneMapping,
            [Description("RoleVendorMapping")]
            RoleVendorMapping,
            [Description("VesselMonitoringDetail")]
            VesselMonitoringDetail,
        }

        public enum TransportOrderChangeLog
        {
            
            [Description("ShipmentDate")]
            ShipmentDate,
            [Description("SenderIDLocation")]
            SenderLocation,
            [Description("ReceiverIDLocation")]
            ReceiverLocation,
            [Description("Vehicle Type")]
            VehicleType,
            [Description("Sequence")]
            Sequence,
            [Description("Remarks")]
            Remarks,
            [Description("MaterialType")]
            MaterialType,
            [Description("Supplier")]
            Supplier,
            [Description("Code")]
            Code,
            [Description("Description")]
            Description,
            [Description("Qty")]
            Quantity,
            [Description("UoM")]
            UoM
        }
        public enum TransportationStatus
        {
            [Description("Draft")]
            Draft,
            [Description("Submit")]
            Submit,
            [Description("In Process")]
            InProcess,
            [Description("On Delivery")]
            OnDeliver,
            [Description("Arrive At Destination and Waiting For Confirmation")]
            Arrive,
            [Description("Complete")]
            Complete,
            [Description("Close")]
            Close
        }

        public enum SIStatus
        {
            [Description("Fullfill")]
            Fullfill,
            [Description("Unfullfill")]
            Unfullfill,
            [Description("Recall")]
            Recall
        }

        public enum MasterialType
        {
            [Description("Finished Good")]
            FinishedGood,
            [Description("Raw Material")]
            RawMaterial
        }

        public enum OrderType
        {
            [Description("Finished Good")]
            FinishedGood,
            [Description("Raw Material")]
            RawMaterial,
            [Description("Bad Stock")]
            BadStock,
            [Description("Other")]
            Other
        }

        public enum TransportationMode
        {
            [Description("Truck")]
            Truck,
            [Description("Ship")]
            Ship,
            [Description("Train")]
            Train,
            [Description("Plane")]
            Plane,
            [Description("Sea")]
            Sea,
            [Description("Rail")]
            Rail,
        }

        public enum VendorCategory
        {
            [Description("KM Based")]
            KMBased,
            [Description("Trip Based")]
            TripBased,
            [Description("Box Based")]
            BoxBased
        }
    }
}
