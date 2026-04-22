using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.Master.Repositories;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public class MasterIMDLRoleLocationBLL : IMasterIMDLRoleLocationBLL
    {
        private readonly IGenericRepository<MasterIMDLRoleLocation> _generalRepo;
        private readonly IMasterIMDLRoleLocationRepo _masterIMDLRoleLocationRepo;
        private readonly IMasterLocationRepo _masterLocationRepo;
        private readonly IMasterIMDLRoleRepo _masterIMDLRoleRepo;
        private readonly IGenericRepository<MasterUserLocationMapTreeListView> _generalUserLocViewRepo;

        public MasterIMDLRoleLocationBLL(IMasterIMDLRoleLocationRepo masterIMDLRoleLocationRepo, 
            IGenericRepository<MasterIMDLRoleLocation> generalRepo, 
            IMasterLocationRepo masterLocationRepo, 
            IMasterIMDLRoleRepo masterIMDLRoleRepo,
            IGenericRepository<MasterUserLocationMapTreeListView> generalUserLocViewRepo)
        {
            _masterIMDLRoleLocationRepo = masterIMDLRoleLocationRepo;
            _generalRepo = generalRepo;
            _masterLocationRepo = masterLocationRepo;
            _masterIMDLRoleRepo = masterIMDLRoleRepo;
            _generalUserLocViewRepo = generalUserLocViewRepo;
        }

        public List<MasterIMDLRoleLocationDTO> GetMasterIMDLRoleLocations(MasterIMDLRoleLocationInput input)
        {
            var queryFilter = PredicateHelper.True<MasterIMDLRoleLocation>();

            if (input.IDIMDLRoleLocation != 0)
                queryFilter = queryFilter.And(y => y.IDIMDLRoleLocation == input.IDIMDLRoleLocation);

            var dbResult = _generalRepo.Get(queryFilter).ToList();

            var context = new TOMContextDB();
            var query = (from a in dbResult
                         join b in context.MasterLocations on a.IDLocation equals b.IDLocation
                         select new MasterIMDLRoleLocationDTO()
                         {
                             IDIMDLRoleLocation = a.IDIMDLRoleLocation,
                             IMDLRole = a.IMDLRole,
                             IDLocation = b.IDLocation,
                             LocationName = b.LocationName,
                             UpdatedBy = a.UpdatedBy,
                             UpdatedDate = a.UpdatedDate
                         });
            query = query.ToList();
            return Mapper.Map<List<MasterIMDLRoleLocationDTO>>(query);
        }

        public List<MasterUserLocationMapTreeListView> GetTreeList()
        {
            var queryFilter = PredicateHelper.True<MasterUserLocationMapTreeListView>();
            return _generalUserLocViewRepo.Get(queryFilter).OrderBy(x => x.NameZone).ThenBy(x => x.NameRegion).ThenBy(x => x.NameWarehouse).ToList();
        }

        public List<string> GetListIDLocationByIdUser(string id)
        {
            var queryFilter = PredicateHelper.True<MasterIMDLRoleLocation>();
            queryFilter = queryFilter.And(p => (p.IsActive == true));
            queryFilter = queryFilter.And(p => p.IMDLRole.Equals(id));
            return _generalRepo.Get(queryFilter).Select(x => x.IDLocation).ToList();
        }

        public MasterIMDLRoleLocationDTO GetById(int id)
        {
            var result = _generalRepo.GetByID(id);
            return Mapper.Map<MasterIMDLRoleLocationDTO>(result);
        }

        public List<string> GetWarehouseNameByIdUser(string id)
        {
            TOMContextDB context = new TOMContextDB();
            var getwarehousename = (from b in context.MasterIMDLRoleLocations
                                    join a in context.MasterLocations on b.IDLocation equals a.IDLocation
                                    where b.IMDLRole == id && a.Type == "Warehouse"
                                    select new { locationName = a.LocationName + " - " + b.IsActive.ToString() }).Select(m => m.locationName).ToList();

            return Mapper.Map<List<string>>(getwarehousename);
        }

        public List<MasterIMDLRoleLocationDTO> GetDistinctLocationMasterUserLocation()
        {
            List<MasterIMDLRoleLocationDTO> tempList = new List<MasterIMDLRoleLocationDTO>();
            List<MasterIMDLRole> listMasterIMDLRole = _masterIMDLRoleRepo.GetAllMasterIMDLRoleActive();
            foreach (MasterIMDLRole mir in listMasterIMDLRole)
            {
                if (mir.MasterIMDLRoleLocations.Count > 0)
                {
                    MasterIMDLRoleLocationDTO temp = new MasterIMDLRoleLocationDTO()
                    {
                        IMDLRole = mir.IMDLRole,
                        IsActive = mir.IsActive,
                        CreatedDate = mir.CreatedDate,
                        CreatedBy = mir.CreatedBy,
                        UpdatedDate = mir.UpdatedDate,
                        UpdatedBy = mir.UpdatedBy
                    };
                    tempList.Add(temp);
                }
            }
            return tempList;
        }

        public MasterIMDLRoleLocationDTO SaveData(MasterIMDLRoleLocationDTO input, string controller, string userid)
        {
            var masterIMDLLocation = new MasterIMDLRoleLocation();
            List<string> tempCheckParentLocation = new List<string>();
            //var dbMasterIMDLRoleLocation = Mapper.Map<MasterIMDLRoleLocation>(input);

            foreach (string temp in input.ListIDLocation)
            {
                string idloc = temp;
                var queryFilter = PredicateHelper.True<MasterIMDLRoleLocation>();
                queryFilter = queryFilter.And(p => p.IMDLRole.Equals(input.IMDLRole));
                queryFilter = queryFilter.And(p => p.IDLocation.Equals(idloc));
                masterIMDLLocation = _generalRepo.Get(queryFilter).SingleOrDefault();
                if (masterIMDLLocation != null)
                {
                    masterIMDLLocation.IsActive = true;
                    masterIMDLLocation.UpdatedDate = DateTime.Now;
                    masterIMDLLocation.UpdatedBy = input.UpdatedBy;
                    _generalRepo.Update(masterIMDLLocation);
                    //var prtId = _masterLocationRepo.Get(c => c.IDLocation == masterIMDLLocation.MasterMappingLocation.IDLocation).Select(v => v.ParentLocation).FirstOrDefault();
                    var prtId = _masterLocationRepo.Get().Select(v => v.ParentLocation).FirstOrDefault();
                    if (!tempCheckParentLocation.Any(x => x == prtId))
                        tempCheckParentLocation.Add(prtId);
                }
                else
                {
                    masterIMDLLocation = Mapper.Map<MasterIMDLRoleLocation>(input);
                    masterIMDLLocation.IDLocation = temp;
                    masterIMDLLocation.CreatedDate = DateTime.Now;
                    masterIMDLLocation.UpdatedDate = DateTime.Now;
                    _generalRepo.Insert(masterIMDLLocation);
                    MasterLocation tempMasterLocation = _masterLocationRepo.GetMasterLocationByID(temp);
                    if (!tempCheckParentLocation.Any(x => x == tempMasterLocation.ParentLocation))
                        tempCheckParentLocation.Add(tempMasterLocation.ParentLocation);
                }
            }
            _generalRepo.Save();

            //foreach (string parentloc in tempCheckParentLocation)
            //{
            //    var tempListUserLoc = _generalRepo.Get().Where(x => x.MasterLocation.ParentLocation == parentloc && x.IMDLRole == input.IMDLRole && x.IsActive).ToList();//cari di tabel master user location mapping yang parent location A dan anak2nya semua aktif
            //    var queryFilter = PredicateHelper.True<MasterIMDLRoleLocation>();
            //    queryFilter = queryFilter.And(p => p.IMDLRole == input.IMDLRole);
            //    queryFilter = queryFilter.And(p => p.IDLocation == parentloc);
            //    MasterIMDLRoleLocation tempUserLoc = _generalRepo.Get(queryFilter).FirstOrDefault();
            //    if (tempListUserLoc.Count > 0)//jika ada 1 yang aktif
            //    {
            //        if (tempUserLoc == null) //jika belum ada di tabel, insert baru
            //        {
            //            tempUserLoc = new MasterIMDLRoleLocation();
            //            tempUserLoc.IMDLRole = input.IMDLRole;
            //            tempUserLoc.IDLocation = parentloc;
            //            tempUserLoc.IsActive = true;
            //            tempUserLoc.CreatedBy = input.CreatedBy;
            //            tempUserLoc.CreatedDate = DateTime.Now;
            //            tempUserLoc.UpdatedBy = input.UpdatedBy;
            //            tempUserLoc.UpdatedDate = DateTime.Now;
            //            _generalRepo.Insert(tempUserLoc);
            //        }
            //        else
            //        {
            //            tempUserLoc.IsActive = true;
            //            tempUserLoc.UpdatedDate = DateTime.Now;
            //            tempUserLoc.UpdatedBy = input.UpdatedBy;
            //            _generalRepo.Update(tempUserLoc);
            //        }
            //    }
            //    else
            //    {
            //        tempUserLoc.IsActive = false;
            //        _generalRepo.Update(tempUserLoc);
            //    }
            //}
            //_generalRepo.Save();

            return Mapper.Map<MasterIMDLRoleLocationDTO>(masterIMDLLocation);
            
        }

        public MasterIMDLRoleLocationDTO EditData(MasterIMDLRoleLocationDTO input, string controller, string userid)
        {
            var masterIMDLLocation = new MasterIMDLRoleLocation();
            List<string> tempCheckParentLocation = new List<string>();

            foreach (string temp in input.ListIDLocation)
            {
                string idloc = temp;
                var queryFilter = PredicateHelper.True<MasterIMDLRoleLocation>();
                queryFilter = queryFilter.And(p => p.IMDLRole.Equals(input.IMDLRole));
                queryFilter = queryFilter.And(p => p.IDLocation.Equals(idloc));
                masterIMDLLocation = _generalRepo.Get(queryFilter).SingleOrDefault();
                if (masterIMDLLocation != null)
                {
                    masterIMDLLocation.IsActive = !masterIMDLLocation.IsActive;
                    masterIMDLLocation.UpdatedDate = DateTime.Now;
                    masterIMDLLocation.UpdatedBy = input.UpdatedBy;
                    _generalRepo.Update(masterIMDLLocation);
                    //var prtId = _masterLocationRepo.Get(c => c.IDLocation == masterIMDLLocation.MasterMappingLocation.IDLocation).Select(v => v.ParentLocation).FirstOrDefault();
                    var prtId = _masterLocationRepo.Get().Select(v => v.ParentLocation).FirstOrDefault();
                    if (!tempCheckParentLocation.Any(x => x == prtId))
                        tempCheckParentLocation.Add(prtId);
                }
                else
                {
                    masterIMDLLocation = Mapper.Map<MasterIMDLRoleLocation>(input);
                    masterIMDLLocation.IDLocation = temp;
                    masterIMDLLocation.IsActive = true;
                    masterIMDLLocation.CreatedBy = masterIMDLLocation.UpdatedBy;
                    masterIMDLLocation.CreatedDate = DateTime.Now;
                    masterIMDLLocation.UpdatedDate = DateTime.Now;
                    _generalRepo.Insert(masterIMDLLocation);
                    MasterLocation tempMasterLocation = _masterLocationRepo.GetMasterLocationByID(temp);
                    if (!tempCheckParentLocation.Any(x => x == tempMasterLocation.ParentLocation))
                        tempCheckParentLocation.Add(tempMasterLocation.ParentLocation);
                }
            }
            _generalRepo.Save();

            return Mapper.Map<MasterIMDLRoleLocationDTO>(masterIMDLLocation);
        }

        public List<MasterLocationDTO> GetDataLocation()
        {
            var queryFil = PredicateHelper.True<MasterLocation>();

            queryFil = queryFil.And(m => m.IsActive == true);

            var dbRes = _masterLocationRepo.Get(queryFil).ToList();

            return Mapper.Map<List<MasterLocationDTO>>(dbRes);
        }

        public List<MasterIMDLRoleDTO> GetDataIMDLRole()
        {
            var queryFilter = PredicateHelper.True<MasterIMDLRole>();
            queryFilter = queryFilter.And(y => y.IsActive == true);
            var res = _masterIMDLRoleRepo.Get(queryFilter).ToList();
            return Mapper.Map<List<MasterIMDLRoleDTO>>(res);
        }

    }
}
