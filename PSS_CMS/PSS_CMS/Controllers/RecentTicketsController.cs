using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class RecentTicketsController : Controller
    {
        // GET: RecentTickets

       

        public async Task<ActionResult> RecentTicket()
        {

            Recenttickets objRecents = new Recenttickets();

         
            string Weburl = ConfigurationManager.AppSettings["AdminTicketURL"];

            List<Recenttickets> RecentTicketList = new List<Recenttickets>();



            //string strparams = "companyId=" + CompanyId + "&CategoryRecid=" + CategoryRecid + "";
            string url = Weburl;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<APIResponseRecenttickets>(jsonString);
                            RecentTicketList = rootObjects.Data;
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

            return View(RecentTicketList);
        }

        public ActionResult Clients()
        {
            List<Recenttickets> Lists = new List<Recenttickets>
            {
                new Recenttickets {Serialnumber = 1,  Name = "Prathesh" },
                new Recenttickets { Serialnumber = 2,  Name = "Manoj" },
                new Recenttickets { Serialnumber = 3,  Name = "Aaksh" },
                new Recenttickets { Serialnumber = 4,  Name = "Ashwath"},
                new Recenttickets { Serialnumber = 5,  Name = "Sanjay" }
            };
            return View(Lists);
        }



        public async Task<ActionResult> AdminTickets(string recid)
        {
            Session["RECORDID"] = recid;

            //TempData["AppUserRecID"] = id;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["AdminGetSingleURL"];
            //   string Authkey = ConfigurationManager.AppSettings["Authkey"];

            //List<Bins> BinsList = new List<Bins>();
            Admintickets bins = null;

            //     string APIKey = Session["APIKEY"].ToString();

            //GlobalVariables.AU_RECID = id;


            string strparams = "RECID=" + recid;
            string finalurl = WEBURLGETBYID + "?" + strparams;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        //  client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        //   client.DefaultRequestHeaders.Add("Authorization", Authkey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                        var response = await client.GetAsync(finalurl);



                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var content = JsonConvert.DeserializeObject<RootObjectsuser>(jsonString);

                            //APIResponseRecenttickets content2 = JsonConvert.DeserializeObject<APIResponseRecenttickets>(jsonString);




                            //List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(content2.Data.);

                            string base64Image = content.Data.TC_REQUEST_ATTACHMENT_PREFIX;

                            if (!string.IsNullOrEmpty(base64Image))
                            {

                                string mimeType = GetImageMimeType(base64Image);

                                ViewBag.ClientsTicketsIMAGE = base64Image;
                                
                            }


                            bins = content.Data;
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);

                        }

                    }
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occured: " + ex.Message);
            }
            return View(bins);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdminResponseTicket(Admintickets Admintickets, HttpPostedFileBase myfile)
        {
            try
            {
                // Declare fileBytes and base64Image once, before the if block
                byte[] fileBytes = null;
                string base64Image = null;

                // Check if files are uploaded
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0]; // Get the first file from the request
                    if (file != null && file.ContentLength > 0)
                    {
                        // If file exists, read and convert it to base64
                        fileBytes = new byte[file.ContentLength];
                        file.InputStream.Read(fileBytes, 0, file.ContentLength);
                        base64Image = Convert.ToBase64String(fileBytes);

                        // Assign the base64 image to the model property
                        Admintickets.TC_REQUEST_ATTACHMENT_PREFIX = base64Image;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No file uploaded.");
                    }

                }
                // Define your API URL and keys
                var AdminResponsePostURL = ConfigurationManager.AppSettings["AdminResponse"];
                //       string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                //     string APIKey = Session["APIKEY"].ToString();

                // Construct the JSON content for the API request
                var content = $@"{{           
            ""tC_RECID"": ""{Session["RECORDID"]}"",           
            ""tC_USERID"": ""{""}"",           
            ""tC_COMPANYID"": ""{""}"",
            ""tC_PROJECTID"": ""{""}"",        
            ""tC_TICKETDATE"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",        
            ""tC_SUBJECT"": ""{""}"",        
            ""tC_OTP"": ""{""}"",
            ""tC_COMMENTS"": ""{""}"",
            ""tC_REQUEST_ATTACHMENT_PREFIX"": ""{""}"",  
            ""tC_REQUEST_DATETIME"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",
            ""tC_RESPONSE_ATTACHMENT_PREFIX"": ""{base64Image}"",
            ""tC_RESPONSE_USERID"": ""{Session["UserID"]}"",
            ""tC_RESPONSE_DATETIME"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",
            ""tC_RESPONSE_COMMENTS"": ""{Admintickets.TC_RESPONSE_COMMENTS}"",            
            ""tC_STATUS"": ""{"R"}"",
            ""tC_PRIORITYTYPE"": ""{""}""
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(AdminResponsePostURL),
                    Method = HttpMethod.Put,
                    Headers =
            {
                { "X-Version", "1" },
                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
            },
                    Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                };

                // Set up HTTP client with custom validation (for SSL certificates)
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler);

                // Add headers for authentication
                //    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                //   client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                // Send the request and await the response
                var response = await client.SendAsync(request);
                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseTicketsResponse>(responseBody);

                    // Return the appropriate result based on the API response
                    if (apiResponse.Status == "Y")

                    {
                        return RedirectToAction("AdminTickets", "RecentTickets");
                    }
                    else if (apiResponse.Status == "U" || apiResponse.Status == "N")
                    {
                        return Json(new { status = "error", message = apiResponse.Message });
                    }
                    else
                    {
                        return RedirectToAction("List", "Quatation");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return View("Ticket");
        }






        //Over all mainmenu list
        public async Task<ActionResult> Index()
        {
            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<MainMenu> mainMenuList = new List<MainMenu>();
            string strparams = "strACCESSID=" + "PS001" + "&strUNICID=" + "PS001_Root_Menu" + "&strGroupID=" + "0";
            string finalurl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(finalurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiResponseMainmenu Mainmenucontent = JsonConvert.DeserializeObject<ApiResponseMainmenu>(jsonString);
                            Session["PS_MENU_RECID"] = Mainmenucontent.Data[0].PS_RECID;
                            Session["PS_MENU_ACCESSID"] = Mainmenucontent.Data[0].PS_ACCESSID;
                            Session["PS_MENU_PAGENAME"] = Mainmenucontent.Data[0].PS_PAGENAME;
                            Session["PS_MENU_CONTENTTYPE"] = Mainmenucontent.Data[0].PS_CONTENTTYPE;
                            Session["PS_MENU_PARENT"] = Mainmenucontent.Data[0].PS_PARENT;
                            Session["PS_MENU_NAME"] = Mainmenucontent.Data[0].PS_NAME;
                            Session["PS_MENU_ID"] = Mainmenucontent.Data[0].PS_ID;
                            Session["PS_MENU_LASTDATETIME"] = Mainmenucontent.Data[0].PS_LASTDATETIME;
                            Session["PS_MENU_TYPE"] = Mainmenucontent.Data[0].PS_TYPE;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(Mainmenucontent.Data[0].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.Menus = rootObjects;


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



            await SubmenuForServices();

            // Return the view with the menu data
            return View();
        }



        public bool IsValidJsonArray(string strInput)
        {
            if (string.IsNullOrEmpty(strInput)) return false;

            strInput = strInput.Trim();

            // Check if the string starts and ends with square brackets (i.e., it's an array)
            if (strInput.StartsWith("[") && strInput.EndsWith("]"))
            {
                try
                {
                    // Try to deserialize the string to a List
                    JsonConvert.DeserializeObject<List<object>>(strInput);
                    return true;
                }
                catch (JsonException)
                {
                    // If deserialization fails, it's not a valid JSON array
                    return false;
                }
            }

            return false;
        }
        //Over all main menu upddate
        [HttpPost]
        public async Task<ActionResult> Save(List<string> psValues, MainMenu ObjmainMenu, string Form)
        {

            string saveurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_RECID = Session["PS_MENU_RECID"].ToString();
            string PS_ACCESSID = Session["PS_MENU_ACCESSID"].ToString();
            string PS_PAGENAME = Session["PS_MENU_PAGENAME"].ToString();
            string PS_CONTENTTYPE = Session["PS_MENU_CONTENTTYPE"].ToString();
            string PS_PARENT = Session["PS_MENU_PARENT"].ToString();
            string PS_NAME = Session["PS_MENU_NAME"].ToString();
            string PS_ID = Session["PS_MENU_ID"].ToString();
            string PS_LASTDATETIME = Session["PS_MENU_LASTDATETIME"].ToString();
            string PS_TYPE = Session["PS_MENU_TYPE"].ToString();
            
            try
            {


                string serializedPsValues = JsonConvert.SerializeObject(psValues);

                // Create the desired format for pS_VALUES as a string
                string wrappedSerializedPsValues = "[" + string.Join(",", psValues.Select(value => $"\"{value}\"")) + "]";

                // Escape the quotes properly for a string value
                string escapedWrappedSerializedPsValues = wrappedSerializedPsValues.Replace("\"", "\\\"");

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_RECID}"",
                        ""pS_ACCESSID"": ""{PS_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_PARENT}"",
                        ""pS_NAME"": ""{PS_NAME}"",
                        ""pS_ID"": ""{PS_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri($"{saveurl}"), // Assuming the ID is used in the URL
                        Method = HttpMethod.Put, // Change from POST to PUT
                        Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                        Content = new StringContent(content, Encoding.UTF8, "application/json")
                    };
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
    };
    var client = new HttpClient(handler)
    {
    };
    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
    var apiResponse = JsonConvert.DeserializeObject<ApiResponseMainmenu>(responseBody);
                        if (apiResponse.Status == "Y")
                        {
                        return RedirectToAction("Index", "Home");
                        }
                      
                        else
                        {
                          return RedirectToAction("Index", "Home");
                        }
                     }
                       else
                        {
                            ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                        }
          }
                     catch (Exception ex)
                        {
                            Console.WriteLine($"Exception occurred: {ex.Message}");
                            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
                        }
           
            return RedirectToAction("Index", "Home");
        }

        //Services submenu list
        public async Task<ActionResult> SubmenuForServices()
        {
            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<SubMenuServices> submenuserviceList = new List<SubMenuServices>();
            string strparams = "strACCESSID=" + "PS001" + "&strUNICID=" + "PS001_Menu_Services" + "&strGroupID=" + "0";
            string serviceurl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(serviceurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiResponseService Servicecontent = JsonConvert.DeserializeObject<ApiResponseService>(jsonString);
                            Session["PS_SERVICE_RECID"] = Servicecontent.Data[0].PS_RECID;
                            Session["PS_SERVICE_ACCESSID"] = Servicecontent.Data[0].PS_ACCESSID;
                            Session["PS_SERVICE_PAGENAME"] = Servicecontent.Data[0].PS_PAGENAME;
                            Session["PS_SERVICE_CONTENTTYPE"] = Servicecontent.Data[0].PS_CONTENTTYPE;
                            Session["PS_SERVICE_PARENT"] = Servicecontent.Data[0].PS_PARENT;
                            Session["PS_SERVICE_NAME"] = Servicecontent.Data[0].PS_NAME;
                            Session["PS_SERVICE_ID"] = Servicecontent.Data[0].PS_ID;
                            Session["PS_SERVICE_LASTDATETIME"] = Servicecontent.Data[0].PS_LASTDATETIME;
                            Session["PS_SERVICE_TYPE"] = Servicecontent.Data[0].PS_TYPE;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(Servicecontent.Data[0].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.SubMenusService = rootObjects;
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


            await SubmenuForHRMS();
            return View();
        }
        //Services submenu list save
        [HttpPost]
        public async Task<ActionResult> SubmenuForServicesSave(List<string> psValues, SubMenuServices ObjsubMenuservices, string Form)
        {
            string SubMenuServicesurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_RECID = Session["PS_SERVICE_RECID"].ToString();
            string PS_ACCESSID = Session["PS_SERVICE_ACCESSID"].ToString();
            string PS_PAGENAME = Session["PS_SERVICE_PAGENAME"].ToString();
            string PS_CONTENTTYPE = Session["PS_SERVICE_CONTENTTYPE"].ToString();
            string PS_PARENT = Session["PS_SERVICE_PARENT"].ToString();
            string PS_NAME = Session["PS_SERVICE_NAME"].ToString();
            string PS_ID = Session["PS_SERVICE_ID"].ToString();
            string PS_LASTDATETIME = Session["PS_SERVICE_LASTDATETIME"].ToString();
            string PS_TYPE = Session["PS_SERVICE_TYPE"].ToString();

            try
            {


                string serializedPsValues = JsonConvert.SerializeObject(psValues);

                // Create the desired format for pS_VALUES as a string
                string wrappedSerializedPsValues = "[" + string.Join(",", psValues.Select(value => $"\"{value}\"")) + "]";

                // Escape the quotes properly for a string value
                string escapedWrappedSerializedPsValues = wrappedSerializedPsValues.Replace("\"", "\\\"");

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_RECID}"",
                        ""pS_ACCESSID"": ""{PS_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_PARENT}"",
                        ""pS_NAME"": ""{PS_NAME}"",
                        ""pS_ID"": ""{PS_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{SubMenuServicesurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseService>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");
        }


        //HRMS deep dropdown submenu list
        public async Task<ActionResult> SubmenuForHRMS()
        {
            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<SubMenuHRMS> submenuhrmsList = new List<SubMenuHRMS>();
            string strparams = "strACCESSID=" + "PS001" + "&strUNICID=" + "PS001_Services_HRMS" + "&strGroupID=" + "HRMSDropDown";
            string hrmsurl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(hrmsurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiResponseHRMS HRMScontent = JsonConvert.DeserializeObject<ApiResponseHRMS>(jsonString);
                            Session["PS_HRMs_RECID"] = HRMScontent.Data[0].PS_RECID;
                            Session["PS_HRMs_ACCESSID"] = HRMScontent.Data[0].PS_ACCESSID;
                            Session["PS_HRMs_PAGENAME"] = HRMScontent.Data[0].PS_PAGENAME;
                            Session["PS_HRMs_CONTENTTYPE"] = HRMScontent.Data[0].PS_CONTENTTYPE;
                            Session["PS_HRMs_PARENT"] = HRMScontent.Data[0].PS_PARENT;
                            Session["PS_HRMs_NAME"] = HRMScontent.Data[0].PS_NAME;
                            Session["PS_HRMs_ID"] = HRMScontent.Data[0].PS_ID;
                            Session["PS_HRMs_LASTDATETIME"] = HRMScontent.Data[0].PS_LASTDATETIME;
                            Session["PS_HRMs_TYPE"] = HRMScontent.Data[0].PS_TYPE;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(HRMScontent.Data[0].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.SubMenusHRMS = rootObjects;
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

            await HomeMainContent();
            // Return the view with the menu data
            return View();
        }

        //HRMS deep dropdown submenu list save
        [HttpPost]
        public async Task<ActionResult> SubmenuForHRMSSave(List<string> psValues, SubMenuHRMS Objhrms, string Form)
        {
            string HRMSurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_RECID = Session["PS_HRMs_RECID"].ToString();
            string PS_ACCESSID = Session["PS_HRMs_ACCESSID"].ToString();
            string PS_PAGENAME = Session["PS_HRMs_PAGENAME"].ToString();
            string PS_CONTENTTYPE = Session["PS_HRMs_CONTENTTYPE"].ToString();
            string PS_PARENT = Session["PS_HRMs_PARENT"].ToString();
            string PS_NAME = Session["PS_HRMs_NAME"].ToString();
            string PS_ID = Session["PS_HRMs_ID"].ToString();
            string PS_LASTDATETIME = Session["PS_HRMs_LASTDATETIME"].ToString();
            string PS_TYPE = Session["PS_HRMs_TYPE"].ToString();

            try
            {


                string serializedPsValues = JsonConvert.SerializeObject(psValues);

                // Create the desired format for pS_VALUES as a string
                string wrappedSerializedPsValues = "[" + string.Join(",", psValues.Select(value => $"\"{value}\"")) + "]";

                // Escape the quotes properly for a string value
                string escapedWrappedSerializedPsValues = wrappedSerializedPsValues.Replace("\"", "\\\"");

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_RECID}"",
                        ""pS_ACCESSID"": ""{PS_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_PARENT}"",
                        ""pS_NAME"": ""{PS_NAME}"",
                        ""pS_ID"": ""{PS_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HRMSurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseHRMS>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> HomeMainContent()
        {


            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<HomeContentMain> homecontentList = new List<HomeContentMain>();
            string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_MainSection" + "&strGroupID=" + "MainSection";
            string hrmsurl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(hrmsurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiHomeContentResponse homemaincontent = JsonConvert.DeserializeObject<ApiHomeContentResponse>(jsonString);
                            Session["PS_HERO_RECID"] = homemaincontent.Data[0].PS_RECID;
                            Session["PS_HEROIMG_RECID"] = homemaincontent.Data[1].PS_RECID;
                            Session["PS_HERO_ACCESSID"] = homemaincontent.Data[0].PS_ACCESSID; 
                            Session["PS_HEROIMG_ACCESSID"] = homemaincontent.Data[1].PS_ACCESSID;
                            Session["PS_HERO_PAGENAME"] = homemaincontent.Data[0].PS_PAGENAME;
                            Session["PS_HEROIMG_PAGENAME"] = homemaincontent.Data[1].PS_PAGENAME;
                            Session["PS_HERO_CONTENTTYPE"] = homemaincontent.Data[0].PS_CONTENTTYPE;
                            Session["PS_HEROIMG_CONTENTTYPE"] = homemaincontent.Data[1].PS_CONTENTTYPE;
                            Session["PS_HERO_PARENT"] = homemaincontent.Data[0].PS_PARENT;
                            Session["PS_HEROIMG_PARENT"] = homemaincontent.Data[1].PS_PARENT;
                            Session["PS_HERO_NAME"] = homemaincontent.Data[0].PS_NAME;
                            Session["PS_HEROIMG_NAME"] = homemaincontent.Data[1].PS_NAME;
                            Session["PS_HERO_ID"] = homemaincontent.Data[0].PS_ID;
                            Session["PS_HEROIMG_ID"] = homemaincontent.Data[1].PS_ID;
                            Session["PS_HERO_LASTDATETIME"] = homemaincontent.Data[0].PS_LASTDATETIME;
                            Session["PS_HEROIMG_LASTDATETIME"] = homemaincontent.Data[1].PS_LASTDATETIME;
                            Session["PS_HERO_TYPE_TEXT"] = homemaincontent.Data[0].PS_TYPE;
                          
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(homemaincontent.Data[0].PS_VALUES);
                            string psValues = homemaincontent.Data[1].PS_VALUES.ToString();
                            


                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            // Get base64 string of the image if it exists
                            //string base64Image = rootObjects.Data[2].PS_VALUES;
                            string base64Image = psValues;

                            // Check if base64 image exists and assign it to ViewBag and Session
                            if (!string.IsNullOrEmpty(base64Image))
                            {
                                // Determine MIME type based on the base64 content
                                string mimeType = GetImageMimeType(base64Image);  // Implement this method to detect MIME type based on base64 image content

                                // Save the base64 image and MIME type to ViewBag and Session
                                ViewBag.HOMECONTENTMAINIMAGE = base64Image;
                                ViewBag.MIMEType = mimeType;  // Pass MIME type to the view
                                Session["Main_content_Images"] = ViewBag.HOMECONTENTMAINIMAGE;
                            }

                            ViewBag.HOMECONTENTMAIN = rootObjects;
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

            await HomeMainContentAboutUs();

            // Return the view with the menu data
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> HomeMainContentSave(List<string> psValues, HomeContentMain Objhomecontent, string Form)
        {
            string HomeContentMainurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_HERO_RECID = Session["PS_HERO_RECID"].ToString();
            string PS_HERO_ACCESSID = Session["PS_HERO_ACCESSID"].ToString();
            string PS_HERO_PAGENAME = Session["PS_HERO_PAGENAME"].ToString();
            string PS_HERO_CONTENTTYPE = Session["PS_HERO_CONTENTTYPE"].ToString();
            string PS_HERO_PARENT = Session["PS_HERO_PARENT"].ToString();
            string PS_HERO_NAME = Session["PS_HERO_NAME"].ToString();
            string PS_HERO_ID = Session["PS_HERO_ID"].ToString();
            string PS_HERO_LASTDATETIME = Session["PS_HERO_LASTDATETIME"].ToString();
            string PS_HERO_TYPE = Session["PS_HERO_TYPE_TEXT"].ToString();
            

            try
            {


                var escapedPsValues = psValues.Select(value =>
      value.Replace("\\", "\\\\")  // Escape backslashes
           .Replace("\"", "\\\"")  // Escape quotes
           .Replace("\n", "\\n")   // Escape newlines
           .Replace("\r", "\\r")); // Escape carriage returns

                // Create the desired format for pS_VALUES as a string
                string wrappedSerializedPsValues = "[" + string.Join(",", escapedPsValues.Select(value => $"\"{value}\"")) + "]";

                // Escape the quotes properly for a string value
                string escapedWrappedSerializedPsValues = wrappedSerializedPsValues.Replace("\"", "\\\"");

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_HERO_RECID}"",
                        ""pS_ACCESSID"": ""{PS_HERO_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_HERO_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_HERO_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_HERO_PARENT}"",
                        ""pS_NAME"": ""{PS_HERO_NAME}"",
                        ""pS_ID"": ""{PS_HERO_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_HERO_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HomeContentMainurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeContentResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");
        }

        //public async Task<ActionResult> HomeMainContentImage()
        //{


        //    string Weburl = ConfigurationManager.AppSettings["GetApi"];
        //    string AuthKey = ConfigurationManager.AppSettings["Authkey"];

        //    string APIKey = ConfigurationManager.AppSettings["APIKey"];



        //    List<HomeContentCoreactivityimage> coreimagelist = new List<HomeContentCoreactivityimage>();
        //    string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_MainSection_Images";
        //    string HomeContentMainImageurl = Weburl + "?" + strparams;

        //    try
        //    {
        //        using (HttpClientHandler handler = new HttpClientHandler())
        //        {
        //            handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        //            using (HttpClient client = new HttpClient(handler))
        //            {
        //                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        //                var response = await client.GetAsync(HomeContentMainImageurl);

        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var jsonString = await response.Content.ReadAsStringAsync();

        //                    ApiHomeContentImageResponse homemainimagecontent = JsonConvert.DeserializeObject<ApiHomeContentImageResponse>(jsonString);
        //                    Session["PS_IMAGEMAINRECID"] = homemainimagecontent.Data[0].PS_RECID;
        //                    Session["PS_ACCESSID"] = homemainimagecontent.Data[0].PS_ACCESSID;
        //                    Session["PS_PAGENAME"] = homemainimagecontent.Data[0].PS_PAGENAME;
        //                    Session["PS_CONTENTTYPE"] = homemainimagecontent.Data[0].PS_CONTENTTYPE;
        //                    Session["PS_PARENT"] = homemainimagecontent.Data[0].PS_PARENT;
        //                    Session["PS_NAME"] = homemainimagecontent.Data[0].PS_NAME;
        //                    Session["PS_IMAGEMAINID"] = homemainimagecontent.Data[0].PS_ID;
        //                    Session["PS_LASTDATETIME"] = homemainimagecontent.Data[0].PS_LASTDATETIME;
        //                    Session["PS_TYPE"] = homemainimagecontent.Data[0].PS_TYPE;
        //                    // Get base64 string of the image if it exists
        //                    string base64Image = homemainimagecontent.Data[0].PS_VALUES;

        //                    // Check if base64 image exists and assign it to ViewBag and Session
        //                    if (!string.IsNullOrEmpty(base64Image))
        //                    {
        //                        // Determine MIME type based on the base64 content
        //                        string mimeType = GetImageMimeType(base64Image);  // Implement this method to detect MIME type based on base64 image content

        //                        // Save the base64 image and MIME type to ViewBag and Session
        //                        ViewBag.HOMECONTENTMAINIMAGE = base64Image;
        //                        ViewBag.MIMEType = mimeType;  // Pass MIME type to the view
        //                        Session["Main_content_Images"] = ViewBag.HOMECONTENTMAINIMAGE;
        //                    }
        //                    else
        //                    {
        //                        // If no image found, set it as null
        //                        ViewBag.HOMECONTENTMAINIMAGE = null;
        //                        Session["Main_content_Images"] = null;
        //                    }
        //                }

        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions (e.g., logging)
        //        ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
        //    }


        //    await HomeMainContentAboutUs();
        //    // Return the view with the menu data
        //    return View();
        //}
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

        [HttpPost]
        public async Task<ActionResult> HomeMainContentImageSave(HttpPostedFileBase file, HomeContentMainImages HomecontentMainimages, string ExistingImageMain)
        {

            string HomeContentMainImageurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_HEROIMG_RECID = Session["PS_HEROIMG_RECID"].ToString();
            string PS_HEROIMG_ACCESSID = Session["PS_HEROIMG_ACCESSID"].ToString();
            string PS_HEROIMG_PAGENAME = Session["PS_HEROIMG_PAGENAME"].ToString();
            string PS_HEROIMG_CONTENTTYPE = Session["PS_HEROIMG_CONTENTTYPE"].ToString();
            string PS_HEROIMG_PARENT = Session["PS_HEROIMG_PARENT"].ToString();
            string PS_HEROIMG_NAME = Session["PS_HEROIMG_NAME"].ToString();
            string PS_HEROIMG_ID = Session["PS_HEROIMG_ID"].ToString();
            string PS_HEROIMG_LASTDATETIME = Session["PS_HEROIMG_LASTDATETIME"].ToString();
            string PS_HEROIMG_TYPE = Session["PS_HERO_TYPE_TEXT"].ToString();


            string base64Image;

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    // Convert the uploaded image to Base64
                    byte[] fileBytes = new byte[file.ContentLength];
                    file.InputStream.Read(fileBytes, 0, file.ContentLength);
                    base64Image = Convert.ToBase64String(fileBytes);
                    // Update session with new image
                    Session["Main_Image"] = base64Image;
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error processing the file: {ex.Message}";
                    return RedirectToAction("HomeMainContent", "RecentTickets"); // Redirect to the original page
                }
            }
            else if (!string.IsNullOrEmpty(ExistingImageMain))
            {
                // Use the existing image
                base64Image = ExistingImageMain;

                // Update the session with the existing image (to ensure it's consistent in the session)
                Session["Main_Image"] = base64Image;
            }
            else
            {
                TempData["ErrorMessage"] = "No file selected or existing image provided.";
                return RedirectToAction("HomeMainContent", "RecentTickets"); // Redirect to the original page
            }
            try
            {
                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_HEROIMG_RECID}"",
                        ""pS_ACCESSID"": ""{PS_HEROIMG_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_HEROIMG_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_HEROIMG_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_HEROIMG_PARENT}"",
                        ""pS_NAME"": ""{PS_HEROIMG_NAME}"",
                        ""pS_ID"": ""{PS_HEROIMG_ID}"",
                        ""pS_VALUES"": ""{base64Image}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_HEROIMG_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HomeContentMainImageurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeContentImageResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("HomeMainContent", "RecentTickets");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");


          
        }

        public async Task<ActionResult> HomeMainContentAboutUs()
        {


            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<HomeContentMainAboutus> homecontentList = new List<HomeContentMainAboutus>();
            string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_AboutUs" + "&strGroupID=" + "AboutUsSection";
            string HomeContentMainaboutusurl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(HomeContentMainaboutusurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiHomeContentAboutusResponse homemaincontentaboutus = JsonConvert.DeserializeObject<ApiHomeContentAboutusResponse>(jsonString);
                            Session["PS_H_ABOUT_RECID"] = homemaincontentaboutus.Data[0].PS_RECID;
                            Session["PS_H_ABOUT_ACCESSID"] = homemaincontentaboutus.Data[0].PS_ACCESSID;
                            Session["PS_H_ABOUT_PAGENAME"] = homemaincontentaboutus.Data[0].PS_PAGENAME;
                            Session["PS_H_ABOUT_CONTENTTYPE"] = homemaincontentaboutus.Data[0].PS_CONTENTTYPE;
                            Session["PS_H_ABOUT_PARENT"] = homemaincontentaboutus.Data[0].PS_PARENT;
                            Session["PS_H_ABOUT_NAME"] = homemaincontentaboutus.Data[0].PS_NAME;
                            Session["PS_H_ABOUT_ID"] = homemaincontentaboutus.Data[0].PS_ID;
                            Session["PS_H_ABOUT_LASTDATETIME"] = homemaincontentaboutus.Data[0].PS_LASTDATETIME;
                            Session["PS_H_ABOUT_TYPE"] = homemaincontentaboutus.Data[0].PS_TYPE;
                            //mainMenuList = rootObjects.Data;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(homemaincontentaboutus.Data[0].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.HOMECONTENTMAINABOUTUS = rootObjects;
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
            await HomeMainContentCoreActivities();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> HomeMainContentAboutusSave(List<string> psValues, HomeContentMainAboutus Objaboutus, string Form)
        {
            string HomeContentAboutUSurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_H_ABOUT_RECID = Session["PS_H_ABOUT_RECID"].ToString();
            string PS_H_ABOUT_ACCESSID = Session["PS_H_ABOUT_ACCESSID"].ToString();
            string PS_H_ABOUT_PAGENAME = Session["PS_H_ABOUT_PAGENAME"].ToString();
            string PS_H_ABOUT_CONTENTTYPE = Session["PS_H_ABOUT_CONTENTTYPE"].ToString();
            string PS_H_ABOUT_PARENT = Session["PS_H_ABOUT_PARENT"].ToString();
            string PS_H_ABOUT_NAME = Session["PS_H_ABOUT_NAME"].ToString();
            string PS_H_ABOUT_ID = Session["PS_H_ABOUT_ID"].ToString();
            string PS_H_ABOUT_LASTDATETIME = Session["PS_H_ABOUT_LASTDATETIME"].ToString();
            string PS_H_ABOUT_TYPE = Session["PS_H_ABOUT_TYPE"].ToString();

            try
            {


                var escapedPsValues = psValues.Select(value =>
      value.Replace("\\", "\\\\")  // Escape backslashes
           .Replace("\"", "\\\"")  // Escape quotes
           .Replace("\n", "\\n")   // Escape newlines
           .Replace("\r", "\\r")); // Escape carriage returns

                // Create the desired format for pS_VALUES as a string
                string wrappedSerializedPsValues = "[" + string.Join(",", escapedPsValues.Select(value => $"\"{value}\"")) + "]";

                // Escape the quotes properly for a string value
                string escapedWrappedSerializedPsValues = wrappedSerializedPsValues.Replace("\"", "\\\"");

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_H_ABOUT_RECID}"",
                        ""pS_ACCESSID"": ""{PS_H_ABOUT_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_H_ABOUT_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_H_ABOUT_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_H_ABOUT_PARENT}"",
                        ""pS_NAME"": ""{PS_H_ABOUT_NAME}"",
                        ""pS_ID"": ""{PS_H_ABOUT_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_H_ABOUT_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HomeContentAboutUSurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeContentAboutusResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                       
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> HomeMainContentCoreActivities()
        {


            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<HomeContentCoreactivity> homecontentList = new List<HomeContentCoreactivity>();
            string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_Core" + "&strGroupID=" + "CoreSection";
            string HomeContentMainaboutusurl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(HomeContentMainaboutusurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiHomeContentCoreactivityResponse coreactivitytext = JsonConvert.DeserializeObject<ApiHomeContentCoreactivityResponse>(jsonString);
                            Session["PS_CORE_RECID"] = coreactivitytext.Data[0].PS_RECID;
                            Session["PS_COREIMG_RECID"] = coreactivitytext.Data[1].PS_RECID;
                            Session["PS_CORE_ACCESSID"] = coreactivitytext.Data[0].PS_ACCESSID;
                            Session["PS_COREIMG_ACCESSID"] = coreactivitytext.Data[1].PS_ACCESSID;
                            Session["PS_CORE_PAGENAME"] = coreactivitytext.Data[0].PS_PAGENAME;
                            Session["PS_COREIMG_PAGENAME"] = coreactivitytext.Data[1].PS_PAGENAME;
                            Session["PS_CORE_CONTENTTYPE"] = coreactivitytext.Data[0].PS_CONTENTTYPE;
                            Session["PS_COREIMG_CONTENTTYPE"] = coreactivitytext.Data[1].PS_CONTENTTYPE;
                            Session["PS_CORE_PARENT"] = coreactivitytext.Data[0].PS_PARENT;
                            Session["PS_COREIMG_PARENT"] = coreactivitytext.Data[1].PS_PARENT;
                            Session["PS_CORE_NAME"] = coreactivitytext.Data[0].PS_NAME;
                            Session["PS_COREIMG_NAME"] = coreactivitytext.Data[1].PS_NAME;
                            Session["PS_CORE_ID"] = coreactivitytext.Data[0].PS_ID;
                            Session["PS_COREIMG_ID"] = coreactivitytext.Data[1].PS_ID;
                            Session["PS_CORE_LASTDATETIME"] = coreactivitytext.Data[0].PS_LASTDATETIME;
                            Session["PS_COREIMG_LASTDATETIME"] = coreactivitytext.Data[1].PS_LASTDATETIME;
                            Session["PS_CORE_TYPE"] = coreactivitytext.Data[0].PS_TYPE;
                            //mainMenuList = rootObjects.Data;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(coreactivitytext.Data[0].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.HOMECORETEXT = rootObjects;
                            string psValuescore = coreactivitytext.Data[1].PS_VALUES.ToString();


                            string base64Image = psValuescore;
                            // Check if base64 image exists and assign it to ViewBag and Session
                            if (!string.IsNullOrEmpty(base64Image))
                            {
                                // Determine MIME type based on the base64 content
                                string mimeType = GetImageMimeType(base64Image);  // Implement this method to detect MIME type based on base64 image content

                                // Save the base64 image and MIME type to ViewBag and Session
                                ViewBag.HOMECONTENTCOREIMAGE = base64Image;
                                ViewBag.CoreMimeType = mimeType;  // Pass MIME type to the view
                                Session["Core_Activity_Images"] = ViewBag.HOMECONTENTCOREIMAGE;
                            }
                            else
                            {
                                // If no image found, set it as null
                                ViewBag.HOMECONTENTCOREIMAGE = null;
                                Session["Core_Activity_Images"] = null;
                            }
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

            await HomeSkillText();
            // Return the view with the menu data
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> HomeMainContentCoreActivityTextSave(List<string> psValues, HomeContentCoreactivity Objcoretext, string Form)
        {
            string HomeContentcoretexturl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_CORE_RECID = Session["PS_CORE_RECID"].ToString();
            string PS_CORE_ACCESSID = Session["PS_CORE_ACCESSID"].ToString();
            string PS_CORE_PAGENAME = Session["PS_CORE_PAGENAME"].ToString();
            string PS_CORE_CONTENTTYPE = Session["PS_CORE_CONTENTTYPE"].ToString();
            string PS_CORE_PARENT = Session["PS_CORE_PARENT"].ToString();
            string PS_CORE_NAME = Session["PS_CORE_NAME"].ToString();
            string PS_CORE_ID = Session["PS_CORE_ID"].ToString();
            string PS_CORE_LASTDATETIME = Session["PS_CORE_LASTDATETIME"].ToString();
            string PS_CORE_TYPE = Session["PS_CORE_TYPE"].ToString();

            try
            {


                var escapedPsValues = psValues.Select(value =>
      value.Replace("\\", "\\\\")  // Escape backslashes
           .Replace("\"", "\\\"")  // Escape quotes
           .Replace("\n", "\\n")   // Escape newlines
           .Replace("\r", "\\r")); // Escape carriage returns

                // Create the desired format for pS_VALUES as a string
                string wrappedSerializedPsValues = "[" + string.Join(",", escapedPsValues.Select(value => $"\"{value}\"")) + "]";

                // Escape the quotes properly for a string value
                string escapedWrappedSerializedPsValues = wrappedSerializedPsValues.Replace("\"", "\\\"");

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_CORE_RECID}"",
                        ""pS_ACCESSID"": ""{PS_CORE_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_CORE_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_CORE_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_CORE_PARENT}"",
                        ""pS_NAME"": ""{PS_CORE_NAME}"",
                        ""pS_ID"": ""{PS_CORE_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_CORE_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HomeContentcoretexturl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeContentCoreactivityResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");
        }
       
        
        
        //public async Task<ActionResult> HomeMainContentCoreImage()
        //{


        //    string Weburl = ConfigurationManager.AppSettings["GetApi"];
        //    string AuthKey = ConfigurationManager.AppSettings["Authkey"];

        //    string APIKey = ConfigurationManager.AppSettings["APIKey"];



        //    List<HomeContentMainImages> homecontentList = new List<HomeContentMainImages>();
        //    string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_MainSection_CoreImage";
        //    string HomeContentMainImageurl = Weburl + "?" + strparams;

        //    try
        //    {
        //        using (HttpClientHandler handler = new HttpClientHandler())
        //        {
        //            handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        //            using (HttpClient client = new HttpClient(handler))
        //            {
        //                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        //                var response = await client.GetAsync(HomeContentMainImageurl);

        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var jsonString = await response.Content.ReadAsStringAsync();

        //                    ApiHomeContentCoreactivityImageResponse coreimagecontent = JsonConvert.DeserializeObject<ApiHomeContentCoreactivityImageResponse>(jsonString);
        //                    Session["PS_RECID"] = coreimagecontent.Data[0].PS_RECID;
        //                    Session["PS_ACCESSID"] = coreimagecontent.Data[0].PS_ACCESSID;
        //                    Session["PS_PAGENAME"] = coreimagecontent.Data[0].PS_PAGENAME;
        //                    Session["PS_CONTENTTYPE"] = coreimagecontent.Data[0].PS_CONTENTTYPE;
        //                    Session["PS_PARENT"] = coreimagecontent.Data[0].PS_PARENT;
        //                    Session["PS_NAME"] = coreimagecontent.Data[0].PS_NAME;
        //                    Session["PS_ID"] = coreimagecontent.Data[0].PS_ID;
        //                    Session["PS_LASTDATETIME"] = coreimagecontent.Data[0].PS_LASTDATETIME;
        //                    Session["PS_TYPE"] = coreimagecontent.Data[0].PS_TYPE;
        //                    // Get base64 string of the image if it exists
        //                    string base64Image = coreimagecontent.Data[0].PS_VALUES;

        //                    // Check if base64 image exists and assign it to ViewBag and Session
        //                    if (!string.IsNullOrEmpty(base64Image))
        //                    {
        //                        // Determine MIME type based on the base64 content
        //                        string mimeType = GetImageMimeType(base64Image);  // Implement this method to detect MIME type based on base64 image content

        //                        // Save the base64 image and MIME type to ViewBag and Session
        //                        ViewBag.HOMECONTENTCOREIMAGE = base64Image;
        //                        ViewBag.CoreMimeType = mimeType;  // Pass MIME type to the view
        //                        Session["Core_Activity_Images"] = ViewBag.HOMECONTENTCOREIMAGE;
        //                    }
        //                    else
        //                    {
        //                        // If no image found, set it as null
        //                        ViewBag.HOMECONTENTCOREIMAGE = null;
        //                        Session["Core_Activity_Images"] = null;
        //                    }
        //                }

        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions (e.g., logging)
        //        ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
        //    }
        //    await HomeSkillText();
        //    return View();
        //}

        [HttpPost]
        public async Task<ActionResult> HomeCoreActivityImageSave(HttpPostedFileBase file, HomeContentCoreactivityimage Homecontentcoreimages, string ExistingImageCore)
        {

            string HomeContentCoreImageurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_COREIMG_RECID = Session["PS_COREIMG_RECID"].ToString();
            string PS_COREIMG_ACCESSID = Session["PS_COREIMG_ACCESSID"].ToString();
            string PS_COREIMG_PAGENAME = Session["PS_COREIMG_PAGENAME"].ToString();
            string PS_COREIMG_CONTENTTYPE = Session["PS_COREIMG_CONTENTTYPE"].ToString();
            string PS_COREIMG_PARENT = Session["PS_COREIMG_PARENT"].ToString();
            string PS_COREIMG_NAME = Session["PS_COREIMG_NAME"].ToString();
            string PS_COREIMG_ID = Session["PS_COREIMG_ID"].ToString();
            string PS_COREIMG_LASTDATETIME = Session["PS_COREIMG_LASTDATETIME"].ToString();
            string PS_COREIMG_TYPE = Session["PS_CORE_TYPE"].ToString();


            string base64Image;

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    // Convert the uploaded image to Base64
                    byte[] fileBytes = new byte[file.ContentLength];
                    file.InputStream.Read(fileBytes, 0, file.ContentLength);
                    base64Image = Convert.ToBase64String(fileBytes);
                    // Update session with new image
                    Session["Core_Image"] = base64Image;
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error processing the file: {ex.Message}";
                    return RedirectToAction("HomeMainContent", "RecentTickets"); // Redirect to the original page
                }
            }
            else if (!string.IsNullOrEmpty(ExistingImageCore))
            {
                // Use the existing image
                base64Image = ExistingImageCore;

                // Update the session with the existing image (to ensure it's consistent in the session)
                Session["Core_Image"] = base64Image;
            }
            else
            {
                TempData["ErrorMessage"] = "No file selected or existing image provided.";
                return RedirectToAction("HomeMainContent", "RecentTickets"); // Redirect to the original page
            }

            try
            {

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_COREIMG_RECID}"",
                        ""pS_ACCESSID"": ""{PS_COREIMG_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_COREIMG_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_COREIMG_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_COREIMG_PARENT}"",
                        ""pS_NAME"": ""{PS_COREIMG_NAME}"",
                        ""pS_ID"": ""{PS_COREIMG_ID}"",
                        ""pS_VALUES"": ""{base64Image}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_COREIMG_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HomeContentCoreImageurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeContentCoreactivityImageResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("HomeMainContent", "RecentTickets");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");



        }
        public async Task<ActionResult> HomeSkillText()
        {


            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<HomeContentSkillText> SkillTextList = new List<HomeContentSkillText>();
            string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_Skill" + "&strGroupID=" + "SkillSection";
            string HomeSkilltextgeturl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(HomeSkilltextgeturl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiHomeSkillTextResponse Skilltext = JsonConvert.DeserializeObject<ApiHomeSkillTextResponse>(jsonString);
                            Session["PS_ST_RECID"] = Skilltext.Data[1].PS_RECID;
                            Session["PS_SIMG_RECID"] = Skilltext.Data[0].PS_RECID;
                            Session["PS_ST_ACCESSID"] = Skilltext.Data[1].PS_ACCESSID;
                            Session["PS_SIMG_ACCESSID"] = Skilltext.Data[0].PS_ACCESSID;
                            Session["PS_ST_PAGENAME"] = Skilltext.Data[1].PS_PAGENAME;
                            Session["PS_SIMG_PAGENAME"] = Skilltext.Data[0].PS_PAGENAME;
                            Session["PS_ST_CONTENTTYPE"] = Skilltext.Data[1].PS_CONTENTTYPE;
                            Session["PS_SIMG_CONTENTTYPE"] = Skilltext.Data[0].PS_CONTENTTYPE;
                            Session["PS_ST_PARENT"] = Skilltext.Data[1].PS_PARENT;
                            Session["PS_SIMG_PARENT"] = Skilltext.Data[0].PS_PARENT;
                            Session["PS_ST_NAME"] = Skilltext.Data[1].PS_NAME;
                            Session["PS_SIMG_NAME"] = Skilltext.Data[0].PS_NAME;
                            Session["PS_ST_ID"] = Skilltext.Data[1].PS_ID;
                            Session["PS_SIMG_ID"] = Skilltext.Data[0].PS_ID;
                            Session["PS_ST_LASTDATETIME"] = Skilltext.Data[1].PS_LASTDATETIME;
                            Session["PS_SIMG_LASTDATETIME"] = Skilltext.Data[0].PS_LASTDATETIME;
                            Session["PS_ST_TYPE"] = Skilltext.Data[1].PS_TYPE;
                            //mainMenuList = rootObjects.Data;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(Skilltext.Data[1].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.HOMESKILLTEXT = rootObjects;
                            string psValuesskill = Skilltext.Data[0].PS_VALUES.ToString();
                            string base64Image = psValuesskill;

                            // Check if base64 image exists and assign it to ViewBag and Session
                            if (!string.IsNullOrEmpty(base64Image))
                            {
                                // Determine MIME type based on the base64 content
                                string mimeType = GetImageMimeType(base64Image);  // Implement this method to detect MIME type based on base64 image content

                                // Save the base64 image and MIME type to ViewBag and Session
                                ViewBag.HOMESKILLIMAGE = base64Image;
                                ViewBag.SkillMimeType = mimeType;  // Pass MIME type to the view
                                Session["Skill_Images"] = ViewBag.HOMESKILLIMAGE;
                            }
                            else
                            {
                                // If no image found, set it as null
                                ViewBag.HOMESKILLIMAGE = null;
                                Session["Skill_Images"] = null;
                            }
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
            await HomeContactUs();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> HomeSkillTextSave(List<string> psValues, HomeContentSkillText Objskilltext, string Form)
        {
            string HomeContentskilltexturl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_ST_RECID = Session["PS_ST_RECID"].ToString();
            string PS_ST_ACCESSID = Session["PS_ST_ACCESSID"].ToString();
            string PS_ST_PAGENAME = Session["PS_ST_PAGENAME"].ToString();
            string PS_ST_CONTENTTYPE = Session["PS_ST_CONTENTTYPE"].ToString();
            string PS_ST_PARENT = Session["PS_ST_PARENT"].ToString();
            string PS_ST_NAME = Session["PS_ST_NAME"].ToString();
            string PS_ST_ID = Session["PS_ST_ID"].ToString();
            string PS_ST_LASTDATETIME = Session["PS_ST_LASTDATETIME"].ToString();
            string PS_ST_TYPE = Session["PS_ST_TYPE"].ToString();

            try
            {


                var escapedPsValues = psValues.Select(value =>
      value.Replace("\\", "\\\\")  // Escape backslashes
           .Replace("\"", "\\\"")  // Escape quotes
           .Replace("\n", "\\n")   // Escape newlines
           .Replace("\r", "\\r")); // Escape carriage returns

                // Create the desired format for pS_VALUES as a string
                string wrappedSerializedPsValues = "[" + string.Join(",", escapedPsValues.Select(value => $"\"{value}\"")) + "]";

                // Escape the quotes properly for a string value
                string escapedWrappedSerializedPsValues = wrappedSerializedPsValues.Replace("\"", "\\\"");

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_ST_RECID}"",
                        ""pS_ACCESSID"": ""{PS_ST_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_ST_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_ST_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_ST_PARENT}"",
                        ""pS_NAME"": ""{PS_ST_NAME}"",
                        ""pS_ID"": ""{PS_ST_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_ST_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HomeContentskilltexturl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeSkillTextResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");
        }
        //public async Task<ActionResult> HomeSkillImage()
        //{


        //    string Weburl = ConfigurationManager.AppSettings["GetApi"];
        //    string AuthKey = ConfigurationManager.AppSettings["Authkey"];

        //    string APIKey = ConfigurationManager.AppSettings["APIKey"];



        //    List<HomeContentMainImages> homecontentList = new List<HomeContentMainImages>();
        //    string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_MainSection_SkillImage";
        //    string Homeskillimagegeturl = Weburl + "?" + strparams;

        //    try
        //    {
        //        using (HttpClientHandler handler = new HttpClientHandler())
        //        {
        //            handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        //            using (HttpClient client = new HttpClient(handler))
        //            {
        //                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        //                var response = await client.GetAsync(Homeskillimagegeturl);

        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var jsonString = await response.Content.ReadAsStringAsync();

        //                    ApiHomeSkillImageResponse skillimagecontent = JsonConvert.DeserializeObject<ApiHomeSkillImageResponse>(jsonString);
        //                    Session["PS_SI_RECID"] = skillimagecontent.Data[0].PS_RECID;
        //                    Session["PS_SI_ACCESSID"] = skillimagecontent.Data[0].PS_ACCESSID;
        //                    Session["PS_SI_PAGENAME"] = skillimagecontent.Data[0].PS_PAGENAME;
        //                    Session["PS_SI_CONTENTTYPE"] = skillimagecontent.Data[0].PS_CONTENTTYPE;
        //                    Session["PS_SI_PARENT"] = skillimagecontent.Data[0].PS_PARENT;
        //                    Session["PS_SI_NAME"] = skillimagecontent.Data[0].PS_NAME;
        //                    Session["PS_SI_ID"] = skillimagecontent.Data[0].PS_ID;
        //                    Session["PS_SI_LASTDATETIME"] = skillimagecontent.Data[0].PS_LASTDATETIME;
        //                    Session["PS_SI_TYPE"] = skillimagecontent.Data[0].PS_TYPE;
        //                    // Get base64 string of the image if it exists
        //                    string base64Image = skillimagecontent.Data[0].PS_VALUES;

        //                    // Check if base64 image exists and assign it to ViewBag and Session
        //                    if (!string.IsNullOrEmpty(base64Image))
        //                    {
        //                        // Determine MIME type based on the base64 content
        //                        string mimeType = GetImageMimeType(base64Image); 

        //                        // Save the base64 image and MIME type to ViewBag and Session
        //                        ViewBag.HOMESKILLIMAGE = base64Image;
        //                        ViewBag.SkillMimeType = mimeType;  // Pass MIME type to the view
        //                        Session["Skill_Images"] = ViewBag.HOMESKILLIMAGE;
        //                    }
        //                    else
        //                    {
        //                        // If no image found, set it as null
        //                        ViewBag.HOMESKILLIMAGE = null;
        //                        Session["Skill_Images"] = null;
        //                    }
        //                }

        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions (e.g., logging)
        //        ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
        //    }
        //    await HomeContactUs();
        //    return View();
        //}

        [HttpPost]
        public async Task<ActionResult> HomeSkillImageSave(HttpPostedFileBase file, HomeSkillimage Homeskillimages, string ExistingImageSkill)
        {

            string HomeSkillImageUpdateurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_SIMG_RECID = Session["PS_SIMG_RECID"].ToString();
            string PS_SIMG_ACCESSID = Session["PS_SIMG_ACCESSID"].ToString();
            string PS_SIMG_PAGENAME = Session["PS_SIMG_PAGENAME"].ToString();
            string PS_SIMG_CONTENTTYPE = Session["PS_SIMG_CONTENTTYPE"].ToString();
            string PS_SIMG_PARENT = Session["PS_SIMG_PARENT"].ToString();
            string PS_SIMG_NAME = Session["PS_SIMG_NAME"].ToString();
            string PS_SIMG_ID = Session["PS_SIMG_ID"].ToString();
            string PS_SIMG_LASTDATETIME = Session["PS_SIMG_LASTDATETIME"].ToString();
            string PS_SIMG_TYPE = Session["PS_ST_TYPE"].ToString();


            string base64Image;
          

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    // Convert the uploaded image to Base64
                    byte[] fileBytes = new byte[file.ContentLength];
                    file.InputStream.Read(fileBytes, 0, file.ContentLength);
                    base64Image = Convert.ToBase64String(fileBytes);

                    // Update session with new image
                    Session["Skill_Image"] = base64Image;
                   
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error processing the file: {ex.Message}";
                    return RedirectToAction("HomeMainContent", "RecentTickets"); // Redirect to the original page
                }
            }
            else if (!string.IsNullOrEmpty(ExistingImageSkill))
            {
                // Use the existing image
                base64Image = ExistingImageSkill;

                // Update the session with the existing image (to ensure it's consistent in the session)
                Session["Skill_Image"] = base64Image;
             
            }
            else
            {
                TempData["ErrorMessage"] = "No file selected or existing image provided.";
                return RedirectToAction("HomeMainContent", "RecentTickets"); // Redirect to the original page
            }

            try
            {

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_SIMG_RECID}"",
                        ""pS_ACCESSID"": ""{PS_SIMG_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_SIMG_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_SIMG_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_SIMG_PARENT}"",
                        ""pS_NAME"": ""{PS_SIMG_NAME}"",
                        ""pS_ID"": ""{PS_SIMG_ID}"",
                        ""pS_VALUES"": ""{base64Image}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_SIMG_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HomeSkillImageUpdateurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeSkillImageResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("HomeMainContent", "RecentTickets");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");



        }
        public async Task<ActionResult> HomeContactUs()
        {


            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<Homecontactus> contactusList = new List<Homecontactus>();
            string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_ContactUs" + "&strGroupID=" + "0";
            string Contactusgeturl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(Contactusgeturl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiHomeContactus Contactuscontent = JsonConvert.DeserializeObject<ApiHomeContactus>(jsonString);
                            Session["PS_CU_RECID"] = Contactuscontent.Data[0].PS_RECID;
                            Session["PS_CU_ACCESSID"] = Contactuscontent.Data[0].PS_ACCESSID;
                            Session["PS_CU_PAGENAME"] = Contactuscontent.Data[0].PS_PAGENAME;
                            Session["PS_CU_CONTENTTYPE"] = Contactuscontent.Data[0].PS_CONTENTTYPE;
                            Session["PS_CU_PARENT"] = Contactuscontent.Data[0].PS_PARENT;
                            Session["PS_CU_NAME"] = Contactuscontent.Data[0].PS_NAME;
                            Session["PS_CU_ID"] = Contactuscontent.Data[0].PS_ID;
                            Session["PS_CU_LASTDATETIME"] = Contactuscontent.Data[0].PS_LASTDATETIME;
                            Session["PS_CU_TYPE"] = Contactuscontent.Data[0].PS_TYPE;
                            //mainMenuList = rootObjects.Data;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(Contactuscontent.Data[0].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.HOMECONTACTUS = rootObjects;
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
        [HttpPost]
        public async Task<ActionResult> HomeContactUsSave(List<string> psValues, Homecontactus Objcontactus, string Form)
        {
            string Homecontactusputurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_RECID = Session["PS_CU_RECID"].ToString();
            string PS_ACCESSID = Session["PS_CU_ACCESSID"].ToString();
            string PS_PAGENAME = Session["PS_CU_PAGENAME"].ToString();
            string PS_CONTENTTYPE = Session["PS_CU_CONTENTTYPE"].ToString();
            string PS_PARENT = Session["PS_CU_PARENT"].ToString();
            string PS_NAME = Session["PS_CU_NAME"].ToString();
            string PS_ID = Session["PS_CU_ID"].ToString();
            string PS_LASTDATETIME = Session["PS_CU_LASTDATETIME"].ToString();
            string PS_TYPE = Session["PS_CU_TYPE"].ToString();

            try
            {


                var escapedPsValues = psValues.Select(value =>
      value.Replace("\\", "\\\\")  // Escape backslashes
           .Replace("\"", "\\\"")  // Escape quotes
           .Replace("\n", "\\n")   // Escape newlines
           .Replace("\r", "\\r")); // Escape carriage returns

                // Create the desired format for pS_VALUES as a string
                string wrappedSerializedPsValues = "[" + string.Join(",", escapedPsValues.Select(value => $"\"{value}\"")) + "]";

                // Escape the quotes properly for a string value
                string escapedWrappedSerializedPsValues = wrappedSerializedPsValues.Replace("\"", "\\\"");

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_RECID}"",
                        ""pS_ACCESSID"": ""{PS_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_PARENT}"",
                        ""pS_NAME"": ""{PS_NAME}"",
                        ""pS_ID"": ""{PS_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{Homecontactusputurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeContactus>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");
        }


    }
}
