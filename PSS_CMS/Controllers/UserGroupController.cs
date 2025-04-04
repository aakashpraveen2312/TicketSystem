using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    public class UserGroupController : Controller
    {
        // GET: UserGroup
        public ActionResult List()
        {
            string filePath = @"D:\Prathesh\GIT_TICKET_SYSTEM_APPLICATION\PSS_CMS\Json\Main.json";
            var jsonData = System.IO.File.ReadAllText(filePath);
            var userGroupList = JsonConvert.DeserializeObject<List<Usergroup>>(jsonData);

            return View(userGroupList);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Usergroup usergroup)
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Edit(Usergroup usergroup)
        {
            return View();
        }
        public ActionResult Delete()
        {
            return View();
        }
    }
}