using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class LocationType
    {
        public int LT_RECID { get; set; }
        public string LT_NAME { get; set; }
        public int? LT_SORTORDER { get; set; }
        public string LT_DISABLE { get; set; }
        public int LT_CRECID { get; set; }
    }
}