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
using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    [ApiKeyAuthorize]
    public class UseradminmappingController : Controller
    {
        // GET: Useradminmapping
        public async Task<ActionResult> List(string Role,string USERID,string Name,string searchPharse)
        {
            Session["Name"] = Name;
            Session["USERID"] = USERID;
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

            string strparams = "companyId=" + Session["CompanyID"] + "&role=" + Role;
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

                            //if (!string.IsNullOrEmpty(searchPharse))
                            //{
                            //    projectmasterlist = projectmasterlist
                            //        .Where(r => r.P_NAME.ToLower().Contains(searchPharse.ToLower()) ||
                            //                    r.P_SORTORDER.ToString().Contains(searchPharse.ToLower()))
                            //        .ToList();
                            //}

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
                    ""uA_CRECID"": ""{Session["CompanyID"]}"",
                    ""uA_USERID"": ""{ Session["USERID"]}"",                  
                    ""uA_ADMINID"":""{Session["SELECTEDID"]}""                
                   
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


        public async Task<ActionResult> ListProject(string USERID, string Name,string searchPharse)
        {

            Session["Name"] = Name;
            Session["USERID"] = USERID;
            Useradminmap objuseradminprojectmap = new Useradminmap();

            string Weburl = ConfigurationManager.AppSettings["PROJECTTYPEMAPPING"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Useradminmap> useradminprojectlist = new List<Useradminmap>();

            string strparams = "companyId=" + Session["CompanyID"];
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
        public async Task<ActionResult> CheckedValueProject(List<int> selectedItems, Useradminmap useradminmap)
        {
            try
            {

                var UserAdminProjectPostURL = ConfigurationManager.AppSettings["USERADMINPROJECTPOST"];
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
                    ""cP_CRECID"": ""{Session["CompanyID"]}"",
                    ""cP_CUSTOMERRECID"": ""{ Session["USERID"]}"",                  
                    ""cP_PROJECTRECID"":""{Session["SELECTEDPROJECTID"]}"",              
                    ""cP_SORTORDER"":""{1}"",                
                    ""cP_DISABLE"":""{"N"}""                
                   
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
    }
}