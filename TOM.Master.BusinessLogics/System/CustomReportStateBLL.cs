using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.Entity.Core.EntityClient;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using DFIS.Contracts;
using DFIS.Universal.Repositories;
using DFIS.Utils;
using DFIS.Universal.BusinessLogics;

namespace TOM.Master.BusinessLogics
{
    public class CustomReportStateBLL : ICustomReportStateBLL
    {
        private readonly IGenericRepository<CustomReportState> _generalRepo;
        private readonly ICustomReportStateRepo _customReportStateRepo;

        public CustomReportStateBLL(IGenericRepository<CustomReportState> generalRepo, ICustomReportStateRepo customReportStateRepo)
        {
            _generalRepo = generalRepo;
            _customReportStateRepo = customReportStateRepo;
        }

        public int InsertCustomReportState(CustomReportStateDTO input, string controller, string userid)
        {
            return _customReportStateRepo.Insert(input, controller, userid);
        }

        public int SaveSelectedLayout(CustomReportStateDTO input, string controller, string userid)
        {
            return _customReportStateRepo.SaveSelectedLayout(input, controller, userid);
        }

        public int DeleteDataBase(string pagename, string layoutname, string controller, string userid)
        {
            return _customReportStateRepo.DeleteDataBase(pagename, layoutname, controller, userid);
        }

        public List<CustomReportStateDTO> GetLayouts(CustomReportStateInput input, string userid)
        {
            var queryFilter = PredicateHelper.True<CustomReportState>();

            queryFilter = queryFilter.And(k => k.PageName == input.PageName);
            queryFilter = queryFilter.And(k => k.IDUser == userid);

            //var dbResult = _generalRepo.Get(queryFilter).OrderBy(k => k.CreatedBy).ToList();
            var dbResult = _generalRepo.Get().OrderBy(k => k.LayoutName).ToList();

            var dbResultDefault = dbResult.Where(x => x.LayoutName.ToUpper() == "DEFAULT" && x.IsGlobal == true && !x.FieldName.Contains(","));

            //var dbResultUser =
            //    dbResult.Where(y => y.PageName == input.PageName && y.IDUser == userid && y.IsGlobal);
            var dbResultUser =
                dbResult.Where(y => y.PageName == input.PageName && y.IDUser == userid);

            var result =
                (from defaultdata in dbResultDefault
                 select new CustomReportStateDTO()
                 {
                     IDCustomReportLayout = defaultdata.IDCustomReportLayout,
                     IDUser = defaultdata.IDUser,
                     PageName = defaultdata.PageName,
                     FieldName = defaultdata.FieldName,
                     LayoutName = defaultdata.LayoutName,
                     IsGlobal = defaultdata.IsGlobal,
                     IsActive = defaultdata.IsActive,
                     CreatedBy = defaultdata.CreatedBy,
                     CreatedDate = defaultdata.CreatedDate,
                     UpdatedBy = defaultdata.UpdatedBy,
                     UpdatedDate = defaultdata.UpdatedDate,
                     Remarks = defaultdata.Remarks
                 })
                .Union
                (from userData in dbResultUser
                 select new CustomReportStateDTO()
                 {
                     IDCustomReportLayout = userData.IDCustomReportLayout,
                     IDUser = userData.IDUser,
                     PageName = userData.PageName,
                     FieldName = userData.FieldName,
                     LayoutName = userData.LayoutName,
                     IsGlobal = userData.IsGlobal,
                     IsActive = userData.IsActive,
                     CreatedBy = userData.CreatedBy,
                     CreatedDate = userData.CreatedDate,
                     UpdatedBy = userData.UpdatedBy,
                     UpdatedDate = userData.UpdatedDate,
                     Remarks = userData.Remarks
                 });

            return Mapper.Map<List<CustomReportStateDTO>>(result);
        }

        public List<CustomReportStateDTO> GetLayoutsJoin(CustomReportStateInput input, string userid)
        {
            var dbResult = _generalRepo.Get().OrderBy(k => k.LayoutName).ToList();

            //get layout by username
            var dbResultByUser = dbResult.Where(x => x.PageName == input.PageName && x.IDUser == userid);
            //get layout by isglobal
            var dbResultIsGlobal = dbResult.Where(x => x.PageName == input.PageName && x.IsGlobal);

            //join current user and isglobal true or false , then distinct same layoutname
            var unionData =
                (from user in dbResultByUser
                 select new
                 {
                     IDCustomReportLayout = user.IDCustomReportLayout,
                     IDUser = user.IDUser,
                     PageName = user.PageName,
                     FieldName = user.FieldName,
                     LayoutName = user.LayoutName,
                     IsGlobal = user.IsGlobal,
                     IsActive = user.IsActive,
                     CreatedBy = user.CreatedBy,
                     CreatedDate = user.CreatedDate,
                     UpdatedBy = user.UpdatedBy,
                     UpdatedDate = user.UpdatedDate,
                     Remarks = user.Remarks
                 })
                .Union
                (from isGlobal in dbResultIsGlobal
                 select new
                 {
                     IDCustomReportLayout = isGlobal.IDCustomReportLayout,
                     IDUser = isGlobal.IDUser,
                     PageName = isGlobal.PageName,
                     FieldName = isGlobal.FieldName,
                     LayoutName = isGlobal.LayoutName,
                     IsGlobal = isGlobal.IsGlobal,
                     IsActive = isGlobal.IsActive,
                     CreatedBy = isGlobal.CreatedBy,
                     CreatedDate = isGlobal.CreatedDate,
                     UpdatedBy = isGlobal.UpdatedBy,
                     UpdatedDate = isGlobal.UpdatedDate,
                     Remarks = isGlobal.Remarks
                 }).ToList().Distinct();

            var res = new List<CustomReportStateDTO>();
            foreach (var data in unionData)
            {
                CustomReportStateDTO dto = new CustomReportStateDTO()
                {
                    IDCustomReportLayout = data.IDCustomReportLayout,
                    IDUser = data.IDUser,
                    PageName = data.PageName,
                    FieldName = data.FieldName,
                    LayoutName = data.LayoutName,
                    IsGlobal = data.IsGlobal,
                    IsActive = data.IsActive,
                    CreatedBy = data.CreatedBy,
                    CreatedDate = data.CreatedDate,
                    UpdatedBy = data.UpdatedBy,
                    UpdatedDate = data.UpdatedDate,
                    Remarks = data.Remarks
                };
                res.Add(dto);
            }

            return Mapper.Map<List<CustomReportStateDTO>>(res);
        }

        public List<CustomReportStateDTO> GetSelectedLayout(CustomReportStateInput input, string userid)
        {
            var queryFilter = PredicateHelper.True<CustomReportState>();

            //string PageNameUserId = input.PageName + '_' + userid;

            //queryFilter = queryFilter.And(k => k.PageName == input.PageName);
            //queryFilter = queryFilter.And(k => k.IDUser == userid);

            //var dbResult = _generalRepo.Get(queryFilter).OrderBy(k => k.CreatedBy).ToList();
            //var dbResult =
            //    _generalRepo.Get()
            //        .Where(
            //            x =>
            //                x.IsGlobal == true && x.LayoutName.ToUpper() == "DEFAULT" && x.FieldName.Contains(",") &&
            //                x.PageName.Contains(input.PageName))
            //        .ToList();
            //var dbResult =
            //    _generalRepo.Get()
            //        .Where(
            //            x =>
            //                x.FieldName.Contains(",") &&
            //                x.PageName.Contains(input.PageName) &&
            //                x.IDUser.Contains(userid))
            //        .ToList();

            var dbResult = _generalRepo.Get().Where(x => x.PageName.Contains(input.PageName + "_")).ToList();

            return Mapper.Map<List<CustomReportStateDTO>>(dbResult);
        }

        public List<CustomReportStateDTO> GetSavedSelectedLayout(CustomReportStateInput input, string userid)
        {
            //var queryFilter = PredicateHelper.True<CustomReportState>();

            //string PageNameUserId = input.PageName + '_' + userid;

            //queryFilter = queryFilter.And(k => k.PageName == input.PageName);
            //queryFilter = queryFilter.And(k => k.IDUser == userid);

            //var dbResult = _generalRepo.Get(queryFilter).OrderBy(k => k.CreatedBy).ToList();

            //return Mapper.Map<List<CustomReportStateDTO>>(dbResult);
            var queryFilter = PredicateHelper.True<CustomReportState>();

            string PageNameUserId = input.PageName + '_' + userid;

            queryFilter = queryFilter.And(k => k.PageName == PageNameUserId);
            queryFilter = queryFilter.And(k => k.IDUser == userid);

            var dbResult = _generalRepo.Get(queryFilter).OrderBy(k => k.CreatedBy).ToList();

            return Mapper.Map<List<CustomReportStateDTO>>(dbResult);
        }

        public CustomReportStateDTO SelectLayout(string userid, string pageName, string layoutName)
        {
            var orep = _generalRepo.Get(gr => gr.PageName == pageName && gr.IsGlobal == false && gr.IDUser == userid);
            foreach (var or in orep)
            {
                or.IsActive = or.LayoutName == layoutName;
                _generalRepo.Update(or);
            }
            _generalRepo.Save();

            return Mapper.Map<CustomReportStateDTO>(_generalRepo.Get(gr => gr.PageName == pageName && gr.LayoutName == layoutName && gr.IDUser == userid).FirstOrDefault());
        }
        public List<CustomReportStateDTO> GetSavedLayouts(string userid, string pageName, string layoutName = null)
        {
            var queryFilter = PredicateHelper.True<CustomReportState>();

            queryFilter = queryFilter.And(k => k.PageName == pageName);
            if (userid != null)
                queryFilter = queryFilter.And(k => k.IDUser == userid);
            else
                queryFilter = queryFilter.And(k => k.IsGlobal);
            if (layoutName != null)
                queryFilter = queryFilter.And(k => k.LayoutName == layoutName);

            var dbResult = _generalRepo.Get(queryFilter).OrderBy(k => k.LayoutName).ToList();

            return Mapper.Map<List<CustomReportStateDTO>>(dbResult);
        }
        public bool SaveLayouts(string userid, string pageName, params CustomReportStateDTO[] states)
        {
            try
            {
                foreach (var data in states)
                {
                    var id = data.IDCustomReportLayout;
                    var lname = data.LayoutName;
                    var odata = _generalRepo.Get(
                        d => d.IDCustomReportLayout == id ||
                        (d.PageName == pageName && d.IDUser == userid && d.LayoutName == lname)
                    ).FirstOrDefault();
                    if (odata != null)
                    {
                        data.CreatedBy = odata.CreatedBy;
                        data.CreatedDate = odata.CreatedDate;
                        data.IDCustomReportLayout = odata.IDCustomReportLayout;
                        Mapper.Map(data, odata);
                        odata.UpdatedBy = userid;
                        odata.UpdatedDate = DateTime.Now;
                        odata.PageName = pageName;
                        odata.IDUser = userid;
                        odata.IsActive = false;
                        odata.IsGlobal = false;
                        _generalRepo.Update(odata);
                    }
                    else
                    {
                        var ndata = Mapper.Map<CustomReportState>(data);
                        ndata.CreatedBy = userid;
                        ndata.CreatedDate = DateTime.Now;
                        ndata.UpdatedBy = userid;
                        ndata.UpdatedDate = DateTime.Now;
                        ndata.PageName = pageName;
                        ndata.IDUser = userid;
                        ndata.IsActive = false;
                        ndata.IsGlobal = false;
                        _generalRepo.Insert(ndata);
                    }
                }
                _generalRepo.Save();
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
