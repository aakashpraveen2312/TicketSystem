using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Models
{
    public class Faq: TicketComboTypes1
    {
        public int F_RECID { get; set; }
        public string F_QUESTION { get; set; }
        public string F_ANSWER { get; set; }
        public string F_ATTACHEMENT { get; set; }
        public string F_CREATEDDATETIME { get; set; }
        public int F_PROJECTRECID { get; set; }
        public int F_SORTORDER { get; set; }
        public string F_USERID { get; set; }
        public string F_DISABLE { get; set; }
        // Dropdown Selections
        public string SelectedProjectType1 { get; set; }
      
        // Dropdown Data
        public TicketCombo1 TicketCombo1{ get; set; } = new TicketCombo1();
       


    }
    public class TicketCombo1
    {
        public List<SelectListItem> TicketTypes1 { get; set; } = new List<SelectListItem>();
    }
    public class TicketComboTypes1
    {
        public int TT_RECID { get; set; }
        public int P_PROJECTRECID { get; set; }
        public int TT_SORTORDER { get; set; }
        public string TT_TICKETTYPE { get; set; }
        public string P_NAME { get; set; }
        public string TT_CODE { get; set; }
        public string TT_DISABLE { get; set; }
    }
    public class RootObjectFAQGET
    {
        public string Status { get; set; }
        public Faq Data { get; set; }

        public string Message { get; set; }


        //public List<Bins> Data { get; set; }
    }
    public class RootObjectFAQ
    {
        public string Status { get; set; }
        public string Message { get; set; }

        public List<Faq> Data { get; set; }

        public int LastRecid { get; set; }
        public string LastStatus { get; set; }
        public List<TicketComboTypes1> Datas { get; set; }

    }

}