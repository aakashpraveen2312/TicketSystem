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
    public class ItemController : Controller
    {
        // GET: Item
        string APIKey = "";
        string Status;
        string Message;
        // GET: ItemGroup
        public async Task<ActionResult> List(int? CompanyId, int? CategoryRecid, string ItemCatName, string searchPhrase)
        {
            if (CategoryRecid != null && CompanyId != null && ItemCatName != null)
            {

                //GlobalVariables.ItemCatName = ItemCatName;
                Session["ItemCatName"] = ItemCatName;
                //Session["ItemCatName1"] = ItemCatName.ToUpper();
                Session["CompanyId"] = CompanyId.ToString();
                //GlobalVariables.ICRECID = CategoryRecid.ToString();
                Session["CategoryRecid"] = CategoryRecid.ToString();
            }



            Items objitems = new Items();

            int SerialNo = objitems.SerialNumber;

            if (SerialNo == 0)
            {
                SerialNo = 1; // Initialize to 1 if it's 0
            }


            string Weburl = ConfigurationManager.AppSettings["GETITEM"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            List<Items> ItemList = new List<Items>();

            APIKey = Session["APIKEY"].ToString();

            string strparams = "companyId=" + Session["CompanyId"] + "&CategoryRecid=" + Session["CategoryRecid"] + "";
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
                            var rootObjects = JsonConvert.DeserializeObject<IRootObjects>(jsonString);

                            ItemList = rootObjects.Data;

                            if (ItemList.Count > 0)
                            {

                                // Assign serial numbers
                                for (int i = 0; i < ItemList.Count; i++)
                                {
                                    ItemList[i].SerialNumber = i + 1;
                                }



                            }
                            if (!string.IsNullOrEmpty(searchPhrase))
                            {
                                ItemList = ItemList
                                 .Where(r => r.I_DESCRIPTION.ToLower().Contains(searchPhrase.ToLower()) ||
                                     r.I_CODE.ToLower().Contains(searchPhrase.ToLower())) // Use OR (||) instead of chaining .Where()
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

            return View(ItemList);
        }

        public async Task<ActionResult> Create(int? carecid, string categoryName, string GroupName)
        {
            if (carecid >= 0)
            {
                Session["CategoryRecid"] = carecid;
                Session["ItemgroupName"] = GroupName;
                Session["ItemCatName"] = categoryName;

            }
            string WEBURLGETBYID = ConfigurationManager.AppSettings["GetVI_HSN"];
            string Authkey = ConfigurationManager.AppSettings["Authkey"];

            List<Items> ItemList = new List<Items>();

            string APIKey = Session["APIKEY"].ToString();
            string companyId = Session["CompanyId"].ToString();


            string strparams = "categoryID=" + Session["CategoryRecid"];
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
                            var rootObjects = JsonConvert.DeserializeObject<ICreateRootObjects>(jsonString);

                            // Assign Data directly since it's an object
                            var ItemGroup = rootObjects.Data;

                            // Populate combo box values
                            //ItemGroup.Outlets = await GetOutletComboAsync();
                            //ItemGroup.Bins = await GetBinsComboAsync();
                            //ItemGroup.Shelves = await GetShelvesComboAsync();

                            // Retrieve any message stored in TempData
                            string message = TempData["Message"] as string;
                            ViewBag.ErrorMessage = message;


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

            return View();

        }

        [HttpPost]
        public async Task<ActionResult> Create(Items objItem)
        {
            try
            {
                var ItemPostURL = ConfigurationManager.AppSettings["POSTITEM"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{
    ""I_CODE"": ""0"",
    ""I_DESCRIPTION"": ""{objItem.I_DESCRIPTION}"",
    ""I_PRICE"": ""{objItem.I_PRICE}"",
    ""I_SMQUANTITY"": ""{objItem.I_SMQUANTITY}"",
    ""I_SMNETAMOUNT"": ""{objItem.I_SMNETAMOUNT}"",
    ""I_SMTOTALAMOUNT"": ""{objItem.I_SMTOTALAMOUNT}"",
    ""I_SMDISCOUNT"": ""0"",
    ""I_CUOM"": ""{objItem.I_CUOM}"",
    ""I_HSN"": ""0"",
    ""I_HSNRECID"": ""0"",
    ""I_SGST"": ""{objItem.I_SGST}"",
    ""I_CGST"": ""{objItem.I_CGST}"",
    ""I_CRECID"": ""{Session["CompanyId"]}"",
    ""I_ICRECID"": ""{Session["CategoryRecid"]}"",
    ""I_SORTORDER"": ""{objItem.I_SORTORDER}"",
    ""I_DISABLE"": ""N"",
    ""I_BOXQUANTITY"": ""0"",
    ""I_PIECEQUANTITY"": ""0"",
    ""I_PUOM"": ""UOM"",
    ""I_CONVERSIONQUANTITY"": ""0"",
    ""I_IMAGE"": ""qwerty"",
    ""I_SHRECID"": ""0"",
    ""I_BINRECID"": ""0"",
    ""I_SPRECID"": ""0"",
    ""I_IFEXPIRYAPPLICABLE"": ""N"",
    ""I_UNDEREMPLOYEECUSTODY"": ""N"",
    ""I_SERVICEANDMAINTANANCE"": ""N"",
    ""I_TRADABLE"": ""N"",
    ""I_EXTENDEDWARRENTYAPPLICABLE"": ""N"",
    ""I_ONDEMAND"": ""N"",
    ""I_SCHEDULEDSERVICE"": ""N"",
    ""I_TOTALWARRENTYPERIOD"": """",
    ""I_WARRENTYENDPERIOD"": """",
    ""I_EXTENDEDWARRENTYPERIOD"": """",
    ""I_EXTENDEDWARRENTYENDPERIOD"": """",
    ""I_BYPRODUCT"": ""N"",
    ""I_SPECREQUIRED"": ""N"",
    ""I_DESIGNIMAGEREQUIRED"": ""N"",
    ""I_CUSTOMERPRICE"": ""0"",
    ""I_CUSTOMERMAJORQUANTITY"": ""0"",
    ""I_CUSTOMERMINORQUANTITY"": ""0"",
    ""I_CUSTOMERMAJORUOM"": ""UOM"",
    ""I_CUSTOMERMINORUOM"": ""UOM"",
    ""I_CUSTOMERCONVERSIONQUANTITY"": ""0"",
    ""I_VENDORPRICE"": ""0"",
    ""I_VENDORMAJORQUANTITY"": ""0"",
    ""I_VENDORMINORQUANTITY"": ""0"",
    ""I_VENDORMAJORUOM"": ""UOM"",
    ""I_VENDORMINORUOM"": ""UOM"",
    ""I_VENDORCONVERSIONQUANTITY"": ""0""
}}";


                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(ItemPostURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<IRootObjects>(responseBody);

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


  
        public async Task<ActionResult> Edit(int? companyId, int? id, string IEditName)
        {
            //GlobalVariables.IEditName = IEditName;
            Session["IEditName"] = IEditName;

            Session["CompanyId"] = companyId.ToString();
            //GlobalVariables.IRECID = (int)id;
            Session["IRECID"] = (int)id;


            string Weburl = ConfigurationManager.AppSettings["GETBYIDITEM"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Items> ItemList = new List<Items>();

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
                            var rootObjects = JsonConvert.DeserializeObject<IRootObjects>(jsonString);
                            //GlobalVariables.IEDITCODE = rootObjects.Data[0].I_CODE;
                            Session["IEDITCODE"] = rootObjects.Data[0].I_CODE;
                            Session["I_CODE"] = rootObjects.Data[0].I_CODE;
                            Session["I_HSNRECID"] = rootObjects.Data[0].I_HSNRECID;
                            ItemList = rootObjects.Data;

                            var Item = ItemList.FirstOrDefault();
                            return View(Item);

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

            return View(new Items()); // Return an empty model or handle the error appropriately
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Items objItems)
        {

            // Intialising Company recid in storagepoint
            objItems.I_CRECID = Session["CompanyId"].ToString();
            objItems.I_ICRECID = Session["CategoryRecid"].ToString();
            try
            {
                var Regurl = ConfigurationManager.AppSettings["PUTITEM"]; // Change this to the appropriate URL for PUT
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();
                Session["I_CODE"] = objItems.I_CODE;
                // Prepare the content to be sent in the request
                var content = $@"{{

            ""I_RECID"": ""{Session["IRECID"]}"",
            ""I_CODE"": ""{objItems.I_CODE}"",
            ""I_CRECID"": ""{Session["CompanyId"]}"",
            ""I_DESCRIPTION"": ""{objItems.I_DESCRIPTION}"",
   ""I_SMQUANTITY"": ""{objItems.I_SMQUANTITY}"",
    ""I_SMNETAMOUNT"": ""{objItems.I_SMNETAMOUNT}"",
    ""I_SMTOTALAMOUNT"": ""{objItems.I_SMTOTALAMOUNT}"",
            ""I_PRICE"": ""{objItems.I_PRICE}"",
            ""I_BOXQUANTITY"": ""{objItems.I_BOXQUANTITY}"",
            ""I_PIECEQUANTITY"": ""{objItems.I_PIECEQUANTITY}"",
            ""I_PUOM"": ""{objItems.I_PUOM}"",
            ""I_CUOM"": ""{objItems.I_CUOM}"",
            ""I_SORTORDER"": ""{objItems.I_SORTORDER}"",
            ""I_ICRECID"": ""{Session["CategoryRecid"]}"",
            ""I_HSNRECID"": ""{Session["I_HSNRECID"]}"",
            ""I_HSN"": ""{objItems.I_HSN}"",
            ""I_SGST"": ""{objItems.I_SGST}"",
            ""I_CGST"": ""{objItems.I_CGST}"",
             ""I_SHRECID"" :""0"",
             ""I_BINRECID"": ""0"",
             ""I_SPRECID"" : ""0"",
             ""I_IFEXPIRYAPPLICABLE"": ""{(objItems.EXPIRYAPPLICABLE ? "Y" : "N")}"",
             ""I_DISABLE"": ""{(objItems.IsDisabled ? "Y" : "N")}"",
             ""I_UNDEREMPLOYEECUSTODY"": ""{ (objItems.UNDER_EMPLOYEE_CUSTODY ? "Y" : "N")}"",
                ""I_TRADABLE"": ""{ (objItems.TRADABLE ? "Y" : "N")}"",
             ""I_BYPRODUCT"": ""{(objItems.BYPRODUCT ? "Y" : "N")}"",
             ""I_SPECREQUIRED"": ""{(objItems.SPECREQUIRED ? "Y" : "N")}"",
             ""I_DESIGNIMAGEREQUIRED"": ""{(objItems.DESIGNIMAGEREQUIRED ? "Y" : "N")}""


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

                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);
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
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }

            return RedirectToAction("List", "Item", new { CompanyId = Session["CompanyId"], CategoryRecid = Session["CategoryRecid"], ItemCatName = Session["ItemCatName"] });
        }

        public async Task<ActionResult> Delete(int? I_RECID, int companyId, ItemGroup ObjItemGroup)
        {
            string Weburl = ConfigurationManager.AppSettings["DELETEITEM"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = ConfigurationManager.AppSettings["Apikey"];

            // Construct the query string correctly
            string strparams = $"companyId={companyId}&RecordId={I_RECID}";
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
                                string redirectUrl = Url.Action("List", "Item", new { CompanyId = Session["CompanyId"], CategoryRecid = Session["CategoryRecid"], ItemCatName = Session["ItemCatName"] });
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

        public async Task<List<SelectListItem>> GetOutletComboAsync()
        {
            var outletCombo = new List<SelectListItem>();
            string apiurl = ConfigurationManager.AppSettings["ITEMCOMBO"];
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
                            var apiResponse = JsonConvert.DeserializeObject<ApiResponses>(content);

                            if (apiResponse.status == "Y")
                            {
                                foreach (var item in apiResponse.Outlet)
                                {
                                    outletCombo.Add(new SelectListItem
                                    {
                                        Value = item.ComboId.ToString(),
                                        Text = item.ComboName
                                    });
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Error: " + apiResponse.message);
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

        public async Task<List<SelectListItem>> GetBinsComboAsync()
        {
            var binsCombo = new List<SelectListItem>();
            string apiurl = ConfigurationManager.AppSettings["ITEMCOMBO"];
            string authkey = ConfigurationManager.AppSettings["AuthKey"];
            APIKey = Session["APIKEY"].ToString();
            string strparams = "companyId=" + Session["CompanyId"];
            string url = apiurl + "?" + strparams;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Bypass SSL certificate validation
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                // Pass the handler to the HttpClient
                using (var httpClient = new HttpClient(handler))  // <- Use handler here
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", authkey);
                    httpClient.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    try
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();

                            try
                            {
                                var apiResponse = JsonConvert.DeserializeObject<ApiResponses>(content);

                                if (apiResponse.status == "Y")
                                {
                                    foreach (var item in apiResponse.Bins)
                                    {
                                        binsCombo.Add(new SelectListItem
                                        {
                                            Value = item.ComboId.ToString(),
                                            Text = item.ComboName
                                        });
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, "Error: " + apiResponse.message);
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
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
                    }
                }
            }


            return binsCombo ?? new List<SelectListItem>();
        }

        public async Task<List<SelectListItem>> GetShelvesComboAsync()
        {
            var shelvesCombo = new List<SelectListItem>();
            string apiurl = ConfigurationManager.AppSettings["ITEMCOMBO"];
            string authkey = ConfigurationManager.AppSettings["AuthKey"];
            APIKey = Session["APIKEY"].ToString();
            string strparams = "companyId=" + Session["CompanyId"];
            string url = apiurl + "?" + strparams;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

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
                            var apiResponse = JsonConvert.DeserializeObject<ApiResponses>(content);

                            if (apiResponse.status == "Y")
                            {
                                foreach (var item in apiResponse.Shelves)
                                {
                                    shelvesCombo.Add(new SelectListItem
                                    {
                                        Value = item.ComboId.ToString(),
                                        Text = item.ComboName
                                    });
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Error: " + apiResponse.message);
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

            return shelvesCombo ?? new List<SelectListItem>();
        }

     
        public async Task<ActionResult> UploadDesignImage(int ItemRecID, HttpPostedFileBase DesignFile)
        {
            if (DesignFile == null || DesignFile.ContentLength == 0)
                return Content("No file selected.");

            string[] allowedExt = { ".png", ".jpg", ".jpeg" };
            string ext = Path.GetExtension(DesignFile.FileName).ToLower();

            if (!allowedExt.Contains(ext))
                return Content("Invalid file format! Only PNG, JPG, JPEG allowed.");

            // Convert file to Base64
            byte[] fileBytes;
            using (var binaryReader = new BinaryReader(DesignFile.InputStream))
            {
                fileBytes = binaryReader.ReadBytes(DesignFile.ContentLength);
            }

            string base64String = Convert.ToBase64String(fileBytes);

            try
            {

                var MaterialcatPostURL = ConfigurationManager.AppSettings["UPDATEITEMSPECIFICATIONIMAGE"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{
                        ""SPEC_IRECID"": {ItemRecID},
                        ""SPEC_CRECID"": ""{Session["CompanyId"]}"",
                        ""SPEC_DESIGNIMAGE_BASE64"": ""{base64String}""                      
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(MaterialcatPostURL),
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

                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseInfo>(responseBody);
                    //Session["P_RECID"]= apiResponse.Recid;
                    string message = apiResponse.Message;

                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("List", "Item");
                    }
                    else if (apiResponse.Status == "U")
                    {
                        return RedirectToAction("List", "Item");
                    }
                    else if (apiResponse.Status == "N")
                    {
                        return RedirectToAction("List", "Item");
                    }
                    else
                    {
                        return RedirectToAction("List", "Item");

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
            return RedirectToAction("List", "Item");
        }



    }
}