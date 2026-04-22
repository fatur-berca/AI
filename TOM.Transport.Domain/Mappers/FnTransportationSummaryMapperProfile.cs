using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Mappers
{
    public class FnTransportationSummaryMapperProfile : Profile
    {
        public FnTransportationSummaryMapperProfile ()
        {
            CreateMap<FnTransportationSummary_Result, FnTransportationSummaryDTO>().ReverseMap();
            CreateMap<KPILoadFactorAvg_Result, KPILoadFactorAvgDTO>().ReverseMap();
            CreateMap<TransportSummaryReclassView, TransportSummaryReclassViewDTO>().ReverseMap();
            CreateMap<FnTransportationSummaryCRate_Result, FnTransportationSummaryCRateDTO>().ReverseMap();
        }
    }
}
