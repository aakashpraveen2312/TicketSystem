using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    [ApiKeyAuthorize]
    public class UserLoginController : Controller
    {
        // GET: UserLogin
        public async Task<ActionResult> Create()
        {
            var outlets = await GetUserGroupComboAsync();
            var project = await GetProjectComboAsync();
            var model = new User
            {
                Outlets = outlets,
                Projects = project
            };

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(User objUser)
        {

            try
            {
                var Regurl = ConfigurationManager.AppSettings["PostPSSLOGIN"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = new
                {
                    L_USERID = objUser.L_USERID,
                    L_USERNAME = objUser.L_USERNAME,
                    L_PASSWORD = objUser.L_PASSWORD,
                    L_ROLE = objUser.L_ROLE,
                    L_EMAILID = objUser.L_EMAILID,
                    L_COMPANYID = Session["CompanyId"],
                    l_DOMAIN= Session["DOMAIN"],
                    l_MOBILENO = objUser.l_MOBILENO,
                    L_SORTORDER = objUser.L_SORTORDER,
                    L_DISABLE = objUser.L_UserDisable ? "Y" : "N"
                };

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(Regurl),
                    Method = HttpMethod.Post,
                    Content = new StringContent(JsonConvert.SerializeObject(content), System.Text.Encoding.UTF8, "application/json")
                };

                request.Headers.Add("X-Version", "1");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                using (var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true })
                using (var client = new HttpClient(handler) { })
                {
                    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {

                        var responseBody = await response.Content.ReadAsStringAsync();


                        var apiResponse = JsonConvert.DeserializeObject<ApiResponseUser>(responseBody);
                        string message = apiResponse.Message;

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
                }
                catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }
            return View(objUser);
        }
        public async Task<List<SelectListItem>> GetUserGroupComboAsync()
        {
            var outletCombo = new List<SelectListItem>();
            string apiurl = ConfigurationManager.AppSettings["USERGROUPGET"];
            string authkey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            string strparams = "cmprecid=" + Session["CompanyId"];
            string url = apiurl + "?" + strparams;

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", authkey);
                    httpClient.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        try
                        {
                            var apiResponse = JsonConvert.DeserializeObject<UserGroupRootObject>(content);

                            if (apiResponse.Status == "Y")
                            {
                                foreach (var item in apiResponse.Data)
                                {
                                    outletCombo.Add(new SelectListItem
                                    {
                                        Value = item.TUG_NAME,
                                        Text = item.TUG_NAME
                                    });
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Error: " + apiResponse.Message);
                            }
                        }
                        catch (JsonException ex)
                        {
                            ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "API call failed: " + response.ReasonPhrase);
                    }
                }
            }

            return outletCombo ?? new List<SelectListItem>();
        }
        public async Task<List<SelectListItem>> GetProjectComboAsync()
        {
            var projectCombo = new List<SelectListItem>();
            string apiurl = ConfigurationManager.AppSettings["PROJECTGET"];
            string authkey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            string strparams = "companyId=" + Session["CompanyId"];
            string url = apiurl + "?" + strparams;

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", authkey);
                    httpClient.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        try
                        {
                            var apiResponse = JsonConvert.DeserializeObject<ProjectMasterRootObject>(content);

                            if (apiResponse.Status == "Y")
                            {
                                foreach (var item in apiResponse.Data)
                                {
                                    projectCombo.Add(new SelectListItem
                                    {
                                        Value = item.P_NAME,
                                        Text = item.P_NAME
                                    });
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Error: " + apiResponse.Message);
                            }
                        }
                        catch (JsonException ex)
                        {
                            ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "API call failed: " + response.ReasonPhrase);
                    }
                }
            }

            return projectCombo ?? new List<SelectListItem>();
        }

        public async Task<ActionResult> List(string searchPharse)
        {
            User objuser = new User();

            string WEBURLGET = ConfigurationManager.AppSettings["GETPSSLOGIN"];
            string Authkey = ConfigurationManager.AppSettings["Authkey"];

            List<User> userList = new List<User>();


            string APIKey = Session["APIKEY"].ToString();


            string strparams = "companyId=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            try
            {



                // Prepare header parameters as per RSGT inputs
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", Authkey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                        var response = await client.GetAsync(finalurl);


                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            //GlobalVariables.ResponseStructure = jsonString;
                            var content = JsonConvert.DeserializeObject<ApiResponseUser>(jsonString);
                            userList = content.Data;

                           
                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                userList = userList
                                    .Where(r => r.L_USERNAME.ToLower().Contains(searchPharse.ToLower()) ||
                                   r.L_USERID.ToString().ToLower().Contains(searchPharse.ToLower())||
                                   r.L_ROLE.ToString().ToLower().Contains(searchPharse.ToLower())||
                                   r.L_EMAILID.ToString().ToLower().Contains(searchPharse.ToLower()))
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
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
            return View(userList);
        }


        public async Task<ActionResult> Edit(int id, string AppUserName,string userid)
        {
            Session["userid"] = userid;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["PSSLOGINgetbyID"];
            string Authkey = ConfigurationManager.AppSettings["Authkey"];

            User user = null;

            // Fetch the user group and project data asynchronously
            var roles = await GetUserGroupComboAsync();
            var projects = await GetProjectComboAsync();

            string APIKey = Session["APIKEY"].ToString();
           
            Session["RECID"] = id;

            string strparams = "Recid=" + id + "&" + "companyId=" + Session["CompanyID"];
            string finalurl = WEBURLGETBYID + "?" + strparams;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (var client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", Authkey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await client.GetAsync(finalurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var content = JsonConvert.DeserializeObject<ApiResponseUserS>(jsonString);
                            user = content.Data;

                            ViewBag.L_PROJECTID = new SelectList(projects, "Value", "Text", user?.L_PROJECTID ?? "");
                            ViewBag.L_ROLE = new SelectList(roles, "Value", "Text", user?.L_ROLE ?? "");

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

            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(User UserEdit)
        {
           
            try
            {

                var WEBURLPUT = ConfigurationManager.AppSettings["PutPSSLOGIN"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();
             
                var content = $@"{{
                   
                    ""L_RECID"": ""{(int)Session["RECID"]}"",
                    ""L_USERNAME"": ""{UserEdit.L_USERNAME}"",                
                    ""L_ROLE"": ""{UserEdit.L_ROLE}"",                  
                    ""L_EMAILID"": ""{UserEdit.L_EMAILID}"",                  
                    ""L_DISABLE"":""{(UserEdit.L_UserDisable ? "Y" : "N")}"",
                    ""L_SORTORDER"":""{UserEdit.L_SORTORDER}"",
                    ""l_MOBILENO"":""{UserEdit.l_MOBILENO}"",
                    ""l_DOMAIN"":""{ Session["DOMAIN"]}"",
                    ""l_USERID"":""{ UserEdit.L_USERID}"",
                    ""L_COMPANYID"":""{Session["CompanyID"]}""
                     }}";
                //""BIN_SPRECID"": ""{ objbins.BIN_SPRECID}"",
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(WEBURLPUT),
                    Method = HttpMethod.Put,
                    Headers =
                        {
                            {"X-Version", "1" },
                            {HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                        },

                    Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
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

                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseUserS>(responseBody);

                    string status = apiResponse.Status;
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

            return View();

        }

        public async Task<ActionResult> Delete(int id)

        {

            Session["PSSLOGINRECID"] = id;
          
            string WEBURLDELETE = ConfigurationManager.AppSettings["DELETEPSSLOGIN"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string strparams = "companyId=" + Session["CompanyID"] + "&RecordId=" + id;
            string finalurl = WEBURLDELETE + "?" + strparams;
            string APIKey = Session["APIKEY"].ToString();
           

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
                            var apiResponse = JsonConvert.DeserializeObject<ApiResponseUser>(responseBody);

                            if (apiResponse.Status == "Y")
                            {

                                string redirectUrl = Url.Action("List", "UserLogin", new { });
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