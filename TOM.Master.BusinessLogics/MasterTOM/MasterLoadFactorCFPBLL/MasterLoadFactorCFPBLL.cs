using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.Master.Repositories;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public class MasterLoadFactorCFPBLL : IMasterLoadFactorCFPBLL
    {
        private readonly IMasterLoadFactorCFPRepo _masterLoadFactorCFPRepo;
        private readonly IGenericRepository<MasterLoadFactorCFP> _generalRepo;
        private readonly IGenericRepository<MasterList> _generalListRepo;
        private readonly IMasterListRepo _masterListRepo;

        public MasterLoadFactorCFPBLL(IMasterLoadFactorCFPRepo masterLoadFactorCFPRepo, IGenericRepository<MasterLoadFactorCFP> generalRepo, IMasterListRepo masterListRepo, IGenericRepository<MasterList> generalListRepo)
        {
            _masterLoadFactorCFPRepo = masterLoadFactorCFPRepo;
            _generalRepo = generalRepo;
            _generalListRepo = generalListRepo;
            _masterListRepo = masterListRepo;
        }

        public List<MasterListDTO> GetListForCPFs(MasterListInput input)
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName == "VehicleType");

            //queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterList>();

            var dbResult = _generalListRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }
        public List<MasterListDTO> GetListForModel(MasterListInput input)
        {
            var queryFilter = PredicateHelper.True<MasterList>();

            queryFilter = queryFilter.And(m => m.FieldName == "TransportationMode");

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterList>();

            var dbResult = _generalListRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterListDTO>>(dbResult);
        }
        public List<MasterLoadFactorCFPDTO> GetMasterLoadFactorCFPs(MasterLoadFactorCFPInput input)
        {
            var queryFilter = PredicateHelper.True<MasterLoadFactorCFP>();
            if (input.VehicleTypeListFilter != null && input.VehicleTypeListFilter.Count > 0)
                queryFilter = queryFilter.And(m => input.VehicleTypeListFilter.Contains(m.VehicleType));
            if (input.StartDate != null)
                queryFilter = queryFilter.And(m => m.StartDate <= input.StartDate && m.EndDate >= input.StartDate);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterLoadFactorCFP>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterLoadFactorCFPDTO>>(dbResult);

        }

        public MasterLoadFactorCFP CheckDataAvailability(MasterLoadFactorCFPDTO input)
        {
            var queryFilter = PredicateHelper.True<MasterLoadFactorCFP>();
            queryFilter = queryFilter.And(m => m.VehicleType == input.VehicleType);
            queryFilter = queryFilter.And(m => m.BrandCategory == input.BrandCategory);
            queryFilter = queryFilter.And(m => m.Mode == input.Mode);
            return Mapper.Map<MasterLoadFactorCFP>(_generalRepo.Get(queryFilter).SingleOrDefault());
        }

        public MasterLoadFactorCFPDTO SaveData(MasterLoadFactorCFPDTO input)
        {
            List<MasterLoadFactorCFP> listCheckData = _generalRepo.Get()
                .Where(x => x.BrandCategory == input.BrandCategory && x.VehicleType == input.VehicleType &&
                            x.Mode == input.Mode && x.IsActive && ((x.StartDate <= input.StartDate && x.EndDate >= input.StartDate) || (x.StartDate <= input.EndDate && x.EndDate >= input.EndDate))).OrderByDescending(x => x.StartDate).ToList();
            var dbMstLoadFactorCFP = Mapper.Map<MasterLoadFactorCFP>(input);
            if (listCheckData.Count == 0)
            {
                dbMstLoadFactorCFP.CreatedDate = DateTime.Now;
                dbMstLoadFactorCFP.UpdatedDate = DateTime.Now;
                _generalRepo.Insert(dbMstLoadFactorCFP);
                _generalRepo.Save();
            }
            else
            {
                MasterLoadFactorCFP checkData = listCheckData[0];
                if (checkData.StartDate < input.StartDate && checkData.EndDate == new DateTime(2999, 12, 31))
                {
                    TOMContextDB context = new TOMContextDB();
                    MasterLoadFactorCFP c = (from x in context.MasterLoadFactorCFPs
                        where x.IDLoadFactorCFP == checkData.IDLoadFactorCFP
                        select x).First();
                    c.EndDate = input.StartDate.Value.AddDays(-1);
                    c.UpdatedBy = input.UpdatedBy;
                    c.UpdatedDate = DateTime.Now;
                    context.SaveChanges();
                    dbMstLoadFactorCFP.CreatedDate = DateTime.Now;
                    dbMstLoadFactorCFP.UpdatedDate = DateTime.Now;
                    _generalRepo.Insert(dbMstLoadFactorCFP);
                    _generalRepo.Save();
                }
                else
                    throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return input;
            //var validateInput = new MasterLoadFactorCFPInput()
            //{
            //    IDLoadFactorCFP = input.IDLoadFactorCFP,
            //    VehicleType = input.VehicleType,
            //    KMperLiter = input.KMperLiter,
            //    Mode = input.Mode,
            //    KgCO2perLiter = input.KgCO2perLiter,
            //    BrandCategory = input.BrandCategory,
            //    MaxQty = input.MaxQty,
            //    WeightperStick = input.WeightperStick,

            //    IsActive = input.IsActive,
            //    Remarks = input.Remarks
            //};
            //var prevData = GetMasterLoadFactorCFPs(validateInput);
            //var dbMstLoadFactorCFP = Mapper.Map<MasterLoadFactorCFP>(input);

            //if (prevData == null || (prevData != null && prevData.Count == 0))
            //{
            //    if (!prevData.Any())
            //    {
            //        dbMstLoadFactorCFP.CreatedDate = DateTime.Now;
            //        dbMstLoadFactorCFP.UpdatedDate = DateTime.Now;
            //        _generalRepo.Insert(dbMstLoadFactorCFP);
            //        _generalRepo.Save();
            //    }
            //    else
            //    {
            //        throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            //    }
            //    return Mapper.Map<MasterLoadFactorCFPDTO>(dbMstLoadFactorCFP);
            //}
            //else
            //{
            //    if (prevData.Select(x => x.IsActive).FirstOrDefault() == false)
            //    {
            //        TOMContextDB context = new TOMContextDB();

            //        MasterLoadFactorCFP u = (from x in context.MasterLoadFactorCFPs
            //                                 where
            //                                 x.BrandCategory == input.BrandCategory &&
            //                                 x.Mode == input.Mode &&
            //                                 x.VehicleType == input.VehicleType
            //                                 select x).First();
            //        u.VehicleType = input.VehicleType;
            //        u.UpdatedDate = DateTime.Now;
            //        u.IsActive = input.IsActive;
            //        u.UpdatedBy = input.UpdatedBy;
            //        u.Remarks = input.Remarks;
            //        u.BrandCategory = input.BrandCategory;
            //        u.KgCO2perLiter = input.KgCO2perLiter;
            //        u.KMperLiter = input.KMperLiter;
            //        u.MaxQty = input.MaxQty;
            //        u.WeightperStick = input.WeightperStick;
            //        u.StartDate = input.StartDate;
            //        u.EndDate = input.EndDate;


            //        context.SaveChanges();
            //    }
            //    else
            //    {
            //        throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            //    }

            //}
            //return Mapper.Map<MasterLoadFactorCFPDTO>(dbMstLoadFactorCFP);
        }
        public string SaveAndUpdateData(MasterLoadFactorCFPDTO input)
        {
            string message = "Upload Failed";
            var dbMstLoadFactorCFP = Mapper.Map<MasterLoadFactorCFP>(input);

            TOMContextDB context = new TOMContextDB();
            var dbContextTransaction = context.Database.BeginTransaction();
            var dbres = context.MasterLoadFactorCFPs.Where(
                f => f.BrandCategory == input.BrandCategory &&
                f.VehicleType == input.VehicleType &&
                f.Mode == input.Mode
                );
            var existingData = _generalRepo.Get().Where(x => x.BrandCategory == input.BrandCategory && x.VehicleType == input.VehicleType && x.Mode == input.Mode).FirstOrDefault();
            //var _vehicletype = _generalRepo.Get().Where(x => x.VehicleType == input.VehicleType && x.BrandCategory == input.BrandCategory).Select(x => x.VehicleType).FirstOrDefault();
            //var _mode = _generalRepo.Get().Where(x => x.Mode == input.Mode && x.Mode == input.Mode).Select(x => x.Mode).FirstOrDefault();
            //var _id = _generalRepo.Get().Where(x => x.VehicleType == input.VehicleType && x.BrandCategory == input.BrandCategory).Select(x => x.IDLoadFactorCFP).FirstOrDefault();

            try
            {
                if (existingData != null)
                {
                    //MasterLoadFactorCFP u = (from x in context.MasterLoadFactorCFPs
                    //                         where
                    //                         x.BrandCategory == input.BrandCategory &&
                    //                         x.Mode == input.Mode &&
                    //                         x.VehicleType == input.VehicleType
                    //                         select x).First();
                    existingData.VehicleType = input.VehicleType;
                    existingData.UpdatedDate = DateTime.Now;
                    existingData.IsActive = input.IsActive;
                    existingData.UpdatedBy = input.UpdatedBy;
                    existingData.Remarks = input.Remarks;
                    existingData.BrandCategory = input.BrandCategory;
                    existingData.KgCO2perLiter = input.KgCO2perLiter;
                    existingData.KMperLiter = input.KMperLiter;
                    existingData.MaxQty = input.MaxQty;
                    existingData.WeightperStick = input.WeightperStick;

                    context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                else
                {
                    dbMstLoadFactorCFP.CreatedDate = DateTime.Now;
                    dbMstLoadFactorCFP.UpdatedDate = DateTime.Now;
                    _generalRepo.Insert(dbMstLoadFactorCFP);
                    _generalRepo.Save();
                    dbContextTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                message = "Failed to delete data. Please refresh the data.";
            }
            finally
            {
                dbContextTransaction.Dispose();
            }
            return message;
        }
        public MasterLoadFactorCFPDTO EditData(MasterLoadFactorCFPDTO input)
        {
            List<MasterLoadFactorCFP> listCheckData = _generalRepo.Get()
                .Where(x => x.BrandCategory == input.BrandCategory && x.VehicleType == input.VehicleType &&
                            x.Mode == input.Mode && x.IsActive && ((x.StartDate <= input.StartDate && x.EndDate >= input.StartDate) || (x.StartDate <= input.EndDate && x.EndDate >= input.EndDate))).ToList();
            if (listCheckData.Count == 0)
            {
                TOMContextDB context = new TOMContextDB();
                MasterLoadFactorCFP c = (from x in context.MasterLoadFactorCFPs
                    where x.IDLoadFactorCFP == input.IDLoadFactorCFP
                    select x).First();
                c.VehicleType = input.VehicleType;
                c.UpdatedDate = DateTime.Now;
                c.IsActive = input.IsActive;
                c.UpdatedBy = input.UpdatedBy;
                c.Remarks = input.Remarks;
                c.BrandCategory = input.BrandCategory;
                c.Mode = input.Mode;
                c.KgCO2perLiter = input.KgCO2perLiter;
                c.KMperLiter = input.KMperLiter;
                c.MaxQty = input.MaxQty;
                c.WeightperStick = input.WeightperStick;
                c.StartDate = input.StartDate;
                c.EndDate = input.EndDate;

                context.SaveChanges();
            }
            else
            {
                if (listCheckData.Count == 1)
                {
                    MasterLoadFactorCFP checkData = listCheckData[0];
                    if (checkData.IDLoadFactorCFP == input.IDLoadFactorCFP)
                    {
                        TOMContextDB context = new TOMContextDB();
                        MasterLoadFactorCFP c = (from x in context.MasterLoadFactorCFPs
                            where x.IDLoadFactorCFP == input.IDLoadFactorCFP
                            select x).First();
                        c.VehicleType = input.VehicleType;
                        c.UpdatedDate = DateTime.Now;
                        c.IsActive = input.IsActive;
                        c.UpdatedBy = input.UpdatedBy;
                        c.Remarks = input.Remarks;
                        c.BrandCategory = input.BrandCategory;
                        c.Mode = input.Mode;
                        c.KgCO2perLiter = input.KgCO2perLiter;
                        c.KMperLiter = input.KMperLiter;
                        c.MaxQty = input.MaxQty;
                        c.WeightperStick = input.WeightperStick;
                        c.StartDate = input.StartDate;
                        c.EndDate = input.EndDate;

                        context.SaveChanges();
                    }
                    else if (checkData.StartDate < input.StartDate && checkData.EndDate == new DateTime(2999, 12, 31))
                    {
                        TOMContextDB context = new TOMContextDB();
                        MasterLoadFactorCFP c = (from x in context.MasterLoadFactorCFPs
                            where x.IDLoadFactorCFP == checkData.IDLoadFactorCFP
                            select x).First();
                        c.EndDate = input.StartDate.Value.AddDays(-1);
                        c.UpdatedBy = input.UpdatedBy;
                        c.UpdatedDate = DateTime.Now;
                        MasterLoadFactorCFP c2 = (from x in context.MasterLoadFactorCFPs
                            where x.IDLoadFactorCFP == input.IDLoadFactorCFP
                            select x).First();
                        c.VehicleType = input.VehicleType;
                        c.UpdatedDate = DateTime.Now;
                        c.IsActive = input.IsActive;
                        c.UpdatedBy = input.UpdatedBy;
                        c.Remarks = input.Remarks;
                        c.BrandCategory = input.BrandCategory;
                        c.Mode = input.Mode;
                        c.KgCO2perLiter = input.KgCO2perLiter;
                        c.KMperLiter = input.KMperLiter;
                        c.MaxQty = input.MaxQty;
                        c.WeightperStick = input.WeightperStick;
                        c.StartDate = input.StartDate;
                        c.EndDate = input.EndDate;

                        context.SaveChanges();
                    }
                    else
                        throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
                }
                else
                    throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return input;
            //var validateInput = new MasterLoadFactorCFPInput()
            //{
            //    KMperLiter = input.KMperLiter,
            //    KgCO2perLiter = input.KgCO2perLiter,
            //    Mode = input.Mode,
            //    MaxQty = input.MaxQty,
            //    IsActive = input.IsActive,
            //    Remarks = input.Remarks,
            //    BrandCategory = input.BrandCategory,
            //    VehicleType = input.VehicleType
            //};

            //var prevData = GetMasterLoadFactorCFPs(validateInput);
            //var existingData = _generalRepo.Get().Where(x => x.BrandCategory == input.BrandCategory && x.VehicleType == input.VehicleType && x.Mode == input.Mode).FirstOrDefault();
            //var _id = _generalRepo.Get().Where(x => x.VehicleType == input.VehicleType && x.BrandCategory == input.BrandCategory && x.Mode == input.Mode).Select(x => x.IDLoadFactorCFP).FirstOrDefault();
            //var dbMstLoadFactorCFP = Mapper.Map<MasterLoadFactorCFP>(input);

            //if (prevData == null || (prevData != null && prevData.Count == 0))
            //{

            //    TOMContextDB context = new TOMContextDB();

            //    MasterLoadFactorCFP c = (from x in context.MasterLoadFactorCFPs
            //                             where x.IDLoadFactorCFP == input.IDLoadFactorCFP
            //                             select x).First();
            //    c.VehicleType = input.VehicleType;
            //    c.UpdatedDate = DateTime.Now;
            //    c.IsActive = input.IsActive;
            //    c.UpdatedBy = input.UpdatedBy;
            //    c.Remarks = input.Remarks;
            //    c.BrandCategory = input.BrandCategory;
            //    c.Mode = input.Mode;
            //    c.KgCO2perLiter = input.KgCO2perLiter;
            //    c.KMperLiter = input.KMperLiter;
            //    c.MaxQty = input.MaxQty;
            //    c.WeightperStick = input.WeightperStick;
            //    c.StartDate = input.StartDate;
            //    c.EndDate = input.EndDate;

            //    context.SaveChanges();
            //}
            //else if (existingData != null)
            //{
            //    TOMContextDB context = new TOMContextDB();

            //    MasterLoadFactorCFP c = (from x in context.MasterLoadFactorCFPs
            //                             where x.IDLoadFactorCFP == input.IDLoadFactorCFP
            //                             select x).First();
            //    c.VehicleType = input.VehicleType;
            //    c.UpdatedDate = DateTime.Now;
            //    c.IsActive = input.IsActive;
            //    c.UpdatedBy = input.UpdatedBy;
            //    c.Remarks = input.Remarks;
            //    c.BrandCategory = input.BrandCategory;
            //    c.Mode = input.Mode;
            //    c.KgCO2perLiter = input.KgCO2perLiter;
            //    c.KMperLiter = input.KMperLiter;
            //    c.MaxQty = input.MaxQty;
            //    c.WeightperStick = input.WeightperStick;
            //    c.StartDate = input.StartDate;
            //    c.EndDate = input.EndDate;

            //    context.SaveChanges();
            //}
            //else
            //{
            //    throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            //}

            //return Mapper.Map<MasterLoadFactorCFPDTO>(dbMstLoadFactorCFP);
        }

        public MasterLoadFactorCFPDTO GetById(int id)
        {
            var result = _generalRepo.GetByID(id);

            return Mapper.Map<MasterLoadFactorCFPDTO>(result);
        }

    }
}
