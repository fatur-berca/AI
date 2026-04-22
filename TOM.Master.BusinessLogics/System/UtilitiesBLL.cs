using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security.AntiXss;
using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Outputs;
using DFIS.Utils.Exceptions;
using TOM.Master.Domain.DTOs;

namespace DFIS.Universal.BusinessLogics
{
    public class UtilitiesBLL : IUtilitiesBLL
    {
        private IGenericRepository<MasterUserRoleMapping> _masterUserRoleMappingRepo;
        private IGenericRepository<MasterLocation> _masterLocationRepo;
        private IGenericRepository<MasterUserLocationMapping> _masterUserLocationMapping;
        private IGenericRepository<UserRolesSecurityView> _userRolesSecurityViewRepo;
        //private IGenericRepository<NewsHighlight> _newsRepo;


        public UtilitiesBLL(IGenericRepository<MasterUserRoleMapping> masterUserRoleMappingRepo,
            IGenericRepository<MasterLocation> masterLocationRepo, IGenericRepository<UserRolesSecurityView> userRolesSecurityViewRepo,
            IGenericRepository<MasterUserLocationMapping> masterUserLocationMapping)
        {
            _masterUserRoleMappingRepo = masterUserRoleMappingRepo;
            _masterLocationRepo = masterLocationRepo;
            _userRolesSecurityViewRepo = userRolesSecurityViewRepo;
            _masterUserLocationMapping = masterUserLocationMapping;
        }
        public List<MasterUserRoleMappingDTO> GetListRole(string username)
        {
            var dbResult = _masterUserRoleMappingRepo.Get(x => x.IDUser == username).ToList();
            return Mapper.Map<List<MasterUserRoleMappingDTO>>(dbResult);
        }

        public List<FunctionPage> GetRolePage(string username)
        {
            var listRolesAndFunction =
                _userRolesSecurityViewRepo.Get(x => x.IDUser.ToLower() == username.ToLower()).Select(x => new FunctionPage() { IDFunction = x.IDFunction, FunctionName = x.FunctionName, Type = x.Type })
                    .OrderBy(x => x.IDFunction).ToList();
            var listPage = listRolesAndFunction.Where(x => x.Type.Contains("Form")).Select(x => new FunctionPage() { IDFunction = x.IDFunction, FunctionName = x.FunctionName, Type = x.Type }).ToList();
            return listPage;
        }

        //public UserSession GetResponsibilityPage(string iduser, List<MasterUserLocationMapping> listLocationMap)
        //{
        //    var result = new UserSession();
        //    // step 1 get role and function from view (page, button etc)
        //    var listRolesAndFunction =
        //        _userRolesSecurityViewRepo.Get(x => x.IDUser.ToLower() == iduser.ToLower()).Select(x => new FunctionPage() { IDFunction = x.IDFunction, FunctionName = x.FunctionName, Type = x.Type })
        //            .OrderBy(x => x.IDFunction).ToList();
        //    var listPage = listRolesAndFunction.Where(x => x.Type.Contains("Form")).Select(x => new FunctionPage() { IDFunction = x.IDFunction, FunctionName = x.FunctionName, Type = x.Type }).ToList(); 
        //    //var listButton = listRolesAndFunction.Where(x => x.Type.Contains("Button")).Select(x => new FunctionPage() { IDFunction = x.IDFunction, FunctionName = x.FunctionName, Type = x.Type }).ToList();
        //    var listRole =
        //        _userRolesSecurityViewRepo.Get(x => x.IDUser == iduser.ToLower()).GroupBy(x => x.IDRole)
        //            .Select(x => new UserRole() {IDRole = x.Key, RoleName = x.Select(y=>y.rolename).FirstOrDefault()})
        //            .ToList();
        //    result.Page = listPage;
        //    //result.Button = listButton;
        //    result.Role = listRole;


        //    // step 2 get user and location
        //    var listLocation = _masterLocationRepo.Get();
        //    var joinedLocation = (from listMap in listLocationMap
        //        join listLoc in listLocation on listMap.IDLocation equals listLoc.IDLocation
        //                          where listMap.IDUser == iduser.ToLower()
            
        //        select new UserLocationMap()
        //        {
        //            IDLocation = listLoc.IDLocation,
        //            LocationName = listLoc.LocationName,
        //            Type = listLoc.Type
        //        }).ToList();

        //    result.Location = joinedLocation;

        //    return result;
        //}

        public UserSession GetResponsibilityPage(string iduser, int idRole)
        {
            var result = new UserSession();
            // step 1 add data by iduser
            //var dbResult = _userRolesSecurityViewRepo.Get(x => x.IDUser.ToLower() == iduser.ToLower() && x.IDRole == idRole).ToList();
            var dbResult = _userRolesSecurityViewRepo.Get(x => x.IDRole == idRole).ToList();
            // step 2. get listpage
            var listPage =
                dbResult.Where(x => x.Type == "Form")
                    .Select(x => new FunctionPage {IDFunction = x.IDFunction, FunctionName = x.FunctionName, Type = x.Type})
                    .ToList();
            //step 3. get listrole
            //var listRole = dbResult
            //        .Select(x => new UserRole { IDRole = x.IDRole, RoleName = x.rolename }).Distinct()
            //        .ToList();
            var listRole =
                dbResult.GroupBy(x => x.IDRole)
                    .Select(x => new UserRole() {IDRole = x.Key, RoleName = x.Select(y => y.rolename).FirstOrDefault()}).ToList();
            // step 4. get listLocation
            var listLocation =
                _masterUserLocationMapping.Get().Where(x => x.IDUser.ToLower() == iduser.ToLower() && x.IsActive).Select(x => new UserLocationMap
            {
                IDLocation = x.IDLocation, IsActive = x.IsActive
            }).ToList();


            foreach(var loc in listLocation)
            {
                var locEntity = _masterLocationRepo.Get(lc => lc.IDLocation == loc.IDLocation).FirstOrDefault();
                if (locEntity != null)
                {
                    loc.LocationName = locEntity.LocationName;
                    loc.Type = locEntity.Type;
                }
            }

            //step 5. getlist button
            var listButton = dbResult.Where(x => x.Type == "Button" && x.IDUser.ToLower() == iduser.ToLower())
                    .Select(x => new FunctionPage { IDFunction = x.IDFunction, FunctionName = x.FunctionName, Type = x.Type, ParentIdFunction = Convert.ToInt32(x.ParentIDFunction)})
                    .ToList();

            // step 6. assign  page, role, location
            result.Page = listPage;
            result.Role = listRole;
            result.Location = listLocation;
            result.Button = listButton;

            return result;
        }

        public List<String> GetResponsibilityButton(string iduser, int parentID)
        {
            var listButton =
                _userRolesSecurityViewRepo.Get()
                    .Where(x => x.IDUser.ToLower() == iduser.ToLower() && x.ParentIDFunction == parentID)
                    .ToList();
            var filteredButton = listButton.Distinct().Select(x => x.FunctionName).ToList();
            
            return filteredButton;
        }

        public List<NewsHighlightDTO> GetNews()
        {
           // var dbResult = _newsRepo.Get().Where(x => x.Status == 2 && x.IsActive).Take(3).ToList();
          //var dbResult = _newsRepo.Get().Where(x => x.Status == 2).Take(3).ToList();


            //return Mapper.Map<List<NewsHighlightDTO>>(dbResult);
            return new List<NewsHighlightDTO>();
        }

        public int SaveMasterUserRoleMapping(MasterUserRoleMappingDTO Input)
        {
            try
            {

                var prevData = _masterUserRoleMappingRepo.Get().Where(x => x.IDUser == Input.IDUser && x.IDRole == Input.IDRole).FirstOrDefault();
                var dbMst = Mapper.Map<MasterUserRoleMapping>(Input);

                if (prevData == null)
                {
                    _masterUserRoleMappingRepo.Insert(dbMst);
                    return _masterUserRoleMappingRepo.Save();
                }
                else
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
                }
            }
            catch (ExceptionBase ex)
            {
                var message = ex;
                return 0;
            }
        }

        public int SaveMasterUserLocationMapping(MasterUserLocationMappingDTO Input)
        {
            try
            {
                var prevData = _masterUserLocationMapping.Get().Where(x => x.IDUser == Input.IDUser && x.IDLocation == Input.IDLocation).FirstOrDefault();
                var dbMst = Mapper.Map<MasterUserLocationMapping>(Input);

                if (prevData == null)
                {
                    dbMst.CreatedDate = DateTime.Now;
                    dbMst.UpdatedDate = DateTime.Now;
                    dbMst.Remarks = null;
                    dbMst.status = "Delegated";
                    _masterUserLocationMapping.Insert(dbMst);
                    return _masterUserLocationMapping.Save();
                }
                else
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
                }
            }
            catch (ExceptionBase ex)
            {
                var message = ex;
                return 0;
            }
        }
    }
}
