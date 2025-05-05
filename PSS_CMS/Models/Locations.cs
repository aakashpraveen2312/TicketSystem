using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Locations
    {
        public int L_RECID { get; set; }
        public string L_CODE { get; set; }
        public int L_CRECID { get; set; }
        public int L_SORTORDER { get; set; }
        public string  L_NAME { get; set; }
        public string  L_TYPE { get; set; }
        public string  L_DISABLE { get; set; }
        public string  L_DATETIME { get; set; }
    }
    public class LocationsObjects
    {
        public List <Locations>Data{get;set;}
    }
}