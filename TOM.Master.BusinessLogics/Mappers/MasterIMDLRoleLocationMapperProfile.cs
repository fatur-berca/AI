using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class MasterIMDLRoleLocationMapperProfile : Profile
    {
        public MasterIMDLRoleLocationMapperProfile()
        {
            CreateMap<MasterIMDLRoleLocationDTO, MasterIMDLRoleLocation>();
            CreateMap<MasterIMDLRoleLocation, MasterIMDLRoleLocationDTO>();
        }

    }
}
