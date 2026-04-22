using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportOrderMapperProfile: Profile
    {
        public TransportOrderMapperProfile()
        {
            CreateMap<TransportOrderDTO, TransportOrder>().ReverseMap();
            CreateMap<TransportOrderTransportExecutionDTO, TransportOrder>().ReverseMap();
            CreateMap<TransportOrderTransportExecutionEditDTO, TransportOrder>().ReverseMap();
            CreateMap<TransportOrderTransportExecutionPrintDTO, TransportOrder>().ReverseMap();
            CreateMap<TransportOrderTransportExecutionEditDTO, TransportOrderDTO>().ReverseMap();
            CreateMap<TransportOrderDetailDTO, TransportOrderDetail>().ReverseMap();
            CreateMap<TransportOrderChangeLogDTO, TransportOrderChangeLog>().ReverseMap();
            CreateMap<TransportOrderDetailChangeLogDTO, TransportOrderDetailChangeLog>().ReverseMap();
            CreateMap<TransportPickingListLog, TransportPickingListLogDTO>().ReverseMap();
            CreateMap<TransportOrderDetailPrintDTO, TransportOrderDetail>().ReverseMap();
            CreateMap<TransportOrderDTO, TransportOrderList>().ReverseMap();
            CreateMap<TransportOrder, TransportOrderList>().ReverseMap();
            CreateMap<LoadingUnloadingExportView, LoadingUnloadingExcelViewDTO>().ReverseMap();

            CreateMap<TransportOrderRequestSummary, TransportOrderRequestSummaryDTO>().ReverseMap();
        }
    }
}
