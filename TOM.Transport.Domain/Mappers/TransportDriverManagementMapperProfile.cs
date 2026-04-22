using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportDriverManagementMapperProfile : Profile
    {
        public TransportDriverManagementMapperProfile()
        {
            CreateMap<TransportDriverManagementDTO, TransportDriverManagement>().ReverseMap();
            CreateMap<TransportDriverManagementDTO, TransportDriverManagementInput>().ReverseMap();
            CreateMap<TransportDriverManagementFilterDTO, TransportDriverManagement>().ReverseMap();
        }
    }
}
