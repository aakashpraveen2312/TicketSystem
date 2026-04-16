using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ProductInvoice
    {
        public int SerialNumber { get; set; }

        public int SIP_PRECID { get; set; }
        public string SIP_PRODUCTSERIALNUMBER { get; set; }

        public string SIH_INVOICENO { get; set; }
        public decimal SIH_INVOICEAMOUNT { get; set; }
        public string SIH_INVOICEDATE { get; set; }
    }
    public class ProductInvoiceRoot
    {
        public string Status { get; set; }
        public List<ProductInvoice> Data { get; set; }
    }
}