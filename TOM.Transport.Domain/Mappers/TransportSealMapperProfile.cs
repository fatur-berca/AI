using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportSealMapperProfile : Profile
    {
        public TransportSealMapperProfile()
        {
            CreateMap<TransportSeal, TransportSealDTO>().ReverseMap();
        }
    }
}
