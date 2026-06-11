using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Models
{
    public class Filtertickets
    {
        public string L_AdminDeligate { get; set; }
        public string U_ASSIGNEDUSERNAME { get; set; }
        public string TC_PICKFLAG { get; set; }
        public string TC_ASSIGNFLAG { get; set; }
        public int TC_RECID { get; set; }
        public string TC_USERID { get; set; }
        public string TC_COMPANYID { get; set; }
        public string TC_PROJECTID { get; set; }
        public string CU_NAME { get; set; }
        public string TC_USERNAME { get; set; }
        public string TC_ADMINNAME { get; set; }
        public string TC_TICKETDATE { get; set; }
        public string TC_TICKETDATES
        {
            get
            {
                if (DateTime.TryParse(TC_TICKETDATE, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy HH:mm");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
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
        public string TC_TICKETTYPE { get; set; }
        public string TC_PRIORITYTYPE { get; set; }

        private string _P_COMPREHENSIVEWARRANTY;

        [JsonProperty("p_COMPREHENSIVEWARRANTY")]
        public string P_COMPREHENSIVEWARRANTY
        {
            get { return _P_COMPREHENSIVEWARRANTY; }
            set { _P_COMPREHENSIVEWARRANTY = value; }
        }

        public bool P_Comprehensivewarranty
        {
            get
            {
                return P_COMPREHENSIVEWARRANTY == "Y";
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
                    case "Q":
                        return "Query";

                    default:
                        return "Re-Opened";
                }
            }
        }
    }

    public class APIResponseRecentticket
    {
        public List<Recenttickets> Data { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }

        public List<Recenttickets> adminTickets { get; set; }

        public List<Recenttickets> managerTickets { get; set; }

    }

}