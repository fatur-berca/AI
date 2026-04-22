using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class MasterRoleMapperProfile : Profile
    {
        public MasterRoleMapperProfile(){
            CreateMap<MasterRoleDTO, MasterRole>().ReverseMap();
        }
    }
}
