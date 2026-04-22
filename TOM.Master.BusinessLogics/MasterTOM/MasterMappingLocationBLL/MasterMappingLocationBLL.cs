using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.Master.Repositories;

namespace TOM.Master.BusinessLogics
{
    public class MasterMappingLocationBLL : IMasterMappingLocationBLL
    {
        private readonly IMasterMappingLocationRepo _mstMapLocation;

        public MasterMappingLocationBLL(IMasterMappingLocationRepo mstMapLocation)
        {
            _mstMapLocation = mstMapLocation;
        }

        public bool SetLocationMappingValue(string id, bool value)
        {
            return _mstMapLocation.SetLocationMappingValue(id, value);
        }
    }
}
