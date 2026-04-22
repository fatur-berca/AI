using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.DTOs
{
    public class KPILoadFactorAvgDTO
    {
        public string RowNumber { get; set; }
        public string RowLabels { get; set; }
        public Nullable<int> TripJanuary { get; set; }
        public Nullable<decimal> January { get; set; }
        public Nullable<int> TripFebruary { get; set; }
        public Nullable<decimal> February { get; set; }
        public Nullable<int> TripMarch { get; set; }
        public Nullable<decimal> March { get; set; }
        public Nullable<int> TripApril { get; set; }
        public Nullable<decimal> April { get; set; }
        public Nullable<int> TripMay { get; set; }
        public Nullable<decimal> May { get; set; }
        public Nullable<int> TripJune { get; set; }
        public Nullable<decimal> June { get; set; }
        public Nullable<int> TripJuly { get; set; }
        public Nullable<decimal> July { get; set; }
        public Nullable<int> TripAugust { get; set; }
        public Nullable<decimal> August { get; set; }
        public Nullable<int> TripSeptember { get; set; }
        public Nullable<decimal> September { get; set; }
        public Nullable<int> TripOctober { get; set; }
        public Nullable<decimal> October { get; set; }
        public Nullable<int> TripNovember { get; set; }
        public Nullable<decimal> November { get; set; }
        public Nullable<int> TripDecember { get; set; }
        public Nullable<decimal> December { get; set; }
    }
}
