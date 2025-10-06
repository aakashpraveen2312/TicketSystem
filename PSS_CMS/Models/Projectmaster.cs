using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Models
{
    public class Projectmaster
    {
        public int CU_RECID { get; set; }
        public int CU_URECID { get; set; }
        [DisplayName("Job Card Reference")]
        public string CU_CODE { get; set; }
        [DisplayName("Address")]
        public string CU_ADDRESS { get; set; }
        [DisplayName("Customer Name")]
        public string CU_NAME { get; set; }
        [DisplayName("GST Number")]
        public string CU_GST { get; set; }
        [DisplayName("Email ID")]
        public string CU_EMAIL { get; set; }
        [DisplayName("Mobile Number")]
        public string CU_MOBILENO { get; set; }
        [DisplayName("Product")]
        public int CU_PRECID { get; set; }
        [DisplayName("Invoice Number")]
        public string CU_INVOICENO { get; set; }
        [DisplayName("Warranty Upto")]
        public string CU_WARRANTYUPTO { get; set; }
        public string CU_WARRANTYUPTODATE
        {
            get
            {
                if (DateTime.TryParse(CU_WARRANTYUPTO, out DateTime parsedDate))
                {
                    return parsedDate.ToString("dd-MM-yyyy");
                }
                return string.Empty; // Return an empty string or handle as needed if parsing fails
            }
        }
        [DisplayName("Warranty free calls")]
        public int CU_WARRANTYFREECALLS { get; set; }
        [DisplayName("Sort")]
        public int CU_SORTORDER { get; set; }
        public int CU_CRECID { get; set; }

        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => CU_DISABLED == "Y";
            set => CU_DISABLED = value ? "Y" : "N";
        }
        [JsonProperty("CU_DISABLE")]

        private string CU_DISABLED { get; set; }
        public string SelectedProduct { get; set; }
        public int SelectedLocation { get; set; }
        public int SelectedLocationRef { get; set; }
        public string P_NAME { get; set; }
        public int P_RECID { get; set; }

        // PAN & TAN Numbers
        [DisplayName("PAN Number")]
        public string CU_PANNUMBER { get; set; }
        [DisplayName("TAN Number")]
        public string CU_TANNUMBER { get; set; }

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
        [DisplayName("Location Type")]
        public int LT_RECID { get; set; }
        [DisplayName("Location Reference")]
        public int SP_RECID { get; set; }

        [DisplayName("Location Type")]
        public int CU_LTRECID { get; set; }
        [DisplayName("Location Reference")]
        public int CU_SPRECID { get; set; }
       
        public string LT_NAME { get; set; }
        public string SP_NAME { get; set; }

    }


    public class ProjectMasterRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<Projectmaster> Data { get; set; }
    }
    public class ProjectMasterObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public Projectmaster Data { get; set; }
    }


}

