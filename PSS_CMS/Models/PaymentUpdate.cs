using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class PaymentUpdate
    {

        [DisplayName("Invoice No")]

        public int InvoiceID { get; set; }
        [DisplayName("Ref No")]

        public string RefNo { get; set; }
        [DisplayName("Mode Of Payment")]

        public string Mode { get; set; }
        [DisplayName("Date")]

        public DateTime Date { get; set; }
        [DisplayName("Amount")]

        public decimal Amount { get; set; }
    }
}