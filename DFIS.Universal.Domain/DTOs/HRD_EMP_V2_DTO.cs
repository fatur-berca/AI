using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Universal.Domain.DTOs
{
    public class HRD_EMP_V2_DTO
    {
        public HRD_EMP_V2_DTO()
        {
            // TODO: Complete member initialization
        }

        public HRD_EMP_V2_DTO (string fn, string id, string name, string title)
        {
            FULL_NAME = fn;
            ID = id;
            NAME = name;
            TITLE_NAME = title;
        }

        public int Row_ID { get; set; }
        public string FULL_NAME { get; set; }
        public string ID { get; set; }
        public string NAME { get; set; }
        public string TITLE_NAME { get; set; }
        public string EMAIL { get; set; }
        public string DIVISION_Q { get; set; }
        public string LOCATION { get; set; }

        public string DIVISION_P { get; set; }
        public string IDUser { get; set; }
    }
}
