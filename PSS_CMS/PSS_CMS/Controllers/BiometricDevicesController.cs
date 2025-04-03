using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class BiometricDevicesController : Controller
    {
        // GET: BiometricDevices
        public async Task<ActionResult> Index()
        {
            var HomeController = new HomeController();

            var data = await HomeController.Index() as ViewResult;
            // If you absolutely must do this, extract the ViewBag data like this
            if (data != null)
            {
                ViewBag.Menus = data.ViewBag.Menus;
                ViewBag.SubMenusContact = data.ViewBag.SubMenusContact;
                ViewBag.SubMenusService = data.ViewBag.SubMenusService;
                ViewBag.SubMenusClient = data.ViewBag.SubMenusClient;
                ViewBag.SubMenusHRMS = data.ViewBag.SubMenusHRMS;
                ViewBag.HOMECONTACTUS = data.ViewBag.HOMECONTACTUS;

                // Transfer other ViewBag data as needed
            }

            return View(data);
        }
    }
}