using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Models
{
    public class Tickets : TicketComboTypes
    {
        public int TC_RECID { get; set; }
        //public int P_PROJECTRECID { get; set; }
        //public string P_NAME { get; set; }
        public string TC_USERID { get; set; }
        public string TC_COMPANYID { get; set; }
        public string TC_PROJECTID { get; set; }
        public string TC_Date { get; set; }
        public string TC_TICKETDATE { get; set; }
        public int TC_REFERENCEID { get; set; }
        public string TC_SUBJECT { get; set; }
        public string TC_OTP { get; set; }
        public string TC_COMMENTS { get; set; }
        public string TC_REQUEST_ATTACHMENT_PREFIX { get; set; }
        public string TC_REQUEST_DATETIME { get; set; }
        public string TC_RESPONSE_ATTACHMENT_PREFIX { get; set; }
        public int TC_RESPONSE_USERID { get; set; }
        public string TC_RESPONSE_DATETIME { get; set; }
        public string TC_RESPONSE_COMMENTS { get; set; }
        public string TC_STATUS { get; set; }
        public string TC_PRIORITYTYPE { get; set; }
        public string TC_TICKETTYPE { get; set; }

        // Dropdown Selections
        public string SelectedTicketType { get; set; }
        public string SelectedProjectType { get; set; }

        // Dropdown Data
        public TicketCombo TicketCombo { get; set; } = new TicketCombo();
        public TicketCombo2 TicketCombo2 { get; set; } = new TicketCombo2();
    }

    public class TicketCombo
    {
        public List<SelectListItem> TicketTypes { get; set; } = new List<SelectListItem>();
    }

    public class TicketCombo2
    {
        public List<SelectListItem> TicketTypes2 { get; set; } = new List<SelectListItem>();
    }

    public class ApiResponseTicketsResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public Tickets Data { get; set; }
        public List<Tickets> Datas { get; set; }
    }


    public class TicketComboTypes
    {
        public int TT_RECID { get; set; }
        public int P_PROJECTRECID { get; set; }
        public int TT_SORTORDER { get; set; }
        public string TT_TICKETTYPE { get; set; }
        public string P_NAME { get; set; }
        public string TT_CODE { get; set; }
        public string TT_DISABLE { get; set; }
    }

    public class ApiResponseTicketsResponseTypes
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<TicketComboTypes> Data { get; set; }
    }

}