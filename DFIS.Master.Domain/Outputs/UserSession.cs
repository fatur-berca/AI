using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Master.Domain.Outputs
{
    public class UserSession
    {
        public string Name { get; set; }
        public string Username { get; set; }
        
        public List<FunctionPage> Page { get; set; }
        public List<UserLocationMap> Location { get; set; }
        public List<FunctionPage> Button { get; set; }

        public List<UserRole> Role { get; set; }

        public List<RangeConfigDate> RangeDate { get; set; }
    }

    public class FunctionPage
    {
        public int IDFunction { get; set; }
        public string FunctionName { get; set; }
        public string Type { get; set; }
        public int ParentIdFunction { get; set; }
    }

    public class UserLocationMap
    {
        public string IDLocation { get; set; }
        public string LocationName { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserRole
    {
        public int IDRole { get; set; }
        public string RoleName { get; set; }
    }

    public class RangeConfigDate
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
    
}
