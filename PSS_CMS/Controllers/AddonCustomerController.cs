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
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    [ApiKeyAuthorize]
    public class AddonCustomerController : Controller
    {
        // GET: AddonCustomer
        public async Task<ActionResult> AddOnCustomerList(string searchPharse)
        {

            User objuser = new User();

            int SerialNo = objuser.SerialNumber;

            if (SerialNo == 0)
            {
                SerialNo = 1; // Initialize to 1 if it's 0
            }

            string WEBURLGET = ConfigurationManager.AppSettings["GETUSERSADDONCUSTOMER"];
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
                            var content = JsonConvert.DeserializeObject<ApiResponseUserObjects>(jsonString);
                            userList = content.Data;
                            if (userList.Count > 0)
                            {
                                // Assign serial numbers
                                for (int i = 0; i < userList.Count; i++)
                                {
                                    userList[i].SerialNumber = i + 1;
                                }
                            }

                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                userList = userList
                                    .Where(r => r.U_USERNAME.ToLower().Contains(searchPharse.ToLower()) ||
                                   r.U_USERCODE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.U_EMAILID.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.U_RCODE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.U_SORTORDER.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.U_MOBILENO.ToString().ToLower().Contains(searchPharse.ToLower()))
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

        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(User objUser)
        {

            try
            {
                var Regurl = ConfigurationManager.AppSettings["POSTADDONCUSTOMER"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = new
                {
                    u_USERNAME = objUser.U_USERNAME ?? "",
                    u_RCODE = "User",
                    u_SORTORDER = objUser.U_SORTORDER ?? "0",
                    U_LOCATIONTYPERECID = '0',
                    u_EMAILID = objUser.U_EMAILID ?? "",
                    u_CRECID = Session["CompanyID"],
                    u_USERCODE = objUser.U_USERCODE ?? "",
                    u_MOBILENO = objUser.U_MOBILENO ?? "",
                    u_ADDRESS = objUser.U_ADDRESS ?? "",
                    u_DOMAIN = Session["DOMAIN"],
                    u_LOCATION = '0',
                    u_DISABLE = objUser.U_UserDisable ? "Y" : "N",
                    u_UserManager = objUser.U_UserManager ? "Y" : "N"
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


                        var apiResponse = JsonConvert.DeserializeObject<ApiResponseUserObject>(responseBody);
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

        public async Task<ActionResult> UserAdminMappingList(string Role, string id, string Name, string searchPharse)
        {
            Session["Name"] = Name;
            Session["RECID"] = id;
            if (Role == "User")
            {
                Role = "Admin";
            }
            else
            {
                Role = "User";
            }

            Useradminmap objuseradminmap = new Useradminmap();

            string Weburl = ConfigurationManager.AppSettings["USERADMINGET"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Useradminmap> useradminlist = new List<Useradminmap>();

            string strparams = "companyId=" + Session["CompanyID"] + "&role=" + Role + "&userid=" + id;
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
                            var rootObjects = JsonConvert.DeserializeObject<UserAdminRootObject>(jsonString);
                            useradminlist = rootObjects.Data;

                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                useradminlist = useradminlist
                                    .Where(r => r.U_USERCODE.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.U_USERNAME.ToString().Contains(searchPharse.ToLower()) ||
                                                r.U_RCODE.ToString().Contains(searchPharse.ToLower()))
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
            return View(useradminlist);
        }

        public async Task<ActionResult> UserProductMappingList(string id, string Name, string searchPharse, string Productmapppingname)
        {

            Session["Name"] = Name;
            Session["RECID"] = id;
            Session["Productmapppingname"] = Productmapppingname;
            Useradminmap objuseradminprojectmap = new Useradminmap();

            string Weburl = ConfigurationManager.AppSettings["COMBOFORPRODUCTANDLISTVIEW"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Useradminmap> useradminprojectlist = new List<Useradminmap>();

            string strparams = "companyId=" + Session["CompanyID"] + "&UserID=" + id;
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
                            var rootObjects = JsonConvert.DeserializeObject<UserAdminRootObject>(jsonString);
                            useradminprojectlist = rootObjects.Data;

                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                useradminprojectlist = useradminprojectlist
                                    .Where(r => r.P_NAME.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.P_SORTORDER.ToString().Contains(searchPharse.ToLower()))
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
            return View(useradminprojectlist);
        }

        [HttpPost]
        public async Task<ActionResult> CheckedValue(List<string> selectedItems, Useradminmap useradminmap)
        {
            try
            {

                var UserAdminPostURL = ConfigurationManager.AppSettings["USERADMINPOST"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                if (selectedItems != null)
                {


                    var selectedCategoryIds = selectedItems.Distinct().ToArray(); // Remove duplicates if necessary
                    string formattedOutput = string.Join(",", selectedCategoryIds);
                    Session["SELECTEDID"] = formattedOutput;
                }
                else
                {
                    //var selectedCategoryIds = selectedItems.Distinct().ToArray();
                    string formattedOutput = "";
                    Session["SELECTEDID"] = formattedOutput;
                }


                var content = $@"{{
                    ""uH_CRECID"": ""{Session["CompanyID"]}"",
                    ""uH_USERRECID"": ""{ Session["RECID"]}"",                  
                    ""uH_HEADRECID"":""{Session["SELECTEDID"]}""                
                   
                        }}";




                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(UserAdminPostURL),
                    Method = HttpMethod.Post,
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
                    Timeout = TimeSpan.FromSeconds(120)
                };





                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                var response = await client.SendAsync(request);




                if (response.IsSuccessStatusCode)

                {

                    string responseBody = await response.Content.ReadAsStringAsync();

                    var apiResponse = JsonConvert.DeserializeObject<UserAdminObjects>(responseBody);
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
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CheckedValueProject(List<int> selectedItems, Useradminmap useradminmap)
        {
            try
            {

                var UserAdminProjectPostURL = ConfigurationManager.AppSettings["USERADMINPRODUCTPOST"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                if (selectedItems != null)
                {


                    var selectedCategoryIds = selectedItems.Distinct().ToArray(); // Remove duplicates if necessary
                    string formattedOutput = string.Join(",", selectedCategoryIds);
                    Session["SELECTEDPROJECTID"] = formattedOutput;
                }
                else
                {
                    //var selectedCategoryIds = selectedItems.Distinct().ToArray();
                    string formattedOutput = "";
                    Session["SELECTEDPROJECTID"] = formattedOutput;
                }


                var content = $@"{{
                    ""pT_CRECID"": ""{Session["CompanyID"]}"",
                    ""pT_URECID"": ""{ Session["RECID"]}"",                  
                    ""pT_PRECID"":""{Session["SELECTEDPROJECTID"]}"",              
                    ""pT_SORTORDER"":""{1}"",                
                    ""pT_DISABLE"":""{"Y"}""                
                   
                        }}";




                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(UserAdminProjectPostURL),
                    Method = HttpMethod.Post,
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
                    Timeout = TimeSpan.FromSeconds(120)
                };





                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                var response = await client.SendAsync(request);




                if (response.IsSuccessStatusCode)

                {

                    string responseBody = await response.Content.ReadAsStringAsync();

                    var apiResponse = JsonConvert.DeserializeObject<UserAdminObjects>(responseBody);
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
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return View();
        }


        public async Task<ActionResult> Edit(int id, string Username, string EditName)
        {
            Session["Names"] = Username;
            Session["EditName"] = EditName;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["GETBYIDADDONCUSTOMER"];
            string Authkey = ConfigurationManager.AppSettings["Authkey"];

            User user = null;

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
                            var content = JsonConvert.DeserializeObject<ApiResponseUserObject>(jsonString);

                            user = content.Data;
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
        public async Task<ActionResult> Edit(User UserEdit)
        {

            try
            {

                var WEBURLPUT = ConfigurationManager.AppSettings["PUTADDONCUSTOMER"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{

                    ""u_RECID"": ""{(int)Session["RECID"]}"",
                    ""u_USERNAME"": ""{UserEdit.U_USERNAME}"",                
                    ""u_RCODE"": ""{"User"}"",                  
                    ""u_SORTORDER"": ""{UserEdit.U_SORTORDER}"",                  
                    ""u_DISABLE"":""{(UserEdit.U_UserDisable ? "Y" : "N")}"",
                    ""u_EMAILID"":""{UserEdit.U_EMAILID}"",
                    ""u_CRECID"":""{Session["CompanyID"]}"",
                    ""u_USERCODE"":""{UserEdit.U_USERCODE }"",
                    ""u_MOBILENO"":""{ UserEdit.U_MOBILENO}"",
                    ""u_ADDRESS"":""{ UserEdit.U_ADDRESS}"",
                    ""u_LOCATION"":""{0}"",
                    ""u_LOCATIONTYPERECID"":""{0}"",
                    ""u_UserManager"":""{ (UserEdit.U_UserManager ? "Y" : "N")}"",
                    ""u_DOMAIN"":""{Session["DOMAIN"]}""
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

                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseUserObject>(responseBody);

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

            string WEBURLDELETE = ConfigurationManager.AppSettings["DELETEADDONCUSTOMER"];
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
                            var apiResponse = JsonConvert.DeserializeObject<ApiResponseUserObject>(responseBody);

                            if (apiResponse.Status == "Y")
                            {

                                string redirectUrl = Url.Action("AddOnCustomerList", "AddonCustomer", new { });
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