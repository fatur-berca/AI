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
    public class HRD_EMP_V2_MapperProfile : Profile
    {
        public HRD_EMP_V2_MapperProfile()
        {
           // CreateMap<HRD_EMP_V2_DTO, HRD_EMP_V2>().ReverseMap();
        }
    }
}
