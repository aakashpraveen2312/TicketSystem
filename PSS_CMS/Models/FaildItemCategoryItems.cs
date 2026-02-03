using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class FaildItemCategoryItems
    {
        public int IC_RECID { get; set; }
        public string IC_CODE { get; set; }
        public string IC_DESCRIPTION { get; set; }
        public string IC_IMAGE { get; set; }
        public string IC_DATETIME { get; set; }
        public int IC_SORTORDER { get; set; }
        public string IC_DISABLE { get; set; }
        public int IC_CRECID { get; set; }
        public int IC_IGRECID { get; set; }
        public string IC_HSNCODE { get; set; }
    }
    public class IRootObjectsItemcategory
    {
        public string Status { get; set; }
        public List<FaildItemCategoryItems> Data { get; set; }
        public string Message { get; set; }
    }


}