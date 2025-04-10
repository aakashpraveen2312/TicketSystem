using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class InActivityController : Controller
    {
        // GET: InActivity
        [HttpPost]
        public ActionResult KeepSessionAlive()
        {
            Session["LastActivity"] = DateTime.Now;
            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult ClearSession()
        {
            Session.Clear();
            Session.Abandon();
            return new HttpStatusCodeResult(200);
        }
    }
}