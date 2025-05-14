using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class MaterialProductmap
    {
        public int mP_RECID { get; set; }
        public int mP_MRECID { get; set; }
        public int mP_PRECID { get; set; }
        public int mP_CRECID { get; set; }
        public int P_RECID { get; set; }
        public int P_SORTORDER { get; set; }
        public int P_CRECID { get; set; }
        public string P_CODE { get; set; }
        public string P_NAME { get; set; }
        public string P_DISABLE { get; set; }
        public bool Selected { get; set; }
    }
    public class MaterialProductmapRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<MaterialProductmap> Data { get; set; }
    }
    public class MaterialProductmapObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public MaterialProductmap Data { get; set; }
    }
}