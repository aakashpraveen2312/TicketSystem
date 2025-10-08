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

        [DisplayName("Amount")]
        public decimal? TC_TotalAmount { get; set; }

    }
    public class PaymentUpdateRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public PaymentUpdate Data { get; set; }
    

    }
}