using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ContractInvoiceList
    {
        public int CP_RECID { get; set; }

        public string CP_CODE { get; set; }

        public string CP_CONTRACTREF { get; set; }

        public string CP_CONTRACTCREATEDBY { get; set; }

        public string CP_CONTRACTAPPROVEDBY { get; set; }

        public DateTime? CP_CONTRACTAPPROVEDDATE { get; set; }

        public int? CP_FREECALLS { get; set; }

        public int? CP_PRECID { get; set; }

        public string CP_PRODUCTNAME { get; set; }

        public decimal CP_CONTRACTAMOUNT { get; set; }

        public int CP_CRECID { get; set; }

        public int? CP_ADMINRECID { get; set; }

        public int? CP_SORT { get; set; }

        public int CP_CTRECID { get; set; }

        public int? CP_CTURECID { get; set; }

        public string CP_USERTYPE { get; set; }

        public decimal CP_PAIDAMOUNT { get; set; }

        public DateTime? CP_FROMDATE { get; set; }

        public DateTime? CP_TODATE { get; set; }

        public string CP_STATUS { get; set; }

        // Calculated values from API
        public decimal CP_EXTRAPAIDAMOUNT { get; set; }

        public decimal CP_BALANCEAMOUNT { get; set; }
    }
    public class ContractProductResponse
    {
        public string Status { get; set; }

        public string Message { get; set; }

        public List<ContractInvoiceList> Data { get; set; }
    }
}