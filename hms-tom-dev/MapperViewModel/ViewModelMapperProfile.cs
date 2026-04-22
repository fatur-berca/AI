using System;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using TOM.Master.Domain.DTOs;
using DFIS.Universal.Domain.Outputs;
using hms_tom_dev.Models.Masters;
using hms_tom_dev.Models.Transport;
using hms_tom_dev.Models.Universal;
using TOM.Master.Domain.Inputs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace hms_tom_dev.MapperViewModel
{
    public class ViewModelMapperProfile : Profile
    {
        public ViewModelMapperProfile()
        {
            // added for mapping siga role 
            /**
             * by Ahadian Akbar
             */
            CreateMap<MappingSigaRoleDTO, MappingSigaRoleViewModel>();

            CreateMap<MasterListDTO, MasterListViewModel>();
            CreateMap<MasterListViewModel, MasterListDTO>();
            
            CreateMap<MasterFunction, MasterFunctionViewModel>().ReverseMap();
            CreateMap<MasterFunction, MasterFunctionViewModel>();

            CreateMap<MasterFunctionDTO, MasterFunctionViewModel>();
            CreateMap<MasterFunctionViewModel, MasterFunctionDTO>();

            CreateMap<MasterDynamicFieldDTO, MasterDynamicFieldViewModel>();
            CreateMap<MasterDynamicFieldViewModel, MasterDynamicFieldDTO>();
            
            CreateMap<MasterLocationDTO, MasterLocationViewModel>();
            CreateMap<MasterLocationViewModel, MasterLocationDTO>();

            CreateMap<TransactionLogDTO, TransactionLogViewModel>();            
            CreateMap<TransactionLogViewModel, TransactionLogDTO>();

            CreateMap<MasterFABrandDTO, MasterFABrandViewModel>();
            CreateMap<MasterFABrandViewModel, MasterFABrandDTO>();

            CreateMap<MasterLoadFactorCFPDTO, MasterLoadFactorCFPViewModel>();
            CreateMap<MasterLoadFactorCFPViewModel, MasterLoadFactorCFPDTO>();

            CreateMap<MasterFGStackingDTO, MasterFGStackingViewModel>();
            CreateMap<MasterFGStackingViewModel, MasterFGStackingDTO>();

            CreateMap<MasterTransportRegionDTO, MasterTransportRegionViewModel>();
            CreateMap<MasterTransportRegionViewModel, MasterTransportRegionDTO>();

            CreateMap<MasterDistanceDTO, MasterDistanceViewModel>();
            CreateMap<MasterDistanceViewModel, MasterDistanceDTO>();           

            CreateMap<MasterMappingDTO, MasterMappingViewModel>();
            CreateMap<MasterMappingViewModel, MasterMappingDTO>();

            CreateMap<MasterUserRoleMappingViewModel, MasterUserRoleMappingDTO>();
            CreateMap<MasterUserRoleMappingDTO, MasterUserRoleMappingViewModel>();
            
            CreateMap<MasterConfiguration, MasterConfigurationViewModel>().ReverseMap();
            CreateMap<MasterConfigurationDTO, MasterConfigurationViewModel>().ReverseMap();

            //CreateMap<NewsHighlight, MasterApprovalNewsViewModel>().ReverseMap();
            CreateMap<MasterApprovalNewsDTO, MasterApprovalNewsViewModel>().ReverseMap();
            
            //CreateMap<MasterGuideline, MasterGuidelineViewModel>().ReverseMap();
            CreateMap<MasterGuidelineDTO, MasterGuidelineViewModel>().ReverseMap();
            
            CreateMap<MasterRolesFunctionMapping, MasterRoleFunctionViewModel>().ReverseMap();
            CreateMap<MasterRoleFunctionDTO, MasterRoleFunctionViewModel>().ReverseMap();
            
            CreateMap<MasterUserLocationMappingDTO, MasterUserLocationMappingViewModel>().ReverseMap();
            CreateMap<NotificationDTO, NotificationViewModel>().ReverseMap();
            
            CreateMap<MasterTruckSealDTO, MasterTruckSealViewModel>();
            CreateMap<MasterTruckSealViewModel, MasterTruckSealDTO>();

            CreateMap<MasterUserDTO, MasteUserViewModel>();
            CreateMap<MasteUserViewModel, MasterUserDTO>();
            
            CreateMap<CustomReportStateDTO, CustomReportStateViewModel>().ReverseMap();
            CreateMap<CustomReportState, CustomReportStateViewModel>().ReverseMap();

            CreateMap<TransportDriverManagementDTO, TransportDriverManagementViewModel>().ReverseMap();
            CreateMap<TransportDriverManagement, TransportDriverManagementViewModel>().ReverseMap();

            CreateMap<TransportOrderDTO, TransportOrderViewModel>().ReverseMap();
            
            CreateMap<TransportVehicleDataDTO, TransportVehicleDataViewModel>().ReverseMap();
            CreateMap<TransportVehicleData, TransportVehicleDataViewModel>().ReverseMap();

            CreateMap<TransportLostClaimDamageViewModel, TransportLostClaimDamageDTO>()
                //.ForMember(dest => dest.SJDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.SJDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.SJDate, opt => opt.MapFrom(src => DateTime.Parse(src.SJDate)))
                .ForMember(dest => dest.DateOfIncident, opt => opt.MapFrom(src => DateTime.ParseExact(src.DateOfIncident, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                //.ForMember(dest => dest.DateOfDelivery, opt => opt.MapFrom(src => DateTime.ParseExact(src.DateOfDelivery, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.DateOfDelivery, opt => opt.MapFrom(src => DateTime.Parse(src.DateOfDelivery)))
                .ForMember(dest => dest.BSWarehouseReceiveDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.BSWarehouseReceiveDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.BSWarehouseReceiveDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.BSWarehouseReceiveDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.HMSMemoDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.HMSMemoDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.InvoiceDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.InvoiceDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.VendorPaymentDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.VendorPaymentDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.EDPSDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.EDPSDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.IncinerationReportDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.IncinerationReportDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                //.ForMember(dest => dest.InvoiceAmount, opt => opt.MapFrom(src => Convert.ToDecimal(src.InvoiceAmount)))
                //.ForMember(dest => dest.GrossLossAmount, opt => opt.MapFrom(src => Convert.ToDecimal(src.GrossLossAmount)))
                //.ForMember(dest => dest.NetClaim, opt => opt.MapFrom(src => Convert.ToDecimal(src.NetClaim)))
                ;
            
            CreateMap<HRD_EMP_V2_DTO, HRD_EMP_V2_ViewModel>().ReverseMap();
            CreateMap<UserLocationMap, MasterLocationViewModel>().ReverseMap();
            
            //CreateMap<MasterLeadTimeDTO, MasterLeadTimeViewModel>().ReverseMap();

            CreateMap<TransportLoadingUnloadingViewModel, TransportLoadingUnloadingDTO>();
            CreateMap<TransportLoadingUnloadingDTO, TransportLoadingUnloadingViewModel>();

            CreateMap<TransportLoadingUnloadingDetailViewModel, TransportLoadingUnloadingDetailDTO>();
            CreateMap<TransportLoadingUnloadingDetailDTO, TransportLoadingUnloadingDetailViewModel>();

            CreateMap<MasterKilometerViewModel, MasterKilometerDTO>().ReverseMap();

            //CreateMap<TransportTruckArrivalViewModel,TransportTruckArrivalDTO>().ReverseMap();

            CreateMap<TransportPickingListLogDTO, TransportPickingListLogViewModel>();
            CreateMap<TransportPickingListLogViewModel, TransportPickingListLogDTO>();

            CreateMap<TransportOrderDTO, TransportOrderViewModel>();
            CreateMap<TransportOrderViewModel, TransportOrderDTO>();

            CreateMap<TransportOrderRequestDTO, TransportOrderRequestViewModel>();
            CreateMap<TransportOrderRequestViewModel, TransportOrderRequestDTO>();

            CreateMap<TransportOrderDetailDTO, TransportOrderDetailViewModel>();
            CreateMap<TransportOrderDetailViewModel, TransportOrderDetailDTO>();
            
            CreateMap<MasterIMDLRoleDTO, MasterIMDLRoleViewModel>();
            CreateMap<MasterIMDLRoleViewModel, MasterIMDLRoleDTO>();

            CreateMap<MasterIMDLRoleDTO, MasterIMDLRole>();
            CreateMap<MasterIMDLRole, MasterIMDLRoleDTO>();

            CreateMap<MasterIMDLRoleLocationDTO, MasterIMDLRoleLocationViewModel>();
            CreateMap<MasterIMDLRoleLocationViewModel, MasterIMDLRoleLocationDTO>();

            CreateMap<MasterIMDLRoleLocationDTO, MasterIMDLRoleLocation>();
            CreateMap<MasterIMDLRoleLocation, MasterIMDLRoleLocationDTO>();

            CreateMap<MasterVendor, MasterVendorTOMViewModel>().ReverseMap();
            CreateMap<MasterVendorDTO, MasterVendorViewModel>().ReverseMap();
            CreateMap<MasterVendorTOMDTO, MasterVendorTOMViewModel>().ReverseMap();

            CreateMap<MasterCost, MasterCostViewModel>().ReverseMap();
            CreateMap<MasterCostDTO, MasterCostViewModel>().ReverseMap();
            
            CreateMap<MasterVendorSuggestion, MasterVendorSuggestionViewModel>().ReverseMap();
            CreateMap<MasterVendorSuggestionDTO, MasterVendorSuggestionViewModel>().ReverseMap();

            CreateMap<MasterUomDTO, MasterUomViewModel>().ReverseMap();

            CreateMap<MasterLeadTime, MasterLeadTimeTomViewModel>().ReverseMap();

            //CreateMap<MasterServicePoTomDTO, MasterServicePoTomViewModel>().ReverseMap();

            CreateMap<MasterServicePo, MasterServicePoTomViewModel>().ReverseMap();
            CreateMap<MasterServicePoTomDTO, MasterServicePoTomViewModel>().ReverseMap();

            CreateMap<MasterLeadTimeTomDTO, MasterLeadTimeTomViewModel>().ReverseMap();

            CreateMap<MasterCostCenter, MasterCostCenterAccountViewModel>().ReverseMap();
            CreateMap<MasterCostCenterAccountDTO, MasterCostCenterAccountViewModel>().ReverseMap();

            CreateMap<UserLocationDelegation, UserLocationDelegationDTO>().ReverseMap();
            //.ForMember(x => x.MasterLocation, opt => opt.MapFrom(x => x.MasterMappingLocation));

            CreateMap<MasterUserRoleDelegationViewDTO, MasterUserRoleDelegationViewViewModel>().ReverseMap();

            CreateMap<MasterUserRoleDelegationDTO, MasterUserRoleDelegationViewModel>().ReverseMap();

            CreateMap<TransportSealDTO, TransportSealViewModel>().ReverseMap();
            CreateMap<TransportSealDTO, TransportSealInput>().ReverseMap();
            CreateMap<MasterLocationDTO, MasterLocationViewModel>().ReverseMap();
            CreateMap<GeneralBuildingFacilityDTO, GeneralBuildingFacilityViewModel>().ReverseMap();
        }
    }
}
