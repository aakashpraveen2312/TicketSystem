using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class SalesInvoiceProducts
    {
        public int SerialNumber { get; set; }
        public int SIP_RECID { get; set; }
        public int SIP_SIHRECID { get; set; }
        [DisplayName("Product")]
        public int SIP_PRECID { get; set; }
        public int SIP_CURECID { get; set; }
        [DisplayName("Warranty Upto")]
        public string SIP_WARRANTYUPTO { get; set; }
        [DisplayName("Warranty free calls")]
        public int SIP_WARRANTYFREECALLS { get; set; }
        public string WARRANTY_STATUS { get; set; }
        public string WARRANTY_TYPE { get; set; }
        [DisplayName("Assigned Admin")]
        public int SIP_ADMINRECID { get; set; }
        public string SIP_PRODUCTSERIALNUMBER { get; set; }
        [DisplayName("Sort")]
        public int SIP_SORTORDER { get; set; }
        [DisplayName("Price(Inclusion of GST)")]
        public decimal SIP_PRODUCTAMOUNT { get; set; }

        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => SIP_DISABLE == "Y";
            set => SIP_DISABLE = value ? "Y" : "N";
        }
        [JsonProperty("SIP_DISABLE")]
        private string SIP_DISABLE { get; set; }
        public string SelectedProduct { get; set; }
        public string ProductName { get; set; }
        public int HiddenCustomerRecid { get; set; }

    }

    public class SalesInvoiceRootObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<SalesInvoiceProducts> Data { get; set; }
    }
    public class SalesInvoiceRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public SalesInvoiceProducts Data { get; set; }
    }
}