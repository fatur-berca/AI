using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.Repositories
{
    public class MasterKilometerRepo : TOMGenericRepository<MasterKilometer>, IMasterKilometerRepo
    {
        private TOMContextDB _context;
        
        public MasterKilometerRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
            _context = contextEntities; _context = contextEntities;
        }

        public List<MasterKilometer> GetAllMasterKilometer()
        {
            return Get().ToList();
        }

        public void DeleteData()
        {
            _context.MasterKilometers.RemoveRange(_context.MasterKilometers);
            _context.SaveChanges();
        }

        public MasterKilometer SaveData(MasterKilometer input, bool status)
        {
            input.CreatedDate = DateTime.Now;
            input.UpdatedDate = DateTime.Now;
            input.Remarks = "-";
            Insert(input);
            Save();
            return input;
        }
    }
}
