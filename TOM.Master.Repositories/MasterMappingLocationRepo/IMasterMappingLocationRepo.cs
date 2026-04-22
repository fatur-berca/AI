using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.Repositories
{
    public interface IMasterMappingLocationRepo
    {
        bool SetLocationMappingValue(string id, bool value);
    }
}
