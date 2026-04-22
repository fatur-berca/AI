using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterMappingLocationBLL
    {
        bool SetLocationMappingValue(string id, bool value);
    }
}
