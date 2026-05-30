using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ServiceInvoicePayment
    {
        public int SerialNumber { get; set; }
        public int SINP_RECID { get; set; }
        public int SINP_TCRECID { get; set; }
        public int SINP_CRECID { get; set; }
        public string SINP_INVOICENUMBER { get; set; }
        public string SINP_MODEOFPAYMENT { get; set; }
        public string SINP_PAYMENTSTATUS { get; set; }
        public string SINP_DATEOFPAYMENT { get; set; }
        public decimal SINP_TOTALAMOUNT { get; set; }
        public decimal SINP_PAIDAMOUNT { get; set; }
        public decimal SINP_SOFARPAID { get; set; }
        public decimal SINP_BALANCEAMOUNT { get; set; }
    }



    public class ServiceInvoicePaymentRootObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<ServiceInvoicePayment> Data { get; set; }
    }
    public class ServiceInvoicePaymentRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public ServiceInvoicePayment Data { get; set; }
    }

}