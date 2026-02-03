using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    public class ItemGroupController : Controller
    {
        // ITEMGROUP
        string APIKey = "";
        string Status;
        string Message;
        // GET: ItemGroup


        public async Task<ActionResult> List(int? CompanyId, string searchphrase, string reload)
        {
            int serialNo = 1;

            // Initialize necessary configurations
            string webUrlGet = ConfigurationManager.AppSettings["GETITEMGROUP"];
            string authKey = ConfigurationManager.AppSettings["Authkey"];



            string apiKey = Session["APIKEY"].ToString();
            if (CompanyId != null)
            {
                Session["CompanyId"] = CompanyId.ToString();
            }
            string strParams = "companyId=" + Session["CompanyId"];
            string finalUrl = $"{webUrlGet}?{strParams}";

            DataTable dt = Session["ItemGroupListTable"] as DataTable;
            List<ItemGroup> itemGroupList = new List<ItemGroup>();

            if (reload == "True" || dt == null)
            {
                dt = new DataTable();

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
                                var rootObjects = JsonConvert.DeserializeObject<IGRootObjects>(jsonString);
                                itemGroupList = rootObjects?.Data ?? new List<ItemGroup>();

                                // Set SerialNumber for each ItemGroup
                                foreach (var item in itemGroupList)
                                {
                                    item.SerialNumber = serialNo++;
                                }

                                // Define DataTable columns
                                dt.Columns.Add("SerialNumber", typeof(int));
                                dt.Columns.Add("IGCode", typeof(string));
                                dt.Columns.Add("IGName", typeof(string));
                                dt.Columns.Add("CRecID", typeof(string));
                                dt.Columns.Add("IG_Recid", typeof(int));
                                dt.Columns.Add("SortOrder", typeof(int));
                                // Additional fields as required

                                // Populate DataTable
                                foreach (var itemgroup in itemGroupList)
                                {
                                    DataRow row = dt.NewRow();
                                    row["SerialNumber"] = itemgroup.SerialNumber;
                                    row["IGCode"] = itemgroup.IG_CODE;
                                    row["IGName"] = itemgroup.IG_DESCRIPTION;
                                    row["CRecID"] = itemgroup.IG_CRECID;
                                    row["SortOrder"] = itemgroup.IG_SORTORDER;
                                    row["IG_Recid"] = itemgroup.IG_RECID;
                                    dt.Rows.Add(row);
                                }

                                Session["ItemGroupListTable"] = dt;
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
            }

            // Convert DataTable to List<ItemGroup>
            itemGroupList = dt.AsEnumerable().Select(row => new ItemGroup
            {

                SerialNumber = row.Field<int>("SerialNumber"),
                IG_CODE = row.Field<string>("IGCode"),
                IG_DESCRIPTION = row.Field<string>("IGName"),
                IG_CRECID = row.Field<string>("CRecID"),
                IG_SORTORDER = row.Field<int>("SortOrder"),
                IG_RECID = row.Field<int>("IG_Recid")
                // Add additional mappings as required
            }).ToList();

            // Perform search if searchPhrase is provided
            if (!string.IsNullOrEmpty(searchphrase))
            {
                DataTable filteredDt = dt.Clone();
                string escapedSearchPhrase = searchphrase.Replace("'", "''");
                foreach (DataRow dr in dt.Select($"CONVERT(SerialNumber, 'System.String') LIKE '%{escapedSearchPhrase}%' OR IGCode LIKE '%{escapedSearchPhrase}%' OR IGName LIKE '%{escapedSearchPhrase}%'"))
                {
                    filteredDt.ImportRow(dr);
                }

                itemGroupList = filteredDt.AsEnumerable().Select(row => new ItemGroup
                {
                    SerialNumber = row.Field<int>("SerialNumber"),
                    IG_CODE = row.Field<string>("IGCode"),
                    IG_DESCRIPTION = row.Field<string>("IGName"),
                    IG_CRECID = row.Field<string>("CRecID"),
                    IG_SORTORDER = row.Field<int>("SortOrder"),
                    IG_RECID = row.Field<int>("IG_Recid")
                    // Add additional mappings as required
                }).ToList();
            }

            return View(itemGroupList);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ItemGroup objItemGroup)
        {


            objItemGroup.IG_CRECID = Session["CompanyId"].ToString();

            try
            {
                var Regurl = ConfigurationManager.AppSettings["POSTITEMGROUP"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = new
                {
                    IG_CODE = "0",
                    IG_DATETIME = DateTime.Now.ToString("dd-MM-yyyy"),
                    IG_DESCRIPTION = objItemGroup.IG_DESCRIPTION,
                    IG_CRECID = objItemGroup.IG_CRECID,
                    IG_SORTORDER = objItemGroup.IG_SORTORDER,
                    IG_DISABLE = objItemGroup.IsDisabled ? "Y" : "N",
                    IG_SALESFLAG = objItemGroup.SalesDisabled ? "Y" : "N",
                    IG_PURCHASEFLAG = objItemGroup.PurchaseDisabled ? "Y" : "N",
                    IG_JOBWORKINFLAG = objItemGroup.JobworkinDisabled ? "Y" : "N",
                    IG_JOBWORKOUTFLAG = objItemGroup.JobworkoutDisabled ? "Y" : "N"
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
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

                        if (apiResponse.Status == "Y")
                        {
                            return Json(new { status = "success", message = "ItemGroup created successfully" });
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
                            return RedirectToAction("List", "ItemGroup", new { CompanyId = Session["CompanyId"] });
                        }

                    }
                    else
                    {
                        return Json(new { status = "error", message = "Request failed. Please try again later." });
                    }
                }
            }
            catch (Exception ex)
            {

                return Json(new { status = "error", message = "An error occurred while processing your request." });
            }

            return RedirectToAction("List", "ItemGroup", new { CompanyId = Session["CompanyId"] });

        }

        public async Task<ActionResult> Edit(int? companyId, int? id, string IGEditName)
        {
            //GlobalVariables.IGEditName = IGEditName;
            Session["IGEditName"] = IGEditName.ToUpper();
            Session["CompanyId"] = companyId.ToString();
            //GlobalVariables.IGRECID = id.ToString();
            Session["IGRECID"] = id.ToString();


            string Weburl = ConfigurationManager.AppSettings["GETBYIDITEMGROUP"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"].ToString();

            List<ItemGroup> ItemGroupList = new List<ItemGroup>();

            //StoragePointViewModel viewModel = new StoragePointViewModel();

            string strparams = "companyId=" + companyId + "&RecordId=" + id;
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
                            var rootObjects = JsonConvert.DeserializeObject<IGRootObjects>(jsonString);
                            //GlobalVariables.SPEDITCODE = rootObjects.Data[0].IG_CODE;
                            Session["SPEDITCODE"] = rootObjects.Data[0].IG_CODE;
                            ItemGroupList = rootObjects.Data;

                            var ItemGroup = ItemGroupList.FirstOrDefault();
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
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return View(new ItemGroup()); // Return an empty model or handle the error appropriately
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ItemGroup objItemGroup)
        {

            // Intialising Company recid in storagepoint
            objItemGroup.IG_CRECID = Session["CompanyId"].ToString();
            try
            {
                var Regurl = ConfigurationManager.AppSettings["PUTITEMGROUP"]; // Change this to the appropriate URL for PUT
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                // Prepare the content to be sent in the request
                var content = $@"{{
            ""IG_CODE"": ""{objItemGroup.IG_CODE}"",
            ""IG_RECID"": ""{Session["IGRECID"]}"",
            ""IG_DATETIME"" :""{DateTime.Now.ToString("dd-MM-yyyy")}"",
            ""IG_DESCRIPTION"": ""{objItemGroup.IG_DESCRIPTION}"",
            ""IG_CRECID"": ""{objItemGroup.IG_CRECID}"",
            ""IG_SORTORDER"": ""{objItemGroup.IG_SORTORDER}"",
            ""IG_DISABLE"": ""{(objItemGroup.IsDisabled ? "Y" : "N")}"",
            ""IG_SALESFLAG"": ""{(objItemGroup.SalesDisabled ? "Y" : "N")}"",
            ""IG_PURCHASEFLAG"": ""{(objItemGroup.PurchaseDisabled ? "Y" : "N")}"",
            ""IG_JOBWORKINFLAG"": ""{(objItemGroup.JobworkinDisabled ? "Y" : "N")}"",
            ""IG_JOBWORKOUTFLAG"": ""{(objItemGroup.JobworkoutDisabled ? "Y" : "N")}""
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

                    var apiResponse = JsonConvert.DeserializeObject<IGRootObjects>(responseBody);
                    string message = apiResponse.Message;

                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { status = "success", message = "ItemGroup Details Edited Successfully" });
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
                        return RedirectToAction("List", "ItemGroup", new { CompanyId = Session["CompanyId"] });
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

        }

        public async Task<ActionResult> Delete(int? IG_RECID, int companyId, ItemGroup ObjItemGroup)
        {
            string Weburl = ConfigurationManager.AppSettings["DELETEITEMGROUP"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = ConfigurationManager.AppSettings["Apikey"];

            // Construct the query string correctly
            string strparams = $"companyId={companyId}&GroupRecId={IG_RECID}";
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
                                string redirectUrl = Url.Action("List", "ItemGroup", new { CompanyId = Session["CompanyId"], reload = "True" });
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
                            //return RedirectToAction("List", "StoragePoint", new { CompanyId = GlobalVariables.CompanyId });
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