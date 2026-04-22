using System.Collections.Generic;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Repositories;
using DFIS.Universal.BusinessLogics;
using DFIS.Universal.Domain.DTOs;
using TOM.Master.Domain.DTOs;

namespace TOM.Master.BusinessLogics
{
    public class MasterApprovalNewsBLL : IMasterApprovalNewsBLL
    {
        private readonly IMasterApprovalNewsRepo _masterApprovalNewsRepo;

        public MasterApprovalNewsBLL(IMasterApprovalNewsRepo masterApprovalNewsRepo)
        {
            _masterApprovalNewsRepo = masterApprovalNewsRepo;
        }

        public List<MasterApprovalNewsDTO> GetAllMasterApprovalNewsByStatus(int status)
        {
            return Mapper.Map<List<MasterApprovalNewsDTO>>(_masterApprovalNewsRepo.GetAllNewsHighlightsByStatus(status));
        }

        public void SaveData(int id, int status, string userid)
        {
            MasterApprovalNewsDTO temp = new MasterApprovalNewsDTO();
            temp.IDNewsHighlight = id;
            temp.Status = status;
            temp.UpdatedBy = userid;
            _masterApprovalNewsRepo.SaveData(temp, false);
        }

        public int UpdateSeen(MasterApprovalNewsDTO input, string controller, string userid)
        {
            return _masterApprovalNewsRepo.UpdateSeen(input, controller, userid);
        }
    }
}
