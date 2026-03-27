using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PSS_CMS.Models
{
    public class Product
    {
        public int P_RECID { get; set; }
        public string P_CODE { get; set; }
        public string P_NAME { get; set; }
        public int P_SORTORDER { get; set; }
        public string P_DISABLE { get; set; }
        public int P_CRECID { get; set; }
        public string P_COMPREHENSIVEWARRANTY { get; set; }
        public int P_FREECALLS { get; set; }
        public int P_DURATION { get; set; }
        public string WarrantyUpto { get; set; }
    }

    public class Inclusions
    {
        public int IN_RECID { get; set; }
        public int IN_PRECID { get; set; }
        public int IN_CRECID { get; set; }
        public string IN_DESCRIPTION { get; set; }
        public int IN_SORTORDER { get; set; }
    }

    public class Exclusions
    {
        public int EX_RECID { get; set; }
        public int EX_PRECID { get; set; }
        public int EX_CRECID { get; set; }
        public string EX_DESCRIPTION { get; set; }
        public int EX_SORTORDER { get; set; }
    }

    public class ViewlistRoot
    {
        public string Status { get; set; }
        public Product Product { get; set; }
        public List<Inclusions> Inclusions { get; set; }
        public List<Exclusions> Exclusions { get; set; }
    }

    public class Viewlist
    {
        public int SerialNumber { get; set; }
        public string IN_DESCRIPTION { get; set; }
        public string EX_DESCRIPTION { get; set; }
    }
}