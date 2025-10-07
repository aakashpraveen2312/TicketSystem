using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class PaymentUpdate
    {
        public int TC_RECID { get; set; }


        [DisplayName("Invoice No")]
        public string TC_InvoiceNumber { get; set; }
        
    
        [DisplayName("Ref No")]
        public string TC_ReferenceNo { get; set; }

        [DisplayName("Mode Of Payment")]
        public string TC_ModeOfPayment { get; set; }

        [DisplayName("Date Of Payment")]
        public DateTime TC_DateOfPayment { get; set; }

        [DisplayName("Amount")]
        public decimal TC_TotalAmount { get; set; }

    }
    public class PaymentUpdateRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public PaymentUpdate Data { get; set; }
    

    }
}