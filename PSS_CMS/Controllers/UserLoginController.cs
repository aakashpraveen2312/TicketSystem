using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class UserLoginController : Controller
    {
        // GET: UserLogin
        public ActionResult Create()
        {
            return View();
        } 
        [HttpPost]
        public ActionResult Create(User objuser)
        {
            return View();
        }
        
        public ActionResult Delete()
        {
            return View();
        }  public ActionResult Edit()
        {
            return View();
        }
    }
}