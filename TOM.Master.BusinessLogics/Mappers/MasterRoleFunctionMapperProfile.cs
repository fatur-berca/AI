using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class MasterRoleFunctionMapperProfile : Profile
    {
        public MasterRoleFunctionMapperProfile()
        {
            CreateMap<MasterRoleFunctionDTO, MasterRolesFunctionMapping>().ReverseMap();
            CreateMap<MasterRoleFunctionDTO, MasterRolesFunctionMapping>();
            CreateMap<MasterRolesFunctionMapping, MasterRoleFunctionDTO>();


          
        }
    }
}
