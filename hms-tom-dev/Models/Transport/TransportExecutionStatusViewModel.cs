namespace hms_tom_dev.Models.Transport
{
    public class TransportExecutionStatusViewModel
    {
        public int IDTransportExecutionStatusLog { get; set; }
        public int IDTransportExcecution { get; set; }
        public string Status { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual TransportExecutionViewModel TransportExecution { get; set; }
    }
}