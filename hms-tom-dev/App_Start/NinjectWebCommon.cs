using DFIS.Contracts;
using TOM.EntitiesDAL;
using TOM.Master.BusinessLogics;
using TOM.Master.Repositories;
using hms_tom_dev.Code;
using TOM.Transport.BusinessLogics.TransportTicketNCRBLL;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(hms_tom_dev.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(hms_tom_dev.App_Start.NinjectWebCommon), "Stop")]

namespace hms_tom_dev.App_Start
{
    using System;
    using System.Web;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using TOM.Master.BusinessLogics;
    using TOM.Master.Repositories;
    using TOM.Master.BusinessLogics;
    using DFIS.Universal.Repositories;
    using DFIS.Universal.BusinessLogics;
    using DFIS.Universal.Repositories;
    using TOM.Transport.BusinessLogics;
    using TOM.Transport.Repositories;
    using TOM.Transport.BusinessLogics;
    using TOM.Transport.Repositories.TransportOrderRequestRepo;
    using TOM.Transport.Repositories;
    using TOM.Transport.Repositories.TransportExecutionRepo;
    using TOM.Transport.BusinessLogics.TransportExecutionBLL;
    using TOM.Transport.Repositories.TransportOrderRepo;
    using TOM.Transport.BusinessLogics.TransportSealBLL;
    using TOM.Transport.BusinessLogics.TransportMonitoringBLL;
    using TOM.Transport.Repositories.TransportRouteRepo;
    using TOM.Transport.Repositories.TransportVendorChangeLogRepo;
    using TOM.Transport.Repositories.TransportVesselMonitoringRepo;
    using TOM.Transport.BusinessLogics.TransportSummaryBLL;
    using TOM.Transport.Repositories.TransportVendorChangeLogRepo;
    using TOM.Transport.BusinessLogics.TransportLostClaimDamageBLL;
    using TOM.Transport.Repositories.TransportLostClaimDamageRepo;
    using TOM.Transport.Repositories.TransportTicketNCRRepo;
    using TOM.Transport.Repositories.TransportExecutionTempRepo;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            #region Master
            kernel.Bind<IMasterListRepo>().To<MasterListRepo>();
            kernel.Bind<IMasterListBLL>().To<MasterListBLL>();

            //kernel.Bind<IMasterDynamicFieldRepo>().To<MasterDynamicFieldRepo>();
            //kernel.Bind<IMasterDynamicFieldBLL>().To<MasterDynamicFieldBLL>();

            kernel.Bind<ITransactionLogRepo>().To<TransactionLogRepo>();
            kernel.Bind<ITransactionLogBLL>().To<TransactionLogBLL>();


            kernel.Bind<IMasterFunctionRepo>().To<MasterFunctionRepo>();
            kernel.Bind<IMasterFunctionBLL>().To<MasterFunctionBLL>();

            kernel.Bind<IMasterConfigurationRepo>().To<MasterConfigurationRepo>();
            kernel.Bind<IMasterConfigurationBLL>().To<MasterConfigurationBLL>();

            //kernel.Bind<IMasterApprovalNewsRepo>().To<MasterApprovalNewsRepo>();
            //kernel.Bind<IMasterApprovalNewsBLL>().To<MasterApprovalNewsBLL>();

            kernel.Bind<IMasterLocationRepo>().To<MasterLocationRepo>();
            kernel.Bind<IMasterLocationBLL>().To<MasterLocationBLL>();

            kernel.Bind<TOM.Master.BusinessLogics.UtilitiesBLL.IMasterUserBLL>().To<TOM.Master.BusinessLogics.UtilitiesBLL.MasterUserBLL>();

            kernel.Bind<IMasterFABrandRepo>().To<MasterFABrandRepo>();
            kernel.Bind<IMasterFABrandBLL>().To<MasterFABrandBLL>();

            kernel.Bind<IMasterLoadFactorCFPRepo>().To<MasterLoadFactorCFPRepo>();
            kernel.Bind<IMasterLoadFactorCFPBLL>().To<MasterLoadFactorCFPBLL>();

            //kernel.Bind<IMasterFGStackingRepo>().To<MasterFGStackingRepo>();
            //kernel.Bind<IMasterFGStackingBLL>().To<MasterFGStackingBLL>();

            kernel.Bind<IMasterTruckSealRepo>().To<MasterTruckSealRepo>();
            kernel.Bind<IMasterTruckSealBLL>().To<MasterTruckSealBLL>();

            kernel.Bind<IMasterRoleFunctionRepo>().To<MasterRoleFunctionRepo>();
            kernel.Bind<IMasterRoleFunctionBLL>().To<MasterRoleFunctionBLL>();

            //kernel.Bind<IMasterGuidelineRepo>().To<MasterGuidelineRepo>();
            //kernel.Bind<IMasterGuidelineBLL>().To<MasterGuidelineBLL>();

            kernel.Bind<IMasterUserLocationMappingRepo>().To<MasterUserLocationMappingRepo>();
            kernel.Bind<IMasterUserLocationMappingBLL>().To<MasterUserLocationMappingBLL>();

            kernel.Bind<IMasterMappingRepo>().To<MasterMappingRepo>();
            kernel.Bind<IMasterMappingBLL>().To<MasterMappingBLL>();

            kernel.Bind<IMasterGenWeekRepo>().To<MasterGenWeekRepo>();

            kernel.Bind<IMasterKilometerRepo>().To<MasterKilometerRepo>();
            kernel.Bind<IMasterKilometerBLL>().To<MasterKilometerBLL>();
            
            kernel.Bind<IMasterIMDLRoleBLL>().To<MasterIMDLRoleBLL>();
            kernel.Bind<IMasterIMDLRoleRepo>().To<MasterIMDLRoleRepo>();

            kernel.Bind<IMasterIMDLRoleLocationBLL>().To<MasterIMDLRoleLocationBLL>();
            kernel.Bind<IMasterIMDLRoleLocationRepo>().To<MasterIMDLRoleLocationRepo>();

            kernel.Bind<IMasterVendorTOMBLL>().To<MasterVendorTOMBLL>();
            kernel.Bind<IMasterVendorTOMRepo>().To<MasterVendorTOMRepo>();

            kernel.Bind<IMasterCostBLL>().To<MasterCostBLL>();
            kernel.Bind<IMasterCostRepo>().To<MasterCostRepo>();

            kernel.Bind<IMasterDistanceRepo>().To<MasterDistanceRepo>();
            kernel.Bind<IMasterDistanceBLL>().To<MasterDistanceBLL>();

            kernel.Bind<IMasterVendorSuggestionRepo>().To<MasterVendorSuggestionRepo>();
            kernel.Bind<IMasterVendorSuggestionBLL>().To<MasterVendorSuggestionBLL>();

            kernel.Bind<IMasterLeadTimeTomRepo>().To<MasterLeadTimeTomRepo>();
            kernel.Bind<IMasterLeadTimeTomBLL>().To<MasterLeadTimeTomBLL>();

            kernel.Bind<IMasterUomRepo>().To<MasterUomRepo>();
            kernel.Bind<IMasterUomBLL>().To<MasterUomBLL>();

            kernel.Bind<IMasterServicePoTomBLL>().To<MasterServicePoTomBLL>();

            kernel.Bind<IMasterCostCenterAccountRepo>().To<MasterCostCenterAccountRepo>();
            kernel.Bind<IMasterCostCenterAccountBLL>().To<MasterCostCenterAccountBLL>();

            kernel.Bind<IMasterUserRoleDelegationBLL>().To<MasterUserRoleDelegationBLL>();
            kernel.Bind<IMasterUserRoleDelegationRepo>().To<MasterUserRoleDelegationRepo>();

            kernel.Bind<IMasterUserRoleBLL>().To<MasterUserRoleBLL>();

            kernel.Bind<IMasterUserBLL>().To<MasterUserBLL>();
            kernel.Bind<IMasterUserRepo>().To<MasterUserRepo>();
            kernel.Bind<IMasterServicePORepo>().To<MasterServicePORepo>();
            #endregion Master

            #region Util
            kernel.Bind<IHomeBLL>().To<HomeBLL>();
            kernel.Bind<IUtilitiesBLL>().To<UtilitiesBLL>();
            #endregion Util

            #region Universal
            kernel.Bind<ICustomReportStateRepo>().To<CustomReportStateRepo>();
            kernel.Bind<ICustomReportStateBLL>().To<CustomReportStateBLL>();
            kernel.Bind<INotificationSystemRepo>().To<NotificationSystemRepo>();
            #endregion Universal

            #region Transaction
            kernel.Bind<ITransportDriverManagementRepo>().To<TransportDriverManagementRepo>();
            kernel.Bind<ITransportDriverManagementBLL>().To<TransportDriverManagementBLL>();
            kernel.Bind<ITransportOrderChangeLogRepo>().To<TransportOrderChangeLogRepo>();
            kernel.Bind<ITransportOrderDetailChangeLogRepo>().To<TransportOrderDetailChangeLogRepo>();
            kernel.Bind<ITransportOrderDetailRepo>().To<TransportOrderDetailRepo>();
            kernel.Bind<ITransportOrderRepo>().To<TransportOrderRepo>();
            kernel.Bind<ITransportOrderBLL>().To<TransportOrderBLL>();
            kernel.Bind<ITransportOrderRequestRepo>().To<TransportOrderRequestRepo>();
            kernel.Bind<ITransportExecutionBLL>().To<TransportExecutionBLL>();
            kernel.Bind<ITransportExecutionRepo>().To<TransportExecutionRepo>();
            kernel.Bind<ITransportPickingListLogRepo>().To<TransportPickingListLogRepo>();
            kernel.Bind<ITransportPickingListBLL>().To<TransportPickingListBLL>();
            kernel.Bind<ITransportTruckArrivalRepo>().To<TransportTruckArrivalRepo>();
            kernel.Bind<ITransportTruckArrivalBLL>().To<TransportTruckArrivalBLL>();
            kernel.Bind<ITransportVehicleDataRepo>().To<TransportVehicleDataRepo>();
            kernel.Bind<ITransportSealBLL>().To<TransportSealBLL>();
            kernel.Bind<ITransportMonitoringBLL>().To<TransportMonitoringBLL>();
            kernel.Bind<ITransportRouteRepo>().To<TransportRouteRepo>();
            kernel.Bind<ITransportVesselMonitoringRepo>().To<TransportVesselMonitoringRepo>();
            kernel.Bind<ITransportVendorChangeLogRepo>().To<TransportVendorChangeLogRepo>();
            kernel.Bind<ITransportVehicleDataBLL>().To<TransportVehicleDataBLL>();
            kernel.Bind<ITransportUnitFrequentBLL>().To<TransportUnitFrequentBLL>();

            kernel.Bind<ITransportLostClaimDamageBLL>().To<TransportLostClaimDamageBLL>();
            kernel.Bind<ITransportLostClaimDamageRepo>().To<TransportLostClaimDamageRepo>();
            kernel.Bind<ITransportExecutionTempRepo>().To<TransportExecutionTempRepo>();
            #endregion

            kernel.Bind(typeof(IGenericRepository<>)).To(typeof(TOMGenericRepository<>));
            kernel.Bind(typeof(ITOMGenericRepository<>)).To(typeof(TOMGenericRepository<>));

            kernel.Bind<IFormsAuthentication>().To<FormsAuthenticationService>();

            kernel.Bind<IMasterMappingLocationBLL>().To<MasterMappingLocationBLL>();
            kernel.Bind<IMasterMappingLocationRepo>().To<MasterMappingLocationRepo>();

            kernel.Bind<ITransportSummaryBLL>().To<TransportSummaryBLL>();
            kernel.Bind<ITransportOrderRequestListViewRepo>().To<TransportOrderRequestListViewRepo>();

            kernel.Bind<ITransportTicketNCRBLL>().To<TransportTicketNCRBLL>();
            kernel.Bind<ITransportTicketNCRRepo>().To<TransportTicketNCRRepo>();

            kernel.Bind<IMasterVendorRepo>().To<MasterVendorRepo>();

            kernel.Bind<ITransportOrderRequestHeaderRepo>().To<TransportOrderRequestHeaderRepo>();
        }
    }
}
