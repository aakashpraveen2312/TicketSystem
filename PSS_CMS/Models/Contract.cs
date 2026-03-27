using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Contract
    {
        public int SerialNumber { get; set; }
        public string SC_CODE { get; set; }
        public string CT_CODE { get; set; }
        public int CT_PRECID { get; set; }
        public int CT_PARECID { get; set; }
        public int CT_URECID { get; set; }
        public int SelectedProduct { get; set; }
        public int SelectedParty { get; set; }
        public int CT_RECID { get; set; }
        [DisplayName("Contract Reference Number")]
        public string CT_CONTRACTREFERENCENUMBER { get; set; }
        [DisplayName("Party")]
        public string CT_PARTY { get; set; }
        [DisplayName("From Date")]
        public string CT_FROMDATE { get; set; }
        public string FromDate
        {
            get
            {
                if (DateTime.TryParse(CT_FROMDATE, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }

        [DisplayName("To Date")]
        public string CT_TODATE { get; set; }

        public string ToDate
        {
            get
            {
                if (DateTime.TryParse(CT_TODATE, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }

        [DisplayName("Contract Amount")]

        public decimal CT_CONTRACTAMOUNT { get; set; }
        [DisplayName("Product")]
        public int CU_PRECID { get; set; } 
        [DisplayName("Free Calls")]
        public int CT_FREECALLS { get; set; }
        [DisplayName("Service Engineer")]
        public int CU_ADMINRECID { get; set; }  
        [DisplayName("Service Engineer")]
        public int CT_ADMINRECID { get; set; }
        [DisplayName("Total Paid Amount")]

        public decimal CT_TOTALPAIDAMOUNT { get; set; }
        [DisplayName("Available Balance")]

        public decimal CT_AVAILABLEBALANACE { get; set; }
        [DisplayName("Contract Created By")]

        public string CT_CONTRACTCREATEDBY { get; set; }
        [DisplayName("Contract Approved By")]


        public string CT_CONTRACTAPPROVEDBY { get; set; }
        [DisplayName("Contract Approved Date")]
        public string CT_CONTRACTAPPROVEDDATE { get; set; }
        [DisplayName("Any Reference")]
        public string CT_ANYREFERENCE { get; set; }
        [DisplayName("Contract Policy")]
        public string CT_ATTACHMENT { get; set; }
        public int CT_CRECID { get; set; }
        [DisplayName("Sort")]
        public int CT_SORTORDER { get; set; }
        public int TotalContractAmount { get; set; }
        public int TotalPaidAmount { get; set; }
        public int PendingAmount { get; set; }
        public int ExtraPaidAmount { get; set; }
      
        public int ExpiredCustomerWarrantyCount { get; set; }
        public int ExpiredProductWarrantyCount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal UnpaidAmount { get; set; }
        public decimal TotalProductAmount { get; set; }
        public string CustomerName { get; set; }

        [DisplayName("Customer Name")]
        public string CT_NAME { get; set; }
        public string CT_CUSTOMERNAME { get; set; }
        public string PartyName { get; set; }
        public string ProductName { get; set; }
        [DisplayName("Email")]
        public string CT_EMAIL{ get; set; } 
        [DisplayName("Mobile")]
        public string CT_MOBILE{ get; set; } 
        [DisplayName("Address")]
        public string CT_ADDRESS{ get; set; }

        [DisplayName("Existing User")]
        public bool EXISTINGUSER
        {
            get => CT_EXISTINGUSER == "Y";
            set => CT_EXISTINGUSER = value ? "Y" : "N";
        }
        [JsonProperty("CT_EXISTINGUSER")]

        public string CT_EXISTINGUSER { get; set; }

        [DisplayName("New User")]
        public bool NEWUSER
        {
            get => CT_NEWUSER == "Y";
            set => CT_NEWUSER = value ? "Y" : "N";
        }
        [JsonProperty("CT_NEWUSER")]

        public string CT_NEWUSER { get; set; }


        [DisplayName("Disable")]
        public bool CPDISABLE
        {
            get => CP_DISABLE == "Y";
            set => CP_DISABLE = value ? "Y" : "N";
        }
        [JsonProperty("CP_DISABLE")]

        public string CP_DISABLE { get; set; }


        public int CP_RECID { get; set; }
        public int CP_FREECALLS { get; set; }
        public int CP_SORTORDER { get; set; }
        public int CP_PRECID { get; set; }
        public string CP_CODE { get; set; }
        public string CP_CONTRACTREF { get; set; }
        public string CP_CONTRACTCREATEDBY { get; set; }
        public string CP_CONTRACTAPPROVEDBY { get; set; }

        public string CP_CONTRACTAPPROVEDDATE { get; set; }
        public string FORMATTEDCONTRACTAPPROVEDDATE
        {
            get
            {
                if (DateTime.TryParse(CP_CONTRACTAPPROVEDDATE, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }

        public string CP_FROMDATE { get; set; }
        public string FORMATTEDCPFROMDATE
        {
            get
            {
                if (DateTime.TryParse(CP_FROMDATE, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }
        public string CP_TODATE { get; set; }
        public string FORMATTEDCPTODATE
        {
            get
            {
                if (DateTime.TryParse(CP_TODATE, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }
        public string CP_PRODUCTNAME { get; set; }
        public decimal CP_CONTRACTAMOUNT { get; set; }
        public decimal CP_PAIDAMOUNT { get; set; }
        public decimal CP_BALANACEAMOUNT { get; set; }
        public decimal CP_TOTALAMOUNT { get; set; }
        public decimal CP_BALANCEAMOUNT { get; set; }
        public decimal CP_EXTRAPAIDAMOUNT { get; set; }
        public int CP_CRECID { get; set; }
        public int CP_ADMINRECID { get; set; }
        public int CP_SORT { get; set; }
        public int CP_CTRECID { get; set; }
        public int CP_CTURECID { get; set; }
        public string CP_USERTYPE { get; set; }




    }
    public class RootObjectsContract
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<Contract> Data { get; set; }


    }
    public class ContractMasterObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public int ExpiredContracts { get; set; }
        public int YetToExpireContracts { get; set; }
        public Contract Data { get; set; }
    }
}