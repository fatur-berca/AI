using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Contracts
{
    public class RowValidationByFieldException : Exception
    {
        public RowValidationByFieldException(string propName, string value = null, string reason = null) : base(
            value == null ?
            "[" + propName + "] is required" :
            (reason == null ?
            "'" + value + "' is not a valid value for [" + propName + "]" :
            "'" + value + "'" + (string.IsNullOrWhiteSpace(propName) ? " " : " is ") + reason + ( string.IsNullOrWhiteSpace(propName) ? "" : " [" + propName + "]" )
            )
        )
        {

        }
    }

    public class EffectiveDateConflictException: Exception
    {
        public EffectiveDateConflictException() : base("Save failed, Effective Start Date / Effective End Date intersect with existing data.") { }
    }
}
