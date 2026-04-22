using TOM.Master.BusinessLogics.Mappers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using hms_tom_dev.MapperViewModel;
using TOM.Transport.Domain.Mappers;

namespace hms_tom_dev
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutoMapper.Mapper.Initialize(x =>
            {
                #region BEGIN : Module General
                x.AddProfile<ViewModelMapperProfile>();
                x.AddProfile<TOMViewModelMapperProfile>();
                #endregion END : Module General

                #region BEGIN : Module Master
                x.AddProfile<MasterListMapperProfile>();
                x.AddProfile<MasterDynamicFieldMapperProfile>();
                x.AddProfile<ViewModelMapperProfile>();
                x.AddProfile<MasterFunctionMapperProfile>();
                x.AddProfile<MasterConfigurationMapperProfile>();
                x.AddProfile<MasterApprovalNewsMapperProfile>();
                x.AddProfile<MasterLocationMapperProfile>();
                x.AddProfile<MasterLoadFactorCFPMapperProfile>();
                x.AddProfile<MasterUserRoleMapperMappingProfile>();
                x.AddProfile<MasterFABrandMapperProfile>();
                x.AddProfile<MasterFGStackingMapperProfile>();
                x.AddProfile<MasterTruckSealMapperProfile>();
                x.AddProfile<MasterFGStackingMapperProfile>();
                x.AddProfile<MasterTransportRegionMapperProfile>();
                x.AddProfile<MasterDistanceMapperProfile>();
                //x.AddProfile<MasterEquipmentTypeMapperProfile>();
                //x.AddProfile<MasterHolidayMapperProfile>();
                x.AddProfile<MasterGuidelineMapperProfile>();
                x.AddProfile<MasterRoleFunctionMapperProfile>();
                x.AddProfile<MasterRoleMapperProfile>();
                x.AddProfile<MasterUserLocationMappingMapperProfile>();
                x.AddProfile<MasterUserMapperProfile>();
                x.AddProfile<MasterMappingMapperProfile>();
                //x.AddProfile<MasterLeadTimeMapperProfile>();
                //x.AddProfile<MasterMappingLocationPercentageMapperProfile>();
                //x.AddProfile<MasterZoneLocationMapperProfile>();
                //x.AddProfile<MasterKilometerMapperProfile>();
                //x.AddProfile<MasterSupportWarehouseMapperProfile>();
                x.AddProfile<MasterVendorTOMMapperProfile>();               
                x.AddProfile<MasterCostMapperProfile>();
                x.AddProfile<MasterVendorSuggestionMapperProfile>();
                x.AddProfile<MasterUomMapperProfile>();
                x.AddProfile<MasterLeadTimeTomMapperProfile>();
                x.AddProfile<MasterServicePoTomMapperProfile>();
                x.AddProfile<MasterCostCenterAccountMapperProfile>();
                x.AddProfile<MasterUserRoleDelegationMappingProfile>();
                #endregion END : Module Master
                
                #region BEGIN : Module Universal
                x.AddProfile<MasterListMapperProfile>();
                x.AddProfile<CustomReportStateMapperProfile>();
                x.AddProfile<HRD_EMP_V2_MapperProfile>();
                x.AddProfile<NotificationSystemMapperProfile>();
                x.AddProfile<GeneralBuildingFacilityMapperProfile>();
                #endregion END : Module Universal

                #region BEGIN : Module Transport
                /*
                
                
                x.AddProfile<TransportLostDamageClaimMapperProfile>();
                x.AddProfile<TransportVesselMonitoringMapperProfile>();
                x.AddProfile<TransportVesselMonitoringDetailMapperProfile>();
                x.AddProfile<TransportVesselMonitoringDetailViewMapperProfile>();
                x.AddProfile<TransportUnitFrequentMapperProfile>();
                x.AddProfile<TransportLoadingUnloadingMapperProfile>();
                x.AddProfile<TransportLoadUnloadDetailMapperProfile>();
                x.AddProfile<TransportTruckArrivalMapperProfile>();
                x.AddProfile<TransportPickingListLogMapperProfile>();
                x.AddProfile<TransportPickingListLogMapperProfile>();
                x.AddProfile<TransportSealMappingProfile>();
                */
                x.AddProfile<TransportSealMapperProfile>();
                x.AddProfile<TransportVehicleDataMapperProfile>();
                x.AddProfile<TransportOrderMapperProfile>();
                x.AddProfile<TransportExecutionMapperProfile>();
                x.AddProfile<TransportOrderRequestMapperProfile>();
                x.AddProfile<TransportMonitoringMapperProfile>();
                x.AddProfile<TransportVehicleMapperProfile>();
                x.AddProfile<TransportDriverManagementMapperProfile>();
                x.AddProfile<TransportRouteMapperProfile>();
                x.AddProfile<TransportVendorChangeLogMapperProfile>();
                x.AddProfile<TransportLostDamageClaimMapperProfile>();
                x.AddProfile<TOMViewModelMapperProfile>();
                x.AddProfile<TransportVesselMonitoringMapperProfile>();
                x.AddProfile<TransportExecutionStatusLogMapperProfile>();
                x.AddProfile<FnTransportationSummaryMapperProfile>();
                x.AddProfile<TransportPositionDetailMapperProfile>();
                x.AddProfile<TransportTicketNCRMapperProfile>();
                #endregion END : Module Transport
            });
        }
    }
}
