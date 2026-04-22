using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using TOM.Master.Domain.DTOs;

namespace TOM.Master.BusinessLogics.Mappers
{
    public class MasterApprovalNewsMapperProfile : Profile
    {
        public MasterApprovalNewsMapperProfile()
        {
            //CreateMap<MasterApprovalNewsDTO, NewsHighlight>().ReverseMap();
            CreateMap<MasterApprovalNewsDTO, MasterApprovalNewsInput>().ReverseMap();
        }
    }
}
