using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOM.EntitiesDAL.EDMX;

namespace hms_tom_dev.Models.Home
{
    public class InitHomeViewModel
    {
        public InitHomeViewModel()
        {
            IsExpired = false;
            IsRuleEmpty = false;
        }

        public bool IsRuleEmpty { get; set; }
        public bool IsExpired { get; set; }

        public string WebRootUrl { get; set; }

        //public List<NewsHighlight> newsHighlight { get; set; }
    }
}