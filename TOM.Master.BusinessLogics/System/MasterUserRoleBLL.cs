using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using DFIS.Universal.BusinessLogics;

namespace TOM.Master.BusinessLogics
{
    public class MasterUserRoleBLL : IMasterUserRoleBLL
    {
        private readonly IGenericRepository<MasterUserRoleMapping> _generalRepo;
        private readonly IGenericRepository<MasterUser> _generalUserRepo;
        private readonly IGenericRepository<MasterRole> _generalRoleRepo;

        public MasterUserRoleBLL(IGenericRepository<MasterUserRoleMapping> generalRepo, IGenericRepository<MasterUser> generalUserRepo, IGenericRepository<MasterRole> generalRoleRepo)
        {
            _generalRepo = generalRepo;
            _generalUserRepo = generalUserRepo;
            _generalRoleRepo = generalRoleRepo;
          
        }
        public List<MasterUserRoleMappingDTO> GetDataByIDs(MasterUserRoleMappingDTO input)
        {
            var queryFilter = PredicateHelper.True<MasterUserRoleMapping>();

            //var dbResult = _generalRepo.Get(queryFilter).ToList();



            queryFilter = queryFilter.And(m => m.IDUserRoleMapping == input.IDUserRoleMapping);
        
            var dbResult = _generalRepo.Get(queryFilter).ToList();


            TOMContextDB context = new TOMContextDB();
            var query = (from a in dbResult
                         join c in context.MasterUsers on a.IDUser equals c.IDUser
                         join b in context.MasterRoles on a.IDRole equals b.IDRole
                         select new MasterUserRoleMappingDTO()
                         {
                             IDUserRoleMapping = a.IDUserRoleMapping,
                             IDUser = a.IDUser,
                             IDRole = a.IDRole,
                             Name = c.FullName,
                             RoleName = b.RoleName,
                             UpdatedBy = a.UpdatedBy,
                             UpdatedDate = a.UpdatedDate,
                             IsActive = a.IsActive,
                             Remarks = a.Remarks

                         });
            query = query.ToList();
            return Mapper.Map<List<MasterUserRoleMappingDTO>>(query);
        }
        public List<MasterUserRoleMappingDTO> GetUserRoleMapping(MasterUserRoleMappingDTO input)
        {
            var queryFilter = PredicateHelper.True<MasterUserRoleMapping>();

            //var dbResult = _generalRepo.Get(queryFilter).ToList();
            if (input.IDUser != null)
            {
                queryFilter = queryFilter.And(x => x.IDUser == input.IDUser);
            }
            var dbResult = _generalRepo.Get(queryFilter).ToList();


            TOMContextDB context = new TOMContextDB();
            var query = (from a in dbResult
                         join c in context.MasterUsers on a.IDUser equals c.IDUser
                         join b in context.MasterRoles on a.IDRole equals b.IDRole
                         select new MasterUserRoleMappingDTO()
                         {
                             IDUserRoleMapping = a.IDUserRoleMapping,
                             IDUser = a.IDUser,
                             IDRole = a.IDRole,
                             Name = c.FullName,
                             RoleName = b.RoleName,
                             UpdatedBy = a.UpdatedBy,
                             UpdatedDate = a.UpdatedDate,
                             IsActive = a.IsActive,
                             Remarks = a.Remarks
                            
                         });
            query = query.ToList();
            return Mapper.Map<List<MasterUserRoleMappingDTO>>(query);
        }

        public List<MasterUserDTO> GetMasterUsers()
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
                            .OrderByDescending(x => x.CreatedDate).Where(m => m.IsActive == true)
                            .ToList();

                return tplv;
            }

            //var queryFilter = PredicateHelper.True<MasterUser>();

            //queryFilter = queryFilter.And(m => m.IsActive == true);       
        
            //var dbResult = _generalUserRepo.Get(queryFilter).ToList();
            //return Mapper.Map<List<MasterUserDTO>>(dbResult);
        }

        public List<MasterRoleDTO> GetMasterRoles()
        {
            var queryFilter = PredicateHelper.True<MasterRole>();
            using (var context = new TOMContextDB())
            {
                var tplv = (from x in context.MasterRoles
                            select new MasterRoleDTO
                            {
                                IDRole = x.IDRole,
                                RoleName = x.RoleName,
                                IsActive = x.IsActive,
                                CreatedBy = x.CreatedBy,
                                CreatedDate = x.CreatedDate,
                                UpdatedBy = x.UpdatedBy,
                                UpdatedDate = x.UpdatedDate,
                                Remarks = x.Remarks
                            })
                            .OrderByDescending(x => x.UpdatedDate)
                            .OrderByDescending(x => x.CreatedDate).Where(m => m.IsActive == true)
                            .ToList();

                return tplv;

                //var queryFilter = PredicateHelper.True<MasterRole>();

                //queryFilter = queryFilter.And(m => m.IsActive == true);

                //var dbResult = _generalRoleRepo.Get(queryFilter).ToList();
                //return Mapper.Map<List<MasterRoleDTO>>(dbResult);
            }
        }

        public MasterUserRoleMappingDTO SaveData(MasterUserRoleMappingDTO input, string controller, string userid)
        {
            var validateInput = new MasterUserRoleMappingDTO()
            {
                IDUser = input.IDUser
               
            };

            var prevData = GetUserRoleMapping(validateInput);
            // var check = GetById(input.IDVendor);
            var dbMstUser = Mapper.Map<MasterUserRoleMapping>(input);

            if (prevData == null || prevData.Count == 0)
            {
                dbMstUser.CreatedDate = DateTime.Now;
                dbMstUser.UpdatedDate = DateTime.Now;
                _generalRepo.Insert(dbMstUser);
                _generalRepo.Save(controller, userid);
            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterUserRoleMappingDTO>(dbMstUser);
        }

        public MasterUserRoleMappingDTO EditData(MasterUserRoleMappingDTO input, string controller, string userid)
        {


            var dbMstVendor = Mapper.Map<MasterUserRoleMapping>(input);

            var validateInput = new MasterUserRoleMappingDTO()
            {
                IDUser = input.IDUser
            };

            var prevData = GetUserRoleMapping(validateInput);
            // var vendorid = _generalRepo.Get().Where(x => x.IDVendor == input.IDVendor).Select(x => x.IDVendor).FirstOrDefault();
            //var namevendor = _generalRepo.Get().Where(x => x.VendorName == input.VendorName && x.IDVendor == input.IDVendor).Select(x => x.VendorName).FirstOrDefault();


            if (prevData != null || prevData.Count != 0)
            {

                TOMContextDB context = new TOMContextDB();


                MasterUserRoleMapping c = (from x in context.MasterUserRoleMappings
                                where x.IDUser == input.IDUser
                                select x).First();
                c.IDRole = input.IDRole;                
                c.UpdatedDate = DateTime.Now;
                c.IsActive = input.IsActive;
                c.UpdatedBy = input.UpdatedBy;
                context.SaveChanges();

            }

            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }
            return Mapper.Map<MasterUserRoleMappingDTO>(dbMstVendor);
        }

        public bool UserHasRole(string userID, params int[] roleIDs)
        {
            var roles = GetUserRoleMapping(new MasterUserRoleMappingDTO() { IDUser = userID }).Select(n => n.IDRole).ToList();
            bool auth = false;
            foreach (var rid in roleIDs)
                if (roles.Contains(rid)) { auth = true; break; }
            return auth;
        }
    }
}
