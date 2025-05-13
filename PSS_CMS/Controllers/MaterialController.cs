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
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    public class MaterialController : Controller
    {
        // GET: Material
        public async Task<ActionResult> List(string searchPharse,int? Recid,string MC_DESCRIPTION)
        {
            if(Recid!=null && MC_DESCRIPTION != null)
            {
                Session["MC_Recid"] = Recid;
                Session["MC_DESCRIPTION"] = MC_DESCRIPTION;
            }
          
            Material objmaterial = new Material();

            string Weburl = ConfigurationManager.AppSettings["MATERIALGET"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Material> materiallist = new List<Material>();

            string strparams = "cmprecid=" + Session["CompanyID"]+ "&Recid=" + Session["MC_Recid"];
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
                            var rootObjects = JsonConvert.DeserializeObject<MaterialRootObject>(jsonString);
                            materiallist = rootObjects.Data;

                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                materiallist = materiallist
                                    .Where(r => r.M_CODE.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.M_NAME.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.M_UOM.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.M_QUANTITY.ToString().Contains(searchPharse.ToLower()) ||
                                                r.M_PRICE.ToString().Contains(searchPharse.ToLower()) ||
                                                r.M_DISCOUNT.ToString().Contains(searchPharse.ToLower()) ||
                                                r.M_CGST.ToString().Contains(searchPharse.ToLower()) ||
                                                r.M_SGST.ToString().Contains(searchPharse.ToLower()) ||
                                                r.M_TOTALAMOUNT.ToString().Contains(searchPharse.ToLower()) ||
                                                r.M_NETAMOUNT.ToString().Contains(searchPharse.ToLower()) ||
                                                r.M_SORTORDER.ToString().Contains(searchPharse.ToLower()))
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
            return View(materiallist);
        }

        public async Task<ActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(Material material)
        {
            try
            {
                var MaterialPostURL = ConfigurationManager.AppSettings["MATERIALPOST"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""m_CODE"": ""{material.M_CODE}"",           
            ""m_NAME"": ""{material.M_NAME}"",           
            ""m_MCRECID"": ""{Session["MC_Recid"]}"",                    
            ""m_UOM"": ""{material.M_UOM}"",                    
            ""m_QUANTITY"": ""{material.M_QUANTITY}"",                    
            ""m_PRICE"": ""{material.M_PRICE}"",                    
            ""m_DISCOUNT"": ""{material.M_DISCOUNT}"",                    
            ""m_TOTALAMOUNT"": ""{material.M_TOTALAMOUNT}"",                    
            ""m_CGST"": ""{material.M_CGST}"",                    
            ""m_SGST"": ""{material.M_SGST}"",                    
            ""m_NETAMOUNT"": ""{material.M_NETAMOUNT}"",                    
            ""m_SORTORDER"": ""{material.M_SORTORDER}"",                    
            ""m_CRECID"": ""{Session["CompanyID"]}"",                    
            ""m_DISABLE"": ""{(material.IsDisabled ? "Y" : "N")}""                
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

        public async Task<ActionResult> Edit(int? Recid, string MName)
        {
            Session["MName"] = MName;
            Session["M_RECID"] = Recid;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["MATERIALBYID"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            Material material = null;

            string strparams = "Recid=" + Recid + "&cmprecid=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<MaterialpObjects>(jsonString);
                            material = content.Data;
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

            return View(material);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(Material material)
        {
            try
            {
                var MaterialUpdateURL = ConfigurationManager.AppSettings["MATERIALPUT"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""m_RECID"": ""{Session["M_RECID"]}"",           
            ""m_CODE"": ""{material.M_CODE}"",           
            ""m_NAME"": ""{material.M_NAME}"",           
            ""m_MCRECID"": ""{Session["MC_Recid"]}"",                    
            ""m_UOM"": ""{material.M_UOM}"",                    
            ""m_QUANTITY"": ""{material.M_QUANTITY}"",                    
            ""m_PRICE"": ""{material.M_PRICE}"",                    
            ""m_DISCOUNT"": ""{material.M_DISCOUNT}"",                    
            ""m_TOTALAMOUNT"": ""{material.M_TOTALAMOUNT}"",                    
            ""m_CGST"": ""{material.M_CGST}"",                    
            ""m_SGST"": ""{material.M_SGST}"",                    
            ""m_NETAMOUNT"": ""{material.M_NETAMOUNT}"",                    
            ""m_SORTORDER"": ""{material.M_SORTORDER}"",                    
            ""m_CRECID"": ""{Session["CompanyID"]}"",                    
            ""m_DISABLE"": ""{(material.IsDisabled ? "Y" : "N")}""                                
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(MaterialUpdateURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<MaterialcategorypObjects>(responseBody);

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

            string WEBURLDELETE = ConfigurationManager.AppSettings["MATERIALDELETE"];
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
                            var apiResponse = JsonConvert.DeserializeObject<MaterialcategorypObjects>(responseBody);

                            if (apiResponse.Status == "Y")
                            {

                                string redirectUrl = Url.Action("List", "Material", new { });
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
    }
}