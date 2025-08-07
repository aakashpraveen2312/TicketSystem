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
       
        public async Task<ActionResult> Index(string U_EMAILID, string U_PASSWORD,string U_DOMAIN, Login model)        
        {
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];           
            var LogInurl = ConfigurationManager.AppSettings["LOGIN"];
            object content = "StrUserid=" + U_EMAILID + "&strPassword=" + U_PASSWORD + "&domain="+ U_DOMAIN;
            string urll = LogInurl + "?"+ content;

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
                    string Warning = Response.expiryWarning;
                    string Status = Response.Status;
                    
                    Session["MaterialConsumptionFlag"] = Response.MaterialConsumption?.Replace(" ", "").Trim();

                    Session["APIKEY"] = Response.APIkey;
                    if (Status == "Y")
                    {
                        TempData["SuccessMessage"] = errormessage;
                        var data = Response.Data[0];
                        string role = data.U_RCODE;
                        string U_USERMANAGER = data.U_USERMANAGER;
                        // Common session assignments
                        Session["DOMAIN"] = data.U_DOMAIN;
                        Session["UserName"] = data.U_USERNAME;
                        Session["UserRole"] = data.U_RCODE;
                        Session["EmailId"] = data.U_EMAILID;
                        Session["UserRECID"] = data.U_RECID;
                        Session["CompanyID"] = data.U_CRECID;

                        int CompanyID = data.U_CRECID;
                        if (role == "User" && U_USERMANAGER == "Y")
                        {
                            return RedirectToAction("Dashboard", "UserManager");
                        }

                        if (role == "User")
                        {
                            await AutoCloseTicket();
                            //return RedirectToAction("Ticket_History", "Tickets");
                            return RedirectToAction("UserDashboardCount", "UserDashboard");
                        }
                        if (role == "Admin" || role == "Manager")
                        {
                            await Info(CompanyID);
                           return RedirectToAction("Dashboard", "DashBoard");
                            //return RedirectToAction("RecentTicket", "RecentTickets");
                        }
                        if (role == "SA")
                        {
                            await Info(CompanyID);
                            return RedirectToAction("SuperAdminCountDashboard", "Dashboard");
                        }
                        if (role == "Accountant")
                        {
                            await Info(CompanyID);
                            return RedirectToAction("ContractDashboard", "Contract");
                        }
                        if (role == "HelpDesk")
                        {                 
                            return RedirectToAction("HDDashboard", "HelpDesk");
                            //return RedirectToAction("Ticket_History", "HelpDesk");
                        }
                    }

                    else
                    {                      
                        TempData["ErrorMessage"] = Warning;
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


        public async Task<ActionResult> IndexRole()
        {

            return View();
        }

        [HttpPost]

        public async Task<ActionResult> IndexRole(string U_EMAILID, string U_PASSWORD, string U_DOMAIN, Login model)
        {
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            var LogInurl = ConfigurationManager.AppSettings["LOGIN"];
            object content = "StrUserid=" + U_EMAILID + "&strPassword=" + U_PASSWORD + "&domain=" + U_DOMAIN;
            string urll = LogInurl + "?" + content;

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

                    string errormessage = Response.expiryWarning;
                    string Status = Response.Status;
                    Session["MaterialConsumptionFlag"] = Response.MaterialConsumption?.Replace(" ", "").Trim();

                    Session["APIKEY"] = Response.APIkey;
                    if (Status == "Y")
                    {
                        var data = Response.Data[0];
                        string role = data.U_RCODE;
                        string U_USERMANAGER = data.U_USERMANAGER;
                        // Common session assignments
                        Session["DOMAIN"] = data.U_DOMAIN;
                        Session["UserName"] = data.U_USERNAME;
                        Session["UserRole"] = data.U_RCODE;
                        Session["EmailId"] = data.U_EMAILID;
                        Session["UserRECID"] = data.U_RECID;
                        Session["CompanyID"] = data.U_CRECID;

                        int CompanyID = data.U_CRECID;
                        if (role == "User" && U_USERMANAGER == "Y")
                        {
                            return RedirectToAction("Dashboard", "UserManager");
                        }

                        if (role == "User")
                        {
                            await AutoCloseTicket();
                            //return RedirectToAction("Ticket_History", "Tickets");
                            return RedirectToAction("UserDashboardCount", "UserDashboard");
                        }
                        if (role == "Admin" || role == "Manager")
                        {
                            await Info(CompanyID);
                            return RedirectToAction("Dashboard", "DashBoard");
                            //return RedirectToAction("RecentTicket", "RecentTickets");
                        }
                        if (role == "SA")
                        {
                            await Info(CompanyID);
                            return RedirectToAction("SuperAdminCountDashboard", "Dashboard");
                        }
                        if (role == "Accountant")
                        {
                            await Info(CompanyID);
                            return RedirectToAction("ContractDashboard", "Contract");
                        }
                        if (role == "HelpDesk")
                        {
                            return RedirectToAction("HDDashboard", "HelpDesk");
                            //return RedirectToAction("Ticket_History", "HelpDesk");
                        }
                    }

                    else
                    {
                        TempData["ErrorMessage"] = " Invalid User name or Password";
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                TempData["ErrorMessage"] = " Invalid User name or Password";
            }
            return View();

        }


        public async Task<ActionResult> Info(int? companyid)
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
                            Session["Logo"] = rootObjects.Data[0].C_LOGO;

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
            string strparams = "userid=" + Session["UserRECID"] +"&cmprecid=" + Session["CompanyID"];
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
