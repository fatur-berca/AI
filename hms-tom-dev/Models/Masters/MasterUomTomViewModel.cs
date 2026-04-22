using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hms_tom_dev.Models.Masters
{
    public class MasterUomTomViewModel
    {
        public int Id { get; set; }
        public string UOM { get; set; }
        public string MaterialType { get; set; }
        public bool IsActive { get; set; }

        public SelectList MaterialTypeList { get; set; }
        public SelectList UOMList { get; set; }
        public SelectList StatusList
        {
            get
            {
                return this.StatusList;
            }
            set
            {
                var Status = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Active", Value = "true" },
                    new SelectListItem() {Text = "Inactive", Value = "false" }
                };
                this.StatusList = new SelectList(Status, "Text", "Value");
            }
        }
    }
}