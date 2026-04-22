using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Universal.Domain.DTOs
{
    public class MasterListDTO
    {
        public int IDList { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
    }
}
