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
    public class MasterTransportRegionMapperProfile : Profile
    {
        public MasterTransportRegionMapperProfile()
        {
            CreateMap<MasterTransportRegionDTO, MasterTransportRegion>();
            CreateMap<MasterTransportRegion, MasterTransportRegionDTO>();
        }
    }
}
