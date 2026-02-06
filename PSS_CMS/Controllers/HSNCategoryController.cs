using Newtonsoft.Json;
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
    public class HSNCategoryController : Controller
    {
        public async Task<ActionResult> List(int? companyId, string searchPhrase, string reload)
        {

            HSNCategory objitems = new HSNCategory();

            int SerialNo = objitems.SerialNumber;

            if (SerialNo == 0)
            {
                SerialNo = 1; // Initialize to 1 if it's 0
            }

            //Session["CompanyId"] = CompanyId.ToString();
            string Weburl = ConfigurationManager.AppSettings["GetHSNCategory"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            List<HSNCategory> HSNCategoryList = new List<HSNCategory>();

            string APIKey = Session["APIKEY"].ToString();
            if (companyId != null)
            {
                Session["CompanyId"] = companyId.ToString();
            }

            string strParams = "companyId=" + Session["CompanyId"];
            string finalUrl = $"{Weburl}?{strParams}";

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


                        var response = await client.GetAsync(finalUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            var rootObjects = JsonConvert.DeserializeObject<HSNCRootObjects>(jsonString);

                            HSNCategoryList = rootObjects.Data;

                            if (HSNCategoryList.Count > 0)
                            {
                                // Assign serial numbers
                                for (int i = 0; i < HSNCategoryList.Count; i++)
                                {
                                    HSNCategoryList[i].SerialNumber = i + 1;
                                }
                            }
                            if (!string.IsNullOrEmpty(searchPhrase))
                            {
                                HSNCategoryList = HSNCategoryList
                                    .Where(r => r.HSNC_CODE.ToLower().Contains(searchPhrase.ToLower()) ||

                                                r.HSNC_CATEGORYNAME.ToLower().Contains(searchPhrase.ToLower()))
                                    .ToList();
                            }
                            else
                            {

                            }
                        }

                        else
                        {
                            // Handle the error response here
                            ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., logging)
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return View(HSNCategoryList);
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
      
        public async Task<ActionResult> Create(HSNCategory objhsnccategory)
        {

            int HSNC_RECID = 0;


            objhsnccategory.HSNC_CRECID = Session["CompanyId"].ToString();

            try
            {

                var WEBURLPOST = ConfigurationManager.AppSettings["PostHSNCategory"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{
                             
                              ""HSNC_CODE"": ""{objhsnccategory.HSNC_CODE}"",
                              ""HSNC_CATEGORYNAME"": ""{objhsnccategory.HSNC_CATEGORYNAME}"",
                              ""HSNC_SORTORDER"": ""{objhsnccategory.HSNC_SORTORDER}"",
                              ""HSNC_CRECID"": ""{objhsnccategory.HSNC_CRECID}"",
                              ""HSNC_DISABLE"": ""{(objhsnccategory.IsDisabled ? "Y" : "N")}""
                     }}";
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(WEBURLPOST),
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
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                var response = await client.SendAsync(request);


                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var apiResponse = JsonConvert.DeserializeObject<HSNCRootObjects>(responseBody);
                    string message = apiResponse.Message;

                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { status = "success", message = "HSN Category created successfully" });
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
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);

                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);

            }


            return RedirectToAction("List", "HSNCategory");
        }

        public async Task<ActionResult> Edit(int? companyId, int? id, string HSNEditName)
        {
            //GlobalVariables.IGEditName = IGEditName;
            //Session["HSNEditName"] = HSNEditName.ToUpper();
            //Session["CompanyId"] = companyId.ToString();
            //GlobalVariables.IGRECID = id.ToString();
            Session["HSNC_RECID"] = id.ToString();


            string Weburl = ConfigurationManager.AppSettings["GetByIdHSNCAT"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"].ToString();

            List<HSNCategory> HSNCATList = new List<HSNCategory>();


            string strparams = "companyId=" + Session["CompanyId"] + "&RecordId=" + Session["HSNC_RECID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<HSNCRootObjects>(jsonString);
                            //GlobalVariables.SPEDITCODE = rootObjects.Data[0].IG_CODE;
                            Session["SPEDITCODE"] = rootObjects.Data[0].HSNC_CODE;
                            HSNCATList = rootObjects.Data;

                            var HSNCATEGORY = HSNCATList.FirstOrDefault();
                            return View(HSNCATEGORY);

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

            return View(new HSNCategory()); // Return an empty model or handle the error appropriately
        }



        [HttpPost]
 
        public async Task<ActionResult> Edit(HSNCategory objhsnccategory)
        {
            //string companyId = "";
            //int HSNC_RECID = 0;

            objhsnccategory.HSNC_CRECID = Session["CompanyId"].ToString();
            string HSNC_RECID = Session["HSNC_RECID"].ToString();

            try
            {

                var WEBURLPOST = ConfigurationManager.AppSettings["UpdateHSNCategory"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{
                             
                              ""HSNC_RECID"": ""{HSNC_RECID}"",
                              ""HSNC_CODE"": ""{objhsnccategory.HSNC_CODE}"",
                              ""HSNC_CATEGORYNAME"": ""{objhsnccategory.HSNC_CATEGORYNAME}"",
                              ""HSNC_SORTORDER"": ""{objhsnccategory.HSNC_SORTORDER}"",
                              ""HSNC_CRECID"": ""{objhsnccategory.HSNC_CRECID}"",
                              ""HSNC_DISABLE"": ""{(objhsnccategory.IsDisabled ? "Y" : "N")}""
                     }}";
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(WEBURLPOST),
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

                    var apiResponse = JsonConvert.DeserializeObject<HSNCRootObjects>(responseBody);
                    string message = apiResponse.Message;

                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { status = "success", message = "HSN Category Detail edited successfully" });
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
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);

                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);

            }


            return RedirectToAction("List", "HSNCategory", new { companyId = Session["CompanyId"] });
        }



        public async Task<ActionResult> Delete(int? HSNC_RECID, int companyId)
        {
            string Weburl = ConfigurationManager.AppSettings["DeleteHSNCategory"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            Session["CompanyId"] = companyId.ToString();
            Session["HSNC_RECID"] = HSNC_RECID.ToString();
            // Construct the query string correctly
            string strparams = $"CategoryRecId={HSNC_RECID}&companyId={companyId}";

            string url = $"{Weburl}?{strparams}";
            string APIKey = Session["APIKEY"].ToString();

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    // Disable SSL validation (only use this if you trust the server)
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        // Add required headers
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Create the DELETE request
                        var request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Delete,
                            RequestUri = new Uri(url)
                        };

                        var response = await client.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();
                            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

                            if (apiResponse.Status == "Y")
                            {
                                string redirectUrl = Url.Action("List", "HSNCategory", new { companyId = Session["CompanyId"] });
                                return Json(new { status = "success", message = apiResponse.Message, redirectUrl = redirectUrl }, JsonRequestBehavior.AllowGet);


                            }
                            else if (apiResponse.Status == "U")
                            {
                                return Json(new { status = "error", message = "You can't delete this record because other records depend on it" });
                            }
                            else
                            {
                                return Json(new { status = "error", message = apiResponse.Message });
                            }
                        }
                        else
                        {
                            // Log the response or handle the error case here
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

            // In case of failure, return to the view
            return View();
        }


    }
}