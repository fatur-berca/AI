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
    public class MasterUserRoleMapperMappingProfile : Profile
    {
        public MasterUserRoleMapperMappingProfile()
        {
            CreateMap<MasterUserRoleMappingDTO, MasterUserRoleMapping>();
            CreateMap<MasterUserRoleMapping, MasterUserRoleMappingDTO>().ForMember(
                idr => idr.MasterUser,
                kn => kn.Ignore()
                );


        }
    }
}
