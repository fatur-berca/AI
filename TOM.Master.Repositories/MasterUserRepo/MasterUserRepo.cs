using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using DFIS.Universal.Domain.DTOs;
using AutoMapper;

namespace TOM.Master.Repositories
{
    public class MasterUserRepo : TOMGenericRepository<MasterUser>, IMasterUserRepo
    {
        TOMContextDB _context { get; set; }
        public MasterUserRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
            _context = contextEntities;
        }
        
        public List<MasterUser> GettAllMasterUserAcive()
        {
            var queryFilter = PredicateHelper.True<MasterUser>();
            queryFilter = queryFilter.And(p => p.IsActive);
            return Get(queryFilter).ToList();
        }

        public MasterUser GetUserNameById(string id)
        {
            var queryFilter = PredicateHelper.True<MasterUser>();
            queryFilter = queryFilter.And(p => p.IsActive);
            if(!String.IsNullOrEmpty(id))
                queryFilter = queryFilter.And(p => p.IDUser == id);
            return Get(queryFilter).FirstOrDefault();
        }

        public List<MasterUserDTO> GetByLocation(params string[] locationIDs)
        {
            var mulm = _context.MasterUserLocationMappings.Where(ur => locationIDs.Contains(ur.IDLocation)).ToList();
            List<MasterUserDTO> res = new List<MasterUserDTO>();
            foreach(var ulm in mulm)
            {
                res.Add(Mapper.Map<MasterUserDTO>(ulm.MasterUser));
            }
            return res;
        }

        public List<string> GetEmailInLocation(string[] locs)
        {
            var usrs = _context.MasterUserLocationMappings.Where(l => locs.Contains(l.IDLocation))
                    .Select(l => l.IDUser).Distinct().ToArray();
            var emails = _context.MasterUsers.Where(u => usrs.Contains(u.IDUser)).Select(e => e.Email).Distinct().ToList();
            return emails;
        }
        public List<MasterUserDTO> GetUserInLocation(string[] locs)
        {
            var usrs = _context.MasterUserLocationMappings.Where(l => locs.Contains(l.IDLocation))
                    .Select(l => l.IDUser).Distinct().ToArray();
            var emails = _context.MasterUsers.Where(u => usrs.Contains(u.IDUser)).Distinct();
            return Mapper.Map<List<MasterUserDTO>>(emails);
        }
        public List<string> GetIDUserInLocation(string[] locs)
        {
            var usrs = _context.MasterUserLocationMappings.Where(l => locs.Contains(l.IDLocation))
                    .Select(l => l.IDUser).Distinct().ToArray();
            var emails = _context.MasterUsers.Where(u => usrs.Contains(u.IDUser)).Select(u=>u.IDUser).Distinct();
            return emails.ToList(); ;
        }
    }
}
