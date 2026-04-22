using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportRouteMapperProfile : Profile
    {
        public TransportRouteMapperProfile()
        {
            CreateMap<TransportRouteDTO, TransportRoute>().ReverseMap();
        }
    }
}
