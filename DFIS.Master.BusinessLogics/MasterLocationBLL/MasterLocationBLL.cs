using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DFIS.Contracts;
using DFIS.EntitiesDAL;
using DFIS.EntitiesDAL.EDMX;
using DFIS.Master.Domain.DTOs;
using DFIS.Master.Domain.Inputs;
using DFIS.Master.Repositories;
using DFIS.Utils;
using DFIS.Utils.Exceptions;

namespace DFIS.Master.BusinessLogics
{
    public class MasterLocationBLL : IMasterLocationBLL
    {
        private readonly IGenericRepository<MasterLocation> _generalRepo;
        private readonly IMasterLocationRepo _mstLocationRepo;

        public MasterLocationBLL(IGenericRepository<MasterLocation> generalRepo, IMasterLocationRepo mstLocationRepo)
        {
            _generalRepo = generalRepo;
            _mstLocationRepo = mstLocationRepo;
        }

        public List<MasterLocationDTO> GetMasterLocations(MasterLocationInput input)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();

            //queryFilter = queryFilter.And(m => m.IsActive == true);

            if (!String.IsNullOrEmpty(input.LocationName))
            {
                queryFilter = queryFilter.And(m => m.LocationName == input.LocationName);
            }
            if (!String.IsNullOrEmpty(input.ParentLocation))
            {
                queryFilter = queryFilter.And(m => m.ParentLocation == input.ParentLocation);
            }
            if (!String.IsNullOrEmpty(input.ParentLocations))
            {
                string[] ParentLocations = input.ParentLocations.Split(',');
                queryFilter = queryFilter.And(m => ParentLocations.Contains(m.ParentLocation));
            }
            if (!String.IsNullOrEmpty(input.Type))
            {
                queryFilter = queryFilter.And(m => m.Type == input.Type);
            }

            input.SortExpression = "LocationName";
            input.SortOrder = "DESC";

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterLocation>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();

            return Mapper.Map<List<MasterLocationDTO>>(dbResult);
        }

        public List<MasterLocationDTO> GetDepartementForKPISSs()
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();

            queryFilter = queryFilter.And(p => p.IsActive == true);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            //var orderByFilter = sortCriteria.GetOrderByFunc<MasterLocation>();

            var dbResult = _generalRepo.Get(queryFilter).ToList();
            return Mapper.Map<List<MasterLocationDTO>>(dbResult);
        }

        public List<MasterLocationDTO> GetMasterLocationDrops(MasterLocationInput input)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();

            if (input.Type == "Warehouse")
            {
                queryFilter = queryFilter.And(m => m.Type == "Region").And(m => m.IsActive == true);
            }
            else if (input.Type == "Region")
            {
                queryFilter = queryFilter.And(m => m.Type == "Zone").And(m => m.IsActive == true);
            }
            else
            {
                queryFilter = queryFilter.And(m => m.ParentLocation == null).And(m => m.IsActive == true);
            }

            //if (!string.IsNullOrEmpty(input.ParentLocation))
            //{
            //    queryFilter = queryFilter.And(m => m.ParentLocation == input.ParentLocation).And(m => m.LocationName == input.LocationName);
            //}
            //if (!string.IsNullOrEmpty(input.Type))
            //{
            //    queryFilter = queryFilter.And(m => m.Type == input.Type).And(m => m.LocationName == input.LocationName).And(m => m.ParentLocation == input.ParentLocation);
            //}

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterLocation>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterLocationDTO>>(dbResult);

        }

        public List<MasterLocationDTO> ValidateEdit(MasterLocationInput input)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();

            if (!string.IsNullOrEmpty(input.LocationName))
                queryFilter = queryFilter.And(m => m.LocationName == input.LocationName).And(m => m.IDLocation == input.IDLocation);
            if (_generalRepo.Get(queryFilter).Count() > 0)
            {
                queryFilter = queryFilter.And(m => m.LocationName == null);
            }
            else
            {
                queryFilter = queryFilter.And(m => m.LocationName == input.LocationName);
                if (_generalRepo.Get(queryFilter).Count() > 0)
                {
                    queryFilter = queryFilter.And(m => m.LocationName == input.LocationName);
                }

            }
            //if (!string.IsNullOrEmpty(input.ParentLocation))
            //{
            //    queryFilter = queryFilter.And(m => m.ParentLocation == input.ParentLocation).And(m => m.LocationName == input.LocationName);
            //}
            //if (!string.IsNullOrEmpty(input.Type))
            //{
            //    queryFilter = queryFilter.And(m => m.Type == input.Type).And(m => m.LocationName == input.LocationName).And(m => m.ParentLocation == input.ParentLocation);
            //}

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterLocation>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterLocationDTO>>(dbResult);
        }

        public MasterLocationDTO SaveData(MasterLocationDTO input)
        {

            var validateInput = new MasterLocationInput()
            {

                IDLocation = input.IDLocation,
                LocationName = input.LocationName,
                ParentLocation = input.ParentLocation,
                Type = input.Type,
                IsActive = input.IsActive,
                Remarks = input.Remarks
            };
            var prevData = GetMasterLocations(validateInput);
            var dbMstLocation = Mapper.Map<MasterLocation>(input);

            if (prevData == null || (prevData != null && prevData.Count == 0))


            {
                try
                {
                    dbMstLocation.CreatedDate = DateTime.Now;
                    dbMstLocation.UpdatedDate = DateTime.Now;
                    _generalRepo.Insert(dbMstLocation);
                    _generalRepo.Save();
                }
                catch
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
                }

            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterLocationDTO>(dbMstLocation);
        }

        public MasterLocationDTO EditData(MasterLocationDTO input)
        {
            //var dbMstLocation = Mapper.Map<MasterLocation>(input);
            //dbMstLocation.CreatedDate = DateTime.Now;
            //dbMstLocation.UpdatedDate = DateTime.Now;
            //_generalRepo.Update(dbMstLocation);
            //_generalRepo.Save();

            //return Mapper.Map<MasterLocationDTO>(dbMstLocation);

            var validateInput = new MasterLocationInput()
            {

                IDLocation = input.IDLocation,
                LocationName = input.LocationName,
                ParentLocation = input.ParentLocation,
                Type = input.Type,
                IsActive = input.IsActive,
                Remarks = input.Remarks
            };

            var prevData = GetMasterLocations(validateInput);
            var prevName = _generalRepo.Get().Where(x => x.LocationName == input.LocationName && x.IDLocation == input.IDLocation).Select(x => x.LocationName).FirstOrDefault();
            var prevID = _generalRepo.Get().Where(x => x.LocationName == input.LocationName && x.IDLocation == input.IDLocation).Select(x => x.IDLocation).FirstOrDefault();

            var dbMstLocation = Mapper.Map<MasterLocation>(input);
            var validatecoy = Mapper.Map<MasterLocationDTO>(dbMstLocation);
            //string prevID = validatecoy.IDLocation;
            ////string locationid = prevData.IDLocation;
            //string prevName = validatecoy.LocationName;
            //var queryFilter = PredicateHelper.True<MasterLocation>();
            //string locationid = "";

            if (prevData == null || (prevData != null && prevData.Count == 0))
            {

                DFISContextDB context = new DFISContextDB();
                //dbMstLocation.CreatedDate = DateTime.Now;
                //dbMstLocation.UpdatedDate = DateTime.Now;
                //_generalRepo.Update(dbMstLocation);
                //_generalRepo.Save();

                MasterLocation c = (from x in context.MasterLocations
                                    where x.IDLocation == input.IDLocation
                                    select x).First();
                c.LocationName = input.LocationName;
                //c.CreatedBy = input.CreatedBy;
                //c.CreatedDate = DateTime.Now;
                c.UpdatedDate = DateTime.Now;
                c.IsActive = input.IsActive;
                c.UpdatedBy = input.UpdatedBy;
                c.Remarks = input.Remarks;
                c.ParentLocation = input.ParentLocation;
                c.Type = input.Type;
                c.IsRegionalOffice = input.IsRegionalOffice;

                context.SaveChanges();
            }
            else if (prevID == input.IDLocation && prevName == input.LocationName)
            {
                DFISContextDB context = new DFISContextDB();
                //dbMstLocation.CreatedDate = DateTime.Now;
                //dbMstLocation.UpdatedDate = DateTime.Now;
                //_generalRepo.Update(dbMstLocation);
                //_generalRepo.Save();

                MasterLocation c = (from x in context.MasterLocations
                                    where x.IDLocation == input.IDLocation
                                    select x).First();
                c.LocationName = input.LocationName;
                //c.CreatedBy = input.CreatedBy;
                //c.CreatedDate = DateTime.Now;
                c.UpdatedDate = DateTime.Now;
                c.IsActive = input.IsActive;
                c.UpdatedBy = input.UpdatedBy;
                c.Remarks = input.Remarks;
                c.ParentLocation = input.ParentLocation;
                c.Type = input.Type;
                c.IsRegionalOffice = input.IsRegionalOffice;

                context.SaveChanges();
            }

            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterLocationDTO>(dbMstLocation);
        }

        public MasterLocationDTO GetById(string id)
        {
            var result = _generalRepo.GetByID(id);

            return Mapper.Map<MasterLocationDTO>(result);
        }

        public List<MasterLocationDTO> GetWarehouseList(string type, string region)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            if (!string.IsNullOrEmpty(type))
            {
                queryFilter = queryFilter.And(m => m.Type == type);
            }
            if (!string.IsNullOrEmpty(region))
            {
                queryFilter = queryFilter.And(m => m.ParentLocation == region);
            }

            var dbResult = _generalRepo.Get(queryFilter).OrderBy(x => x.LocationName).ToList();
            return Mapper.Map<List<MasterLocationDTO>>(dbResult);
        }

        public List<MasterLocationDTO> GetMasterLocationsBySource(string source, MasterLocationInput input)
        {
            var active = _mstLocationRepo.GetAllMasterLocationFromSource("TOM");
            var activeids = active.Select(ml => ml.IDLocation);
            var allloc = GetMasterLocations(input).Where(loc => activeids.Contains(loc.IDLocation));
            return Mapper.Map<List<MasterLocationDTO>>(allloc);
        }

        public bool SetMasterLocationMapping(string source, string IDLocation, bool value)
        {
            return _mstLocationRepo.SetMasterLocationMapping(source, IDLocation, value);
        }
    }
}
