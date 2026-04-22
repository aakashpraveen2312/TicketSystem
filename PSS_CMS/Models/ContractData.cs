using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ContractApiResponse
    {
        public string Status { get; set; }
        public string CustomerType { get; set; }
        public List<ContractData> Data { get; set; }
    }

    public class ContractData
    {
        public int CP_RECID { get; set; }
        public string CP_CODE { get; set; }
        public decimal CP_CONTRACTAMOUNT { get; set; }
        public int CU_RECID { get; set; }
        public DateTime CU_WARRANTYUPTO { get; set; }
    }
}