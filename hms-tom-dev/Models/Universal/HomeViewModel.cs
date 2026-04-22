using System.Web;

namespace hms_tom_dev.Models.Universal
{
    public class HomeViewModel
    {
        public string Title { get; set; }
        public HttpPostedFileBase Picture { get; set; }
        public HttpPostedFileBase File { get; set; } 
    }
}