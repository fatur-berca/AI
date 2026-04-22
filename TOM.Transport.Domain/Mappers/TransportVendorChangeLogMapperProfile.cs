using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportVendorChangeLogMapperProfile : Profile
    {
        public TransportVendorChangeLogMapperProfile()
        {
            CreateMap<TransportVendorChangeLogDTO, TransportVendorChangeLog>().ReverseMap();
        }
    }
}
