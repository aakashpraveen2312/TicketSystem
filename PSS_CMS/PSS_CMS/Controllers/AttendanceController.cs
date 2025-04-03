using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    public class AttendanceController : Controller
    {
        // GET: Attendance
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
            await AttendanceMainSection();

            return View(data);
        }
        private string GetImageMimeType(string base64Image)
        {
            if (base64Image.Contains("data:image/jpeg;base64,"))
                return "image/jpeg";
            if (base64Image.Contains("data:image/png;base64,"))
                return "image/png";
            if (base64Image.Contains("data:image/gif;base64,"))
                return "image/gif";
            if (base64Image.Contains("data:image/bmp;base64,"))
                return "image/bmp";
            // Default to JPEG if not found
            return "image/jpeg";
        }
        public async Task<ActionResult> AttendanceMainSection()
        {


            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<Attendance> AttendanceList = new List<Attendance>();
            string strparams = "strACCESSID=" + "PS00AD" + "&strUNICID=" + "PS00AD_HRMS_Attendance" + "&strGroupID=" + "MainSection_Attendance";
            string attendancemainurl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(attendancemainurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiResponseAttendanceMain Attendancemain = JsonConvert.DeserializeObject<ApiResponseAttendanceMain>(jsonString);
                            Session["PS_T_AttendanceMAIN_RECID"] = Attendancemain.Data[0].PS_RECID;
                            Session["PS_T_AttendanceMAIN_ACCESSID"] = Attendancemain.Data[0].PS_ACCESSID;
                            Session["PS_T_AttendanceMAIN_PAGENAME"] = Attendancemain.Data[0].PS_PAGENAME;
                            Session["PS_T_AttendanceMAIN_CONTENTTYPE"] = Attendancemain.Data[0].PS_CONTENTTYPE;
                            Session["PS_T_AttendanceMAIN_PARENT"] = Attendancemain.Data[0].PS_PARENT;
                            Session["PS_T_AttendanceMAIN_NAME"] = Attendancemain.Data[0].PS_NAME;
                            Session["PS_T_AttendanceMAIN_ID"] = Attendancemain.Data[0].PS_ID;
                            Session["PS_T_AttendanceMAIN_LASTDATETIME"] = Attendancemain.Data[0].PS_LASTDATETIME;
                            Session["PS_T_AttendanceMAIN_TYPE"] = Attendancemain.Data[0].PS_TYPE;

                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(Attendancemain.Data[1].PS_VALUES);
                            string psValues = Attendancemain.Data[1].PS_VALUES.ToString();
                            string base64Image = psValues;


                            if (!string.IsNullOrEmpty(base64Image))
                            {

                                string mimeType = GetImageMimeType(base64Image);

                                ViewBag.ATTENDANCEMAINIMAGE = base64Image;
                                ViewBag.ATTENDANCEMIMEType = mimeType;
                                Session["ATTENDANCE_content_Images"] = ViewBag.ATTENDANCEMAINIMAGE;
                            }

                            ViewBag.AttendanceMainText = rootObjects;
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