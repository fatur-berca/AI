using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DFIS.Utils
{
    public class UtilBaseInput : UtilSortBaseInput
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool IncludeDeleted { get; set; }
        public bool IsExportOrSearch { get; set; }
        public string IncludeInActive { get; set; }
        public List<int> ListIsActiveId { get; set; }
        public List<string> ListIsActiveIdString { get; set; }
        public List<string> ListDate { get; set; }
        public List<string> ListStatus { get; set; }
        public string UserRole { get; set; }
    }
}
