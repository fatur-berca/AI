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
    public class MasterFGStackingBLL : IMasterFGStackingBLL
    {
        private readonly IMasterFGStackingRepo _masterFGStackingRepo;
        private readonly IGenericRepository<MasterFGStacking> _generalRepo;
        private readonly IGenericRepository<MasterLocation> _generalLocationRepo;
        private readonly IGenericRepository<MasterUserLocationMapping> _masterUserLocationMapping;
        private readonly IMasterFABrandRepo _masterFaBrandRepo;

        public MasterFGStackingBLL(IMasterFGStackingRepo masterFGStackingRepo, IGenericRepository<MasterFGStacking> generalRepo, IGenericRepository<MasterLocation> generalLocationRepo, IMasterFABrandRepo masterFaBrandRepo, IGenericRepository<MasterUserLocationMapping> masterUserLocationMapping)
        {
            _masterFGStackingRepo = masterFGStackingRepo;
            _generalRepo = generalRepo;
            _generalLocationRepo = generalLocationRepo;
            _masterFaBrandRepo = masterFaBrandRepo;
            _masterUserLocationMapping = masterUserLocationMapping;
        }

        public List<MasterFGStackingDTO> GetMasterFGStackings(MasterFGStackingInput input, string iduser)
        {
            List<MasterLocationDTO> listMasterLocation = GetMasterLocations(iduser);
            List<string> listLocation = new List<string>();
            foreach (MasterLocationDTO var in listMasterLocation)
            {
                listLocation.Add(var.IDLocation);
            }

            TOMContextDB context = new TOMContextDB();
            var query = (from a in context.MasterFGStackings.Where(x => listLocation.Contains(x.IDLocation))
                         join c in context.MasterLocations on a.IDLocation equals c.IDLocation
                         select new MasterFGStackingDTO()
                         {
                             IDFGStacking = a.IDFGStacking,
                             IDLocation = a.IDLocation,
                             LocationName = c.LocationName,
                             MaxStacking = a.MaxStacking,
                             DoubleStacking = a.DoubleStacking,
                             IsActive = a.IsActive,
                             Brand = a.Brand,
                             UpdatedBy = a.UpdatedBy,
                             UpdatedDate = a.UpdatedDate,
                             CreatedBy = a.CreatedBy,
                             CreatedDate = a.CreatedDate,
                             Remarks = a.Remarks
                         });
            var dbResult = query.ToList();
            return Mapper.Map<List<MasterFGStackingDTO>>(dbResult);
        }

        public List<MasterLocationDTO> GetMasterLocations(string iduser)
        {
            List<MasterLocation> listMasterLocation = new List<MasterLocation>();
            List<MasterUserLocationMapping> listuserLocaMap = _masterUserLocationMapping.Get().Where(x => x.IDUser.ToLower() == iduser.ToLower()).ToList();
            foreach (MasterUserLocationMapping var in listuserLocaMap)
            {
                var location = _generalLocationRepo.GetByID(var.IDLocation);
                if (location != null && location.Type == "Warehouse")
                    listMasterLocation.Add(location);
            }
            return Mapper.Map<List<MasterLocationDTO>>(listMasterLocation);
            //var queryFilter = PredicateHelper.True<MasterLocation>();
            //queryFilter = queryFilter.And(m => m.IsActive);
            //queryFilter = queryFilter.And(m => m.Type == "Warehouse");
            //var dbResult = _generalLocationRepo.Get(queryFilter).ToList();
            //return Mapper.Map<List<MasterLocationDTO>>(dbResult);
        }

        public List<string> ListAvailableSpeakingCode()
        {
            return _masterFaBrandRepo.ListAvailableSpeakingCode();
        }

        public List<MasterFGStackingDTO> ValidateMasterFGStackings(MasterFGStackingInput input)
        {
            var queryFilter = PredicateHelper.True<MasterFGStacking>();

            if (!string.IsNullOrEmpty(input.Brand))
                queryFilter = queryFilter.And(m => m.Brand == input.Brand && m.IDLocation == input.IDLocation);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterFGStacking>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterFGStackingDTO>>(dbResult);
        }

        public List<MasterFGStackingDTO> CheckDataAvailability(MasterFGStackingInput input)
        {
            var queryFilter = PredicateHelper.True<MasterFGStacking>();
            queryFilter = queryFilter.And(m => m.Brand == input.Brand && m.IDLocation == input.IDLocation);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterFGStacking>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterFGStackingDTO>>(dbResult);
        }


        public MasterFGStackingDTO SaveData(MasterFGStackingDTO input)
        {
            var validateInput = new MasterFGStackingInput()
            {
                Brand = input.Brand,
                MaxStacking = input.MaxStacking,
                DoubleStacking = input.DoubleStacking,
                IsActive = input.IsActive,
                IDLocation = input.IDLocation
            };
            var prevData = ValidateMasterFGStackings(validateInput);
            var dbMstFGStacking = Mapper.Map<MasterFGStacking>(input);
            if (prevData == null || (prevData != null && prevData.Count == 0))
            {
                dbMstFGStacking.CreatedDate = DateTime.Now;
                dbMstFGStacking.UpdatedDate = DateTime.Now;
                _generalRepo.Insert(dbMstFGStacking);
                _generalRepo.Save();
            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterFGStackingDTO>(dbMstFGStacking);
        }

        public MasterFGStackingDTO EditData(MasterFGStackingDTO input)
        {
            //var validateInput = new MasterFGStackingInput()
            //{
            //    Brand = input.Brand,
            //    MaxStacking = input.MaxStacking,
            //    DoubleStacking = input.DoubleStacking,
            //    IDLocation = input.IDLocation,
            //    IsActive = input.IsActive
            //};
            //var temp = CheckDataAvailability(validateInput);
            var dbMstFGStacking = Mapper.Map<MasterFGStacking>(input);

            //if (temp == null || (temp != null && temp.Count == 0))
            //{
            dbMstFGStacking.CreatedDate = DateTime.Now;
            dbMstFGStacking.UpdatedDate = DateTime.Now;
            _generalRepo.Update(dbMstFGStacking);
            _generalRepo.Save();
            //}
            //else
            //{
            //    throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            //}
            //if (temp != null)
            //{
            //    temp.MinStacking = input.MinStacking;
            //    temp.MaxStacking = input.MaxStacking;
            //    temp.IsActive = input.IsActive;
            //    temp.UpdatedDate = DateTime.Now;
            //    _generalRepo.Update(temp);
            //    _generalRepo.Save();
            //}
            //else{ 
            //    dbMstFGStacking.CreatedDate = DateTime.Now;
            //    dbMstFGStacking.UpdatedDate = DateTime.Now;
            //    _generalRepo.Update(dbMstFGStacking);
            //    _generalRepo.Save();
            //}
            return Mapper.Map<MasterFGStackingDTO>(dbMstFGStacking);

        }

        public List<MasterFGStackingDTO> GetById(int id)
        {

            TOMContextDB context = new TOMContextDB();
            var query = (from a in context.MasterFGStackings

                         join c in context.MasterLocations on a.IDLocation equals c.IDLocation

                         select new MasterFGStackingDTO()
                         {
                             IDFGStacking = a.IDFGStacking,
                             IDLocation = a.IDLocation,
                             LocationName = c.LocationName,
                             MaxStacking = a.MaxStacking,
                             DoubleStacking = a.DoubleStacking,
                             IsActive = a.IsActive,
                             Brand = a.Brand,
                             UpdatedBy = a.UpdatedBy,
                             UpdatedDate = a.UpdatedDate,
                             CreatedBy = a.CreatedBy,
                             CreatedDate = a.CreatedDate,
                             Remarks = a.Remarks


                         });
            var dbResult = query.ToList();

            dbResult = dbResult.Where(m => m.IDFGStacking == id).ToList();

            return Mapper.Map<List<MasterFGStackingDTO>>(dbResult);
        }

    }
}
