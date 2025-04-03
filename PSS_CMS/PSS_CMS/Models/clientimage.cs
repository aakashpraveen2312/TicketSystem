using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class clientimage
    {
        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_NAME { get; set; }
        public string PS_ID { get; set; }
        public string PS_VALUES { get; set; }  // This will now store the list of values
        public string PS_LASTDATETIME { get; set; }
        public string PS_TYPE { get; set; }      
        public string PS_GROUPID { get; set; }
    }

    public class ApiClientimage
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<clientimage> Data { get; set; }
    }
}