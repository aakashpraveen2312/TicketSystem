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
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
       
        public async Task<ActionResult> Index(string L_USERNAME, string L_PASSWORD,string L_DOMAIN, Login model)        
        {
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
                     string Role = Response.Data[0].L_ROLE;
                  
                    if (Status == "Y"&& Role=="User")
                     {
                        string Username = Response.Data[0].L_USERNAME;
                        string UserRole = Response.Data[0].L_ROLE;
                        string EmailID = Response.Data[0].L_EMAILID;

                          string UserID = Response.Data[0].L_USERID;
                    string CompanyID = Response.Data[0].L_COMPANYID;
                    string ProjectID = Response.Data[0].L_PROJECTID;

                        Session["UserName"] = Username;
                        Session["UserRole"] = UserRole;
                        Session["EmailId"] = EmailID;
                        Session["UserID"] = UserID;
                        Session["CompanyID"] = CompanyID;
                        Session["ProjectID"] = ProjectID;
                        await AutoCloseTicket();
                        return RedirectToAction("Ticket_History", "Tickets");

                       
                    }
                    else if (Status == "Y" && Role == "Admin")
                    {
                        string Username = Response.Data[0].L_USERNAME;
                        string UserRole = Response.Data[0].L_ROLE;
                        string EmailID = Response.Data[0].L_EMAILID;

                        string UserID = Response.Data[0].L_USERID;
                        string CompanyID = Response.Data[0].L_COMPANYID;
                        string ProjectID = Response.Data[0].L_PROJECTID;

                        Session["UserName"] = Username;
                        Session["UserRole"] = UserRole;
                        Session["EmailId"] = EmailID;
                        Session["UserID"] = UserID;
                        Session["CompanyID"] = CompanyID;
                        Session["ProjectID"] = ProjectID;

                        return RedirectToAction("Dashboard", "DashBoard");

                    }
                    else if (Status == "Y" && Role == "SuperAdmin")
                    {
                        string Username = Response.Data[0].L_USERNAME;
                        string UserRole = Response.Data[0].L_ROLE;
                        string EmailID = Response.Data[0].L_EMAILID;

                        string UserID = Response.Data[0].L_USERID;
                        string CompanyID = Response.Data[0].L_COMPANYID;
                        string ProjectID = Response.Data[0].L_PROJECTID;

                        Session["UserName"] = Username;
                        Session["UserRole"] = UserRole;
                        Session["EmailId"] = EmailID;
                        Session["UserID"] = UserID;
                        Session["CompanyID"] = CompanyID;
                        Session["ProjectID"] = ProjectID;

                        return RedirectToAction("Dashboard", "DashBoard");

                    }
                    else if (Status == "N")
                    {                      
                        TempData["ErrorMessage"] = errormessage;
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
