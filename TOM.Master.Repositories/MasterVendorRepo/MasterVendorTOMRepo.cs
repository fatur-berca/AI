using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;

namespace TOM.Master.Repositories
{
    public class MasterVendorTOMRepo : TOMGenericRepository<MasterVendor>, IMasterVendorTOMRepo
    {
        public MasterVendorTOMRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }
        
        public List<MasterVendor> GetAllMasterVendor()
        {
            return Get().ToList();
        }

        public List<MasterVendor> GetAllMasterVendorActive()
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = queryFilter.And(p => p.IsActive);
            return Get(queryFilter).ToList();
        }

        public List<MasterVendor> GetAllMasterVendorByListVendorName(List<string> vendorName)
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => vendorName.Contains(p.VendorName));
            return Get(queryFilter).ToList();
        }

        public List<MasterVendor> GetAllMasterVendorActiveNoChild()
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.ParentVendor == null);
            return Get(queryFilter).ToList();
        }

        public MasterVendor GetMasterVendorByIDVendor(string IDVendor)
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = queryFilter.And(p => p.IDVendor.Equals(IDVendor));
            return Get(queryFilter).SingleOrDefault();
        }
        public MasterVendor GetMasterVendorByID(int IDVendor)
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = queryFilter.And(p => p.IDVendor == IDVendor);
            return Get(queryFilter).SingleOrDefault();
        }
        public MasterVendor GetMasterVendorByZone(int IDVendor, string zoneBased = "")
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = queryFilter.And(p => p.ParentVendor == IDVendor);
            //queryFilter = queryFilter.And(p => p.VendorRegion.ToLower() == zoneBased.ToLower());
            return Get(queryFilter).SingleOrDefault();
        }
        public MasterVendor GetMasterVendorByVendorName(string namaVendor)
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = queryFilter.And(p => p.VendorName.Equals(namaVendor));
            return Get(queryFilter).SingleOrDefault();
        }

        public void SaveData(MasterVendor input, bool status)
        {
            if (status)
            {
                input.CreatedDate = DateTime.Now;
                input.UpdatedDate = DateTime.Now;
                Insert(input);
            }
            else
            {
                input.UpdatedDate = DateTime.Now;
                Update(input);
            }
            Save();
        }
    }
}
