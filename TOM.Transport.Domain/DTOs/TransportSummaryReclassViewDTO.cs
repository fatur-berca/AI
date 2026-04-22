using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.DTOs
{
    public class TransportSummaryReclassViewDTO
    {
        public Nullable<long> Row { get; set; }
        public string reclass { get; set; }
        public string Account { get; set; }
        public string Mapping { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<int> January { get; set; }
        public Nullable<int> February { get; set; }
        public Nullable<int> March { get; set; }
        public Nullable<int> April { get; set; }
        public Nullable<int> May { get; set; }
        public Nullable<int> June { get; set; }
        public Nullable<int> July { get; set; }
        public Nullable<int> August { get; set; }
        public Nullable<int> September { get; set; }
        public Nullable<int> October { get; set; }
        public Nullable<int> November { get; set; }
        public Nullable<int> December { get; set; }
        public Nullable<int> total { get; set; }
    }
}
