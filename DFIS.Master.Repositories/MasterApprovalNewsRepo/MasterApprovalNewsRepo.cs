using System;
using System.Collections.Generic;
using System.Linq;
using DFIS.EntitiesDAL;
using DFIS.EntitiesDAL.EDMX;
using DFIS.Utils;
using DFIS.Universal.Domain.DTOs;
using DFIS.Master.Domain.DTOs;

namespace DFIS.Master.Repositories.MasterApprovalNewsRepo
{
    public class MasterApprovalNewsRepo : GenericRepository<NewsHighlight>, IMasterApprovalNewsRepo
    {
        public MasterApprovalNewsRepo(DFISContextDB contextEntities) : base(contextEntities)
        {

        }

        public List<NewsHighlight> GetAllNewsHighlights()
        {
            var queryFilter = PredicateHelper.True<NewsHighlight>();
            return Get(queryFilter).OrderByDescending(x => x.CreatedDate).ToList();
        }

        public List<NewsHighlight> GetAllNewsHighlightsByStatus(int status)
        {
            var queryFilter = PredicateHelper.True<NewsHighlight>();
            if(status != -1)
                queryFilter = queryFilter.And(m => m.Status == status);
            return Get(queryFilter).OrderByDescending(x => x.CreatedDate).ToList();
        }

        public NewsHighlight Checkavailability(NewsHighlight input)
        {
            var queryFilter = PredicateHelper.True<NewsHighlight>();
            queryFilter = queryFilter.And(m => m.IDNewsHighlight == input.IDNewsHighlight);
            return Get(queryFilter).SingleOrDefault();
        }

        public NewsHighlight SaveData(NewsHighlight input, bool status)
        {
            if (input.FileUpload == null)
                input.FileUpload = "";
            if (input.Image == null)
                input.Image = "";
            if (status)
            {
                input.CreatedDate = DateTime.Now;
                input.UpdatedDate = DateTime.Now;
                Insert(input);
            }
            else
            {
                NewsHighlight temp = Checkavailability(input);
                temp.Status = input.Status;
                //temp.IsActive = input.IsActive;
                temp.IsActive = true;
                temp.UpdatedBy = input.UpdatedBy;
                temp.UpdatedDate = DateTime.Now;
                if(input.Status == 2)
                    temp.Remarks = "Approved by " + input.UpdatedBy + " on " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm");
                else if(input.Status == 0)
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
            DFISContextDB context = new DFISContextDB();

            NewsHighlight dbRes = context.NewsHighlights.FirstOrDefault(x => x.IDNewsHighlight == input.IDNewsHighlight);

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
