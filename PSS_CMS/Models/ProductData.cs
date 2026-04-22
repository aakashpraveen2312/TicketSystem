using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ProductApiResponse
    {
        public string Status { get; set; }
        public List<ProductData> Data { get; set; }
    }

    public class ProductData
    {
        public int CSP_RECID { get; set; }
        public int CSP_PRECID { get; set; }
        public string CSP_PRODUCTNAME { get; set; }
        public string CSP_PRODUCTUNIQUENUMBER { get; set; }
        public int CSP_FREESERVICE { get; set; }
    }
}