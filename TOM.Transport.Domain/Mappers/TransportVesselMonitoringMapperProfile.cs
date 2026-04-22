using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportVesselMonitoringMapperProfile: Profile
    {
        public TransportVesselMonitoringMapperProfile()
        {
            CreateMap<TransportVesselMonitoringDTO, TransportVesselMonitoring>().ReverseMap();
            CreateMap<TransportPositionDetail, TransportPositionDetailDTO>().ReverseMap();
        }
    }
}
