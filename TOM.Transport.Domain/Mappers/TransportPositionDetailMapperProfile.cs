using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportPositionDetailMapperProfile : Profile
    {
        public TransportPositionDetailMapperProfile()
        {
            CreateMap<TransportPositionDetailDTO, TransportPositionDetail>().ReverseMap();
        }
    }
}
