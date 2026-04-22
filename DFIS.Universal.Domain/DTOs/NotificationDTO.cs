namespace DFIS.Universal.Domain.DTOs
{
    public class NotificationDTO
    {
        public int IDNotification { get; set; }
        public string IDUser { get; set; }
        public string PageName { get; set; }
        public string Description { get; set; }
        public bool IsRead { get; set; }
        public bool IsOpen { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }

    }
}
