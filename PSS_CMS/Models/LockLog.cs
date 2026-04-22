using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class LockLog
    {
        public int SerialNumber { get; set; }
        public int LL_RECID { get; set; }
        [DisplayName("Date of Unlock")]
        public string LL_DATETIME { get; set; }
        [DisplayName("Unlock By")]
        public string UnlockUserName { get; set; }
        public string LL_UNLOCKBY { get; set; }
        [DisplayName("Reason")]
        public string LL_REASON { get; set; }
        [DisplayName("Transaction Type")]
        public string LL_TRANSACTIONTYPE { get; set; }
    }
    public class LockLogRootObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<LockLog> Data { get; set; }
    }
    public class LockLogRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public LockLog Data { get; set; }
    }
}