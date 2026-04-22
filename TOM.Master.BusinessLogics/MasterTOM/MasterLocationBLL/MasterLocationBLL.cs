using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.Master.Repositories;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using DFIS.Universal;

namespace TOM.Master.BusinessLogics
{
    public class MasterLocationBLL : IMasterLocationBLL
    {
        private readonly IGenericRepository<MasterLocation> _generalRepo;
        private readonly IGenericRepository<MasterMappingLocation> _mapRepo;
        private readonly IGenericRepository<MasterConfiguration> _mapConfig;


        public MasterLocationBLL(IGenericRepository<MasterConfiguration> mapConfig, IGenericRepository<MasterLocation> generalRepo, IGenericRepository<MasterMappingLocation> mapRepo)
        {
            _generalRepo = generalRepo;
            _mapRepo = mapRepo;
            _mapConfig = mapConfig;
        }

        public List<MasterLocationDTO> GetMasterLocations(MasterLocationInput input)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            List<MasterLocation> dbResult = null;
            //queryFilter = queryFilter.And(m => m.IsActive == true);
            if (input != null)
            {
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
                if (input.IsAssigned != null)
                {
                    queryFilter = queryFilter.And(m => m.IsAssigned == input.IsAssigned);
                }

                input.SortExpression = "LocationName";
                input.SortOrder = "DESC";
                var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
                var orderByFilter = sortCriteria.GetOrderByFunc<MasterLocation>();
                dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
                return Mapper.Map<List<MasterLocationDTO>>(dbResult);
            }
            else
            {
                dbResult = _generalRepo.Get().ToList();
            }
            dbResult = dbResult.Where(loc => (bool)loc.IsAssigned).ToList();
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

            /*
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
            */
            //if (!string.IsNullOrEmpty(input.ParentLocation))
            //{
            //    queryFilter = queryFilter.And(m => m.ParentLocation == input.ParentLocation).And(m => m.LocationName == input.LocationName);
            //}
            //if (!string.IsNullOrEmpty(input.Type))
            //{
            //    queryFilter = queryFilter.And(m => m.Type == input.Type).And(m => m.LocationName == input.LocationName).And(m => m.ParentLocation == input.ParentLocation);
            //}

            queryFilter = queryFilter.And(m => m.IsActive == true);

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
                    DFISContextDB DFISCtx = new DFISContextDB();
                    DFISCtx.InsertOrUpdateMstLocation(true, input.IDLocation, input.LocationName, input.ParentLocation, input.Type, input.IsActive, input.Remarks, input.CreatedBy);
                    /*
                    dbMstLocation.CreatedDate = DateTime.Now;
                    dbMstLocation.UpdatedDate = DateTime.Now;
                    _generalRepo.Insert(dbMstLocation);
                    _generalRepo.Save();
                    */
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
            /*
            if (prevData == null || (prevData != null && prevData.Count == 0))
            {

                TOMContextDB context = new TOMContextDB();
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
                TOMContextDB context = new TOMContextDB();
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
            */
            try
            {
                DFISContextDB DFISCtx = new DFISContextDB();
                DFISCtx.InsertOrUpdateMstLocation(false, input.IDLocation, input.LocationName, input.ParentLocation, input.Type, input.IsActive, input.Remarks, input.CreatedBy);
            }
            catch
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterLocationDTO>(dbMstLocation);
        }

        public MasterLocationDTO GetById(string id)
        {
            var result = _generalRepo.Get(c => c.IDLocation == id).FirstOrDefault();

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

        public List<MasterLocationDTO> GetSenderReceiverTOM(List<string> ListLocation)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            //queryFilter = queryFilter.And(m => m.Type == "Warehouse" && m.IsActive == true && ListLocation.Contains(m.IDLocation));
            queryFilter = queryFilter.And(m => m.IsActive == true && m.IsAssigned == true &&
            (
            m.Type == "Warehouse" || m.Type == "Factory" || m.Type == "Agent" || m.Type == "Other"
            ));
            var dbResult = _generalRepo.Get(queryFilter).OrderByDescending(m => m.LocationName).ToList();
            return Mapper.Map<List<MasterLocationDTO>>(dbResult);
        }
        public bool SetMasterLocationMapping(MasterLocationDTO input, bool value)
        {
            var obj = _mapRepo.Get(c => c.IDLocation == input.IDLocation).FirstOrDefault();
            if (obj != null)
            {
                if (!value)
                {
                    _mapRepo.Delete(obj);
                    _mapRepo.Save();
                    return true;
                }
            }
            else if (value)
            {
                _mapRepo.Insert(new MasterMappingLocation() { IDLocation = input.IDLocation });
                _mapRepo.Save();
                return true;
            }
            return false;
        }

        public List<MasterLocationDTO> GetMasterLocationMapping(MasterLocationDTO criteria)
        {
            var mapped = _mapRepo.Get(c => criteria == null || c.IDLocation == criteria.IDLocation);
            throw new NotImplementedException("This method is not implemented");
            return null;
        }

        public List<MasterLocationDTO> Get(Expression<Func<MasterLocation, bool>> filter)
        {
            return Mapper.Map<List<MasterLocationDTO>>(_generalRepo.Get(filter));
        }

        public List<MasterLocationDTO> GetByConfig(string configName, Expression<Func<MasterLocation, bool>> filter = null)
        {
            var cfg_filter = _mapConfig.Get(cfg => cfg.PageName == configName).Select(cfg => cfg.Value).Distinct().ToList();
            if (filter == null)
                filter = loc => true;
            if (cfg_filter.Count() > 0)
                filter = filter.And(loc => cfg_filter.Contains(loc.Type));

            filter = filter.And(loc => loc.IsActive && loc.IsAssigned == true);
            var locs = _generalRepo.Get(filter).OrderBy(f => f.LocationName);
            return Mapper.Map<List<MasterLocationDTO>>(locs);
        }

        public string GetNameById(string id)
        {
            var lc = _generalRepo.Get(loc => loc.IDLocation == id).FirstOrDefault();
            return lc == null ? null : lc.LocationName;
        }

        public void InsertOrUpdate(MasterLocationDTO value, bool canUpdate = true)
        {
            var existing = GetNameById(value.IDLocation);

            TOMContextDB ctx = new TOMContextDB();
            ctx.InsertOrUpdateMstLocation(
                value.IDLocation,
                value.LocationName,
                value.ParentLocation,
                value.Type,
                value.IsActive,
                value.Remarks,
                value.CreatedBy);
        }
    }
}