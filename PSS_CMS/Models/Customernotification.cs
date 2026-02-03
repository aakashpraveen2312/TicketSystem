using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Customernotification
{

    public int CN_RECID { get; set; }
        [DisplayName("Invoice Number")]

        public string CN_INVOICENO { get; set; }
        [DisplayName("Follow Up Date")]

        public string CN_FOLLOWUPDATE { get; set; }
        [DisplayName("Status")]

        public string CN_STATUS { get; set; }

        [DisplayName("Comments")]

        public string CN_COMMENTS { get; set; }
    public int CN_CTRECID { get; set; }
    public int CN_CURECID { get; set; }
    public int CN_CRECID { get; set; }
}
    public class CustomernotificationRootObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<Customernotification> Data { get; set; }
    }  
    public class CustomernotificationRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public Customernotification Data { get; set; }
    }
}