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
    public class MasterListMapperProfile : Profile
    {
        public MasterListMapperProfile()
	    {
            CreateMap<MasterListDTO, MasterList>();
            CreateMap<MasterList, MasterListDTO>();
	    }
    }
}
