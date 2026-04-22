using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportExecutionMapperProfile : Profile
    {
        public TransportExecutionMapperProfile()
        {
            CreateMap<TransportExecutionDTO, TransportExecution>().ReverseMap();
            CreateMap<TransportExecutionAddNewDTO, FN_TRANSPORT_EXECUTION_LIST_ADD_Result>().ReverseMap();
            CreateMap<TransportExecutionEditDTO, TransportExecution>().ReverseMap();
            CreateMap<TransportExecutionAddNewDTO, TransportExecutionTemp>().ReverseMap();
        }
    }
}
