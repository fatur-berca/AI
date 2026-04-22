using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class MasterUserRoleDelegationMappingProfile : Profile
    {
        public MasterUserRoleDelegationMappingProfile()
        {
            CreateMap<MasterUserRoleDelegationDTO, MasterUserRoleDelegationInput>().ReverseMap();
            CreateMap<MasterUserRoleDelegationDTO, MasterUserRoleDelegation>().ReverseMap();
            CreateMap<MasterUserRoleDelegationViewDTO, MasterUserRoleDelegationView>().ReverseMap();
        }
    }
}
