using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DFIS.EntitiesDAL.EDMX;
using DFIS.Master.Domain.DTOs;

namespace DFIS.Master.Domain.Mappers
{
    public class MasterFABrandMapperProfile : Profile
    {
        public MasterFABrandMapperProfile()
       {
           CreateMap<MasterFABrandDTO, MasterFABrand>();
           CreateMap<MasterFABrand, MasterFABrandDTO>();
       }
    }
}
