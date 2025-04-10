using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    [ApiKeyAuthorize]
    public class ChangePasswordController : Controller
    {
        // GET: ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ChangePassword(Changepassword changepassword)
        {
            try
            {
                var ChnagepasswordURL = ConfigurationManager.AppSettings["CHANGEPASSWORD"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""userId"": ""{Session["UserID"]}"",           
            ""newpassword"": ""{changepassword.U_NewPassword}"",                                 
            ""compRecordID"": ""{ Session["CompanyID"]}""                              
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(ChnagepasswordURL),
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
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                // Send the request and await the response
                var response = await client.SendAsync(request);
                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<UserGroupObjects>(responseBody);

                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { success = true, message = apiResponse.Message });
                    }
                    else
                    {
                        return Json(new { success = false, message = apiResponse.Message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Error: Something went wrong." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception: " + ex.Message });
            }
        }
    }
}