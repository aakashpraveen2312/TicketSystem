using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Prioritywise
    {
       public int TC_RECID { get; set; }
        public string UserName { get; set; }
        public string Date { get; set; }
        public string DATES
        {
            get
            {
                if (DateTime.TryParse(Date, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }
        public string Product { get; set; }
        public string Subject { get; set; }
        public string Priority { get; set; }
        public string TicketType { get; set; }
        public string Status { get; set; }


        public string customerName { get; set; }
        public string customerEmail { get; set; }
        public string customerMobile { get; set; }
        public string invoiceNumber { get; set; }
        public string expiryDate { get; set; }
        public int remainingDays { get; set; }



        public int ProductRecId { get; set; }
        public int ContractRecId { get; set; }
        public string ProductName { get; set; }
        public string ContractNumber { get; set; }
        public decimal ContractAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public decimal subtotalContractAmount { get; set; }
        public decimal subtotalPaidAmount { get; set; }
        public decimal subtotalPendingAmount { get; set; }

        public string comments { get; set; }
        public string assignedTo { get; set; }

        public string TicketNo { get; set; }
        public string Type { get; set; }
        public string ServiceEngineer { get; set; }
        public string MaterialName { get; set; }
        public int QtyUsed { get; set; }
        public decimal NetAmount { get; set; }

    }
    public class Prioritywisepdfobjects
    {
        public List<Prioritywise> Data { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string fileUrl { get; set; }
    }
}