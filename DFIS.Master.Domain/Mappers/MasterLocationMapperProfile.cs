using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DFIS.EntitiesDAL.EDMX;
using DFIS.Master.Domain.DTOs;
using DFIS.Master.Domain.Inputs;

namespace DFIS.Master.Domain.Mappers
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
