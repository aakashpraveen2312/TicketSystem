using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    [ApiKeyAuthorize]
    public class HSNMasterController : Controller
    {
        // GET: HSNMaster
        string APIKey = "";
        public async Task<ActionResult> List(int? HSNC_RECID, string HSNC_CATEGORYNAME, string searchPhrase)
        {
            if (HSNC_RECID != null && HSNC_CATEGORYNAME != null)
            {
                Session["HSN_HSNCRECID"] = HSNC_RECID;
                Session["HSNC_CATEGORYNAME"] = HSNC_CATEGORYNAME;
            }

            //Session["HSN_HSNCRECID"] = 76;

            HSNMaster objitems = new HSNMaster();

            int SerialNo = objitems.SerialNumber;

            if (SerialNo == 0)
            {
                SerialNo = 1; // Initialize to 1 if it's 0
            }

            //Session["CompanyId"] = CompanyId.ToString();
            string Weburl = ConfigurationManager.AppSettings["GETHSN"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            List<HSNMaster> HSNMasterList = new List<HSNMaster>();

            APIKey = Session["APIKEY"].ToString();

            string url = Weburl;
            string strParams = "HSNCRECID=" + Session["HSN_HSNCRECID"] + "&companyId=" + Session["CompanyId"];
            string finalUrl = $"{url}?{strParams}";

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

                            var rootObjects = JsonConvert.DeserializeObject<HSNRootObjects>(jsonString);

                            HSNMasterList = rootObjects.Data;

                            if (HSNMasterList.Count > 0)
                            {
                                // Assign serial numbers
                                for (int i = 0; i < HSNMasterList.Count; i++)
                                {
                                    HSNMasterList[i].SerialNumber = i + 1;
                                }
                            }
                            if (!string.IsNullOrEmpty(searchPhrase))
                            {
                                HSNMasterList = HSNMasterList
                                    .Where(r => r.HM_CODE.ToLower().Contains(searchPhrase.ToLower()) ||
                                   r.HM_IGST.ToString().ToLower().Contains(searchPhrase.ToLower()) ||
                                   r.HM_CGST.ToString().ToLower().Contains(searchPhrase.ToLower()) ||
                                   r.HM_SGST.ToString().ToLower().Contains(searchPhrase.ToLower()) ||
                                   r.HM_SORTORDER.ToString().ToLower().Contains(searchPhrase.ToLower()) ||
                                   r.HM_DESCRIPTION.ToLower().Contains(searchPhrase.ToLower()))
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

            return View(HSNMasterList);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(HSNMaster objHSNMaster)
        {

            try
            {
                var WEBURLPOST = ConfigurationManager.AppSettings["POSTHSN"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{
                              ""HM_CODE"": ""{objHSNMaster.HM_CODE}"",
                              ""HM_DESCRIPTION"": ""{objHSNMaster.HM_DESCRIPTION}"",
                              ""HM_IGST"": ""{objHSNMaster.HM_IGST}"",
                              ""HM_SGST"": ""{objHSNMaster.HM_SGST}"",
                              ""HM_CGST"": ""{objHSNMaster.HM_CGST}"",
                              ""HM_SORTORDER"": ""{objHSNMaster.HM_SORTORDER}"",
                              ""HM_HSNCRECID"": ""{Session["HSN_HSNCRECID"]}"",
                              ""HM_HSNCCRECID"": ""{Session["CompanyId"]}"",
                              ""HM_DISABLE"": ""{(objHSNMaster.IsDisabled ? "Y" : "N")}""

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

                    var apiResponse = JsonConvert.DeserializeObject<HSNRootObjects>(responseBody);
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
        public async Task<ActionResult> Edit(int? RECID, string HM_Name)
        {

            //Session["HM_CODE"] = HM_CODE.ToUpper();
            Session["HRECID"] = RECID;
            Session["HM_Name"] = HM_Name.ToUpper();

            string WEBURLGETBYID = ConfigurationManager.AppSettings["GETBYIDHSN"];
            string Authkey = ConfigurationManager.AppSettings["Authkey"];


            HSNMaster HSNMasterList = null;

            string APIKey = Session["APIKEY"].ToString();
            string companyId = Session["CompanyId"].ToString();
            //GlobalVariables.AU_RECID = id;
            //Session["HSN_RECID"] = RECID;


            string strparams = "CompanyRecID=" + Session["CompanyId"] + "&" + "RecID=" + RECID;
            string finalurl = WEBURLGETBYID + "?" + strparams;

            try
            {
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
                            var content = JsonConvert.DeserializeObject<HSNRootObjects>(jsonString);
                            HSNMasterList = content.Data.FirstOrDefault();
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
            return View(HSNMasterList);

        }
        [HttpPost]
        
        public async Task<ActionResult> Edit(HSNMaster objHSNMaster)
        {
            string companyId = "";
            companyId = Session["CompanyId"].ToString();
            try
            {

                var WEBURLPUT = ConfigurationManager.AppSettings["PUTHSN"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();
                //int AU_RECID = GlobalVariSession["HM_RECID"]ables.AU_RECID;
                objHSNMaster.HM_RECID = (int)Session["HRECID"];
                var content = $@"{{
                              ""HM_RECID"": ""{objHSNMaster.HM_RECID}"",
                             ""HM_CODE"": ""{objHSNMaster.HM_CODE}"",
                             ""HM_DESCRIPTION"": ""{objHSNMaster.HM_DESCRIPTION}"",
                              ""HM_IGST"": ""{objHSNMaster.HM_IGST}"",
                              ""HM_SGST"": ""{objHSNMaster.HM_SGST}"",
                              ""HM_CGST"": ""{objHSNMaster.HM_CGST}"",
                              ""HM_SORTORDER"": ""{objHSNMaster.HM_SORTORDER}"",
                              ""HM_HSNCRECID"": ""{Session["HSN_HSNCRECID"]}"",
                              ""HM_HSNCCRECID"": ""{Session["CompanyId"]}"",
                              ""HM_DISABLE"": ""{(objHSNMaster.IsDisabled ? "Y" : "N")}""

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

                    var apiResponse = JsonConvert.DeserializeObject<HSNRootObjects>(responseBody);

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


            return RedirectToAction("List", "HSNMaster", new { HSNC_RECID = Session["HSN_HSNCRECID"], HSNC_CATEGORYNAME = Session["HSNC_CATEGORYNAME"] });

        }
        public async Task<ActionResult> Delete(int RECID)
        {

            string Weburl = ConfigurationManager.AppSettings["DELETEHSN"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = ConfigurationManager.AppSettings["Apikey"];

            // Construct the query string correctly
            string strparams = $"RECID={RECID}&companyId={Session["CompanyId"]}";
            string url = $"{Weburl}?{strparams}";
            APIKey = Session["APIKEY"].ToString();

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
                                string redirectUrl = Url.Action("List", "HSNMaster", new { HSNC_RECID = Session["HSN_HSNCRECID"], HSNC_CATEGORYNAME = Session["HSNC_CATEGORYNAME"] });
                                return Json(new { status = "success", message = apiResponse.Message, redirectUrl = redirectUrl });
                            }
                            else if (apiResponse.Status == "U")
                            {
                                return Json(new { status = "error", message = "You can't delete this record" });
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
        public async Task<ActionResult> BulkUpload(HttpPostedFileBase postedFile)
        {
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            APIKey = Session["APIKEY"].ToString();

            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filePath = Path.Combine(path, Path.GetFileName(postedFile.FileName));
                postedFile.SaveAs(filePath);

                string csvData = System.IO.File.ReadAllText(filePath);

                var HSNmaster = new List<Dictionary<string, string>>();

                int intj = 0;
                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrWhiteSpace(row) && row.Split(',').Any(c => !string.IsNullOrWhiteSpace(c)))
                    {
                        if (intj > 0) // Skip header row
                        {
                            var hsnmaster = new Dictionary<string, string>();
                            var columns = row.Split(',').Select(c => c.Trim()).ToArray();

                            if (columns.Length >= 4) // Assuming 9 columns in the CSV
                            {
                                hsnmaster["hM_CODE"] = columns[0].Trim();
                                hsnmaster["hM_DESCRIPTION"] = columns[1].Trim();
                                hsnmaster["hM_IGST"] = columns[2].Trim().Replace("%", "");
                                hsnmaster["hM_CGST"] = columns[3].Trim().Replace("%", "");
                                hsnmaster["hM_SGST"] = columns[4].Trim().Replace("%", "");
                                hsnmaster["hM_SORTORDER"] = columns[5].Trim();
                                hsnmaster["hM_DISABLE"] = columns[6].Trim();
                                hsnmaster["hM_HSNCMRECID"] = Session["CompanyId"].ToString();
                                hsnmaster["hM_HSNCRECID"] = Session["HSN_HSNCRECID"].ToString();
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Invalid CSV format!";
                                return RedirectToAction("List", "HSNMaster", new { HSNC_RECID = Session["HSN_HSNCRECID"], HSNC_CATEGORYNAME = Session["HSNC_CATEGORYNAME"] });
                            }

                            HSNmaster.Add(hsnmaster);
                        }
                        intj++;
                    }
                }

                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {

                        var BULKHSNMASTER = ConfigurationManager.AppSettings["BULKUPLOADHSNMASTER"];

                        client.BaseAddress = new Uri(BULKHSNMASTER);


                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Add APIKey and AuthKey to the request headers
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", AuthKey);



                        var content = new StringContent(JsonConvert.SerializeObject(HSNmaster), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync(new Uri(client.BaseAddress, BULKHSNMASTER), content);



                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var HSNResponse = JsonConvert.DeserializeObject<HSNRootObjects>(jsonResponse);
                        var Status = HSNResponse.Status;
                        var Message = HSNResponse.Message;

                        if (Status == "Y")
                        {
                            TempData["SuccessMessage"] = "Data uploaded successfully!";
                            return RedirectToAction("List", "HSNMaster", new { HSNC_RECID = Session["HSN_HSNCRECID"], HSNC_CATEGORYNAME = Session["HSNC_CATEGORYNAME"] });

                        }
                        else if (Status == "N")
                        {
                            TempData["ErrorMessage"] = Message;
                            return RedirectToAction("List", "HSNMaster", new { HSNC_RECID = Session["HSN_HSNCRECID"], HSNC_CATEGORYNAME = Session["HSNC_CATEGORYNAME"] });

                        }
                        else if (Status == "U")
                        {
                            TempData["ErrorMessage"] = Message;
                            return RedirectToAction("List", "HSNMaster", new { HSNC_RECID = Session["HSN_HSNCRECID"], HSNC_CATEGORYNAME = Session["HSNC_CATEGORYNAME"] });

                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Data not uploaded!";
                            return RedirectToAction("List", "HSNMaster", new { HSNC_RECID = Session["HSN_HSNCRECID"], HSNC_CATEGORYNAME = Session["HSNC_CATEGORYNAME"] });

                        }
                    }
                }
            }

            return RedirectToAction("List", "HSNMaster", new { HSNC_RECID = Session["HSN_HSNCRECID"], HSNC_CATEGORYNAME = Session["HSNC_CATEGORYNAME"] });
        }

    }
}