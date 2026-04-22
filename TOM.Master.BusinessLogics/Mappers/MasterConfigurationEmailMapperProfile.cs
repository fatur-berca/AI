using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class MasterConfigurationEmailMapperProfile : Profile
    {
        public MasterConfigurationEmailMapperProfile()
        {
            CreateMap<MasterConfigurationEmailDTO, MasterConfigurationEmail>().ReverseMap();
        }
    }
}
