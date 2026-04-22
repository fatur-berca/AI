using hms_tom_dev.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TOM.Transport.Domain.DTOs;

namespace hms_tom_dev.Models.Transport
{
    public class TransportPositionDetailViewModel : ViewModelBase
    {
        public int IDTransportPositionDetail { get; set; }
        public int IDTransportExecution { get; set; }
        public DateTime PositionDate { get; set; }
        public string PositionName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}