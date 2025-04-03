using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Reviewtickets
    {
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
        public string TC_RESPONSE_DATETIMES
        {
            get
            {
                if (DateTime.TryParse(TC_RESPONSE_DATETIME, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd MM yyyy");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }


        public int TC_RECID { get; set; }
        public string TC_USERID { get; set; }
        public string TC_COMPANYID { get; set; }
        public string TC_PROJECTID { get; set; }
        public string TC_Date { get; set; }
        public string TC_TICKETDATE { get; set; }



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
        public string Subject { get; set; }
        public string Combo { get; set; }
    }

    public class APIIReviewticketsGetAllREC
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public Reviewtickets Data { get; set; }
        public int TC_RECID { get; set; }
        public string TC_USERID { get; set; }
        public string TC_COMPANYID { get; set; }
        public string TC_PROJECTID { get; set; }
        public string TC_Date { get; set; }
        public string TC_TICKETDATE { get; set; }



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
    }

}