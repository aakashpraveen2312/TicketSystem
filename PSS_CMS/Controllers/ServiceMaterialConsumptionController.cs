using PSS_CMS.Fillter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSS_CMS.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace PSS_CMS.Controllers
{
    public class ServiceMaterialConsumptionController : Controller
    {
        // GET: ServiceMaterialConsumption
        public async Task<ActionResult> List(string searchPharse, string Type, int? CT_PRECID,int? CT_RECID,string ProductName)
        {
            if (Type != null && CT_PRECID != null && CT_RECID!=null && ProductName!=null)
            {
                Session["Type"] = Type;
                Session["CT_PRECID"] = CT_PRECID;
                Session["CT_RECID"] = CT_RECID;
                Session["ProductName"] = ProductName;
            }

            ServiceMaterialConsumption objmaterialconsumption = new ServiceMaterialConsumption();

            string Weburl = ConfigurationManager.AppSettings["MATERIALCONSUMPTIONSERVICEGET"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<ServiceMaterialConsumption> materialconsumptionlist = new List<ServiceMaterialConsumption>();

            string strparams = "cmprecid=" + Session["CompanyID"] + "&contractrecid=" + Session["CT_RECID"] + "&type=" + Session["Type"];
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
                            var rootObjects = JsonConvert.DeserializeObject<ServiceMaterialConsumptionRootObject>(jsonString);
                            materialconsumptionlist = rootObjects.Data;

                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                materialconsumptionlist = materialconsumptionlist
                                    .Where(r => r.sMT_UOM.ToString().Contains(searchPharse.ToLower()) ||
                                                r.sMT_QUANTITY.ToString().Contains(searchPharse.ToLower()) ||
                                                r.sMT_TYPE.ToString().Contains(searchPharse.ToLower()) ||
                                                r.sMT_PRICE.ToString().Contains(searchPharse.ToLower()) ||
                                                r.sMT_DISCOUNT.ToString().Contains(searchPharse.ToLower()) ||
                                                r.sMT_SGST.ToString().Contains(searchPharse.ToLower()) ||
                                                r.sMT_CGST.ToString().Contains(searchPharse.ToLower()) ||
                                                r.sMT_TOTALAMOUNT.ToString().Contains(searchPharse.ToLower()) ||
                                                r.sMT_NETAMOUNT.ToString().Contains(searchPharse.ToLower()) ||
                                                
                                                r.sMT_SORTORDER.ToString().Contains(searchPharse.ToLower()))
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
            return View(materialconsumptionlist);
        }

        public async Task<ActionResult> Create()
        {
            await ItemGroupCombo();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(Materialconsumption materialcategory)
        {
            try
            {
                var MaterialPostURL = ConfigurationManager.AppSettings["MATERIALCONSUMPTIONSERVICEPOST"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""sMT_UOM"": ""{materialcategory.tM_UOM}"",           
            ""sMT_TYPE"": ""{Session["Type"]}"",           
            ""sMT_QUANTITY"": ""{materialcategory.tM_QUANTITY}"",           
            ""sMT_PRICE"": ""{materialcategory.tM_PRICE}"",                    
            ""sMT_DISCOUNT"": ""{materialcategory.tM_DISCOUNT}"",                    
            ""sMT_TOTALAMOUNT"": ""{materialcategory.tM_TOTALAMOUNT}"",                    
            ""sMT_CGST"": ""{materialcategory.tM_CGST}"",                    
            ""sMT_SGST"": ""{materialcategory.tM_SGST}"",                    
            ""sMT_NETAMOUNT"": ""{materialcategory.tM_NETAMOUNT}"",                    
            ""sMT_CTRECID"": ""{Session["CT_RECID"]}"",                    
            ""sMT_CRECID"": ""{Session["CompanyID"]}"",                    
            ""sMT_MCRECID"": ""{materialcategory.SelectedItemCategory}"",                    
            ""sMT_MGRECID"": ""{materialcategory.SelectedItemGroup}"",                    
            ""sMT_MRECID"": ""{materialcategory.SelectedMaterial}"",                    
            ""sMT_SORTORDER"": ""{materialcategory.tM_SORTORDER}"",                    
            ""sMT_BILLABLE"": ""{"Y"}"",            
            ""sMT_DISABLE"": ""{"N"}""                
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(MaterialPostURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<MaterialpObjects>(responseBody);

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

        public async Task<ActionResult> Edit(int? sMT_RECID,string I_DESCRIPTION,int? sMT_CTRECID)
        {
            Session["sMT_RECID"] = sMT_RECID;
            Session["I_DESCRIPTION"] = I_DESCRIPTION;
            Session["I_DESCRIPTION"] = I_DESCRIPTION;
            Session["sMT_CTRECID"] = sMT_CTRECID;


            string WEBURLGETBYID = ConfigurationManager.AppSettings["MATERIALCONSUMPTIONSERVICEBYID"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            ServiceMaterialConsumption materialconsumption = null;

            string strparams = "Recid=" + sMT_RECID + "&cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGETBYID + "?" + strparams;

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
                        var response = await client.GetAsync(finalurl);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var content = JsonConvert.DeserializeObject<ServiceMaterialConsumptionObjects>(jsonString);
                            materialconsumption = content.Data;
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
            return View(materialconsumption);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(ServiceMaterialConsumption materialcategory)
        {
            try
            {
                var MaterialconsumptionUpdateURL = ConfigurationManager.AppSettings["MATERIALCONSUMPTIONSERVICEPUT"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""sMT_RECID"": ""{Session["sMT_RECID"]}"",           
            ""sMT_UOM"": ""{materialcategory.sMT_UOM}"",           
            ""sMT_TYPE"": ""{Session["Type"]}"",           
            ""sMT_QUANTITY"": ""{materialcategory.sMT_QUANTITY}"",           
            ""sMT_PRICE"": ""{materialcategory.sMT_PRICE}"",                    
            ""sMT_DISCOUNT"": ""{materialcategory.sMT_DISCOUNT}"",                    
            ""sMT_TOTALAMOUNT"": ""{materialcategory.sMT_TOTALAMOUNT}"",                    
            ""sMT_CGST"": ""{materialcategory.sMT_CGST}"",                    
            ""sMT_SGST"": ""{materialcategory.sMT_SGST}"",                    
            ""sMT_NETAMOUNT"": ""{materialcategory.sMT_NETAMOUNT}"",                    
            ""sMT_CTRECID"": ""{Session["sMT_CTRECID"]}"",                    
            ""sMT_CRECID"": ""{Session["CompanyID"]}"",                    
            ""sMT_MCRECID"": ""{materialcategory.sMT_MCRECID}"",                    
            ""sMT_MGRECID"": ""{materialcategory.sMT_MGRECID}"",                    
            ""sMT_MRECID"": ""{materialcategory.sMT_MRECID}"",                    
            ""sMT_SORTORDER"": ""{materialcategory.sMT_SORTORDER}"",                    
            ""sMT_BILLABLE"": ""{"Y"}"" ,               
            ""sMT_DISABLE"": ""{"N"}""                
        }}";
                  //""sMT_BILLABLE"": ""{(materialcategory.sMT_BILLABLE ? "Y" : "N")}"" ,           
                // if any error came kindly replace before
                //before  ""tM_MRECID"": ""{materialcategory.tM_MRECID}"",  
                //after  ""tM_MRECID"": ""{materialcategory.M_RECID}"",  
                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(MaterialconsumptionUpdateURL),
                    Method = HttpMethod.Put,
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
                // Send the request and await the response
                var response = await client.SendAsync(request);
                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ServiceMaterialConsumptionObjects>(responseBody);

                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { success = true, message = apiResponse.Message });
                    }
                    else
                    {
                        return Json(new { success = false, message = apiResponse.Message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Error: Something went wrong." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception: " + ex.Message });
            }
        }
        public async Task<ActionResult> Delete(int sMT_RECID)
        {
            string WEBURLDELETE = ConfigurationManager.AppSettings["MATERIALCONSUMPTIONSERVICEDELETE"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string strparams = "cmprecid=" + Session["CompanyID"] + "&RECID=" + sMT_RECID;
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
                            var apiResponse = JsonConvert.DeserializeObject<MaterialconsumptionObjects>(responseBody);

                            if (apiResponse.Status == "Y")
                            {

                                string redirectUrl = Url.Action("List", "ServiceMaterialConsumption", new { });
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

        public async Task<ActionResult> ItemGroupCombo()
        {
            var viewModel = new Materialconsumption();
            List<Materialconsumption> itemgroup = new List<Materialconsumption>();

            string webUrlGet = ConfigurationManager.AppSettings["ITEMGROUPCOMBO"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];

            if (Session["APIKEY"] == null || Session["CompanyID"] == null)
            {
                ModelState.AddModelError(string.Empty, "Session expired. Please login again.");
                return View(viewModel);
            }

            string APIKey = Session["APIKEY"].ToString();
            string strparams = "companyId=" + Session["CompanyID"];
            string url = webUrlGet + "?" + strparams;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                        client.DefaultRequestHeaders.Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects =
                                JsonConvert.DeserializeObject<MaterialconsumptionRootObject>(jsonString);

                            itemgroup = rootObjects?.Data ?? new List<Materialconsumption>();

                            viewModel.ItemGroups = itemgroup.Select(item => new SelectListItem
                            {
                                Value = item.IG_RECID.ToString(),
                                Text = item.IG_DESCRIPTION
                            }).ToList();
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty,
                                "Error: " + response.ReasonPhrase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty,
                    "Exception occurred: " + ex.Message);
            }

            return View(viewModel);
        }

        public async Task<ActionResult> ItemGroupCategoryCombo(int? Grouprecid)
        {
            List<Materialconsumption> itemcategory = new List<Materialconsumption>();

            string webUrlGet = ConfigurationManager.AppSettings["ITEMCATEGORYCOMBO"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "companyId=" + Session["CompanyID"] + "&GroupRecid=" + Grouprecid;
            string url = webUrlGet + "?" + strparams;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (s, c, ch, e) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                        var response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<MaterialconsumptionRootObject>(jsonString);
                            itemcategory = rootObjects?.Data ?? new List<Materialconsumption>();


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return Json(itemcategory, JsonRequestBehavior.AllowGet);
        }



        public async Task<ActionResult> ItemCombo(int? Categoryrecid)
        {
            List<Materialconsumption> itemcategory = new List<Materialconsumption>();

            string webUrlGet = ConfigurationManager.AppSettings["ITEMCOMBO"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "companyId=" + Session["CompanyID"] + "&CategoryRecid=" + Categoryrecid;
            string url = webUrlGet + "?" + strparams;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (s, c, ch, e) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                        var response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<MaterialconsumptionRootObject>(jsonString);
                            itemcategory = rootObjects?.Data ?? new List<Materialconsumption>();


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return Json(itemcategory, JsonRequestBehavior.AllowGet);
        }
    }
}