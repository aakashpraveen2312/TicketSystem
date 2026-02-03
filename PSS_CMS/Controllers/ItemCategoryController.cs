using Newtonsoft.Json;
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
    public class ItemCategoryController : Controller
    {
        public object IC_IGRECId;
        string Status;
        string Message;
        // GET: ItemCategory
        public async Task<ActionResult> List(string IGRECID, string ItemgroupName, string searchPhrase, string reload)
        {

            if (IGRECID != null)
            {
                Session["IGRECID"] = IGRECID;
                IGRECID = Session["IGRECID"].ToString();
            }
            int serialNo = 1;

            // Initialize configurations for ItemCategory API call
            string webUrlGet = ConfigurationManager.AppSettings["ITEMCATEGORYGET"];
            string authKey = ConfigurationManager.AppSettings["Authkey"];

            // Check API Key
            if (Session["APIKEY"] == null)
            {
                ModelState.AddModelError(string.Empty, "API Key is missing.");
                return View(new List<ItemCategory>());
            }

            string apiKey = Session["APIKEY"].ToString();
            if (ItemgroupName != null && IGRECID != null)
            {
                Session["ItemgroupName"] = ItemgroupName;
                Session["IC_IGRECID"] = IGRECID;
            }
            string companyId = Session["CompanyId"]?.ToString();
            string strParams = $"GroupRecid={Session["IGRECID"]}&companyId={companyId}";
            string finalUrl = $"{webUrlGet}?{strParams}";


            //DataTable dt = Session["ItemCategoryListTable"] as DataTable;
            List<ItemCategory> itemCategoryList = new List<ItemCategory>();

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", apiKey);
                        client.DefaultRequestHeaders.Add("Authorization", authKey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                        var response = await client.GetAsync(finalUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<ItemconetntObject>(jsonString);
                            itemCategoryList = rootObjects?.Data ?? new List<ItemCategory>();

                            // Set SerialNumber for each ItemCategory
                            foreach (var item in itemCategoryList)
                            {
                                item.SerialNumber = serialNo++;
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

            return View(itemCategoryList);
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Create(ItemCategory itemCategory)
        {

            string IC_IGRECID = Session["IC_IGRECID"].ToString();



            //string Name = GlobalVariables.IC_DESCRIPTION;
            string companyId = "";
            companyId = Session["CompanyId"].ToString();



            try
            {

                var Regurl = ConfigurationManager.AppSettings["ITEMCATEGORYPOST"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{
                           
                            ""IC_CODE"": ""0"",
                            ""IC_DESCRIPTION"": ""{itemCategory.IC_DESCRIPTION}"",                                                                            
                            ""IC_IMAGE"": ""{"-"}"",                                                                            
                            ""IC_SORTORDER"": ""{itemCategory.IC_SORTORDER}"",
                            ""IC_HSNCODE"": ""{itemCategory.IC_HSNCODE}"",
                            ""IC_DISABLE"": ""{(itemCategory.IsDisabled ? "Y" : "N")}"",
                            ""IC_IGRECID"": ""{IC_IGRECID}"",
                            ""IC_CRECID"": ""{companyId}""
                     }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(Regurl),
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

                    var apiResponse = JsonConvert.DeserializeObject<ItemconetntObject>(responseBody);
                    string message = apiResponse.Message;
                    string Status = apiResponse.Status;
                    //string IC_IGRECId = apiResponse.IC_IGRECID;
                    //Session["IC_IGRECID"] = IC_IGRECId;


                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { status = "success", message = "ItemCategory created successfully" });

                    }
                    else if (apiResponse.Status == "U")
                    {

                        return Json(new { status = "error", message = apiResponse.Message });
                    }

                    else if (apiResponse.Status == "N")
                    {
                        return Json(new { status = "error", message = apiResponse.Message });
                    }

                    else
                    {
                        return RedirectToAction("List", "ItemCategory", new { IGRECID = Session["IC_IGRECID"], ItemgroupName = Session["ItemgroupName"] });
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

            return RedirectToAction("List", "ItemCategroy", new { IGRECID = Session["IC_IGRECID"], ItemgroupName = Session["ItemgroupName"] });
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id, string ICEdit, string EdititemCatName)
        {
            //GlobalVariables.EdititemCatName = EdititemCatName;
            Session["EdititemCatName"] = EdititemCatName.ToUpper();

            //GlobalVariables.IC_RECID = id.ToString();
            Session["IC_RECID"] = id.ToString();
            string IC_RECID = Session["IC_RECID"].ToString();
            //string Name = GlobalVariables.IC_DESCRIPTION;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["ITEMCATEGORYGETBYID"];
            string Authkey = ConfigurationManager.AppSettings["Authkey"];

            List<ItemCategory> ItemList = new List<ItemCategory>();

            string APIKey = Session["APIKEY"].ToString();
            string companyId = Session["CompanyId"].ToString();


            string strparams = "RecordId=" + id + "&" + "companyId=" + companyId;
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
                            var rootObjects = JsonConvert.DeserializeObject<ItemconetntObject>(jsonString);
                            //GlobalVariables.SPEDITCODE = rootObjects.Data[0].IG_CODE;
                            ItemList = rootObjects.Data;

                            var ItemGroup = ItemList.FirstOrDefault();
                            return View(ItemGroup);


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
            return View(new ItemCategory());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ItemCategory itemCategory)
        {

            // Intialising Company recid in storagepoint
            string companyId = Session["CompanyId"].ToString();
            string IC_IGRECID = Session["IC_IGRECID"].ToString();
            string IC_RECID = Session["IC_RECID"].ToString();
            //string Name = GlobalVariables.IC_DESCRIPTION;
            try
            {
                var Regurl = ConfigurationManager.AppSettings["ITEMCATEGORYPUT"]; // Change this to the appropriate URL for PUT
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                // Prepare the content to be sent in the request
                var content = $@"{{
                           
                            ""IC_RECID"": ""{IC_RECID}"",
                            ""IC_CODE"": ""{itemCategory.IC_CODE}"",
                            ""IC_DESCRIPTION"": ""{itemCategory.IC_DESCRIPTION}"",                                                                            
                            ""IC_IMAGE"": ""{"-"}"",                                                                            
                            ""IC_SORTORDER"": ""{itemCategory.IC_SORTORDER}"",
                            ""IC_HSNCODE"": ""{itemCategory.IC_HSNCODE}"",
                            ""IC_DISABLE"": ""{(itemCategory.IsDisabled ? "Y" : "N")}"",
                            ""IC_IGRECID"": ""{IC_IGRECID}"",
                            ""IC_CRECID"": ""{companyId}""
                     }}";

                // Prepare the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{Regurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
            {
                { "X-Version", "1" },
                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
            },
                    Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                };

                // Set up the HTTP client
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {

                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                // Send the request and get the response
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var apiResponse = JsonConvert.DeserializeObject<ItemconetntObject>(responseBody);
                    string message = apiResponse.Message;

                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { status = "success", message = "ItemCategory details edited successfully" });
                    }
                    else if (apiResponse.Status == "U")
                    {
                        return Json(new { status = "error", message = apiResponse.Message });
                    }

                    else if (apiResponse.Status == "N")
                    {
                        return Json(new { status = "error", message = apiResponse.Message });
                    }

                    else
                    {
                        return RedirectToAction("List", "ItemCategory", new { IGRECID = Session["IC_IGRECID"], ItemgroupName = Session["ItemgroupName"] });
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Request failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }


            return RedirectToAction("List", "ItemCategory", new { CompanyId = Session["CompanyId"], IGRECID = Session["IC_IGRECID"] });
        }

        public async Task<ActionResult> Delete(int id)
        {
            string companyId = "";
            string gid = Session["IC_IGRECID"].ToString();
            companyId = Session["CompanyId"].ToString();
            //string Name = GlobalVariables.IC_DESCRIPTION;
            string Weburl = ConfigurationManager.AppSettings["ITEMCATEGORYDELETE"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];


            // Construct the query string correctly
            string strparams = $"RecordId={id}&companyId={companyId}";
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
                            var apiResponse = JsonConvert.DeserializeObject<ItemconetntObject>(responseBody);

                            if (apiResponse.Status == "Y")
                            {
                                string redirectUrl = Url.Action("List", "ItemCategory", new { IGRECID = Session["IC_IGRECID"], ItemgroupName = Session["ItemgroupName"], reload = "True" });
                                return Json(new { status = "success", message = apiResponse.Message, redirectUrl = redirectUrl });
                            }
                            else if (apiResponse.Status == "U")
                            {
                                return Json(new { status = "error", message = "You can't delete this record because other records depend on it" });
                            }
                            else
                            {
                                return Json(new { status = "error", message = apiResponse.Message });
                            }




                            //return RedirectToAction("List", "Material", new { id = GlobalVariables.MC_RECID, MCNAME = GlobalVariables.MCNAME });
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


        public async Task<ActionResult> GetApiData()
        {

            string WEBURLGET = ConfigurationManager.AppSettings["GETHSNLOOKUP"];
            string Authkey = ConfigurationManager.AppSettings["Authkey"];

            List<HSNMaster> quatationList = new List<HSNMaster>();


            string APIKey = Session["APIKEY"].ToString();


            string strparams = "?companyId=" + Session["CompanyId"];
            string finalurl = WEBURLGET + strparams;
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
                            var responseData = await response.Content.ReadAsStringAsync();

                            // Deserialize directly into a list of Lookup objects
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var data = JsonConvert.DeserializeObject<HSNRootObjects>(jsonString);
                            return Json(data, JsonRequestBehavior.AllowGet); // Return data as JSON
                        }
                        else
                        {
                            // Handle failure
                            return Json(new { status = "error", message = "Request failed. Please try again later." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return Json(new { status = "error", message = "An error occurred while processing your request." });
            }
        }

        public async Task<ActionResult> BulkUpload(HttpPostedFileBase postedFile)
        {

            string Authkey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"].ToString();

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

                var ItemCategory = new List<Dictionary<string, string>>();

                int intj = 0;
                foreach (string row in csvData.Split('\n'))
                {
                    //if (!string.IsNullOrEmpty(row))
                    if (!string.IsNullOrWhiteSpace(row) && row.Split(',').Any(c => !string.IsNullOrWhiteSpace(c)))
                    {
                        if (intj > 0) // Skip header row
                        {
                            var itemcategory = new Dictionary<string, string>();
                            var columns = row.Split(',').Select(c => c.Trim()).ToArray();

                            if (columns.Length >= 4) // Assuming at least 7 columns in the CSV
                            {
                                itemcategory["iC_CODE"] = columns[0].Trim();
                                itemcategory["iC_DESCRIPTION"] = columns[1].Trim();
                                itemcategory["iC_IMAGE"] = "-";
                                itemcategory["iC_HSNCODE"] = columns[2].Trim();
                                itemcategory["iC_SORTORDER"] = columns[3].Trim();
                                itemcategory["iC_DISABLE"] = columns[4].Trim();
                                itemcategory["iC_CRECID"] = Session["CompanyId"].ToString();
                                itemcategory["iC_IGRECID"] = Session["IGRECID"].ToString();

                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Invalid CSV format!";
                                return RedirectToAction("List", "ItemCategory", new { CompanyId = Session["CompanyId"], IGRECID = Session["IC_IGRECID"], ItemgroupName = Session["ItemgroupName"] });
                            }

                            ItemCategory.Add(itemcategory);
                        }
                        intj++;
                    }
                }

                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        var PostBulkItemCategory = ConfigurationManager.AppSettings["PostBulkItemCategory"];
                        client.BaseAddress = new Uri(PostBulkItemCategory);

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Add APIKey and AuthKey to the request headers
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", Authkey);


                        var content = new StringContent(JsonConvert.SerializeObject(ItemCategory), Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync(new Uri(client.BaseAddress, PostBulkItemCategory), content);

                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var itemResponse = JsonConvert.DeserializeObject<IRootObjectsItemcategory>(jsonResponse);
                        Status = itemResponse.Status;
                        Message = itemResponse.Message;

                        if (Status == "Y")
                        {
                            TempData["SuccessMessage"] = "Data uploaded successfully!";
                            return RedirectToAction("List", "ItemCategory", new { CompanyId = Session["CompanyId"], IGRECID = Session["IC_IGRECID"], ItemgroupName = Session["ItemgroupName"] });

                        }
                        else if (Status == "N")
                        {
                            TempData["ErrorMessage"] = Message;
                            TempData["ErrorData"] = itemResponse.Data;
                            return RedirectToAction("List", "ItemCategory", new { CompanyId = Session["CompanyId"], IGRECID = Session["IC_IGRECID"], ItemgroupName = Session["ItemgroupName"] });

                        }
                        else if (Status == "U")
                        {
                            TempData["ErrorMessage"] = Message;
                            return RedirectToAction("List", "ItemCategory", new { CompanyId = Session["CompanyId"], IGRECID = Session["IC_IGRECID"], ItemgroupName = Session["ItemgroupName"] });

                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Data not uploaded!";
                            return RedirectToAction("List", "ItemCategory", new { CompanyId = Session["CompanyId"], IGRECID = Session["IC_IGRECID"], ItemgroupName = Session["ItemgroupName"] });

                        }
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "No file uploaded!";
            }

            return RedirectToAction("List", "ItemCategory", new { CompanyId = Session["CompanyId"], IGRECID = Session["IC_IGRECID"], ItemgroupName = Session["ItemgroupName"] });
        }
    }
}