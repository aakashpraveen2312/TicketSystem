using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class InvoicePayment
    {
        public int SerialNumber { get; set; }
        public int PP_RECID { get; set; }
        public int PP_SIHRECID { get; set; }
        public int PP_CRECID { get; set; }
        [DisplayName("Invoice Number")]
        public string PP_INVOICENUMBER { get; set; }
        [DisplayName("Mode of Payment")]
        public string PP_MODEOFPAYMENT { get; set; }
        [DisplayName("Date of Payment")]
        public string PP_DATEOFPAYMENT { get; set; }
        [DisplayName("Invoice Date")]
        public string PP_INVOICEDATE { get; set; }
        [DisplayName("Total Amount")]
        public decimal PP_TOTALAMOUNT { get; set; }
        [DisplayName("Amount")]
        public decimal PP_PAIDAMOUNT { get; set; }
        [DisplayName("So Far Paid")]
        public decimal PP_SOFARPAID { get; set; }
        [DisplayName("Balance Amount")]
        public decimal PP_BALANCEAMOUNT { get; set; }
        [DisplayName("Payment Status")]
        public string PP_PAYMENTSTATUS { get; set; }
    }

    public class InvoicePaymentRootObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<InvoicePayment> Data { get; set; }
    }
    public class InvoicePaymentRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public InvoicePayment Data { get; set; }
    }
}