using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class DashBoardList
    {
        public string TC_USERNAME { get; set; }
        public string TC_TICKETDATE { get; set; }
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
        public string TC_PROJECTID { get; set; }
        public string TC_COMMENTS { get; set; }
        public string TC_PRIORITYTYPE { get; set; }
        public string TC_TICKETTYPE { get; set; }
        public string TC_STATUS { get; set; }
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

    }
    public class ObjectsDashBoardList
    {       
        public List<DashBoardList> TotalTickets { get; set; }
        public List<DashBoardList> OpenTickets { get; set; }
        public List<DashBoardList> CloseTickets { get; set; }
        public List<DashBoardList> ResolvedTickets { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}