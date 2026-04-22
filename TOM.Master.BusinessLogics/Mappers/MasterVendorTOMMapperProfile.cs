using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class MasterVendorTOMMapperProfile : Profile
    {
        public MasterVendorTOMMapperProfile()
        {
            CreateMap<MasterVendorTOMDTO, MasterVendor>();
            CreateMap<MasterVendor, MasterVendorTOMDTO>();
            CreateMap<MasterVendorDTO, MasterVendor>();
            CreateMap<MasterVendor, MasterVendorDTO>();
        }
    }
}
