using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using DFIS.Utils;
using DFIS.Universal.Repositories;
using TOM.Master.Domain.DTOs;

namespace TOM.Master.Repositories
{
    public class CustomReportStateRepo : ICustomReportStateRepo
    {
        public int Insert(CustomReportStateDTO input, string controller, string userid)
        {
            TOMContextDB context = new TOMContextDB();

            var inputCustomReportState = new CustomReportState()
            {
                IDUser = input.IDUser,
                PageName = input.PageName,
                FieldName = input.FieldName,
                LayoutName = input.LayoutName,
                IsGlobal = input.IsGlobal,
                IsActive = input.IsActive,
                CreatedBy = userid,
                CreatedDate = DateTime.Now,
                UpdatedBy = userid,
                UpdatedDate = DateTime.Now,
                Remarks = input.Remarks
            };
            //throw new NotImplementedException();
            context.CustomReportStates.Add(inputCustomReportState);

            return context.SaveChanges();
        }

        public int DeleteDataBase(string pagename, string layoutname, string controller, string userid)
        {
            TOMContextDB context = new TOMContextDB();
            var queryFilter = PredicateHelper.True<CustomReportState>();
            queryFilter = queryFilter.And(p => p.IDUser == userid);
            queryFilter = queryFilter.And(p => p.PageName == pagename);
            queryFilter = queryFilter.And(p => p.LayoutName == layoutname);

            //throw new NotImplementedException();

            List<CustomReportState> list = context.CustomReportStates.Where(queryFilter).ToList();
            if (list.Count > 0)
            {
                foreach (CustomReportState temp in list)
                {
                    context.CustomReportStates.Remove(context.CustomReportStates.FirstOrDefault(x => x.IDCustomReportLayout == temp.IDCustomReportLayout));
                }
            }

            return context.SaveChanges();
        }

        public int SaveSelectedLayout(CustomReportStateDTO input, string controller, string userid)
        {
            TOMContextDB context = new TOMContextDB();
            //throw new NotImplementedException();
            CustomReportState dbRes = context.CustomReportStates.FirstOrDefault(x => x.PageName == input.PageName);

            // update
            if (dbRes != null)
            {
                dbRes.IDUser = input.IDUser;
                dbRes.PageName = input.PageName;
                dbRes.FieldName = input.FieldName;
                dbRes.LayoutName = input.LayoutName;
                dbRes.IsGlobal = input.IsGlobal;
                dbRes.IsActive = input.IsActive;
                dbRes.UpdatedBy = userid;
                dbRes.UpdatedDate = DateTime.Now;
                dbRes.Remarks = input.Remarks;
            }
            else // save
            {
                CustomReportState inputCustomReportState = new CustomReportState()
                {
                    IDUser = input.IDUser,
                    PageName = input.PageName,
                    FieldName = input.FieldName,
                    LayoutName = input.LayoutName,
                    IsGlobal = input.IsGlobal,
                    IsActive = input.IsActive,
                    CreatedBy = userid,
                    CreatedDate = DateTime.Now,
                    UpdatedBy = userid,
                    UpdatedDate = DateTime.Now,
                    Remarks = input.Remarks
                };
                //throw new NotImplementedException();
                context.CustomReportStates.Add(inputCustomReportState);
            }
            return context.SaveChanges();
        }
    }
}
