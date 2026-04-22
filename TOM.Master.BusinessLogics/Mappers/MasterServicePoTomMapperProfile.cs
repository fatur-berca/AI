using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.Master.Domain.DTOs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class MasterServicePoTomMapperProfile : Profile
    {
        public MasterServicePoTomMapperProfile()
        {
            CreateMap<MasterServicePoTomDTO, MasterServicePo>();
            CreateMap<MasterServicePo, MasterServicePoTomDTO>();            
        }
    }
}
