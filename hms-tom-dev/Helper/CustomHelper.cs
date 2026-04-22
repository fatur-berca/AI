using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hms_tom_dev.Helper
{
    public static class CustomHelper
    {
        public static IHtmlString BaseUrl(this HtmlHelper helper, string url = null)
        {
            var path = HttpContext.Current.Request.ApplicationPath ?? "";
            return new HtmlString("'" + HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                   path.TrimEnd('/') + "/" + url + "'");
        }
    }
}