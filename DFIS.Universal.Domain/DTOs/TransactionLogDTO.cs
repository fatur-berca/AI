using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Universal.Domain.DTOs
{
    public class TransactionLogDTO
    {
        public int IdLog { get; set; }
        public string ControllerName { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Remark { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
}
