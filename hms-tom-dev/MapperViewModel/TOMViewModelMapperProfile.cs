using AutoMapper;
using TOM.Transport.Domain.DTOs;
using hms_tom_dev.Models.Transport;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using hms_tom_dev.Models.Masters;
using System;

namespace hms_tom_dev.MapperViewModel
{
    public class TOMViewModelMapperProfile : Profile
    {
        public TOMViewModelMapperProfile()
        {
            #region Master
            //CreateMap<MasterVendorTOMDTO, MasterVendorTOMViewModel>().ReverseMap();
            #endregion

            CreateMap<TransportOrder, TransportOrderViewModel>().ReverseMap();
            CreateMap<TransportOrderDTO, TransportOrderViewModel>().ReverseMap();
            CreateMap<TransportOrderDTO, TransportOrderTransportExecutionViewModel>().ReverseMap();
            CreateMap<TransportOrderDTO, TransportOrderTransportExecutionEditViewModel>().ReverseMap();
            CreateMap<TransportOrderTransportExecutionDTO, TransportOrderTransportExecutionViewModel>().ReverseMap();
            CreateMap<TransportOrderTransportExecutionEditDTO, TransportOrderTransportExecutionEditViewModel>().ReverseMap();
            CreateMap<TransportOrderDetail, TransportOrderDetailViewModel>().ReverseMap();
            CreateMap<TransportOrderDetailDTO, TransportOrderDetailViewModel>().ReverseMap();
            CreateMap<TransportOrderChangeLog, TransportOrderChangeLogViewModel>().ReverseMap();
            CreateMap<TransportOrderChangeLogDTO, TransportOrderChangeLogViewModel>().ReverseMap();
            CreateMap<TransportOrderDetailChangeLog, TransportOrderDetailChangeLogViewModel>().ReverseMap();
            CreateMap<TransportOrderDetailChangeLogDTO, TransportOrderDetailChangeLogViewModel>().ReverseMap();

            CreateMap<TransportExecution, TransportExecutionViewModel>().ReverseMap();
            CreateMap<TransportExecutionDTO, TransportExecutionViewModel>().ReverseMap();
            CreateMap<TransportOrderRequest, TransportOrderRequestViewModel>().ReverseMap();
            CreateMap<TransportMonitoringDTO, TransportMonitoringViewModel>().ReverseMap();
            CreateMap<TransportMonitoringViewDTO, TransportMonitoringViewViewModel>().ReverseMap();

            CreateMap<TransportExecutionAddNewDTO, TransportExecutionAddNewViewModel>().ReverseMap();
            CreateMap<TransportExecutionEditDTO, TransportExecutionEditViewModel>().ReverseMap();

            CreateMap<TransportRouteDTO, TransportRouteViewModel>().ReverseMap();
            CreateMap<TransportOrderRequestDTO, TransportOrderRequestViewModel>().ReverseMap();
            CreateMap<TransportPositionDetailDTO, TransportPositionDetailViewModel>().ReverseMap();
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

            CreateMap<TransportLostClaimDamageDTO, TransportLostClaimDamageViewModel>()
                .ForMember(dest => dest.SJDate, opt => opt.MapFrom(src => src.SJDate.HasValue ? src.SJDate.Value.ToString("dd-MMM-yyyy") : "")) //src.SJDate.Value.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.DateOfIncident, opt => opt.MapFrom(src => src.DateOfIncident.ToString("dd-MMM-yyyy")))
                .ForMember(dest => dest.DateOfDelivery, opt => opt.MapFrom(src => src.DateOfDelivery.ToString("dd-MMM-yyyy")))
                .ForMember(dest => dest.BSWarehouseReceiveDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.BSWarehouseReceiveDate).ToString("dd-MMM-yyyy")))
                .ForMember(dest => dest.HMSMemoDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.HMSMemoDate).ToString("dd-MMM-yyyy")))
                .ForMember(dest => dest.InvoiceDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.InvoiceDate).ToString("dd-MMM-yyyy")))
                .ForMember(dest => dest.VendorPaymentDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.VendorPaymentDate).ToString("dd-MMM-yyyy")))
                .ForMember(dest => dest.EDPSDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.EDPSDate).ToString("dd-MMM-yyyy")))
                .ForMember(dest => dest.IncinerationReportDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.IncinerationReportDate).ToString("dd-MMM-yyyy")))
                .ForMember(dest => dest.InitClaimedAmount, opt => opt.MapFrom(src => src.InitClaimedAmount.HasValue ? Convert.ToString(src.InitClaimedAmount.Value) : ""))
                .ForMember(dest => dest.InvoiceAmount, opt => opt.MapFrom(src => src.InvoiceAmount.HasValue ? Convert.ToString(src.InvoiceAmount.Value) : ""))
                .ForMember(dest => dest.GrossLossAmount, opt => opt.MapFrom(src => src.GrossLossAmount.HasValue ? Convert.ToString(src.GrossLossAmount.Value) : ""))
                //.ForMember(dest => dest.NetClaim, opt => opt.MapFrom(src => src.NetClaim.ToString()))
                ;

            CreateMap<TransportLostClaimFACodeViewModel, TransportLostClaimFACodeDTO>()
                .ForMember(dest => dest.Pack, opt => opt.MapFrom(src => src.Pack.ToString()));

            CreateMap<TransportLostClaimFACodeDTO, TransportLostClaimFACodeViewModel>()
                .ForMember(dest => dest.Pack, opt => opt.MapFrom(src => Convert.ToInt32(src.Pack)));
                    

            CreateMap<TransportVendorChangeLogDTO, TransportVendorChangeLogViewModel>().ReverseMap();
            CreateMap<TransportExecutionStatusLogDTO, TransportExecutionStatusLogViewModel>().ReverseMap();
            CreateMap<TransportOrderTransportExecutionPrintDTO, TransportOrderTransportExecutionPrintViewModel>().ReverseMap();
            CreateMap<TransportVesselMonitoringDTO, TransportVesselMonitoringViewModel>().ReverseMap();
            CreateMap<TransportExecutionPrintDTO, TransportExecutionPrintViewModel>().ReverseMap();
            CreateMap<TransportOrderDetailPrintDTO, TransportOrderDetailPrintViewModel>().ReverseMap();
            CreateMap<TransportExecutionPrintMemoDTO, TransportExecutionPrintMemoViewModel>().ReverseMap();
            CreateMap<FnTransportationSummaryDTO, FnTransportationSummaryViewModel>();
            CreateMap<FnTransportationSummaryViewModel, FnTransportationSummaryDTO>();
            CreateMap<KPILoadFactorAvgDTO, KPILoadFactorAvgViewModel>().ReverseMap();
            CreateMap<TransportSummaryReclassViewDTO, TransportSummaryReclassViewViewModel>().ReverseMap();
            CreateMap<FnTransportationSummaryCRateDTO, FnTransportationSummaryCRateViewModel>().ReverseMap();
            CreateMap<hms_tom_dev.Models.Transport.Series, TOM.Transport.Domain.DTOs.Series>().ReverseMap();
            CreateMap<hms_tom_dev.Models.Transport.SeriesDecimalsViewModel, TOM.Transport.Domain.DTOs.SeriesDecimalsDTO>().ReverseMap();
            CreateMap<TransportTicketNCRDTO, TransportTicketNCRViewModel>().ReverseMap();
        }
    }
}