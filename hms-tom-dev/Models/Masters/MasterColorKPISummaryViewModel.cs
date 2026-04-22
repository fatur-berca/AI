using System;
using hms_tom_dev.Models.Common;

namespace hms_tom_dev.Models.Masters
{
    public class MasterColorKPISummaryViewModel : ViewModelBase
    {
        public int IDColorKPISummary { get; set; }
        public string KPIName { get; set; }
        public string UOM { get; set; }
        public string Type { get; set; }
        public int? ValueLowBottom { get; set; }
        public int? ValueLowUpper { get; set; }
        public int? Year { get; set; }
        public string ColorLow { get; set; }
        public decimal? January { get; set; }
        public decimal? February { get; set; }
        public decimal? March { get; set; }
        public decimal? April { get; set; }
        public decimal? May { get; set; }
        public decimal? June { get; set; }
        public decimal? July { get; set; }
        public decimal? August { get; set; }
        public decimal? September { get; set; }
        public decimal? October { get; set; }
        public decimal? November { get; set; }
        public decimal? December { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public int? ValueNormalBottom { get; set; }
        public int? ValueNormalUpper { get; set; }
        public string ColorNormal { get; set; }
        public int? ValueHighBottom { get; set; }
        public int? ValueHighUpper { get; set; }
        public string ColorHigh { get; set; }
    }
}