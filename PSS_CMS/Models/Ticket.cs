using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{

    public class TicketModel
    {
        public string Status { get; set; }
        public List<Ticket> Data { get; set; }
        public int Count { get; set; }
        public string LastStatus { get; set; }
    }
    public class Ticket
    {

        public int TC_RECID { get; set; }
        public string TC_URECID { get; set; }
        public string TC_CRECID { get; set; }
        public string TC_PRECID { get; set; }
        public string TC_TICKETDATE { get; set; }
        public string TC_SUBJECT { get; set; }
        public string TC_OTP { get; set; }
        public string TC_COMMENTS { get; set; }
        public string TC_REQUEST_ATTPREFIX { get; set; }
        public string TC_REQUEST_DATETIME { get; set; }
        public string TC_RESPONSE_ATTPREFIX { get; set; }
        public string TC_RESPONSE_URECID { get; set; }
        public string TC_RESPONSE_DATETIME { get; set; }
        public string TC_RESPONSECOMMENTS { get; set; }
        public string TC_STATUS { get; set; }
        public string TC_PRIORITYTYPE { get; set; }
        public string TC_TICKETTYPE { get; set; }
        public int TC_REFERENCETRECID { get; set; }
        public string TC_CLOSEDDATE { get; set; }
        public string TC_USERNAME { get; set; }
        public string TC_ASSIGNTOURECID { get; set; }
        public string TC_ASSIGNETODATETIME { get; set; }
        public string TC_EXPECTEDDATETIME { get; set; }
        public string TC_ASSIGNBY { get; set; }
        public string TC_ADMINNAME { get; set; }
        public string Combo { get; set; }
        public string LastStatus { get; set; }
    }
}