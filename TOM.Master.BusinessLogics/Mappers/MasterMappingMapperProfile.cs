using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class MasterMappingMapperProfile : Profile
    {
        public MasterMappingMapperProfile()
        {
            CreateMap<MasterMappingDTO, MasterMapping>();
            CreateMap<MasterMapping, MasterMappingDTO>();

        }
    }
}
