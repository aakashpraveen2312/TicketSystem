using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ContractServiceProduct
    {
        public int CSP_RECID { get; set; }          // Identity (Primary Key)
        public int CSP_CTRECID { get; set; }         
          

        [DisplayName("Product Name")]
        public int? CSP_PRECID { get; set; }

        public int SelectedProduct { get; set; }

        [DisplayName("Product Name")]
        public string CSP_PRODUCTNAME { get; set; }

        [DisplayName("Admin Name")]
        public int? CSP_ARECID { get; set; }

        [DisplayName("Admin Name")]
        public string CSP_ADMINNAME { get; set; }
        public string CSP_USERTYPE { get; set; }

        [DisplayName("Free Service")]
        public int? CSP_FREESERVICE { get; set; }

        [DisplayName("Invoice Amount")]
        public decimal? CSP_INVOICEAMOUNT { get; set; }

        [DisplayName("Paid Amount")]
        public decimal? CSP_PAIDAMOUNT { get; set; }

        public int? CSP_CRECID { get; set; }
        public int? CSP_CURECID { get; set; }

        [DisplayName("Product Unique Number")]
        public string CSP_PRODUCTUNIQUENUMBER { get; set; }
    }


    public class RootObjectsContractServiceProduct
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<ContractServiceProduct> Data { get; set; }


    }
    public class RootObjectContractServiceProduct
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public ContractServiceProduct Data { get; set; }


    }







}