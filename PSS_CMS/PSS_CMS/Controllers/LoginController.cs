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

namespace PSS_CMS.Controllers
{
    public class LoginController : Controller
    {
        // Connection string to your database
        //private string connectionString = "Data Source=SQL5111.site4now.net;Initial Catalog=db_9cba9c_bcloud;User Id=db_9cba9c_bcloud_admin; Password =Bcloud@123;Integrated Security=False";

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string L_USERNAME, string L_PASSWORD, Login model)
        
        {

            var Logurl = ConfigurationManager.AppSettings["Login"];
            //string AuthKey = ConfigurationManager.AppSettings["AuthKey"];

            //object content = $@"{{""StrUserid"": ""{L_USERNAME}""&""strPassword"":""{L_PASSWORD}""}}";
            //object content = $@"{{""StrUserid"": ""{L_USERNAME}""&""strPassword"":""{L_PASSWORD}""}}";
            object content = $"StrUserid={Uri.EscapeDataString(L_USERNAME)}&strPassword={Uri.EscapeDataString(L_PASSWORD)}";

            string urll = Logurl +"?"+ content;


            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(urll),
                    Method = HttpMethod.Get,
                    Headers = {
                        { "X-Version", "1" }, // HERE IS HOW TO ADD HEADERS,
                        //{ HttpRequestHeader.Authorization.ToString(), AuthKey },
                        { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" },
                        { HttpRequestHeader.ContentType.ToString(), "application/json" },  //use this content type if you want to send more than one content type
                    },
                };
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };
                HttpClient client = new HttpClient(handler);

                //client.DefaultRequestHeaders.Add("Authorization", "Custom " + AuthKey);
                // client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                var responseTask = client.SendAsync(request).GetAwaiter().GetResult();
                if (responseTask.IsSuccessStatusCode)
                {
                    var responseContent = responseTask.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    
                    var Response = JsonConvert.DeserializeObject<APIResponseLogin>(responseContent);

                    string errormessage = Response.Message;
                    string Status = Response.Status;
                  

                    // Based on this input the page will be Redirect





                    //if (Status == "Y")
                    //{
                    //    string Username = Response.Data[0].L_USERNAME;
                    //    string UserRole = Response.Data[0].L_ROLE;
                    //    string EmailID = Response.Data[0].L_EMAILID;

                    //    Session["UserName"] = Username;
                    //    Session["UserRole"] = UserRole;
                    //    Session["EmailId"] = EmailID;
                    //    //return View(model);
                    //    return RedirectToAction("Index","Home");
                    //}
                    //else if(Status == "N")
                    //{
                    //    return View();
                    //}
                    if (Status == "Y")
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

                        // Set success message in ViewBag or TempData
                        TempData["SuccessMessage"] = errormessage;

                        //return RedirectToAction("Index", "Home");
                    }
                    else if (Status == "N")
                    {
                        // Set error message in ViewBag or TempData
                        TempData["ErrorMessage"] = errormessage;

                        //return View();
                    }


                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
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


            return RedirectToAction("Index", "Home");
        }
    }
}
