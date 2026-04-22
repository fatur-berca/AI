using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Transport.Domain.DTOs
{
    public class FnTransportationSummaryDTO
    {
        public string p1 { get; set; }
        public string p2 { get; set; }
        public string p3 { get; set; }
        public string p4 { get; set; }
        public int p3Int { get; set; }
        public string val { get; set; }
        public string res { get; set; }
        public List<string> CategoryList { get; set; }
        public List<Series> SeriesList { get; set; }
        public List<SeriesDecimalsDTO> SeriesListDecimal { get; set; }
    }

    public class Series
    {
        public string name { get; set; }
        public List<int> data { get; set; }
    }
    public class SeriesDecimalsDTO
    {
        public string name { get; set; }
        public List<long> data { get; set; }
    }
}
