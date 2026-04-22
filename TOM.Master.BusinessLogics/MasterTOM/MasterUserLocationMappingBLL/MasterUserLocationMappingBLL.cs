using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using TOM.Master.Repositories;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using System.Linq.Expressions;

namespace TOM.Master.BusinessLogics
{
    public class MasterUserLocationMappingBLL : IMasterUserLocationMappingBLL
    {
        private readonly IGenericRepository<MasterUserLocationMapping> _generalRepo;
        private readonly IGenericRepository<MasterLocation> _generalLocationRepo;
        private readonly IGenericRepository<MasterUser> _generalUserRepo;
        private readonly IGenericRepository<MasterRole> _generalRoleRepo;
        private readonly IGenericRepository<MasterRolesFunctionMapping> _generalMaterRoleFunctionMapping;
        private readonly IGenericRepository<MasterUserLocationMapTreeListView> _generalUserLocViewRepo;
        private readonly IMasterUserRepo _masterUserRepo;
        private readonly IMasterLocationRepo _masterLocationRepo;

        public MasterUserLocationMappingBLL(IGenericRepository<MasterUserLocationMapping> generalRepo,
            IGenericRepository<MasterLocation> generalLocationRepo, IGenericRepository<MasterUser> generalUserRepo, IGenericRepository<MasterRole> generalRoleRepo, IGenericRepository<MasterUserLocationMapTreeListView> generalUserLocViewRepo, IGenericRepository<MasterRolesFunctionMapping> generalMaterRoleFunctionMapping, IMasterUserRepo masterUserRepo, IMasterLocationRepo masterLocationRepo)
        {
            _generalRepo = generalRepo;
            _generalLocationRepo = generalLocationRepo;
            _generalUserRepo = generalUserRepo;
            _generalRoleRepo = generalRoleRepo;
            _generalUserLocViewRepo = generalUserLocViewRepo;
            _generalMaterRoleFunctionMapping = generalMaterRoleFunctionMapping;
            _masterUserRepo = masterUserRepo;
            _masterLocationRepo = masterLocationRepo;
        }

        public List<MasterUserLocationMappingDTO> GetDistinctLocationMasterUserLocation()
        {
            List<MasterUserLocationMappingDTO> tempList = new List<MasterUserLocationMappingDTO>();
            List<MasterUser> listMasterUser = _masterUserRepo.GettAllMasterUserAcive();
            foreach (MasterUser mu in listMasterUser)
            {
                if (mu.MasterUserLocationMappings.Count > 0)
                {
                    MasterUserLocationMappingDTO temp = new MasterUserLocationMappingDTO();
                    foreach (var tempRole in mu.MasterUserRoleMappings)
                    {
                        temp.MasterRole += tempRole.MasterRole.RoleName + ",";
                    }
                    if (temp.MasterRole != null)
                        temp.MasterRole = temp.MasterRole.Remove(temp.MasterRole.Length - 1);
                    temp.fullName = mu.FullName;
                    temp.IDUser = mu.IDUser;
                    temp.IsActive = mu.IsActive;
                    temp.CreatedDate = mu.CreatedDate;
                    temp.CreatedBy = mu.CreatedBy;
                    temp.UpdatedDate = mu.UpdatedDate;
                    temp.UpdatedBy = mu.UpdatedBy;
                    tempList.Add(temp);
                }
            }
            return tempList;
        }

        public List<MasterUserLocationMappingDTO> GetMasterUserByLocations(List<string> Locations)
        {
            int[] IncludeRoleIDs = new int[] { 3, 4 };
            List<MasterUserLocationMappingDTO> tempList = new List<MasterUserLocationMappingDTO>();
            var distinct = from c in _generalRepo.Get().Where(f => Locations.Contains(f.IDLocation) && f.IsActive)
                           group c by new
                           {
                               c.IDUser
                           }
                           into grp
                           select grp.FirstOrDefault();

            foreach (var x in distinct)
            {
                if (x.MasterUser == null) continue;
                //if (x.MasterUser.MasterUserRoleMappings.Where(f => f.IDRole == 1 || f.IDRole == 2).ToList().Count == 0)
                if (x.MasterUser.MasterUserRoleMappings.Where(f => IncludeRoleIDs.Contains(f.IDRole)).ToList().Count > 0)
                {
                    MasterUserLocationMappingDTO temp = new MasterUserLocationMappingDTO()
                    {
                        MasterUser = Mapper.Map<MasterUserDTO>(_generalUserRepo.GetByID(x.IDUser))
                    };
                    foreach (var tempRole in temp.MasterUser.MasterUserRoleMappings)
                    {
                        temp.MasterRole += tempRole.MasterRole.RoleName + ",";
                    }
                    if (temp.MasterRole != null)
                        temp.MasterRole = temp.MasterRole.Remove(temp.MasterRole.Length - 1);
                    temp.IDUser = x.IDUser;
                    temp.IsActive = x.IsActive;
                    temp.CreatedDate = x.CreatedDate;
                    temp.CreatedBy = x.CreatedBy;
                    temp.UpdatedDate = x.UpdatedDate;
                    temp.UpdatedBy = x.UpdatedBy;
                    tempList.Add(temp);
                }
            }
            return tempList;
        }

        public List<string> GetListIDLocationByIdUser(string id)
        {
            var queryFilter = PredicateHelper.True<MasterUserLocationMapping>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.IDUser.Equals(id));
            return _generalRepo.Get(queryFilter).Select(x => x.IDLocation).ToList();
        }

        public List<string> GetWarehouseNameByIdUser(string id)
        {
            TOMContextDB context = new TOMContextDB();
            var getwarehousename = (from b in context.MasterUserLocationMappings
                                    join a in context.MasterLocations on b.IDLocation equals a.IDLocation
                                    where b.IDUser == id && a.Type == "Warehouse"
                                    select new { locationName = a.LocationName + " - " + b.IsActive.ToString() }).Select(m => m.locationName).ToList();

            return Mapper.Map<List<string>>(getwarehousename);
        }

        public List<MasterUserLocationMappingDTO> GetMasterFunctionByIdUser(string id)
        {
            List<MasterUserLocationMappingDTO> tempList = new List<MasterUserLocationMappingDTO>();
            TOMContextDB context = new TOMContextDB();
            var joinTable = (from mrf in context.MasterRolesFunctionMappings
                             join mr in context.MasterRoles on mrf.IDRole equals mr.IDRole
                             join mf in context.MasterFunctions on mrf.IDFunction equals mf.IDFunction
                             join mur in context.MasterUserRoleMappings on mrf.IDRole equals mur.IDRole
                             where mur.IDUser == id && mf.Type == "form"
                             select new { mr.RoleName, mf.FunctionName }).OrderBy(m => m.RoleName).ToList();
            var distinct = from c in joinTable
                           group c by new
                           {
                               c.RoleName,
                               c.FunctionName
                           }
                into grp
                           select grp.FirstOrDefault();
            foreach (var x in distinct)
            {
                MasterUserLocationMappingDTO temp = new MasterUserLocationMappingDTO()
                {
                    roleName = x.RoleName,
                    functionName = x.FunctionName
                };
                tempList.Add(temp);
            }
            return tempList;
        }

        public List<MasterUserLocationMappingDTO> GetMasterUserLocationMappingss(MasterUserLocationMappingInput input)
        {
            var queryFilter = PredicateHelper.True<MasterUserLocationMapping>();

            if (!string.IsNullOrEmpty(input.IDUser))
                queryFilter = queryFilter.And(m => m.IDUser == input.IDUser);
            if (!string.IsNullOrEmpty(input.IDLocation))
            {
                queryFilter = queryFilter.And(m => m.IDLocation == input.IDLocation);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterUserLocationMapping>();

            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterUserLocationMappingDTO>>(dbResult);
        }

        public List<MasterLocationDTO> GetMasterLocationLists(MasterLocationInput input)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();

            queryFilter = queryFilter.And(m => m.IsActive == input.IsActive);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterLocation>();

            var dbResult = _generalLocationRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterLocationDTO>>(dbResult);
        }

        public List<MasterUserDTO> GetMasterUserLists(MasterUserInput input)
        {
            var queryFilter = PredicateHelper.True<MasterUser>();
            using (var context = new TOMContextDB())
            {
                var tplv = (from x in context.MasterUsers
                            select new MasterUserDTO
                            {
                                IDUser = x.IDUser,
                                FullName = x.FullName,
                                Email = x.Email,
                                Address = x.Address,
                                Phone = x.Phone,
                                IsActive = x.IsActive,
                                CreatedDate = x.CreatedDate,
                                UpdatedDate = x.UpdatedDate
                            })
                            .OrderByDescending(x => x.UpdatedDate)
                            .OrderByDescending(x => x.CreatedDate)
                            .ToList();



                Tuple<IEnumerable<string>, string> sortCriteria = null;

                if (!string.IsNullOrEmpty(input.SortExpression) && !string.IsNullOrEmpty(input.SortOrder))
                {
                    sortCriteria = new Tuple<IEnumerable<string>, string>(
                    new[] { input.SortExpression },
                    input.SortOrder);
                }



                return tplv;
            }

            //queryFilter = queryFilter.And(m => m.IsActive == true);
            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            //var orderByFilter = sortCriteria.GetOrderByFunc<MasterUser>();

            //var dbResult = _generalUserRepo.Get(queryFilter, orderByFilter).ToList();
            //return Mapper.Map<List<MasterUserDTO>>(dbResult);
        }


        public MasterUserLocationMappingDTO SaveData(MasterUserLocationMappingDTO input)
        {
            MasterUserLocationMapping masterUserLocation = new MasterUserLocationMapping();
            List<string> tempCheckParentLocation = new List<string>();
            foreach (string temp in input.ListIDLocation)
            {
                string idloc = temp;
                var queryFilter = PredicateHelper.True<MasterUserLocationMapping>();
                queryFilter = queryFilter.And(p => p.IDUser.Equals(input.IDUser));
                queryFilter = queryFilter.And(p => p.IDLocation.Equals(idloc));
                masterUserLocation = _generalRepo.Get(queryFilter).SingleOrDefault();
                if (masterUserLocation != null)
                {
                    masterUserLocation.IsActive = true;
                    masterUserLocation.UpdatedDate = DateTime.Now;
                    masterUserLocation.UpdatedBy = input.UpdatedBy;
                    _generalRepo.Update(masterUserLocation);
                    var prtId = _generalLocationRepo.Get(c => c.IDLocation == masterUserLocation.IDLocation).Select(s => s.ParentLocation).FirstOrDefault();
                    if (!tempCheckParentLocation.Contains(prtId))
                        tempCheckParentLocation.Add(prtId);
                }
                else
                {
                    masterUserLocation = Mapper.Map<MasterUserLocationMapping>(input);
                    masterUserLocation.IDLocation = temp;
                    masterUserLocation.CreatedDate = DateTime.Now;
                    masterUserLocation.UpdatedDate = DateTime.Now;
                    _generalRepo.Insert(masterUserLocation);
                    MasterLocation tempMasterLocation = _masterLocationRepo.GetMasterLocationByID(temp);
                    if (!tempCheckParentLocation.Any(x => x == tempMasterLocation.ParentLocation))
                        tempCheckParentLocation.Add(tempMasterLocation.ParentLocation);
                }
            }
            _generalRepo.Save();
            foreach (string parentloc in tempCheckParentLocation)
            {
                // ambil semua anak parentloc
                var validLocation = _generalLocationRepo.Get(c => c.ParentLocation == parentloc);
                // filter user location mapping yang lokasinya adalah anak parentloc dan datanya aktif
                var tempListUserLoc = _generalRepo.Get().Where(x => validLocation.Any(w => w.IDLocation == x.IDLocation) && x.IDUser == input.IDUser && x.IsActive).ToList();//cari di tabel master user location mapping yang parent location A dan anak2nya semua aktif
                var queryFilter = PredicateHelper.True<MasterUserLocationMapping>();
                queryFilter = queryFilter.And(p => p.IDUser == input.IDUser);
                queryFilter = queryFilter.And(p => p.IDLocation == parentloc);
                MasterUserLocationMapping tempUserLoc = _generalRepo.Get(queryFilter).FirstOrDefault();
                if (tempListUserLoc.Count > 0)//jika ada 1 yang aktif
                {
                    if (tempUserLoc == null) //jika belum ada di tabel, insert baru
                    {
                        tempUserLoc = new MasterUserLocationMapping()
                        {
                            IDUser = input.IDUser,
                            IDLocation = parentloc,
                            IsActive = true,
                            CreatedBy = input.CreatedBy,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = input.UpdatedBy,
                            UpdatedDate = DateTime.Now
                        };
                        _generalRepo.Insert(tempUserLoc);
                    }
                    else
                    {
                        tempUserLoc.IsActive = true;
                        tempUserLoc.UpdatedDate = DateTime.Now;
                        tempUserLoc.UpdatedBy = input.UpdatedBy;
                        _generalRepo.Update(tempUserLoc);
                    }
                }
                else
                {
                    tempUserLoc.IsActive = false;
                    _generalRepo.Update(tempUserLoc);
                }
            }
            _generalRepo.Save();
            return Mapper.Map<MasterUserLocationMappingDTO>(masterUserLocation);
        }

        public MasterUserLocationMappingDTO EditData(MasterUserLocationMappingDTO input)
        {
            MasterUserLocationMapping masterUserLocation = new MasterUserLocationMapping();
            List<string> tempCheckParentLocation = new List<string>();
            foreach (string temp in input.ListIDLocation)
            {
                string idloc = temp;
                var queryFilter = PredicateHelper.True<MasterUserLocationMapping>();
                queryFilter = queryFilter.And(p => p.IDUser.Equals(input.IDUser));
                queryFilter = queryFilter.And(p => p.IDLocation.Equals(idloc));
                masterUserLocation = _generalRepo.Get(queryFilter).SingleOrDefault();
                if (masterUserLocation != null)
                {
                    masterUserLocation.IsActive = !masterUserLocation.IsActive;
                    masterUserLocation.UpdatedDate = DateTime.Now;
                    masterUserLocation.UpdatedBy = input.UpdatedBy;
                    _generalRepo.Update(masterUserLocation);
                    var prtId = _generalLocationRepo.Get(c => c.IDLocation == masterUserLocation.IDLocation).Select(s => s.ParentLocation).FirstOrDefault();
                    if (!tempCheckParentLocation.Contains(prtId))
                        tempCheckParentLocation.Add(prtId);
                }
                else
                {
                    MasterLocation tempMasterLocation = _masterLocationRepo.GetMasterLocationByID(temp);
                    masterUserLocation = Mapper.Map<MasterUserLocationMapping>(input);
                    masterUserLocation.IDLocation = temp;
                    masterUserLocation.IsActive = true;
                    masterUserLocation.CreatedBy = masterUserLocation.UpdatedBy;
                    masterUserLocation.CreatedDate = DateTime.Now;
                    masterUserLocation.UpdatedBy = masterUserLocation.UpdatedBy;
                    masterUserLocation.UpdatedDate = DateTime.Now;
                    _generalRepo.Insert(masterUserLocation);
                    if (!tempCheckParentLocation.Any(x => x == tempMasterLocation.ParentLocation))
                        tempCheckParentLocation.Add(tempMasterLocation.ParentLocation);
                }
            }
            _generalRepo.Save();
            _generalRepo.DetachAll();

            foreach (string parentloc in tempCheckParentLocation)
            {
                // ambil semua anak parentloc
                var validLocation = _generalLocationRepo.Get(c => c.ParentLocation == parentloc);
                // filter user location mapping yang lokasinya adalah anak parentloc dan datanya aktif
                var tempListUserLoc = _generalRepo.Get().Where(x => validLocation.Any(w => w.IDLocation == x.IDLocation) && x.IDUser == input.IDUser && x.IsActive).ToList();//cari di tabel master user location mapping yang parent location A dan anak2nya semua aktif
                var queryFilter = PredicateHelper.True<MasterUserLocationMapping>();
                queryFilter = queryFilter.And(p => p.IDUser == input.IDUser);
                queryFilter = queryFilter.And(p => p.IDLocation == parentloc);
                MasterUserLocationMapping tempUserLoc = _generalRepo.Get(queryFilter).FirstOrDefault();
                if (tempListUserLoc.Count > 0)//jika ada 1 yang aktif
                {
                    if (tempUserLoc == null) //jika belum ada di tabel, insert baru
                    {
                        tempUserLoc = new MasterUserLocationMapping()
                        {
                            IDUser = input.IDUser,
                            IDLocation = parentloc,
                            IsActive = true,
                            CreatedBy = input.UpdatedBy,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = input.UpdatedBy,
                            UpdatedDate = DateTime.Now,
                            Remarks = "",
                            status = null
                        };
                        _generalRepo.Insert(tempUserLoc);
                        //_generalRepo.InsertOrUpdate(tempUserLoc);
                    }
                    else
                    {
                        tempUserLoc.IsActive = true;
                        tempUserLoc.UpdatedDate = DateTime.Now;
                        tempUserLoc.UpdatedBy = input.UpdatedBy;
                        _generalRepo.Update(tempUserLoc);
                        //_generalRepo.InsertOrUpdate(tempUserLoc);
                    }
                }
                else
                {
                    tempUserLoc.IsActive = false;
                    _generalRepo.Update(tempUserLoc);
                    //_generalRepo.InsertOrUpdate(tempUserLoc);
                }
            }
            _generalRepo.Save();
            return Mapper.Map<MasterUserLocationMappingDTO>(masterUserLocation);
        }

        public MasterUserLocationMappingDTO GetById(string id)
        {
            var result = _generalRepo.GetByID(id);
            return Mapper.Map<MasterUserLocationMappingDTO>(result);
        }

        public void UpdateIsActiveByIDUser(string iduser, bool status)
        {
            var queryFilter = PredicateHelper.True<MasterUserLocationMapping>();
            queryFilter = queryFilter.And(p => p.IDUser.Equals(iduser));
            List<MasterUserLocationMapping> listMasterUserLocation = _generalRepo.Get(queryFilter).ToList();
            foreach (MasterUserLocationMapping temp in listMasterUserLocation)
            {
                temp.IsActive = status;
                _generalRepo.Update(temp);
                _generalRepo.Save();
            }
        }

        public List<MasterUserLocationMapTreeListView> GetMasterUserLocationMapTreeList()
        {
            var queryFilter = PredicateHelper.True<MasterUserLocationMapTreeListView>();
            return _generalUserLocViewRepo.Get(queryFilter).OrderBy(x => x.NameZone).ThenBy(x => x.NameRegion).ThenBy(x => x.NameWarehouse).ToList();
        }

        public List<MasterUserLocationMappingDTO> GetLocationsByIdUserAndWarehouseType(string id)
        {
            /*id = id == null ? "" : id.ToUpper();

            var locs = _generalLocationRepo.Get(c => c.IsAssigned == true & type.Contains(c.Type)).Select(l => l.IDLocation).ToList();
            var map = Mapper.Map<List<MasterUserLocationMappingDTO>>(_generalRepo.Get(v => v.IDUser.ToUpper() == id && locs.Contains(v.IDLocation)).ToList());
            foreach (var m in map)
            {
                m.MasterUser = null;
                m.MasterRole = null;
                m.MasterLocation = Mapper.Map<MasterLocationDTO>(_generalLocationRepo.Get(c => c.IDLocation == m.IDLocation).FirstOrDefault());
            }

            return map;*/
            
            var context = new TOMContextDB();
            var joinResult = (from mstLoc in context.MasterLocations
                              join mstmapping in context.MasterUserLocationMappings on mstLoc.IDLocation equals mstmapping.IDLocation
                              where mstmapping.IDUser.ToUpper() == id.ToUpper() //&& mstLoc.Type == type

                              select new MasterUserLocationMappingDTO()
                              {
                                  IDUserLocationMapping = mstmapping.IDUserLocationMapping,
                                  IDUser = mstmapping.IDUser,
                                  IDLocation = mstmapping.IDLocation,
                                  LocationName = mstLoc.LocationName
                              }).ToList();

            return Mapper.Map<List<MasterUserLocationMappingDTO>>(joinResult);
            
        }
    }
}