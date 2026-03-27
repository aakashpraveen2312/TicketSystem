using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class WarrantyViewModel
    {
        public int FreeCalls { get; set; }

        public DateTime WarrantyUpto { get; set; }

        public List<Inclusion> Inclusions { get; set; }

        public List<Exclusion> Exclusions { get; set; }
    }
}