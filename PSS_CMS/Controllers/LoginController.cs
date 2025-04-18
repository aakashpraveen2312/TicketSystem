using System.Data.SqlClient;
using System.Web.Mvc;
using PSS_CMS.Models;
using System.Web.Security;
using System.Configuration;
using System;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace PSS_CMS.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public async Task<ActionResult> Index()
        {
            
            return View();
        }

        [HttpPost]
       
        public async Task<ActionResult> Index(string L_USERNAME, string L_PASSWORD,string L_DOMAIN, Login model)        
        {
            string CompanyID = "";
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];           
            
            var Logurl = ConfigurationManager.AppSettings["Login"];

            object content = "StrUserid=" + L_USERNAME + "&strPassword=" + L_PASSWORD + "&domain="+ L_DOMAIN;
            string urll = Logurl +"?"+ content;


            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(urll),
                    Method = HttpMethod.Get,
                    Headers = {
                        { "X-Version", "1" }, // HERE IS HOW TO ADD HEADERS,
                        { HttpRequestHeader.Authorization.ToString(), AuthKey },
                        { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" },
                        { HttpRequestHeader.ContentType.ToString(), "application/json" },  //use this content type if you want to send more than one content type
                    },
                };
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };
                HttpClient client = new HttpClient(handler);

                client.DefaultRequestHeaders.Add("Authorization", "Custom " + AuthKey);
                // client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                  var responseTask = client.SendAsync(request).GetAwaiter().GetResult();
                 if (responseTask.IsSuccessStatusCode)
                {
                    var responseContent = responseTask.Content.ReadAsStringAsync().GetAwaiter().GetResult();


                    var Response = JsonConvert.DeserializeObject<APIResponseLogin>(responseContent);

                    string errormessage = Response.Message;
                    string Status = Response.Status;
                    Session["APIKEY"] = Response.APIkey;
                    if (Status == "Y")
                    {
                        var data = Response.Data[0];
                        string role = data.L_ROLE;

                        // Common session assignments
                        Session["DOMAIN"] = data.L_DOMAIN;
                        Session["UserName"] = data.L_USERNAME;
                        Session["UserRole"] = data.L_ROLE;
                        Session["EmailId"] = data.L_EMAILID;
                        Session["UserID"] = data.L_USERID;
                        Session["CompanyID"] = data.L_COMPANYID;
                        CompanyID = data.L_COMPANYID;
                        //ViewBag.Logo = data.C_LOGO;
                       


                        if (role == "User")
                        {
                            
                            await AutoCloseTicket();
                            return RedirectToAction("Ticket_History", "Tickets");
                        }
                        else if (role == "Admin" || role == "SuperAdmin")
                        {
                            await Info(CompanyID);
                            return RedirectToAction("Dashboard", "DashBoard");
                        }
                    }

                    else if (Status == "N")
                    {                      
                        TempData["ErrorMessage"] = " Invalid User name or Password";
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                TempData["ErrorMessage"] =" Invalid User name or Password";
            }
            return View();

        }




        public async Task<ActionResult> Info(string companyid)
        {
            CompanyInfo companyinfo = null;

            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            //string APIKey = Session["APIKEY"].ToString();
            string Weburl = ConfigurationManager.AppSettings["COMPANYINFOGET"];
            string Url = Weburl + "?CmpRECID=" + companyid;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    using (HttpClient client = new HttpClient(handler))
                    {
                        //client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var response = await client.GetAsync(Url);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<ApiResponseInfo>(jsonString);
                            ViewBag.Logo = rootObjects.Data[0].C_LOGO;
                            TempData["Logo"] = rootObjects.Data[0].C_LOGO;
                            
                            return View();
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
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }
            return View();
        }


        public ActionResult Logout()
        {
            // Clear the session
            Session.Abandon();
            Session.Clear();
            FormsAuthentication.SignOut();

            // Set the cache control to prevent browser from caching the page
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            return RedirectToAction("Index", "Login");
        }

        public async Task<ActionResult> AutoCloseTicket()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["TICKETAUTOCLOSE"];
            string strparams = "userid=" + Session["UserID"]+"&cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
          

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await client.GetAsync(finalurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var status = JsonConvert.DeserializeObject<APIResponseLogin>(jsonString);

                            if (status.Status=="Y")
                            {
                                return View();
                            }
                            else if (status.Status == "N")
                            {
                                return View();
                            }
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
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }

            return View();
        }
    }
}
