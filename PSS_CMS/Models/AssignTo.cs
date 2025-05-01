using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class AssignTo
    {
        public string SelectedAdmin { get; set; }
        public string A_COMMENTS { get; set; }
        public string A_ASSIGNEDDATEANDTIME { get; set; }
        public string A_EXPECTEDDATEANDTIME { get; set; }
    }
    public class AssignToObject
    {
        public AssignTo AssignTo { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}