using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Prioritywise
    {
    }
    public class Prioritywisepdfobjects
    {
        public List<Prioritywise> Data { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string fileUrl { get; set; }
    }
}