using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ContractInvoice
    {
        public int CI_RECID { get; set; }

        [DisplayName("Invoice Date")]
        public string CI_INVOICEDATE { get; set; }

        public string Invoicedate
        {
            get
            {
                if (DateTime.TryParse(CI_INVOICEDATE, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }

        [DisplayName("Invoice Number")]
        public string CI_INVOICENUMBER { get; set; }
        [DisplayName("Invoice Amount")]
        public decimal CI_INVOICEAMOUNT { get; set; }
        [DisplayName("Payment Received Date")]
        public string CI_PAYMENTRECEIVEDDATE { get; set; }

        public string Paymentreceiveddate
        {
            get
            {
                if (DateTime.TryParse(CI_PAYMENTRECEIVEDDATE, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }

        [DisplayName("Payment Received Amount")]
        public decimal CI_PAYMENTRECEIVEDAMOUNT { get; set; }
        public int CI_CRECID { get; set; }
        public int CI_CTRECID { get; set; }
        [DisplayName("Sort")]
        public int CI_SORTORDER { get; set; }
        [DisplayName("Payment Due Date")]
        public string CI_PAYMENTDUEDATE { get; set; }
        public string WARRANTY_STATUS { get; set; }
        public string WARRANTY_TYPE { get; set; }

        public string Paymentduedate
        {
            get
            {
                if (DateTime.TryParse(CI_PAYMENTDUEDATE, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }

    }
    public class ContractInvoiceRootObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<ContractInvoice> Data { get; set; }
    }
    public class ContractInvoiceObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public ContractInvoice Data { get; set; }
    }
}