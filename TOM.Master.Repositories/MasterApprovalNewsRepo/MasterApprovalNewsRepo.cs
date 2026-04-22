using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Repositories;
using TOM.Master.Domain.DTOs;
using AutoMapper;
using System.Data.Entity;

namespace TOM.Master.Repositories
{
    public interface IMasterApprovalNewsRepo : IMasterApprovalNewsRepo<NewsHighlight> { }

    public class MasterApprovalNewsRepo : TOMGenericRepository<NewsHighlight>, IMasterApprovalNewsRepo
    {
        public MasterApprovalNewsRepo(TOMContextDB contextEntities) : base(contextEntities)
        {

        }

        public List<MasterApprovalNewsDTO> GetAllNewsHighlights()
        {
            var queryFilter = PredicateHelper.True<NewsHighlight>();
            return Mapper.Map<List<MasterApprovalNewsDTO>>(
             Get(queryFilter).OrderByDescending(x => x.CreatedDate).ToList());
        }

        public List<MasterApprovalNewsDTO> GetAllNewsHighlightsByStatus(int status)
        {
            var queryFilter = PredicateHelper.True<NewsHighlight>();
            if (status != -1)
                queryFilter = queryFilter.And(m => m.Status == status);
            return Mapper.Map<List<MasterApprovalNewsDTO>>(Get(queryFilter).OrderByDescending(x => x.CreatedDate).ToList());
        }

        public NewsHighlight Checkavailability(MasterApprovalNewsDTO input)
        {
            var queryFilter = PredicateHelper.True<NewsHighlight>();
            queryFilter = queryFilter.And(m => m.IDNewsHighlight == input.IDNewsHighlight);
            return Get(queryFilter).SingleOrDefault();
        }

        public MasterApprovalNewsDTO SaveData(MasterApprovalNewsDTO input, bool status)
        {
            if (input.FileUpload == null)
                input.FileUpload = "";
            if (input.Image == null)
                input.Image = "";
            if (status)
            {
                input.CreatedDate = DateTime.Now;
                input.UpdatedDate = DateTime.Now;
                Insert(Mapper.Map<NewsHighlight>(input));
            }
            else
            {
                NewsHighlight temp = Checkavailability(input);
                temp.Status = input.Status;
                //temp.IsActive = input.IsActive;
                temp.IsActive = true;
                temp.UpdatedBy = input.UpdatedBy;
                temp.UpdatedDate = DateTime.Now;
                if (input.Status == 2)
                    temp.Remarks = "Approved by " + input.UpdatedBy + " on " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm");
                else if (input.Status == 0)
                    temp.Remarks = "Rejected by " + input.UpdatedBy + " on " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm");
                else
                    temp.Remarks = "";
                Update(temp);
            }
            Save();
            return input;
        }

        public int UpdateSeen(MasterApprovalNewsDTO input, string controller, string userid)
        {
            TOMContextDB context = new TOMContextDB();

            throw new NotImplementedException();
            NewsHighlight dbRes = null;
            //context.NewsHighlights.FirstOrDefault(x => x.IDNewsHighlight == input.IDNewsHighlight);

            // update
            if (dbRes != null)
            {
                var clicked = (dbRes.Click == null) ? 0 : dbRes.Click;

                dbRes.Click = Convert.ToInt16(clicked) + 1;
            }

            return context.SaveChanges();
        }
    }
}
