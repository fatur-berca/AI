using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.Repositories
{
    public class MasterMappingLocationRepo : IMasterMappingLocationRepo
    {
        TOMContextDB Context { get; set; }

        public MasterMappingLocationRepo(TOMContextDB db)
        {
            Context = db;
        }

        public bool SetLocationMappingValue(string id, bool value)
        {
            var obj = Context.MasterMappingLocations.Find(id);
            if (value && obj == null && id != null)
            {
                // insert
                var nobj = Context.MasterMappingLocations.Create();
                nobj.IDLocation = id;
                Context.MasterMappingLocations.Add(nobj);
                Context.SaveChanges();
                return true;
            }
            else if (!value && obj != null)
            {
                // delete
                Context.MasterMappingLocations.Remove(obj);
                Context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
