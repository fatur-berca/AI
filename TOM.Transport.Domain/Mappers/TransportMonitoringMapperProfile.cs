using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportMonitoringMapperProfile: Profile
    {
        public TransportMonitoringMapperProfile()
        {
            CreateMap<TransportVesselMonitoring, TransportMonitoringDTO>().ReverseMap();
            CreateMap<TransportMonitoringView, TransportMonitoringViewDTO>().ReverseMap();
        }
    }
}
