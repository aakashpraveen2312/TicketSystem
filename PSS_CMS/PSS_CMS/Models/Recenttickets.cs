using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Recenttickets
    {
        public int TC_RECID { get; set; }
        public string TC_USERID { get; set; }
        public string TC_COMPANYID { get; set; }
        public string TC_PROJECTID { get; set; }
        public string TC_TICKETDATE { get; set; }
        public string TC_TICKETDATES
        {
            get
            {
                if (DateTime.TryParse(TC_TICKETDATE, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd MM yyyy");
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
        public string TC_PRIORITYTYPE { get; set; }
   



        public int Serialnumber { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
        public string Date { get; set; }
        public string Query { get; set; }


        public string L_RECID { get; set; }

       
        public string L_USERNAME { get; set; }

      
        public string L_PASSWORD { get; set; }
        public string L_ROLE { get; set; }
        public string L_SORTORDER { get; set; }
        public string L_DISABLE { get; set; }

      
        public string L_EMAILID { get; set; }

        [Key]
        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_ID { get; set; }

        public string PS_NAME { get; set; }


        // Change PS_VALUES from string to List<string>
        public List<string> PS_VALUES { get; set; }  // This will now store the list of values
        public string PS_VALUE { get; set; }  // This will now store the list of values
    }

    public class APIResponseRecenttickets
    {
        public List<Recenttickets> Data { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }   



    }


    public class RecentticketsGetREC
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public Recenttickets Data { get; set; }

    }




}