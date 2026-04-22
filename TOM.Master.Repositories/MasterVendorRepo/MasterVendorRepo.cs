using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using DFIS.Contracts;
using System;
using System.Linq.Expressions;

namespace TOM.Master.Repositories
{
    public class MasterVendorRepo : TOMGenericRepository<MasterVendor>, IMasterVendorRepo
    {
        public MasterVendorRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }
        
        public List<MasterVendor> GetAllMasterVendorActive()
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = queryFilter.And(p => p.IsActive);
            return Get(queryFilter).ToList();
        }

        public MasterVendor GetMasterVendorByIDVendor(string iDVendor)
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = queryFilter.And(p => p.IDVendor.Equals(iDVendor));
            return Get(queryFilter).SingleOrDefault();
        }

        public MasterVendor GetMasterVendorByVendorName(string namaVendor)
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = queryFilter.And(p => p.VendorName.Equals(namaVendor));
            return Get(queryFilter).SingleOrDefault();
        }
        

        void IGenericRepository<MasterVendor>.Delete(object id)
        {
            throw new NotImplementedException();
        }

        void IGenericRepository<MasterVendor>.Delete(MasterVendor entityToDelete)
        {
            throw new NotImplementedException();
        }

        void IGenericRepository<MasterVendor>.DeleteAll()
        {
            throw new NotImplementedException();
        }

        void IGenericRepository<MasterVendor>.DetachAll()
        {
            throw new NotImplementedException();
        }

        void IGenericRepository<MasterVendor>.EndTransaction(bool commit)
        {
            throw new NotImplementedException();
        }

        bool IGenericRepository<MasterVendor>.Exists(MasterVendor entity)
        {
            throw new NotImplementedException();
        }

        IEnumerable<MasterVendor> IGenericRepository<MasterVendor>.Get(Expression<Func<MasterVendor, bool>> filter, Func<IQueryable<MasterVendor>, IOrderedQueryable<MasterVendor>> orderBy, string includeProperties)
        {
            throw new NotImplementedException();
        }

        IEnumerable<MasterVendor> IGenericRepository<MasterVendor>.Get(int pageIndex, int pageSize, Expression<Func<MasterVendor, bool>> filter, Func<IQueryable<MasterVendor>, IOrderedQueryable<MasterVendor>> orderBy, string includeProperties)
        {
            throw new NotImplementedException();
        }

        List<MasterVendor> IMasterVendorRepo.GetAllMasterVendorActive()
        {
            throw new NotImplementedException();
        }

        MasterVendor IGenericRepository<MasterVendor>.GetByID(object id)
        {
            throw new NotImplementedException();
        }

        MasterVendor IGenericRepository<MasterVendor>.GetByID(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        MasterVendor IGenericRepository<MasterVendor>.GetByName(object name)
        {
            throw new NotImplementedException();
        }

        MasterVendor IMasterVendorRepo.GetMasterVendorByIDVendor(string iDVendor)
        {
            throw new NotImplementedException();
        }

        MasterVendor IMasterVendorRepo.GetMasterVendorByVendorName(string namaVendor)
        {
            throw new NotImplementedException();
        }

        void IGenericRepository<MasterVendor>.Insert(MasterVendor entity)
        {
            throw new NotImplementedException();
        }

        void IGenericRepository<MasterVendor>.InsertOrUpdate(MasterVendor entity)
        {
            throw new NotImplementedException();
        }

        int IGenericRepository<MasterVendor>.Save()
        {
            throw new NotImplementedException();
        }

        void IGenericRepository<MasterVendor>.Save(string controller, string userid)
        {
            throw new NotImplementedException();
        }

        void IGenericRepository<MasterVendor>.Update(MasterVendor entityToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
