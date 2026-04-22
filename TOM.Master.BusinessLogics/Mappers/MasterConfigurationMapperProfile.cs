using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class MasterConfigurationMapperProfile : Profile
    {
        public MasterConfigurationMapperProfile()
        {
            CreateMap<MasterConfigurationDTO, MasterConfiguration>().ReverseMap();
            CreateMap<MasterConfigurationInput, MasterConfigurationDTO>().ReverseMap();

            //CreateMap<NewsHighlight, NewsHighlightDTO>().ReverseMap();
        }
    }
}
