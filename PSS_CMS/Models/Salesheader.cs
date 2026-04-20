using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Salesheader
    {
        public int SerialNumber { get; set; }
        public int SIH_RECID { get; set; }
        public string SIH_STATUS { get; set; }
        public int CU_RECID { get; set; }
        public int SIH_CURECID { get; set; }
        public int SIH_CRECID { get; set; }
        [DisplayName("Invoice Code")]
        public string SIH_CODE { get; set; }
        [DisplayName("Invoice Number")]
        public string SIH_INVOICENO { get; set; }
        [DisplayName("Invoice Net Amount")]
        public decimal SIH_INVOICEAMOUNT { get; set; }
        [DisplayName("Invoice Date")]
        public string SIH_INVOICEDATE { get; set; }
        public string CU_INVOICEDATEDATE
        {
            get
            {
                if (DateTime.TryParse(SIH_INVOICEDATE, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }
        [DisplayName("Sort")]
        public int SIH_SORTORDER { get; set; }
        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => SIH_DISABLE == "Y";
            set => SIH_DISABLE = value ? "Y" : "N";
        }
        [JsonProperty("SIH_DISABLE")]

        private string SIH_DISABLE { get; set; }
        [DisplayName("Sales Source")]
        public int SIH_LTRECID { get; set; }
        [DisplayName("Sales Location")]
        public int SIH_SPRECID { get; set; }
        public int SelectedLocation { get; set; }
        public int SelectedLocationRef { get; set; }

        public string ProductName { get; set; }
        public string AdminName { get; set; }
        [DisplayName("Assigned Admin")]
        public int CU_ADMINRECID { get; set; }
        public string SelectedProduct { get; set; }

        // ✅ CUSTOMER
        public int HiddenCustomerRecid { get; set; }
        [DisplayName("Customer Name")]
        public string CU_NAME { get; set; }
        [DisplayName("Email ID")]
        public string CU_EMAIL { get; set; }
        [DisplayName("Mobile Number")]
        public string CU_MOBILENO { get; set; }
        [DisplayName("Address")]
        public string CU_ADDRESS { get; set; }
        [DisplayName("GST Number")]
        public string CU_GST { get; set; }
        [DisplayName("PAN Number")]
        public string CU_PANNUMBER { get; set; }
        [DisplayName("TAN Number")]
        public string CU_TANNUMBER { get; set; }

        // CONTACT
        // Contact Person 1
        [DisplayName("Name")]
        public string CU_CONTACTPERSONNAME1 { get; set; }
        [DisplayName("Email")]
        public string CU_CONTACTPERSONEMAILID1 { get; set; }
        [DisplayName("Mobile")]
        public string CU_CONTACTPERSONMOBILE1 { get; set; }
        [DisplayName("Designation")]
        public string CU_CONTACTPERSONDESIGINATION1 { get; set; }

        // Contact Person 2
        [DisplayName("Name")]
        public string CU_CONTACTPERSONNAME2 { get; set; }
        [DisplayName("Email")]
        public string CU_CONTACTPERSONEMAILID2 { get; set; }
        [DisplayName("Mobile")]
        public string CU_CONTACTPERSONMOBILE2 { get; set; }
        [DisplayName("Designation")]
        public string CU_CONTACTPERSONDESIGINATION2 { get; set; }

        // Contact Person 3
        [DisplayName("Name")]
        public string CU_CONTACTPERSONNAME3 { get; set; }
        [DisplayName("Email")]
        public string CU_CONTACTPERSONEMAILID3 { get; set; }
        [DisplayName("Mobile")]
        public string CU_CONTACTPERSONMOBILE3 { get; set; }
        [DisplayName("Designation")]
        public string CU_CONTACTPERSONDESIGINATION3 { get; set; }
        [DisplayName("Access Credential 1")]
        public bool CU_CONTACTACCESS1_BOOL
        {
            get => (CU_CONTACTACCESS1 ?? "").Trim() == "Y";
            set => CU_CONTACTACCESS1 = value ? "Y" : "N";
        }

        [JsonProperty("CU_CONTACTACCESS1")]
        public string CU_CONTACTACCESS1 { get; set; }
        public bool CU_CONTACTACCESS2_BOOL
        {
            get => (CU_CONTACTACCESS2 ?? "").Trim() == "Y";
            set => CU_CONTACTACCESS2 = value ? "Y" : "N";
        }

        [JsonProperty("CU_CONTACTACCESS2")]
        public string CU_CONTACTACCESS2 { get; set; }
        public bool CU_CONTACTACCESS3_BOOL
        {
            get => (CU_CONTACTACCESS3 ?? "").Trim() == "Y";
            set => CU_CONTACTACCESS3 = value ? "Y" : "N";
        }

        [JsonProperty("CU_CONTACTACCESS3")]
        public string CU_CONTACTACCESS3 { get; set; }
    }
    public class SalesheaderRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<Salesheader> Data { get; set; }
    }
    public class SalesheaderObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public Salesheader Data { get; set; }
    }
}