using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ContractInvoiceHeader
    {
        public int CIH_RECID { get; set; }
        public int CIH_CONTRACTUSERRECID { get; set; }
        public int CIH_CONTRACTRECID { get; set; }
        public int CIH_COMPANYRECID { get; set; }
        [DisplayName("Invoice No")]
        public string CIH_INVOICENO { get; set; }
        [DisplayName("Contract Reference Number")]
        public decimal CIH_TOTALINVOICEAMOUNT { get; set; }
        [DisplayName("Invoice Date")]
        public DateTime? CIH_INVOICEDATE { get; set; }
        [DisplayName("Status")]

        public string CIH_STATUS { get; set; }
    }
    public class ContractInvoiceHeaderObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<ContractInvoiceHeader> Data { get; set; }
    }

    public class ContractInvoicePdfResponse
    {
        public string Status { get; set; }

        public string Message { get; set; }

        public string FileUrl { get; set; }
    }
}