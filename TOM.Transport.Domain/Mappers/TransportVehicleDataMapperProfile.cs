using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportVehicleDataMapperProfile : Profile
    {
        public TransportVehicleDataMapperProfile()
        {
            CreateMap<TransportVehicleDataDTO, TransportVehicleData>();
            CreateMap<TransportVehicleData, TransportVehicleDataDTO>();
            CreateMap<TransportVehicleDataInput, TransportVehicleDataDTO>();
            CreateMap<TransportVehicleDataDTO, TransportVehicleDataInput>();
        }
    }
}
