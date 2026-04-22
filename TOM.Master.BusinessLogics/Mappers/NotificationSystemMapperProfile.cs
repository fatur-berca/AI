using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class NotificationSystemMapperProfile : Profile
    {
        public NotificationSystemMapperProfile()
        {
            CreateMap<NotificationDTO, NotificationSystem>();
            CreateMap<NotificationSystem, NotificationDTO>();
        }
    }
}
