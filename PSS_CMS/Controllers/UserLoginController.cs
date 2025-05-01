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
            await ComboRoleSelection();//we cannot call the combo gteby id here we need to pass the model class here its showing error cauz it already have post method
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(User objUser)
        {
            
                try
            {
                var Regurl = ConfigurationManager.AppSettings["POSTUSERS"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = new
                {
                    u_USERNAME = objUser.U_USERNAME,
                    u_PASSWORD = objUser.U_PASSWORD,
                    u_RCODE = objUser.SelectedRole,
                    u_SORTORDER = objUser.U_SORTORDER,
                    u_EMAILID = objUser.U_EMAILID,
                    u_CRECID = Session["CompanyID"],
                    u_USERCODE = objUser.U_USERCODE,
                    u_MOBILENO = objUser.U_MOBILENO,
                    u_DOMAIN = Session["DOMAIN"],
                    u_DISABLE = objUser.U_UserDisable ? "Y" : "N"
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


        public async Task<ActionResult> List(string searchPharse)
        {
            User objuser = new User();

            string WEBURLGET = ConfigurationManager.AppSettings["GETUSERS"];
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


                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                userList = userList
                                    .Where(r => r.U_USERNAME.ToLower().Contains(searchPharse.ToLower()) ||
                                   r.U_USERCODE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.R_CODE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.U_EMAILID.ToString().ToLower().Contains(searchPharse.ToLower()))
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

        public async Task<ActionResult> Edit(int id,string Username)
        {
            Session["Names"] = Username;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["GETBYIDUSERS"];
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
            await ComboRoleSelectionGetbyID(user?.U_RCODE);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(User UserEdit)
        {
          
            try
            {

                var WEBURLPUT = ConfigurationManager.AppSettings["PUTUSERS"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{

                    ""u_RECID"": ""{(int)Session["RECID"]}"",
                    ""u_USERNAME"": ""{UserEdit.U_USERNAME}"",                
                    ""u_RCODE"": ""{UserEdit.U_RCODE}"",                  
                    ""u_SORTORDER"": ""{UserEdit.U_SORTORDER}"",                  
                    ""u_DISABLE"":""{(UserEdit.U_UserDisable ? "Y" : "N")}"",
                    ""u_EMAILID"":""{UserEdit.U_EMAILID}"",
                    ""u_CRECID"":""{Session["CompanyID"]}"",
                    ""u_USERCODE"":""{UserEdit.U_USERCODE }"",
                    ""u_MOBILENO"":""{ UserEdit.U_MOBILENO}"",
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

            Session["PSSLOGINRECID"] = id;

            string WEBURLDELETE = ConfigurationManager.AppSettings["DELETEUSERS"];
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

        //[HttpPost]
        //public async Task<ActionResult> CheckedValue(List<string> selectedItems)
        //{
        //    try
        //    {

        //        var AdmindeleigatePostURL = ConfigurationManager.AppSettings["ADMINCHECKBOXDELIGATE"];
        //        string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
        //        string APIKey = Session["APIKEY"].ToString();

        //        if (selectedItems != null)
        //        {


        //            var selectedCategoryIds = selectedItems.Distinct().ToArray(); // Remove duplicates if necessary
        //            string formattedOutput = string.Join(",", selectedCategoryIds);
        //            Session["SELECTEDID"] = formattedOutput;
        //        }
        //        else
        //        {
        //            //var selectedCategoryIds = selectedItems.Distinct().ToArray();
        //            string formattedOutput = "";
        //            Session["SELECTEDID"] = formattedOutput;
        //        }


        //        var content = $@"{{
        //            ""l_COMPANYID"": ""{Session["CompanyID"]}"",                
        //            ""l_AdminDeligate"":""{Session["SELECTEDID"]}""                

        //                }}";




        //        var request = new HttpRequestMessage
        //        {
        //            RequestUri = new Uri(AdmindeleigatePostURL),
        //            Method = HttpMethod.Post,
        //            Headers =
        //                {
        //                    {"X-Version", "1" },
        //                    {HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
        //                },

        //            Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
        //        };

        //        var handler = new HttpClientHandler
        //        {
        //            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true

        //        };
        //        var client = new HttpClient(handler)
        //        {
        //            Timeout = TimeSpan.FromSeconds(120)
        //        };





        //        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //        client.DefaultRequestHeaders.Add("Authorization", AuthKey);

        //        var response = await client.SendAsync(request);




        //        if (response.IsSuccessStatusCode)

        //        {

        //            string responseBody = await response.Content.ReadAsStringAsync();

        //            var apiResponse = JsonConvert.DeserializeObject<ApiResponseUserS>(responseBody);
        //            string message = apiResponse.Message;

        //            if (apiResponse.Status == "Y")
        //            {
        //                return Json(new { success = true, message = apiResponse.Message });
        //            }
        //            else if (apiResponse.Status == "U" || apiResponse.Status == "N")
        //            {
        //                return Json(new { success = false, message = apiResponse.Message });
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = "An unexpected status was returned." });
        //            }
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "Error: " + response.ReasonPhrase });
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
        //    }

        //    return View();
        //}

        public async Task<ActionResult> ComboRoleSelection()

        {
            List<SelectListItem> Roles = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["USERSCREATECOMBO"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "CompanyRecID=" + Session["CompanyID"];
            string url = webUrlGet + "?" + strparams;
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
                            var rootObjects = JsonConvert.DeserializeObject<ApiResponseUserObjects>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Roles = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.R_CODE, // or the appropriate value field
                                    Text = t.R_NAME,
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            // Assuming you are passing ticketTypes to the view
            ViewBag.Roles = Roles;

            return View();
        }


        public async Task ComboRoleSelectionGetbyID(string selectedRoleCode)
        {
            List<SelectListItem> Roles = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["USERSCREATECOMBO"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "CompanyRecID=" + Session["CompanyID"];
            string url = webUrlGet + "?" + strparams;

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
                            var rootObjects = JsonConvert.DeserializeObject<ApiResponseUserObjects>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Roles = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.R_CODE,
                                    Text = t.R_NAME,
                                    Selected = (t.R_CODE == selectedRoleCode) // ✅ compare with passed selectedRoleCode
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            ViewBag.Roles = Roles;
        }

    }
}