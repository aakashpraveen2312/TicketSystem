using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    public class PartyController : Controller
    {
        // GET: Party
        public async Task<ActionResult> List(string searchPharse)
        {
            Party objitems = new Party();

            int SerialNo = objitems.SerialNumber;

            if (SerialNo == 0)
            {
                SerialNo = 1; // Initialize to 1 if it's 0
            }
            string Weburl = ConfigurationManager.AppSettings["PARTYGET"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Party> partylist = new List<Party>();

            string strparams = "CompanyRecID=" + Session["CompanyID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<PartyObjects>(jsonString);
                            partylist = rootObjects.Data;
                            if (partylist.Count > 0)
                            {
                                // Assign serial numbers
                                for (int i = 0; i < partylist.Count; i++)
                                {
                                    partylist[i].SerialNumber = i + 1;
                                }
                            }
                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                partylist = partylist
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
            return View(partylist);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(Party party, string P_IGST, HttpPostedFileBase GSTCertificate,
            HttpPostedFileBase IGSTCertificate,HttpPostedFileBase TINCertificate,HttpPostedFileBase PanCertificate)
        {
            try
            {
                var PostURL = ConfigurationManager.AppSettings["PARTYPOST"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                byte[] gstfileBytes = null;
                byte[] igstfileBytes = null;
                byte[] tinfileBytes = null;
                byte[] panfileBytes = null;

                string gstattachmentBase64 = null;
                string igstattachmentBase64 = null;
                string tinattachmentBase64 = null;
                string panattachmentBase64 = null;

                if (GSTCertificate != null && GSTCertificate.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await GSTCertificate.InputStream.CopyToAsync(ms);
                        gstfileBytes = ms.ToArray();
                    }

                    // Convert byte[] → Base64 for JSON
                    gstattachmentBase64 = Convert.ToBase64String(gstfileBytes);
                }
                if (IGSTCertificate != null && IGSTCertificate.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await IGSTCertificate.InputStream.CopyToAsync(ms);
                        igstfileBytes = ms.ToArray();
                    }

                    // Convert byte[] → Base64 for JSON
                    igstattachmentBase64 = Convert.ToBase64String(igstfileBytes);
                }
                if (TINCertificate != null && TINCertificate.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await TINCertificate.InputStream.CopyToAsync(ms);
                        tinfileBytes = ms.ToArray();
                    }

                    // Convert byte[] → Base64 for JSON
                    tinattachmentBase64 = Convert.ToBase64String(tinfileBytes);
                }
                if (PanCertificate != null && PanCertificate.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await PanCertificate.InputStream.CopyToAsync(ms);
                        panfileBytes = ms.ToArray();
                    }

                    // Convert byte[] → Base64 for JSON
                    panattachmentBase64 = Convert.ToBase64String(panfileBytes);
                }

                var content = $@"{{           
            ""p_CODE"": ""{""}"",           
            ""p_NAME"": ""{party.P_NAME}"",           
            ""P_ADDRESS1"": ""{HttpUtility.JavaScriptStringEncode(party.P_ADDRESS1)}"",        
            ""p_POSTALCODE"": ""{ party.P_POSTALCODE}"",                    
            ""p_MOBILE"": ""{ party.P_MOBILE}"",                                       
            ""p_ALTERNATEMOBILE"": ""{ party.P_ALTERNATEMOBILE}"",                    
            ""p_TELEPHONE"": ""{ party.P_TELEPHONE}"",                    
            ""p_EMAIL"": ""{ party.P_EMAIL}"",                    
            ""p_ALTERNATEMAIL"": ""{ party.P_ALTERNATEMAIL}"",                    
            ""p_GSTNUMBER"": ""{ party.P_GSTNUMBER}"",                    
            ""p_GSTATTACHMENT"": ""{(gstattachmentBase64)}"",        
            ""p_IGSTNUMBER"": ""{(party.P_IGSTNUMBER)}"",        
            ""p_IGSTATTACHMENT"": ""{(igstattachmentBase64)}"",        
            ""p_PAN"": ""{(party.P_PAN)}"",        
            ""p_PANATTACHMENT"": ""{(panattachmentBase64)}"",        
            ""p_FAXNUMBER"": ""{(party.P_FAXNUMBER)}"",        
            ""p_CONTACTPERSON1"": ""{(party.P_CONTACTPERSON1)}"",        
            ""p_CONTACTPERSON1MOBILE"": ""{(party.P_CONTACTPERSON1MOBILE)}"",        
            ""p_CONTACTPERSON2"": ""{(party.P_CONTACTPERSON2)}"",        
            ""p_CONTACTPERSON2MOBILE"": ""{(party.P_CONTACTPERSON2MOBILE)}"",        
            ""p_CONTACTPERSON1EMAIL"": ""{(party.P_CONTACTPERSON1EMAIL)}"",        
            ""p_CONTACTPERSON2EMAIL"": ""{(party.P_CONTACTPERSON2EMAIL)}"",        
            ""p_TINNUMBER"": ""{(party.P_TINNUMBER)}"",        
            ""p_TINATTACHMENT"": ""{(tinattachmentBase64)}"",        
            ""p_IGST"": ""{(P_IGST)}"",        
            ""p_AUTHSIGNNAME"": ""{(party.P_AUTHSIGNNAME)}"",        
            ""p_AUTHPOSITION"": ""{(party.P_AUTHPOSITION)}"",        
            ""p_AUTHSIGNATTACHMENT"": ""{(party.P_AUTHSIGNATTACHMENT)}"",        
            ""p_HEADERIMGATTACHMENT"": ""{(party.P_HEADERIMGATTACHMENT)}"",        
            ""p_FOOTERIMGATTACHMENT"": ""{(party.P_FOOTERIMGATTACHMENT)}"",        
            ""p_FLGCUSTOMER"": ""{(party.IscustomerDisabled ? "Y" : "N")}"",
            ""p_FLGSUPPLIER"": ""{(party.IsSupplierDisabled ? "Y" : "N")}"",
            ""p_FLGVENDOR"": ""{(party.IsVendorDisabled ? "Y" : "N")}"",
            ""p_FLGDISTRIBUTOR"": ""{(party.IsDistributorDisabled ? "Y" : "N")}"",
            ""p_FLGEMPLOYEE"": ""{(party.IsEmployeeDisabled ? "Y" : "N")}"",
            ""p_SORTORDER"": ""{(party.P_SORTORDER)}"",        
            ""P_DISABLE"": ""{(party.IsDisabled ? "Y" : "N")}"" ,    
            ""p_CORECID"": ""{Session["CompanyID"]}"",        
            ""p_BANKNAME"": ""{(party.P_BANKNAME)}"",        
            ""p_BRANCHNAME"": ""{(party.P_BRANCHNAME)}"",        
            ""p_ACCOUNTNAME"": ""{(party.P_ACCOUNTNAME)}"",        
            ""p_ACCOUNTNO"": ""{(party.P_ACCOUNTNO)}"",        
            ""p_ACCOUNTTYPE"": ""{(party.P_ACCOUNTTYPE)}"",        
            ""p_IFSCCODE"": ""{(party.P_IFSCCODE)}"",        
            ""p_BANKLOCATION"": ""{(party.P_BANKLOCATION)}"",        
            ""p_BANKADDRESS"": ""{(party.P_BANKADDRESS)}"",        
            ""p_VERIFIEDWHATSAPP"": ""{(party.IsVERIFIEDWHATSAPPDisabled ? "Y" : "N")}"",
            ""p_VERIFIEDEMAIL"": ""{(party.IsVERIFIEDEMAILDisabled ? "Y" : "N")}"",   
            ""p_Verify"": ""{"N"}""        
                  
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(PostURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<PartyObjects>(responseBody);

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
        public async Task<ActionResult> Edit(int? P_RECID,string P_NAME)
        {
            Session["PARTY_RECID"] = P_RECID;
            Session["PARTY_NAME"] = P_NAME;

            string WEBURLGETBYID = ConfigurationManager.AppSettings["PARTYGETBYID"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            Party party = null;

            string strparams = "Recid=" + P_RECID + "&CompanyRecID=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<PartyObject>(jsonString);
                            party = content.Data;
                            if (!string.IsNullOrEmpty(party?.P_GSTATTACHMENT))
                            {
                                ViewBag.GstImageBase64 = party.P_GSTATTACHMENT;

                            }

                            if (!string.IsNullOrEmpty(party?.P_IGSTATTACHMENT))
                            {
                                ViewBag.IGstImageBase64 = party.P_IGSTATTACHMENT;

                            }

                            if (!string.IsNullOrEmpty(party?.P_TINATTACHMENT))
                            {
                                ViewBag.TinImageBase64 = party.P_TINATTACHMENT;
                            }

                            if (!string.IsNullOrEmpty(party?.P_PANATTACHMENT))
                            {
                                ViewBag.PanImageBase64 = party.P_PANATTACHMENT;
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
                ModelState.AddModelError(string.Empty, "Exception occured: " + ex.Message);
            }
            return View(party);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(Party party,HttpPostedFileBase GSTCertificate,
            HttpPostedFileBase IGSTCertificate, HttpPostedFileBase TINCertificate, HttpPostedFileBase PanCertificate)
        {
            try
            {
                var UpdateURL = ConfigurationManager.AppSettings["PARTYPUT"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                byte[] gstfileBytes = null;
                byte[] igstfileBytes = null;
                byte[] tinfileBytes = null;
                byte[] panfileBytes = null;

                string gstattachmentBase64 = null;
                string igstattachmentBase64 = null;
                string tinattachmentBase64 = null;
                string panattachmentBase64 = null;

                if (GSTCertificate != null && GSTCertificate.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await GSTCertificate.InputStream.CopyToAsync(ms);
                        gstfileBytes = ms.ToArray();
                    }

                    // Convert byte[] → Base64 for JSON
                    gstattachmentBase64 = Convert.ToBase64String(gstfileBytes);
                }
                else if (!string.IsNullOrEmpty(Request.Form["ExistingGstImage"]))
                {
                    gstattachmentBase64 = Request.Form["ExistingGstImage"];
                }

                if (IGSTCertificate != null && IGSTCertificate.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await IGSTCertificate.InputStream.CopyToAsync(ms);
                        igstfileBytes = ms.ToArray();
                    }

                    // Convert byte[] → Base64 for JSON
                    igstattachmentBase64 = Convert.ToBase64String(igstfileBytes);
                }
                else if (!string.IsNullOrEmpty(Request.Form["ExistingIGstImage"]))
                {
                    igstattachmentBase64 = Request.Form["ExistingIGstImage"];
                }
                if (TINCertificate != null && TINCertificate.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await TINCertificate.InputStream.CopyToAsync(ms);
                        tinfileBytes = ms.ToArray();
                    }

                    // Convert byte[] → Base64 for JSON
                    tinattachmentBase64 = Convert.ToBase64String(tinfileBytes);
                }
                else if (!string.IsNullOrEmpty(Request.Form["ExistingTinImage"]))
                {
                    tinattachmentBase64 = Request.Form["ExistingTinImage"];
                }

                if (PanCertificate != null && PanCertificate.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await PanCertificate.InputStream.CopyToAsync(ms);
                        panfileBytes = ms.ToArray();
                    }

                    // Convert byte[] → Base64 for JSON
                    panattachmentBase64 = Convert.ToBase64String(panfileBytes);
                }
                else if (!string.IsNullOrEmpty(Request.Form["ExistingPanImage"]))
                {
                    panattachmentBase64 = Request.Form["ExistingPanImage"];
                }

                var content = $@"{{           
            ""p_CODE"": ""{party.P_CODE}"",           
            ""p_RECID"": ""{(Session["PARTY_RECID"])}"",           
            ""p_NAME"": ""{party.P_NAME}"",           
            ""P_ADDRESS1"": ""{HttpUtility.JavaScriptStringEncode(party.P_ADDRESS1)}"",        
            ""p_POSTALCODE"": ""{ party.P_POSTALCODE}"",                    
            ""p_MOBILE"": ""{ party.P_MOBILE}"",                                       
            ""p_ALTERNATEMOBILE"": ""{ party.P_ALTERNATEMOBILE}"",                    
            ""p_TELEPHONE"": ""{ party.P_TELEPHONE}"",                    
            ""p_EMAIL"": ""{ party.P_EMAIL}"",                    
            ""p_ALTERNATEMAIL"": ""{ party.P_ALTERNATEMAIL}"",                    
            ""p_GSTNUMBER"": ""{ party.P_GSTNUMBER}"",                    
            ""p_GSTATTACHMENT"": ""{(gstattachmentBase64)}"",        
            ""p_IGSTNUMBER"": ""{(party.P_IGSTNUMBER)}"",        
            ""p_IGSTATTACHMENT"": ""{(igstattachmentBase64)}"",        
            ""p_PAN"": ""{(party.P_PAN)}"",        
            ""p_PANATTACHMENT"": ""{(panattachmentBase64)}"",        
            ""p_FAXNUMBER"": ""{(party.P_FAXNUMBER)}"",        
            ""p_CONTACTPERSON1"": ""{(party.P_CONTACTPERSON1)}"",        
            ""p_CONTACTPERSON1MOBILE"": ""{(party.P_CONTACTPERSON1MOBILE)}"",        
            ""p_CONTACTPERSON2"": ""{(party.P_CONTACTPERSON2)}"",        
            ""p_CONTACTPERSON2MOBILE"": ""{(party.P_CONTACTPERSON2MOBILE)}"",        
            ""p_CONTACTPERSON1EMAIL"": ""{(party.P_CONTACTPERSON1EMAIL)}"",        
            ""p_CONTACTPERSON2EMAIL"": ""{(party.P_CONTACTPERSON2EMAIL)}"",        
            ""p_TINNUMBER"": ""{(party.P_TINNUMBER)}"",        
            ""p_TINATTACHMENT"": ""{(tinattachmentBase64)}"",        
            ""p_IGST"": ""{(party.PIGSTEmail ? "Y" : "N")}"",        
            ""p_AUTHSIGNNAME"": ""{(party.P_AUTHSIGNNAME)}"",        
            ""p_AUTHPOSITION"": ""{(party.P_AUTHPOSITION)}"",        
            ""p_AUTHSIGNATTACHMENT"": ""{(party.P_AUTHSIGNATTACHMENT)}"",        
            ""p_HEADERIMGATTACHMENT"": ""{(party.P_HEADERIMGATTACHMENT)}"",        
            ""p_FOOTERIMGATTACHMENT"": ""{(party.P_FOOTERIMGATTACHMENT)}"",        
            ""p_FLGCUSTOMER"": ""{(party.IscustomerDisabled ? "Y" : "N")}"",
            ""p_FLGSUPPLIER"": ""{(party.IsSupplierDisabled ? "Y" : "N")}"",
            ""p_FLGVENDOR"": ""{(party.IsVendorDisabled ? "Y" : "N")}"",
            ""p_FLGDISTRIBUTOR"": ""{(party.IsDistributorDisabled ? "Y" : "N")}"",
            ""p_FLGEMPLOYEE"": ""{(party.IsEmployeeDisabled ? "Y" : "N")}"",
            ""p_SORTORDER"": ""{(party.P_SORTORDER)}"",        
            ""P_DISABLE"": ""{(party.IsDisabled ? "Y" : "N")}"" ,    
            ""p_CORECID"": ""{Session["CompanyID"]}"",        
            ""p_BANKNAME"": ""{(party.P_BANKNAME)}"",        
            ""p_BRANCHNAME"": ""{(party.P_BRANCHNAME)}"",        
            ""p_ACCOUNTNAME"": ""{(party.P_ACCOUNTNAME)}"",        
            ""p_ACCOUNTNO"": ""{(party.P_ACCOUNTNO)}"",        
            ""p_ACCOUNTTYPE"": ""{(party.P_ACCOUNTTYPE)}"",        
            ""p_IFSCCODE"": ""{(party.P_IFSCCODE)}"",        
            ""p_BANKLOCATION"": ""{(party.P_BANKLOCATION)}"",        
            ""p_BANKADDRESS"": ""{(party.P_BANKADDRESS)}"",        
            ""p_VERIFIEDWHATSAPP"": ""{(party.IsVERIFIEDWHATSAPPDisabled ? "Y" : "N")}"",
            ""p_VERIFIEDEMAIL"": ""{(party.IsVERIFIEDEMAILDisabled ? "Y" : "N")}"",   
            ""p_Verify"": ""{"N"}""        
                  
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(UpdateURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<PartyObjects>(responseBody);

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
        public async Task<ActionResult> Delete(int P_RECID)
        {
            string WEBURLDELETE = ConfigurationManager.AppSettings["PARTYDELETE"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string strparams = "companyId=" + Session["CompanyID"] + "&PARTY_RECID=" + P_RECID;
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
                            var apiResponse = JsonConvert.DeserializeObject<PartyObject>(responseBody);

                            if (apiResponse.Status == "Y")
                            {

                                string redirectUrl = Url.Action("List", "Party", new { });
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