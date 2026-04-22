using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.Enums
{
    public class TransportEnums
    {
        public enum LostClaimProgress
        {
            INITIATION=1,
            VERIFICATION=2,
            PROCESS=3,
            INVOICING=4,
            DISPOSE=5,
            COMPLETED=6
        }
    }
}
