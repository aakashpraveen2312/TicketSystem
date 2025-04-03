using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class CanteenManagementController : Controller
    {
        // GET: CanteenManagement
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

            await CanteenMainSection();
            return View(data);
        }

        public async Task<ActionResult> CanteenMainSection()
        {
           

                string Weburl = ConfigurationManager.AppSettings["GetApi"];
                string AuthKey = ConfigurationManager.AppSettings["Authkey"];

                string APIKey = ConfigurationManager.AppSettings["APIKey"];



                List<CanteenManagement> canteenmainList = new List<CanteenManagement>();
                string strparams = "strACCESSID=" + "PS003" + "&strUNICID=" + "PS003_Services_CanteenManagementSystem_Main" + "&strGroupID=" + "MainSection_CanteenManagement";
                string canteenmainurl = Weburl + "?" + strparams;

                try
                {
                    using (HttpClientHandler handler = new HttpClientHandler())
                    {
                        handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                            client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                            var response = await client.GetAsync(canteenmainurl);

                            if (response.IsSuccessStatusCode)
                            {
                                var jsonString = await response.Content.ReadAsStringAsync();

                                ApiResponsecanteenmanagementmain canteenmain = JsonConvert.DeserializeObject<ApiResponsecanteenmanagementmain>(jsonString);
                                Session["PS_I_CanteenMAIN_RECID"] = canteenmain.Data[0].PS_RECID;

                                Session["PS_I_CanteenMAIN_ACCESSID"] = canteenmain.Data[0].PS_ACCESSID;

                                Session["PS_I_CanteenMAIN_PAGENAME"] = canteenmain.Data[0].PS_PAGENAME;

                                Session["PS_I_CanteenMAIN_CONTENTTYPE"] = canteenmain.Data[0].PS_CONTENTTYPE;

                                Session["PS_I_CanteenMAIN_PARENT"] = canteenmain.Data[0].PS_PARENT;

                                Session["PS_I_CanteenMAIN_NAME"] = canteenmain.Data[0].PS_NAME;

                                Session["PS_I_CanteenMAIN_ID"] = canteenmain.Data[0].PS_ID;

                                Session["PS_I_CanteenMAIN_LASTDATETIME"] = canteenmain.Data[0].PS_LASTDATETIME;

                                Session["PS_I_CanteenMAIN_TYPE"] = canteenmain.Data[0].PS_TYPE;

                                List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(canteenmain.Data[0].PS_VALUES);
                                //string psValues = canteenmain.Data[1].PS_VALUES.ToString();
                                //string base64Image = psValues;


                                //if (!string.IsNullOrEmpty(base64Image))
                                //{

                                //    string mimeType = GetImageMimeType(base64Image);  

                                //    ViewBag.HOMECONTENTMAINIMAGE = base64Image;
                                //    ViewBag.MIMEType = mimeType; 
                                //    Session["Main_content_Images"] = ViewBag.HOMECONTENTMAINIMAGE;
                                //}

                                ViewBag.CanteenMainText = rootObjects;
                            }

                            else
                            {
                                // Handle the error response here
                                ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., logging)
                    ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
                }

                
            return View();
        }
    }
}