using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Models
{
    public class Tickethistory
    {
        public int Serialnumber { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
        public string Date { get; set; }
        public int TC_RECID { get; set; }
        public string TC_USERID { get; set; }
        public string TC_COMPANYID { get; set; }
        public string TC_PROJECTID { get; set; }
        public string TC_TICKETDATE { get; set; }
        public string TC_TICKETTYPE { get; set; }
        public string TC_USERNAME { get; set; }
        public string TC_ADMINNAME { get; set; }
        public string AdminNameDisplay => string.IsNullOrEmpty(TC_ADMINNAME) ? "Fresh Ticket" : TC_ADMINNAME;

        public string TC_TICKETDATES
        {
            get
            {
                if (DateTime.TryParse(TC_TICKETDATE, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy HH:mm:ss");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }



        public string TC_STATUS_DISPLAY
        {
            get
            {
                switch (TC_STATUS)
                {
                    case "S":
                        return "Submitted";
                    case "R":
                        return "Resolved";
                    case "C":
                        return "Closed";

                    default:
                        return "Re-Opened";
                }
            }
        }

        public string TC_SUBJECT { get; set; }
        public string TC_OTP { get; set; }
        public string TC_COMMENTS { get; set; }
        public string TC_REQUEST_ATTACHMENT_PREFIX { get; set; }
        public string TC_REQUEST_DATETIME { get; set; }
        public string TC_RESPONSE_ATTACHMENT_PREFIX { get; set; }
        public string TC_RESPONSE_USERID { get; set; }
        public string TC_RESPONSE_DATETIME { get; set; }
        public string TC_RESPONSE_COMMENTS { get; set; }
        public string TC_STATUS { get; set; }
        public string TC_PRIORITYTYPE { get; set; }
        public string TC_PROJECTWISEFILTER { get; set; }
        public string TC_TICKETTYPEWISEFILTER { get; set; }
       


        public string TT_RECID { get; set; }
        public string TT_TICKETTYPE { get; set; }
        public string TT_CODE { get; set; }
        public string TT_SORTORDER { get; set; }
        public string TT_DISABLE { get; set; }



        public string P_PROJECTRECID { get; set; }
        public string P_CODE { get; set; }
        public string P_NAME { get; set; }
        public string P_SORTORDER { get; set; }
        public string P_DISABLE { get; set; }

        [DisplayName("Ticket Type")]
        public string SelectedTicketType { get; set; }
        [DisplayName("Project Type")]
        public string SelectedProjectType { get; set; }
        [DisplayName("Status")]
        public string Statustype { get; set; }

        public List<SelectListItem> TicType { get; set; }
        public List<SelectListItem> ProType { get; set; }

        public Tickethistory()
        {

            TicType = new List<SelectListItem>();
            ProType = new List<SelectListItem>();

        }



    }
    public class TicketTypeModel
    {
        public string Status { get; set; }
        public List<Tickethistory> Data { get; set; }
        public int Count { get; set; }
        public string LastStatus { get; set; }
    }
    public class ApiResponseTicketsHistoryResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<Tickethistory> Data { get; set; }
    }

   
}