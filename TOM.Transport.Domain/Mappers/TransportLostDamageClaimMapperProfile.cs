using AutoMapper;
//using DFIS.EntitiesDAL.EDMX;
//using DFIS.Transport.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Domain.Mappers
{
    public class TransportLostDamageClaimMapperProfile: Profile
    {
        public TransportLostDamageClaimMapperProfile()
        {
            CreateMap<TransportLostClaimDamage, TransportLostClaimDamageDTO>().ReverseMap();
        }
    }
}
