using AutoMapper;
using TOM.Transport.Domain.DTOs;
using hms_tom_dev.Models.Transport;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using hms_tom_dev.Models.Masters;

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
            CreateMap<TransportOrderTransportExecutionDTO, TransportOrderTransportExecutionViewModel>().ReverseMap();
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
            CreateMap<TransportOrderRequestDTO, TransportOrderRequestViewModel>().ReverseMap();

            CreateMap<TransportOrderTransportExecutionPrintDTO, TransportOrderTransportExecutionPrintViewModel>().ReverseMap();
            CreateMap<TransportVesselMonitoringDTO, TransportVesselMonitoringViewModel>().ReverseMap();
            CreateMap<TransportExecutionPrintDTO, TransportExecutionPrintViewModel>().ReverseMap();            
            CreateMap<TransportOrderDetailPrintDTO, TransportOrderDetailPrintViewModel>().ReverseMap();
            CreateMap<TransportExecutionPrintMemoDTO, TransportExecutionPrintMemoViewModel>().ReverseMap();
        }
    }
}