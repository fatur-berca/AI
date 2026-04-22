using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class MasterCostMapperProfile : Profile
    {
        public MasterCostMapperProfile()
        {
            CreateMap<MasterCostDTO, MasterCost>();
            CreateMap<MasterCost, MasterCostDTO>();
            CreateMap<MasterCostDTO, MasterCostLatestEffectiveDateDataView>();
            CreateMap<MasterCostLatestEffectiveDateDataView, MasterCostDTO>();
        }
    }
}
