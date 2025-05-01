using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Models
{
    public class WhitePaper: TicketComboTypes2
    {
        public int WP_RECID { get; set; }
        public int wP_PRECID { get; set; }
        public int wP_CRECID { get; set; }
        public string WP_TITLE { get; set; }
        public string WP_Description { get; set; }
        public string WP_ATTACHEMENT { get; set; }
        public DateTime WP_CREATEDDATETIME { get; set; }
        public int WP_SORTORDER { get; set; }
        public int WP_PROJECTID { get; set; }
        public string WP_USERID { get; set; }
        public string WP_DISABLE { get; set; }
       

        public string SelectedProjectType2 { get; set; }

        // Dropdown Data
        public TicketCombo3 TicketCombo3 { get; set; } = new TicketCombo3();

    }
    public class TicketCombo3
    {
        public List<SelectListItem> TicketTypes2 { get; set; } = new List<SelectListItem>();
    }
    public class TicketComboTypes2
    {
        public int TT_RECID { get; set; }
        public int P_PROJECTRECID { get; set; }
        public int TT_SORTORDER { get; set; }
        public string TT_TICKETTYPE { get; set; }
        public string P_NAME { get; set; }
        public string TT_CODE { get; set; }
        public string TT_DISABLE { get; set; }
    }
    public class APIResponsewhitepaper
    {

        public List<WhitePaper> Data { get; set; }
        public WhitePaper Datas { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }

    }
    public class RootObjectWHITEPAPERGET
    {
        public string Status { get; set; }
        public WhitePaper Data { get; set; }

        public string Message { get; set; }


        //public List<Bins> Data { get; set; }
    }

}
