using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.Inputs;
using DFIS.Universal.Domain.DTOs;
using TOM.Master.Repositories;

namespace TOM.Master.BusinessLogics
{
    public class MasterIMDLRoleBLL : IMasterIMDLRoleBLL
    {
        private readonly IGenericRepository<MasterIMDLRole> _generalRepo;
        private readonly IMasterIMDLRoleRepo _masterIMDLRoleRepo;
        private readonly IGenericRepository<MasterRole> _masterRoleRepo;

        public MasterIMDLRoleBLL(IMasterIMDLRoleRepo masterIMDLRoleRepo, IGenericRepository<MasterIMDLRole> generalRepo, IGenericRepository<MasterRole> masterRoleRepo)
        {
            _masterIMDLRoleRepo = masterIMDLRoleRepo;
            _generalRepo = generalRepo;
            _masterRoleRepo = masterRoleRepo;
        }

        public List<MasterIMDLRoleDTO> GetMasterIMDLRoles(MasterIMDLRoleInput input)
        {
            var queryFilter = PredicateHelper.True<MasterIMDLRole>();

            if (!string.IsNullOrEmpty(input.IMDLRole))
                queryFilter = queryFilter.And(y => y.IMDLRole == input.IMDLRole);

            queryFilter = queryFilter.And(y => y.IsActive == true);
            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            //var orderByFilter = sortCriteria.GetOrderByFunc<MasterIMDLRole>();

            var dbResult = _generalRepo.Get(queryFilter).ToList();

            var context = new TOMContextDB();
            var query = (from a in dbResult
                         join b in context.MasterRoles on a.IDRole equals b.IDRole
                         select new MasterIMDLRoleDTO()
                         {
                             IMDLRole = a.IMDLRole,
                             IDRole = a.IDRole,
                             RoleName = b.RoleName,
                             UpdatedBy = a.UpdatedBy,
                             UpdatedDate = a.UpdatedDate,
                             IsActive = a.IsActive

                         });
            query = query.ToList();
            return Mapper.Map<List<MasterIMDLRoleDTO>>(query);
        }

        public MasterIMDLRoleDTO GetById(string id)
        {
            var result = _generalRepo.GetByID(id);
            return Mapper.Map<MasterIMDLRoleDTO>(result);
        }

        public MasterIMDLRoleDTO SaveData(MasterIMDLRoleDTO input, string controller, string userid)
        {
            var validateInput = new MasterIMDLRoleInput()
            {
                IMDLRole = input.IMDLRole,
                IDRole = input.IDRole,
                IsActive = input.IsActive
            };

            var prevData = GetMasterIMDLRoles(validateInput);
            var dbMasterIMDLRole = Mapper.Map<MasterIMDLRole>(input);

            if (prevData == null || (prevData != null && prevData.Count == 0))
            {
                dbMasterIMDLRole.IMDLRole = input.IMDLRole;
                dbMasterIMDLRole.IDRole = input.IDRole;
                dbMasterIMDLRole.CreatedDate = DateTime.Now;
                dbMasterIMDLRole.UpdatedDate = DateTime.Now;

                _generalRepo.Insert(dbMasterIMDLRole);
                _generalRepo.Save(controller, userid);
            }
            else
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }

            return Mapper.Map<MasterIMDLRoleDTO>(dbMasterIMDLRole);
        }

        public MasterIMDLRoleDTO EditData(MasterIMDLRoleDTO input, string controller, string userid)
        {
            var dbMasterIMDLRole = Mapper.Map<MasterIMDLRole>(input);

            var validateInput = new MasterIMDLRoleInput()
            {
                IMDLRole = input.IMDLRole,
                IDRole = input.IDRole,
                IsActive = input.IsActive
            };

            var prevData = GetMasterIMDLRoles(validateInput);

            if (prevData == null || prevData.Count != 0)
            {
                TOMContextDB context = new TOMContextDB();

                MasterIMDLRole c = context.MasterIMDLRoles
                    .Where(y => y.IMDLRole == input.IMDLRole).First();

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

            return Mapper.Map<MasterIMDLRoleDTO>(dbMasterIMDLRole);
        }

        public List<MasterRoleDTO> GetDataRole()
        {
            var queryFil = PredicateHelper.True<MasterRole>();

            queryFil = queryFil.And(m => m.IsActive == true);

            var dbRes = _masterRoleRepo.Get(queryFil).ToList();

            return Mapper.Map<List<MasterRoleDTO>>(dbRes);
        }

    }
}
