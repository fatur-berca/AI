using TOM.Master.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Master.Repositories
{
    public interface IMasterUserRoleDelegationRepo
    {
        int Save(MasterUserRoleDelegationDTO input);
    }
}
