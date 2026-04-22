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
    public class CustomReportStateMapperProfile : Profile
    {
        public CustomReportStateMapperProfile()
        {
            CreateMap<CustomReportStateDTO, CustomReportState>();
            CreateMap<CustomReportState, CustomReportStateDTO>();
            CreateMap<CustomReportStateDTO, CustomReportStateInput>().ReverseMap();
        }
    }
}
