using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ContractInvoicePayment
    {
        public int CIP_RECID { get; set; }
        public int CIP_CIHRECID { get; set; }
        public int CIP_COMPANYRECID { get; set; }

        [DisplayName("Payment Date")]
        public DateTime CIP_PAYMENTDATE { get; set; }

        [DisplayName("Payment Amount")]
        public decimal CIP_PAYMENTAMOUNT { get; set; }

        [DisplayName("Invoice Amount")]
        public decimal CIP_INVOICEAMOUNT { get; set; }
        
     
        public decimal CIP_PREVIOUSLYPAIDAMOUNT { get; set; }  
        
        
        [DisplayName("Invoice Amount")]
        public decimal CIP_REMAININGAMOUNT { get; set; }




        [DisplayName("Payment Mode")]
        public string CIP_PAYMENTMODE { get; set; }

        [DisplayName("Reference No.")]
        public string CIP_REFERENCENO { get; set; }

        [DisplayName("Remarks")]
        public string CIP_REMARKS { get; set; }

        [DisplayName("Status")]
        public string CIP_STATUS { get; set; }

    }
    public class ContractInvoicePaymentObject
    {
     
        public string Message { get; set; }
        public string Status { get; set; }
        public int CIP_CIHRECID { get; set; }
        public List<ContractInvoicePayment> Data { get; set; }
    }

    public class ContractInvoicePaymentObjectbyID
    {
        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("Data")]
        public List<ContractInvoicePayment> Data { get; set; }
    }


}