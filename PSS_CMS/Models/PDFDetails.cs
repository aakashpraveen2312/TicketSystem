using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class PDFDetails
    {
        public int GP_RECORDID { get; set; }
        public string GP_MAINTYPE { get; set; }
        public string GP_SUBTYPE { get; set; }
        public string GP_IMAGE { get; set; }
        public int GP_COMPANYID { get; set; }
        public int GP_REFERENCERECID { get; set; }
        public string GP_CREATEDDATE { get; set; }
    }
    public class pdfdetail
    {
        public string Status { get; set; }
        public PDFDetails Data { get; set; }

        public string Message { get; set; }
        public string fileUrl { get; set; }

    }
}