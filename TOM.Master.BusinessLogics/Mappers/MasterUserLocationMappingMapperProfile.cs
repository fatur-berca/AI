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
    public class MasterUserLocationMappingMapperProfile : Profile
    {
        public MasterUserLocationMappingMapperProfile()
        {
            CreateMap<MasterUserLocationMappingDTO, MasterUserLocationMapping>();
            CreateMap<MasterUserLocationMapping, MasterUserLocationMappingDTO>();
        }
    }
}
