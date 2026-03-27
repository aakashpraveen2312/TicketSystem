using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class PaymentUpdate
    {
        public int TC_RECID { get; set; }


        [DisplayName("Invoice No")]
        public string TC_InvoiceNumber { get; set; }
        
    
        [DisplayName("Reference No")]
        public string TC_ReferenceNo { get; set; }

        [DisplayName("Mode Of Payment")]
        public string TC_ModeOfPayment { get; set; }

        [DisplayName("Payment Status")]
        public string TC_PaymentStatus { get; set; }

        [DisplayName("Date Of Payment")]
        public string TC_DateOfPayment { get; set; }

        public string TC_DateOfPayments
        {
            get
            {
                if (DateTime.TryParse(TC_DateOfPayment, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }

        [DisplayName("Due Date")]
        public string TC_DueDate { get; set; }

        public string TC_DueDates
        {
            get
            {
                if (DateTime.TryParse(TC_DueDate, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }

        [DisplayName("Total Amount")]
        public decimal? TC_TotalAmount { get; set; }

        [DisplayName("Paid Amount")]
        public decimal? TC_PaidAmount { get; set; }

        [DisplayName("Balance Amount")]
        public decimal? TC_BalanceAmount { get; set; }

    }
    public class PaymentUpdateRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public PaymentUpdate Data { get; set; }
    

    }
}