using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
            await ComboLocationSelection();//we cannot call the combo gteby id here we need to pass the model class here its showing error cauz it already have post method
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
                    u_RCODE = Session["R_CODE"],
                    u_SORTORDER = objUser.U_SORTORDER,
                    u_EMAILID = objUser.U_EMAILID,
                    u_CRECID = Session["CompanyID"],
                    u_USERCODE = objUser.U_USERCODE,
                    u_MOBILENO = objUser.U_MOBILENO,
                    u_DOMAIN = Session["DOMAIN"],
                    u_LOCATION = objUser.SelectedRole,
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


        public async Task<ActionResult> List(string searchPharse, string R_CODE, string Role)
        {
            if (R_CODE!=null)
            {
                Session["R_CODE"] = R_CODE;
            }
            if (Role != null)
            {
                Session["UNAME"] = Role;
            }

            User objuser = new User();

            string WEBURLGET = ConfigurationManager.AppSettings["GETUSERSBASEDONROLE"];
            string Authkey = ConfigurationManager.AppSettings["Authkey"];

            List<User> userList = new List<User>();


            string APIKey = Session["APIKEY"].ToString();


            string strparams = "role=" + Session["R_CODE"] + "&companyId=" + Session["CompanyID"];
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

        public async Task<ActionResult> Edit(int id,string Username,string EditName)
        {
            Session["Names"] = Username;
            Session["EditName"] = EditName;
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
            await ComboLocationSelectionGetbyID(user?.U_RCODE);
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
                    ""u_LOCATION"":""{ UserEdit.U_LOCATION}"",
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

        public async Task<ActionResult> ComboLocationSelection()

        {
            List<SelectListItem> Location = new List<SelectListItem>();

            string connectionString = ConfigurationManager.ConnectionStrings["Mystring"].ConnectionString;
            string cmpRecId = Session["CompanyID"].ToString();

            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                string cmd1 = "SELECT SP_CRECID, SP_RECID, SP_Code, SP_Name, SP_Sortorder FROM IM_StoragePoint WHERE SP_CRECID = @cmpRecId";

                SqlCommand sqlCdm = new SqlCommand(cmd1, sqlcon);
                sqlCdm.Parameters.AddWithValue("@cmpRecId", cmpRecId);

                SqlDataReader sqlread = sqlCdm.ExecuteReader();
                while (sqlread.Read())
                {
                    Location.Add(new SelectListItem
                    {
                        Value = sqlread["SP_RECID"].ToString(),
                        Text = sqlread["SP_Name"].ToString()
                    });
                }
                sqlcon.Close();
            }

            ViewBag.Location = Location;
            return View();
        }

        public async Task<ActionResult> ComboLocationSelectionGetbyID(string selectedRoleCode)
        {
            List<SelectListItem> Location = new List<SelectListItem>();

            string connectionString = ConfigurationManager.ConnectionStrings["Mystring"].ConnectionString;
            string cmpRecId = Session["CompanyID"].ToString();

            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                string query = "SELECT SP_CRECID, SP_RECID, SP_Code, SP_Name, SP_Sortorder FROM IM_StoragePoint WHERE SP_CRECID = @cmpRecId";

                using (SqlCommand cmd = new SqlCommand(query, sqlcon))
                {
                    cmd.Parameters.AddWithValue("@cmpRecId", cmpRecId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Location.Add(new SelectListItem
                            {
                                Value = reader["SP_RECID"].ToString(),
                                Text = reader["SP_Name"].ToString(),
                                Selected = (reader["SP_RECID"].ToString() == selectedRoleCode)
                            });
                        }
                    }
                }
            }

            ViewBag.Location = Location;
            return View(); 
        }


    }
}