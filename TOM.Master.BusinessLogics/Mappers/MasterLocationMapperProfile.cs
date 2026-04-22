using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace TOM.Master.BusinessLogics.Mappers
{
   public class MasterLocationMapperProfile : Profile
    {
       public MasterLocationMapperProfile()
       {
           CreateMap<MasterLocationDTO, MasterLocation>().ReverseMap();
           CreateMap<MasterLocationDTO, MasterLocationInput>().ReverseMap();
       }
    }
}
