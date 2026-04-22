using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace hms_tom_dev.Models.Transport
{
    public class FnTransportationSummaryViewModel
    {
        [JsonProperty(PropertyName = "p1 ")]
        public string p1 { get; set; }
        [JsonProperty(PropertyName = "p2 ")]
        public string p2 { get; set; }
        [JsonProperty(PropertyName = "p3 ")]
        public string p3 { get; set; }
        [JsonProperty(PropertyName = "p4 ")]
        public string p4 { get; set; }
        [JsonProperty(PropertyName = "val ")]
        public string val { get; set; }
        [JsonProperty(PropertyName = "res ")]
        public string res { get; set; }

        public List<string> CategoryList { get; set; }
        public List<Series> SeriesList { get; set; }
        public List<SeriesDecimalsViewModel> SeriesListDecimal { get; set; }
    }

    public class Series
    {
        public string name { get; set; }
        public List<int> data { get; set; }
    }
    public class SeriesDecimalsViewModel
    {
        public string name { get; set; }
        public List<long> data { get; set; }
    }
}