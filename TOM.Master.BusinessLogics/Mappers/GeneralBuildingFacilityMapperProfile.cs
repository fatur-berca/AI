using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class GeneralBuildingFacilityMapperProfile : Profile
    {
        public GeneralBuildingFacilityMapperProfile()
        {
            CreateMap<GeneralBuildingFacilityDTO, GeneralBuildingFacility>().ReverseMap();
        }
    }
}
