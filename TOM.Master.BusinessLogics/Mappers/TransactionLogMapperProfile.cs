using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class TransactionLogMapperProfile : Profile
    {
        public TransactionLogMapperProfile()
	    {
            CreateMap<TransactionLogDTO, TransactionLog>();
            CreateMap<TransactionLog, TransactionLogDTO>();
	    }
    }
}
