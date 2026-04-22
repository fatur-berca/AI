using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class MasterFunctionMapperProfile : Profile
    {
        public MasterFunctionMapperProfile()
        {
            CreateMap<MasterFunctionDTO, MasterFunction>().ReverseMap();
            //CreateMap<MasterList, MasterFunctionDTO>();
        }
    }
}
