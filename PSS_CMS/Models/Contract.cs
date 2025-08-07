using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Contract
    {
        public int CT_PRECID { get; set; }
        public int CT_RECID { get; set; }
        [DisplayName("Contract Reference Number")]
        public string CT_CONTRACTREFERENCENUMBER { get; set; }
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
        [DisplayName("Total Paid Amount")]

        public decimal CT_TOTALPAIDAMOUNT { get; set; }
        [DisplayName("Contract Created By")]

        public string CT_CONTRACTCREATEDBY { get; set; }
        [DisplayName("Contract Approved By")]


        public string CT_CONTRACTAPPROVEDBY { get; set; }
        [DisplayName("Contract Approved Date")]
        public string CT_CONTRACTAPPROVEDDATE { get; set; }
        [DisplayName("Any Reference")]
        public string CT_ANYREFERENCE { get; set; }
        public int CT_CRECID { get; set; }
        [DisplayName("Sort")]
        public int CT_SORTORDER { get; set; }
        public int TotalContractAmount { get; set; }
        public int TotalPaidAmount { get; set; }
        public int PendingAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal UnpaidAmount { get; set; }
        public string CustomerName { get; set; }
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
        public Contract Data { get; set; }
    }
}