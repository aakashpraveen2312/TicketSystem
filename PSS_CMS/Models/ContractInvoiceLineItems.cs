using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ContractInvoiceLineItems
    {
        public int CIL_RECID { get; set; }

        public int CIL_CIHRECID { get; set; }
        [DisplayName("Description")]
        public string CIL_DESCRIPTION { get; set; }
        [DisplayName("Price")]

        public decimal CIL_PRICE { get; set; }

        public int CIL_COMPANYRECID { get; set; }
    }
    public class ContractInvoiceLineObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public int CIL_COMPANYRECID { get; set; }
        public int CIL_CIHRECID { get; set; }
        public List<ContractInvoiceLineItems> Data { get; set; }
    } 
    public class ContractInvoiceLineObjectbyID
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public int CIL_COMPANYRECID { get; set; }
        public int CIL_CIHRECID { get; set; }
        public ContractInvoiceLineItems Data { get; set; }
    }
}