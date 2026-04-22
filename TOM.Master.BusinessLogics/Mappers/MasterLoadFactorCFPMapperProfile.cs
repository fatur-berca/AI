using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;

namespace TOM.Master.BusinessLogics.Mappers
{
   public class MasterLoadFactorCFPMapperProfile : Profile
    {
       public MasterLoadFactorCFPMapperProfile()
       {
           CreateMap<MasterLoadFactorCFPDTO, MasterLoadFactorCFP>();
           CreateMap<MasterLoadFactorCFP, MasterLoadFactorCFPDTO>();

       }
    }
}
