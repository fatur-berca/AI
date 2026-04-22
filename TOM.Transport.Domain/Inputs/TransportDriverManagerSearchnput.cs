using DFIS.Universal.Domain.Inputs;

namespace TOM.Transport.Domain.Inputs
{
    public class TransportDriverManagerSearchnput : BaseInput
    {
        public string ID { get; set; }
        public Vendor IdVendor { get; set; }
        public string BaseTown { get; set; }
        public DriverName Name { get; set; }
        public string PerformanceLevel { get; set; }
        public Incomplete IncompleteRequirement { get; set; }

    }

    public class Vendor
    {
        public string IdVendor { get; set; }
    }

    public class DriverName
    {
        public string Name { get; set; }
    }

    public class Incomplete
    {
        public string IncompliteName { get; set; }
    }
}
