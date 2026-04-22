using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class MasterVendorSuggestionMapperProfile : Profile
    {
        public MasterVendorSuggestionMapperProfile()
        {
            CreateMap<MasterVendorSuggestionDTO, MasterVendorSuggestion>();
            CreateMap<MasterVendorSuggestion, MasterVendorSuggestionDTO>();
        }
    }
}
