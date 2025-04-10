using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    [ApiKeyAuthorize]
    public class UserGroupController : Controller
    {
        // GET: UserGroup
        public async Task<ActionResult> List(string searchPharse)
        {
           Usergroup objusergroup = new Usergroup();

            string Weburl = ConfigurationManager.AppSettings["USERGROUPGET"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Usergroup> usergrouplist = new List<Usergroup>();
            
            string strparams ="cmprecid=" + Session["CompanyID"];
            string url = Weburl + "?" + strparams;

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
                        var response = await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<UserGroupRootObject>(jsonString);
                            usergrouplist = rootObjects.Data;

                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                usergrouplist = usergrouplist
                                    .Where(r => r.TUG_CODE.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.TUG_NAME.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.TUG_ROLEDESCRIPTION.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.TUG_SORTORDER.ToString().Contains(searchPharse.ToLower()))
                                    .ToList();
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
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }
            return View(usergrouplist);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(Usergroup usergroup)
        {
           
                try
                {
                    var UsergroupPostURL = ConfigurationManager.AppSettings["USERGROUPGETPOST"];
                    string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                    string APIKey = Session["APIKEY"].ToString();

                    var content = $@"{{           
            ""tuG_CODE"": ""{usergroup.TUG_CODE}"",           
            ""tuG_NAME"": ""{ usergroup.TUG_NAME}"",          
            ""tuG_ROLEDESCRIPTION"": ""{usergroup.TUG_ROLEDESCRIPTION}"",        
            ""tuG_SORTORDER"": ""{usergroup.TUG_SORTORDER}"",        
            ""tuG_DISABLE"": ""{(usergroup.IsDisabled ? "Y" : "N")}"",        
            ""tuG_CRECID"": ""{Session["CompanyID"]}""           
        }}";

                    // Create the HTTP request
                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(UsergroupPostURL),
                        Method = HttpMethod.Post,
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


                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<UserGroupRootObject>(responseBody);

                        if (apiResponse.Status == "Y")
                        {
                            return Json(new { success = true, message = apiResponse.Message });
                        }
                        else if (apiResponse.Status == "U" || apiResponse.Status == "N")
                        {
                            return Json(new { success = false, message = apiResponse.Message });
                        }
                        else
                        {
                            return Json(new { success = false, message = "An unexpected status was returned." });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error: " + response.ReasonPhrase });
                    }


                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
                }
        
            return View(usergroup);
        }
        public async Task<ActionResult> Edit(int ? Recid)
        {           
            Session["UsergroupRecid"] = Recid;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["USERGROUPGETBYID"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            Usergroup usergroup = null;

            string strparams = "recID=" + Recid + "&cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGETBYID + "?" + strparams;

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
                            var content = JsonConvert.DeserializeObject<UserGroupObjects>(jsonString);                        
                                usergroup = content.Data;
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

            return View(usergroup);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(Usergroup usergroup)
        {
            try
            {
                var UsergroupUpdateURL = ConfigurationManager.AppSettings["USERGROUPGETPUT"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""tuG_RECID"": ""{Session["UsergroupRecid"]}"",           
            ""tuG_CODE"": ""{usergroup.TUG_CODE}"",           
            ""tuG_NAME"": ""{usergroup.TUG_NAME}"",
            ""tuG_ROLEDESCRIPTION"": ""{usergroup.TUG_ROLEDESCRIPTION}"",
            ""tuG_SORTORDER"": ""{usergroup.TUG_SORTORDER}"",
            ""tuG_DISABLE"": ""{(usergroup.IsDisabled ? "Y" : "N")}"",                              
            ""tuG_CRECID"": ""{ Session["CompanyID"]}""                              
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(UsergroupUpdateURL),
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
        public async Task<ActionResult> Delete(int? Recid)
        {
            string UsergroupDeleteUrl = ConfigurationManager.AppSettings["USERGROUPGETDELETE"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            string strparams = "RECID=" + Recid + "&cmprecid=" + Session["CompanyID"];
            string finalurl = UsergroupDeleteUrl + "?" + strparams;

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


                        var request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Delete,
                            RequestUri = new Uri(finalurl)
                        };

                        var response = await client.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();
                            var apiResponse = JsonConvert.DeserializeObject<UserGroupObjects>(responseBody);

                            if (apiResponse.Status == "Y")
                            {

                                string redirectUrl = Url.Action("List", "UserGroup", new { });
                                return Json(new { status = "success", message = apiResponse.Message, redirectUrl = redirectUrl });
                            }
                            else if (apiResponse.Status == "U")
                            {
                                return Json(new { status = "error", message = apiResponse.Message });
                            }
                            else if (apiResponse.Status == "N")
                            {
                                return Json(new { status = "error", message = apiResponse.Message });
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Failed to delete: {response.StatusCode} - {response.ReasonPhrase}");
                        }
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Request error occurred: {httpEx.Message}");
            }
            catch (TaskCanceledException tcEx)
            {
                Console.WriteLine($"Request timed out: {tcEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
            return View();
        }
    } 
}