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
    [ApiKeyAuthorize]
    public class MaterialConsumptionController : Controller
    {
        // GET: MaterialConsumption
        public async Task<ActionResult> List(string searchPharse,int? TC_RECID,string Type,int? P_RECID)
        {
            if (TC_RECID!=null && Type!=null && P_RECID!=null)
            {
                Session["TC_RECID"] = TC_RECID;
                Session["Type"] = Type;
                Session["P_RECID"] = P_RECID;
            }

            Materialconsumption objmaterialconsumption = new Materialconsumption();

            string Weburl = ConfigurationManager.AppSettings["MATERIALCONSUMPTIONGET"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Materialconsumption> materialconsumptionlist = new List<Materialconsumption>();

            string strparams = "cmprecid=" + Session["CompanyID"]+ "&ticketrecid=" + Session["TC_RECID"] + "&type=" + Session["Type"];
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
                            var rootObjects = JsonConvert.DeserializeObject<MaterialconsumptionRootObject>(jsonString);
                            materialconsumptionlist = rootObjects.Data;

                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                materialconsumptionlist = materialconsumptionlist
                                    .Where(r => r.tM_UOM.ToString().Contains(searchPharse.ToLower()) ||
                                                r.tM_QUANTITY.ToString().Contains(searchPharse.ToLower()) ||
                                                r.tM_TYPE.ToString().Contains(searchPharse.ToLower()) ||
                                                r.tM_PRICE.ToString().Contains(searchPharse.ToLower()) ||
                                                r.tM_DISCOUNT.ToString().Contains(searchPharse.ToLower()) ||
                                                r.tM_SGST.ToString().Contains(searchPharse.ToLower()) ||
                                                r.tM_CGST.ToString().Contains(searchPharse.ToLower()) ||
                                                r.tM_TOTALAMOUNT.ToString().Contains(searchPharse.ToLower()) ||
                                                r.tM_NETAMOUNT.ToString().Contains(searchPharse.ToLower()) ||
                                                r.tM_SORTORDER.ToString().Contains(searchPharse.ToLower()))
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
                var MaterialPostURL = ConfigurationManager.AppSettings["MATERIALCONSUMPTIONPOST"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""tM_UOM"": ""{materialcategory.tM_UOM}"",           
            ""tM_TYPE"": ""{Session["Type"]}"",           
            ""tM_QUANTITY"": ""{materialcategory.tM_QUANTITY}"",           
            ""tM_PRICE"": ""{materialcategory.tM_PRICE}"",                    
            ""tM_DISCOUNT"": ""{materialcategory.tM_DISCOUNT}"",                    
            ""tM_TOTALAMOUNT"": ""{materialcategory.tM_TOTALAMOUNT}"",                    
            ""tM_CGST"": ""{materialcategory.tM_CGST}"",                    
            ""tM_SGST"": ""{materialcategory.tM_SGST}"",                    
            ""tM_NETAMOUNT"": ""{materialcategory.tM_NETAMOUNT}"",                    
            ""tM_TCRECID"": ""{Session["TC_RECID"]}"",                    
            ""tM_CRECID"": ""{Session["CompanyID"]}"",                    
            ""tM_MCRECID"": ""{materialcategory.SelectedItemCategory}"",                    
            ""tM_MGRECID"": ""{materialcategory.SelectedItemGroup}"",                    
            ""tM_MRECID"": ""{materialcategory.SelectedMaterial}"",                    
            ""tM_SORTORDER"": ""{materialcategory.tM_SORTORDER}"",                    
            ""tM_BILLABLE"": ""{(materialcategory.tM_BILLABLE ? "Y" : "N")}"" ,               
            ""tM_DISABLE"": ""{"N"}""                
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

        public async Task<ActionResult> Edit(int? MACRecid, string MACNAME,int? TICRECID,int? TM_MCRECID,int? TM_MRECID,int? TM_MGRECID)
        {
            Session["MACRecid"] = MACRecid;
            Session["MACNAME"] = MACNAME;
            Session["TICRECID"] = TICRECID;
            Session["TM_MCRECID"] = TM_MCRECID;
            Session["TM_MRECID"] = TM_MRECID;
            Session["TM_MGRECID"] = TM_MGRECID;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["MATERIALCONSUMPTIONBYID"];
             string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            Materialconsumption materialconsumption = null;

            string strparams = "Recid=" + MACRecid + "&cmprecid=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<MaterialconsumptionObjects>(jsonString);
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
        public async Task<ActionResult>Edit(Materialconsumption materialcategory)
        {
            try
            {
                var MaterialconsumptionUpdateURL = ConfigurationManager.AppSettings["MATERIALCONSUMPTIONPUT"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""tM_RECID"": ""{Session["MACRecid"]}"",           
            ""tM_UOM"": ""{materialcategory.tM_UOM}"",           
            ""tM_TYPE"": ""{Session["Type"]}"",           
            ""tM_QUANTITY"": ""{materialcategory.tM_QUANTITY}"",           
            ""tM_PRICE"": ""{materialcategory.tM_PRICE}"",                    
            ""tM_DISCOUNT"": ""{materialcategory.tM_DISCOUNT}"",                    
            ""tM_TOTALAMOUNT"": ""{materialcategory.tM_TOTALAMOUNT}"",                    
            ""tM_CGST"": ""{materialcategory.tM_CGST}"",                    
            ""tM_SGST"": ""{materialcategory.tM_SGST}"",                    
            ""tM_NETAMOUNT"": ""{materialcategory.tM_NETAMOUNT}"",                    
            ""tM_TCRECID"": ""{Session["TC_RECID"]}"",                    
            ""tM_CRECID"": ""{Session["CompanyID"]}"",                    
            ""tM_MCRECID"": ""{materialcategory.tM_MCRECID}"",                    
            ""tM_MGRECID"": ""{materialcategory.tM_MGRECID}"",                    
            ""tM_MRECID"": ""{materialcategory.tM_MRECID}"",                    
            ""tM_SORTORDER"": ""{materialcategory.tM_SORTORDER}"",                    
            ""tM_BILLABLE"": ""{(materialcategory.tM_BILLABLE ? "Y" : "N")}"" ,               
            ""tM_DISABLE"": ""{"N"}""                
        }}";

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
                    var apiResponse = JsonConvert.DeserializeObject<MaterialconsumptionObjects>(responseBody);

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
        public async Task<ActionResult> Delete(int Recid)
        {
            string WEBURLDELETE = ConfigurationManager.AppSettings["MATERIALCONSUMPTIONDELETE"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string strparams = "cmprecid=" + Session["CompanyID"] + "&RECID=" + Recid;
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

                                string redirectUrl = Url.Action("List", "MaterialConsumption", new { });
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

        //public async Task<JsonResult> ComboMaterial()
        //{
        //    List<Materialconsumption> materialList = new List<Materialconsumption>();

        //    string webUrlGet = ConfigurationManager.AppSettings["MATERIALTYPECOMBO"];
        //    string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
        //    string APIKey = Session["APIKEY"]?.ToString();
        //    string cmpRecId = Session["CompanyID"]?.ToString();
        //    string strparams = "cmprecid=" + cmpRecId + "&type=" + Session["Type"] + "&productrecid=" + Session["P_RECID"];
        //    string url = webUrlGet + "?" + strparams;

        //    try
        //    {
        //        using (HttpClientHandler handler = new HttpClientHandler())
        //        {
        //            handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        //            using (HttpClient client = new HttpClient(handler))
        //            {
        //                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //                var response = await client.GetAsync(url);
        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var jsonString = await response.Content.ReadAsStringAsync();
        //                    var rootObjects = JsonConvert.DeserializeObject<MaterialconsumptionRootObject>(jsonString);

        //                    if (rootObjects?.Data != null)
        //                    {
        //                        materialList = rootObjects.Data.Select(m => new Materialconsumption
        //                        {
        //                            M_RECID = m.M_RECID,
        //                            M_NAME = m.M_NAME,
        //                            M_UOM = m.M_UOM,
        //                            M_QUANTITY = m.M_QUANTITY,
        //                            M_PRICE = m.M_PRICE,
        //                            M_DISCOUNT = m.M_DISCOUNT,
        //                            M_TOTALAMOUNT = m.M_TOTALAMOUNT,
        //                            M_CGST = m.M_CGST,
        //                            M_SGST = m.M_SGST,
        //                            M_NETAMOUNT = m.M_NETAMOUNT,
        //                            M_SORTORDER = m.M_SORTORDER,
        //                            M_TYPE = m.M_TYPE
        //                        }).ToList();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(materialList, JsonRequestBehavior.AllowGet);
        //}


        //public async Task<JsonResult> ComboMaterialEdit()
        //{
        //    List<Materialconsumption> MaterialList = new List<Materialconsumption>(); // Use your full model here

        //    string webUrlGet = ConfigurationManager.AppSettings["MATERIALTYPECOMBO"];
        //    string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
        //    string APIKey = Session["APIKEY"]?.ToString();
        //    string cmpRecId = Session["CompanyID"]?.ToString();
        //    string strparams = "cmprecid=" + cmpRecId + "&type=" + Session["Type"] + "&productrecid=" + Session["P_RECID"];
        //    string url = webUrlGet + "?" + strparams;

        //    try
        //    {
        //        using (HttpClientHandler handler = new HttpClientHandler())
        //        {
        //            handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        //            using (HttpClient client = new HttpClient(handler))
        //            {
        //                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //                var response = await client.GetAsync(url);
        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var jsonString = await response.Content.ReadAsStringAsync();
        //                    var rootObjects = JsonConvert.DeserializeObject<MaterialconsumptionRootObject>(jsonString);

        //                    if (rootObjects?.Data != null)
        //                    {
        //                        MaterialList = rootObjects.Data.ToList(); // Move assignment to outer scope
                               
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(MaterialList, JsonRequestBehavior.AllowGet);
        //}


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