using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ServiceInvoice
    {
        public int InvoiceID { get; set; }
        public string TC_INVOICENUMBER { get; set; }
        public string TC_SISTATUS { get; set; }
        public DateTime? InvoiceDate { get; set; }

        public string TicketRef { get; set; }
        public string CustomerID { get; set; }
        [DataType(DataType.Date)]

        public decimal Amount { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public string Status { get; set; }
        // Invoice Info
        public string InvoiceNo { get; set; }
        [DataType(DataType.Date)]
        public DateTime? TicketRaisedDate { get; set; }
        public string CustomerName { get; set; }
        public string Location { get; set; }
        public string GSTIN { get; set; }
        public string Comments { get; set; }
        public string ProductName { get; set; }
        [DataType(DataType.Date)]
        public DateTime? WarrantyUpto { get; set; }

        // Totals
        public decimal? Total { get; set; }
        public decimal? NetAmount { get; set; }
        public string AmountInWords { get; set; }

        // Bank Details
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string AccountType { get; set; }
        public string IFSC { get; set; }
        public string BranchName { get; set; }
        public string BankLocation { get; set; }
        public string BankAddress { get; set; }


        //API RETURN VALUES

        public int TC_RECID { get; set; }
        public int TC_URECID { get; set; }
        public int TC_CRECID { get; set; }
        public int TC_PRECID { get; set; }
        public DateTime? TC_TICKETDATE { get; set; }
        public string TC_SUBJECT { get; set; }
        public string TC_OTP { get; set; }
        public string TC_COMMENTS { get; set; }
        public string TC_REQUEST_ATTPREFIX { get; set; }
        [DisplayName("Ticket Rasied Date")]

        public DateTime? TC_REQUEST_DATETIME { get; set; }
        public DateTime? TC_INVOICEDATE { get; set; }
        public string TC_RESPONSE_ATTPREFIX { get; set; }
        public int TC_RESPONSE_URECID { get; set; }
        public DateTime? TC_RESPONSE_DATETIME { get; set; }
        public DateTime? TC_CLOSEDDATE { get; set; }
        public string TC_RESPONSECOMMENTS { get; set; }
        public string TC_STATUS { get; set; }
        public string TC_PRIORITYTYPE { get; set; }
        public int? TC_REFERENCETRECID { get; set; }
        public string TC_TICKETTYPE { get; set; }
        [DisplayName("Customer Name")]

        public string TC_USERNAME { get; set; } = string.Empty;
        public string TC_ADMINNAME { get; set; }
        public int? TC_ASSIGNTOURECID { get; set; }
        public DateTime? TC_ASSIGNETODATETIME { get; set; }
        public DateTime? TC_EXPECTEDDATETIME { get; set; }
        [DisplayName("Ticket Reference No")]

        public string TC_REFERENCENO { get; set; }
        public int TC_ASSIGNBY { get; set; }
        public int TC_CURECID { get; set; }
        public int TC_RATINGS { get; set; }
        public string TC_PAIDSERVICE { get; set; }
        [DisplayName("Total NetAmount")]
        public decimal TotalNetAmount { get; set; }
        [DisplayName("Total CGST")]
        public decimal TotalCGST { get; set; }
        [DisplayName("Total SGST")]
        public decimal TotalSGST { get; set; }





    }
    public class ServiceInvoiceRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<ServiceInvoice> Data { get; set; }
        public ServiceInvoice Datass { get; set; }
        public ServiceInvoiceData Datas { get; set; }

    }
    public class ServiceMaterials
    {

        public string MaterialName { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? CGST { get; set; }
        public decimal? SGST { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetAmount { get; set; }

        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }
        [DisplayName("Invoice No")]
        public string InvoiceNumber { get; set; }
        [DisplayName("Invoice Date")]
        public string InvoiceDate { get; set; }
        [DisplayName("Comments")]
        public string Comments { get; set; }
        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        [DisplayName("Address")]
        public string CustomerAddress { get; set; }
        [DisplayName("Ticket Rasied Date")]
        public string TicketRaisedDate { get; set; }
        [DisplayName("Warrent Upto")]
        public string WarrantyUpto { get; set; }
        [DisplayName("GST")]
        public string GST { get; set; }
        [DisplayName("Bank Name")]
        public string BankName { get; set; }
        [DisplayName("Acc No")]
        public string AccountNumber { get; set; }
        [DisplayName("Acc Type")]
        public string AccountType { get; set; }
        [DisplayName("IFSC Code")]
        public string IFSCCode { get; set; }
        [DisplayName("Branch Name")]
        public string BranchName { get; set; }
        [DisplayName("Branch Location")]
        public string BankLocation { get; set; }
        [DisplayName("Branch Address")]
        public string BankAddress { get; set; }
        [DisplayName("Total Amount")]
        public decimal TotalAmount { get; set; }
        [DisplayName("Total Net Amount")]
        public decimal TotalNetAmount { get; set; }
        













    }

    public class ServiceInvoiceData
    {
        public List<ServiceMaterials> BillableMaterials { get; set; }
        public List<ServiceMaterials> NonBillableMaterials { get; set; }
    }
    public class ApiPdfResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string FileUrl { get; set; }
    }
    public class ServiceInvoiceRootObjects
    {
        public string Status { get; set; }
        public ServiceInvoiceData Data { get; set; }   // ✅ NOT a List, it’s a single object
    }

}

