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
    public class MasterDynamicFieldMapperProfile : Profile
    {
        public MasterDynamicFieldMapperProfile()
        {
            //CreateMap<MasterDynamicFieldDTO, MasterDynamicField>();
            //CreateMap<MasterDynamicField, MasterDynamicFieldDTO>();
        }
    }
}
