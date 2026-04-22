using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportVehicleMapperProfile : Profile
    {
        public TransportVehicleMapperProfile()
        {
            CreateMap<TransportVehicleDataDTO, TransportVehicleData>().ReverseMap();
        }
    }
}
