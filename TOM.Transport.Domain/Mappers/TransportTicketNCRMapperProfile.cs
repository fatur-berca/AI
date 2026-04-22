using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportTicketNCRMapperProfile : Profile
    {
        public TransportTicketNCRMapperProfile()
        {
            CreateMap<TransportTicketNCRDTO, TransportTicketNCR>().ReverseMap();
        }
    }
}
