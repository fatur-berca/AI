using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportExecutionStatusLogMapperProfile: Profile
    {
        public TransportExecutionStatusLogMapperProfile()
        {
            CreateMap<TransportExecutionStatusLogDTO, TransportExecutionStatusLog>().ReverseMap();
        }
    }
}
