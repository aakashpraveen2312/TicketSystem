using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Admintickets
    {
        public int Serialnumber { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
        public string Date { get; set; }
        public string Date1 { get; set; }
        public string Query { get; set; }
        public string Subject { get; set; }


        public int TC_RECID { get; set; }

        public string TC_TICKETDATE { get; set; }
        public string TC_SUBJECT { get; set; }
        public string TC_COMMENTS { get; set; }
        public string TC_REQUEST_ATTACHMENT_PREFIX { get; set; }
        public string TC_RESPONSE_ATTACHMENT_PREFIX { get; set; }
        public string TC_RESPONSE_COMMENTS { get; set; }




    }

    public class RootObjectsuser
    {
        public string Status { get; set; }
        public Admintickets Data { get; set; }
        public List<Admintickets> Datas { get; set; }

        public string Message { get; set; }


        //public List<Bins> Data { get; set; }
    }


}