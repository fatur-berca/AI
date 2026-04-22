using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportOrderRequestMapperProfile: Profile
    {
        public TransportOrderRequestMapperProfile()
        {
            CreateMap<TransportOrderRequestDTO, TransportOrderRequest>().ReverseMap().PreserveReferences();
            CreateMap<TransportOrderDTO, TransportOrder>().ReverseMap();
            CreateMap<SP_GetTransportOrderRequestDTO, SP_GetTransportOrderRequest_Result>().ReverseMap();
        }
    }
}
