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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class CompanyInfoController : Controller
    {
        // GET: CompanyInfo
        [HttpGet]
        public async Task<ActionResult> Info()
        {
            CompanyInfo companyinfo = null;

            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"].ToString();
            string Weburl = ConfigurationManager.AppSettings["COMPANYINFOGET"];
            string Url = Weburl + "?CmpRECID=" + Session["CompanyId"];

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
                        var response = await client.GetAsync(Url);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<ApiResponseInfo>(jsonString);
                            companyinfo = rootObjects.Data.FirstOrDefault();
                            ViewBag.Logo = rootObjects.Data[0].C_LOGO;
                            Session["Logo"] = rootObjects.Data[0].C_LOGO;
                            //if (logoBytes != null)
                            //{
                            //    string base64Logo = Convert.ToBase64String(logoBytes);
                            //    ViewBag.Logo = base64Logo;
                            //}
                            await LocationList();
                            return View(companyinfo);
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
            await LocationList();
            return View();
        }


        private byte[] ConvertToByteArray(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    return binaryReader.ReadBytes(file.ContentLength);
                }
            }
            else
            {
                // Log an error if the file is missing
                Console.WriteLine("No file uploaded or file is empty.");
            }
            return null;
        }


        [HttpPost]
        public async Task<ActionResult> UpdateLogo(CompanyInfo CompanyInfo, HttpPostedFileBase C_LOGO)
        {
            string companyId = Session["CompanyId"].ToString();
            var MaterialcatPostURL = ConfigurationManager.AppSettings["UpdateLOGO"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            // Define byte arrays for the certificates
            byte[] btgstCertificateData = null;

            // Check for file uploads, otherwise use existing Base64 from the form hidden fields
            if (C_LOGO != null && C_LOGO.ContentLength > 0)
            {
                btgstCertificateData = ConvertToByteArray(C_LOGO);
            }
            else
            {
                // No new file uploaded, use existing image data from the hidden field
                if (!string.IsNullOrEmpty(Request.Form["ExistingGstImage"]))
                {
                    btgstCertificateData = Convert.FromBase64String(Request.Form["ExistingGstImage"]);
                }
            }

            // Validate that all attachments are available (either uploaded or from hidden fields)
           
            bool blresult = true;
           
            if (blresult == true)
            {

                try
                {


                    var content = $@"{{
            ""c_RECID"": {companyId},
            ""c_LOGO"": ""{(btgstCertificateData != null ? Convert.ToBase64String(btgstCertificateData) : Request.Form["ExistingGstImage"])}""
           
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

                    var client = new HttpClient(handler);
                    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponseInfo>(responseBody);

                        string message = apiResponse.Message;

                        if (apiResponse.Status == "Y")
                        {
                            return Json(new { status = "success", message = "CompanyLogo Updated Successfully" });
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
                            return RedirectToAction("Info", "CompanyInfo", new { id = companyId });

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
            }
            return View();
        }

        public async Task<ActionResult> LocationList()
        {
           
            string Weburl = ConfigurationManager.AppSettings["LOCATIONS"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Locations> Locationslist = new List<Locations>();

            string strparams = "cmprecid=" + Session["CompanyID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<LocationsObjects>(jsonString);
                            Locationslist = rootObjects.Data;
                            ViewBag.LocationList = Locationslist;
                            //if (!string.IsNullOrEmpty(searchPharse))
                            //{
                            //    projectmasterlist = projectmasterlist
                            //        .Where(r => r.CU_CODE.ToLower().Contains(searchPharse.ToLower()) ||
                            //                    r.CU_EMAIL.ToString().Contains(searchPharse.ToLower()) ||
                            //                    r.CU_NAME.ToString().Contains(searchPharse.ToLower()) ||
                            //                    r.CU_MOBILENO.ToString().Contains(searchPharse.ToLower()) ||
                            //                    r.CU_INVOICENO.ToString().Contains(searchPharse.ToLower()) ||
                            //                    r.CU_WARRANTYFREECALLS.ToString().Contains(searchPharse.ToLower()) ||
                            //                    r.CU_WARRANTYUPTO.ToString().Contains(searchPharse.ToLower()) ||
                            //                    r.CU_SORTORDER.ToString().Contains(searchPharse.ToLower()))
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
            return View(Locationslist);
        }
    }
}