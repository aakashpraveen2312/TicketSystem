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
    [ApiKeyAuthorize]
    public class ContractController : Controller
    {
        public async Task<ActionResult> ContractDashboard()
        {
            await GetContractExpiryCount();
            await SendContractExpiryMails();
            await SendFollowupReminderMails();
            string WEBURLGET = ConfigurationManager.AppSettings["CONTRACTDASHBOARD"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "CompanyRecID=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            Contract contract  = null;

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
                            contract = JsonConvert.DeserializeObject<Contract>(jsonString);

                            ViewBag.TotalContractAmount = contract.TotalContractAmount;
                            ViewBag.TotalPaidAmount = contract.TotalPaidAmount;
                            ViewBag.PendingAmount = contract.PendingAmount;


                            ViewBag.TotalContractAmount1 = Convert.ToDecimal(contract.TotalContractAmount).ToString("N0");
                            ViewBag.TotalPaidAmount1 = Convert.ToDecimal(contract.TotalPaidAmount).ToString("N0");
                            ViewBag.PendingAmount1 = Convert.ToDecimal(contract.PendingAmount).ToString("N0");
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
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
            await StackedBarChartCustomer();           
            return View(contract);
        }

        public async Task<ActionResult> StackedBarChartCustomer()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["CONTRACTSTACKEDBAR"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            List<Contract> dashboardData = new List<Contract>();

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
                            var rootObjects = JsonConvert.DeserializeObject<RootObjectsContract>(jsonString);
                            dashboardData = rootObjects.Data;

                            ViewBag.CustomerNames = JsonConvert.SerializeObject(dashboardData.Select(d => d.CustomerName));
                            ViewBag.PaidAmount = JsonConvert.SerializeObject(dashboardData.Select(d => d.PaidAmount));
                            ViewBag.UnpaidAmount = JsonConvert.SerializeObject(dashboardData.Select(d => d.UnpaidAmount));
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
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
            await YettoInvoiceDashboard();
            return View(dashboardData);
        }

        public async Task<ActionResult> CustomerList(string searchPharse)
        {
            //ProductMaster objproductmaster = new ProductMaster();

            string Weburl = ConfigurationManager.AppSettings["PRODUCTGET"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<ProductMaster> productmasterlist = new List<ProductMaster>();

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
                            var rootObjects = JsonConvert.DeserializeObject<ProductMasterRootObject>(jsonString);
                            productmasterlist = rootObjects.Data;

                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                productmasterlist = productmasterlist
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
            return View(productmasterlist);
        }

        public async Task<ActionResult> ContractList(int? Recid,string name,string searchPharse)
        {
            if (Recid!=null && name!=null)
            {
                Session["ConRecid"] = Recid;
                Session["name"] = name;
            }
            string WEBURLGET = ConfigurationManager.AppSettings["CONTRACTGET"];
            string Authkey = ConfigurationManager.AppSettings["Authkey"];

            List<Contract> contractsList = new List<Contract>();


            string APIKey = Session["APIKEY"].ToString();


            string strparams = "CompanyRecID=" + Session["CompanyID"] + "&Recid=" + Session["ConRecid"];
            string finalurl = WEBURLGET + "?" + strparams;
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
                            var jsonString = await response.Content.ReadAsStringAsync();
                            //GlobalVariables.ResponseStructure = jsonString;
                            var content = JsonConvert.DeserializeObject<RootObjectsContract>(jsonString);
                            contractsList = content.Data;


                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                contractsList = contractsList
                                    .Where(r => r.CT_CONTRACTREFERENCENUMBER.ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_FROMDATE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_TODATE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_CONTRACTAMOUNT.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_TOTALPAIDAMOUNT.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_CONTRACTCREATEDBY.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_CONTRACTAPPROVEDBY.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_ANYREFERENCE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_TOTALPAIDAMOUNT.ToString().ToLower().Contains(searchPharse.ToLower()))
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
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
            return View(contractsList);

        }

        public ActionResult CreateContract()
        {
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> CreateContract(Contract contract, HttpPostedFileBase Attachment)
        {
         
                try
            {
                var ContractPostURL = ConfigurationManager.AppSettings["CONTRACTPOST"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                byte[] fileBytes = null;
                string attachmentBase64 = null;

                if (Attachment != null && Attachment.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await Attachment.InputStream.CopyToAsync(ms);
                        fileBytes = ms.ToArray();
                    }

                    // Convert byte[] → Base64 for JSON
                    attachmentBase64 = Convert.ToBase64String(fileBytes);
                }
                var content = $@"{{           
            ""cT_PRECID"": ""{Session["Recid"]}"",           
            ""cT_CONTRACTREFERENCENUMBER"": ""{contract.CT_CONTRACTREFERENCENUMBER}"",           
            ""cT_FROMDATE"": ""{contract.CT_FROMDATE}"",           
            ""cT_TODATE"": ""{ contract.CT_TODATE}"",                    
            ""cT_CONTRACTAMOUNT"": ""{ contract.CT_CONTRACTAMOUNT}"",                                       
            ""cT_CONTRACTCREATEDBY"": ""{ contract.CT_CONTRACTCREATEDBY}"",                    
            ""cT_CONTRACTAPPROVEDBY"": ""{ contract.CT_CONTRACTAPPROVEDBY}"",                    
            ""cT_CONTRACTAPPROVEDDATE"": ""{ contract.CT_CONTRACTAPPROVEDDATE}"",                    
            ""cT_ATTACHMENT"": ""{ attachmentBase64}"",                    
            ""cT_ANYREFERENCE"": ""{ contract.CT_ANYREFERENCE}"",                    
            ""cT_SORTORDER"": ""{(contract.CT_SORTORDER)}"",        
            ""cT_CRECID"": ""{Session["CompanyID"]}""           
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(ContractPostURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<ContractMasterObject>(responseBody);

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

        public async Task<ActionResult> EditContract(int? id, string Name,int? Precid)
        {
            Session["Precid"] = Precid;
            Session["Contractrecid"] = id;
            Session["Name"] = Name;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["CONTRACTGETBYID"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            Contract contract  = null;

            string strparams = "Recid=" + id + "&companyId=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<ContractMasterObject>(jsonString);
                            contract = content.Data;
                           
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
            return View(contract);
        }
        [HttpPost]
        public async Task<ActionResult> EditContract(Contract contract)
        {
            try
            {
                var UpdateURL = ConfigurationManager.AppSettings["CONTRACTPUT"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""cT_RECID"": ""{Session["Contractrecid"]}"",           
            ""cT_PRECID"": ""{Session["Precid"]}"",           
            ""cT_CONTRACTREFERENCENUMBER"": ""{contract.CT_CONTRACTREFERENCENUMBER}"",
            ""cT_FROMDATE"": ""{contract.CT_FROMDATE}"",
            ""cT_TODATE"": ""{contract.CT_TODATE}"",
            ""cT_CONTRACTAMOUNT"": ""{contract.CT_CONTRACTAMOUNT}"",
            ""cT_TOTALPAIDAMOUNT"": ""{contract.CT_TOTALPAIDAMOUNT}"",
            ""cT_CONTRACTCREATEDBY"": ""{contract.CT_CONTRACTCREATEDBY}"",
            ""cT_CONTRACTAPPROVEDBY"": ""{contract.CT_CONTRACTAPPROVEDBY}"",
            ""cT_CONTRACTAPPROVEDDATE"": ""{contract.CT_CONTRACTAPPROVEDDATE}"",
            ""cT_ANYREFERENCE"": ""{(contract.CT_ANYREFERENCE)}"",                              
            ""cT_SORTORDER"": ""{(contract.CT_SORTORDER)}"",                              
            ""cT_CRECID"": ""{ Session["CompanyID"]}""                              
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
                    var apiResponse = JsonConvert.DeserializeObject<ContractMasterObject>(responseBody);

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

        public async Task<ActionResult> DeleteContract(int id)

        {

            string WEBURLDELETE = ConfigurationManager.AppSettings["CONTRACTDELETE"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string strparams = "companyId=" + Session["CompanyID"] + "&RecordId=" + id;
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
                            var apiResponse = JsonConvert.DeserializeObject<ContractMasterObject>(responseBody);

                            if (apiResponse.Status == "Y")
                            {

                                string redirectUrl = Url.Action("ContractList", "Contract", new { });
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

        public async Task<ActionResult> YettoInvoiceDashboard()
        {

            string WEBURLGET = ConfigurationManager.AppSettings["YETTOINVOICE"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "CompanyRecID=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            Contract contract = null;

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
                            contract = JsonConvert.DeserializeObject<Contract>(jsonString);

                            ViewBag.ExpiredCustomerWarrantyCount = contract.ExpiredCustomerWarrantyCount;
                            ViewBag.ExpiredProductWarrantyCount = contract.ExpiredProductWarrantyCount;
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
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }          
            return View(contract);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ChangePassword(Changepassword changepassword)
        {
            try
            {
                var ChnagepasswordURL = ConfigurationManager.AppSettings["CHANGEPASSWORD"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""userId"": ""{Session["UserRECID"]}"",           
            ""newpassword"": ""{changepassword.U_NewPassword}"",  
 ""confirmpassword"": ""{changepassword.U_Changepassword}"",     
            ""compRecordID"": ""{ Session["CompanyID"]}""                              
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(ChnagepasswordURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<UserGroupObjects>(responseBody);

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


        //CONTRACT TEMPLATE 

        public ActionResult DownloadContractPdf()
        {
            using (var stream = new MemoryStream())
            {

                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);
                // Set margins
                document.SetMargins(40, 40, 40, 40);

                // Fonts
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                // Define blue color matching the image
                DeviceRgb primaryBlue = new DeviceRgb(41, 128, 185);
                DeviceRgb lightGray = new DeviceRgb(240, 240, 240);

                // Header Section with colored background
                Table headerTable = new Table(1).UseAllAvailableWidth();
                Cell headerCell = new Cell()
                    .Add(new Paragraph("CONTRACT TEMPLATE")
                        .SetFont(boldFont)
                        .SetFontSize(16)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetTextAlignment(TextAlignment.CENTER))
                    .SetBackgroundColor(new DeviceRgb(41, 128, 185)) // Professional blue
                    .SetPadding(10)
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                headerTable.AddCell(headerCell);
                document.Add(headerTable);

                // Document info section
                Table infoTable = new Table(new float[] { 1, 2 }).UseAllAvailableWidth();
                infoTable.SetMarginBottom(10);

                AddInfoRow(infoTable, "Document Type:", "Service Contract", boldFont, regularFont);
                AddInfoRow(infoTable, "Generated Date:", System.DateTime.Now.ToString("dd/MM/yyyy HH:mm"), boldFont, regularFont);

                document.Add(infoTable);

                // Section divider
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("CONTRACT DETAILS")
                    .SetFont(boldFont)
                    .SetFontSize(14)
                    .SetFontColor(new DeviceRgb(41, 128, 185))
                    .SetMarginBottom(10));

                // Main contract table with hand-drawn style borders (2 columns)
                Table mainTable = new Table(new float[] { 150f, 350f});
                mainTable.SetWidth(UnitValue.CreatePointValue(500));
                mainTable.SetBorder(new iText.Layout.Borders.SolidBorder(primaryBlue, 2));

                // Add all rows with hand-drawn style
                AddHandDrawnRow(mainTable, "Contract Reference Number", boldFont, primaryBlue, lightGray);
                AddHandDrawnRow(mainTable, "From Date", boldFont, primaryBlue, lightGray);
                AddHandDrawnRow(mainTable, "To Date", boldFont, primaryBlue, lightGray);
                AddHandDrawnRow(mainTable, "Contract Amount", boldFont, primaryBlue, lightGray);
                AddHandDrawnRow(mainTable, "Created By", boldFont, primaryBlue, lightGray);
                AddHandDrawnRow(mainTable, "Approved By", boldFont, primaryBlue, lightGray);
                AddHandDrawnRow(mainTable, "Approved Date", boldFont, primaryBlue, lightGray);
                AddHandDrawnRow(mainTable, "Any Reference", boldFont, primaryBlue, lightGray);

                document.Add(mainTable);

                // Terms & Conditions Section
       
                document.Add(new Paragraph("TERMS & CONDITIONS")
                    .SetFont(boldFont)
                    .SetFontSize(14)
                    .SetFontColor(new DeviceRgb(41, 128, 185))
                    .SetMarginBottom(10));

                Table termsTable = new Table(1).UseAllAvailableWidth();
                Cell termsCell = new Cell()
                    .Add(new Paragraph("All terms and conditions as per company policy apply.")
                        .SetFont(regularFont)
                        .SetFontSize(9)
                        .SetFontColor(new DeviceRgb(52, 73, 94)))
                    .SetPadding(15)
                    .SetBackgroundColor(new DeviceRgb(236, 240, 241))
                    .SetBorder(new iText.Layout.Borders.SolidBorder(new DeviceRgb(189, 195, 199), 1));
                termsTable.AddCell(termsCell);
                document.Add(termsTable);

                // Signature Section
                document.Add(new Paragraph("\n"));
                Table signatureTable = new Table(3).UseAllAvailableWidth();

                signatureTable.AddCell(CreateSignatureCell("Client Signature", boldFont, regularFont));
                signatureTable.AddCell(CreateSignatureCell("Authorized Signature", boldFont, regularFont));
                signatureTable.AddCell(CreateSignatureCell("Date", boldFont, regularFont));

                document.Add(signatureTable);

                // Footer
                document.Add(new Paragraph("\n"));
                Table footerTable = new Table(1).UseAllAvailableWidth();
                Cell footerCell = new Cell()
                    .Add(new Paragraph("Generated by BEYONDEXS – Service Management")
                        .SetFont(regularFont)
                        .SetFontSize(8)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetTextAlignment(TextAlignment.CENTER))
                    .SetBackgroundColor(new DeviceRgb(52, 73, 94))
                    .SetPadding(10)
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                footerTable.AddCell(footerCell);

                // Position footer at bottom
                footerTable.SetFixedPosition(40, 20, pdf.GetDefaultPageSize().GetWidth() - 80);
                document.Add(footerTable);

                document.Close();

                return File(
                    stream.ToArray(),
                    "application/pdf",
                    $"Contract_Template_{System.DateTime.Now:yyyyMMdd}.pdf"
                );
            }
        }

        private void AddInfoRow(Table table, string label, string value, PdfFont boldFont, PdfFont regularFont)
        {
            table.AddCell(new Cell()
                .Add(new Paragraph(label)
                    .SetFont(boldFont)
                    .SetFontSize(10))
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                .SetPadding(5));

            table.AddCell(new Cell()
                .Add(new Paragraph(value)
                    .SetFont(regularFont)
                    .SetFontSize(10))
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                .SetPadding(5));
        }

        private void AddHandDrawnRow(Table table, string label, PdfFont boldFont, DeviceRgb primaryBlue, DeviceRgb lightGray)
        {
            // Label cell with light gray background and blue borders
            Cell labelCell = new Cell()
                .Add(new Paragraph(label)
                    .SetFont(boldFont)
                    .SetFontSize(11)
                    .SetFontColor(new DeviceRgb(44, 62, 80)))
                .SetBackgroundColor(lightGray)
                .SetPadding(6)
                .SetMinHeight(20)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(new iText.Layout.Borders.SolidBorder(primaryBlue, 1.5f));

            // Value cell with white background and blue borders
            Cell valueCell = new Cell()
                .Add(new Paragraph("\n\n")
                    .SetFontSize(10))
                .SetBackgroundColor(ColorConstants.WHITE)
                .SetPadding(6)
                .SetMinHeight(20)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(new iText.Layout.Borders.SolidBorder(primaryBlue, 1.5f));

            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }
      
        private Cell CreateSignatureCell(string label, PdfFont boldFont, PdfFont regularFont)
        {
            return new Cell()
                .Add(new Paragraph(label)
                    .SetFont(boldFont)
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.CENTER))
                .Add(new Paragraph("\n\n_____________________")
                    .SetFont(regularFont)
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.CENTER))
                .SetPadding(10)
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER);
        }

        //Count
        [HttpGet]
        public async Task<ActionResult> GetContractExpiryCount()
        {
            try
            {
                int cmprecid = Convert.ToInt32(Session["CompanyID"]);

                if (cmprecid == 0)
                {
                    return Json(new
                    {
                        Status = "N",
                        Message = "Invalid Company ID"
                    }, JsonRequestBehavior.AllowGet);
                }

                string apiUrl = ConfigurationManager.AppSettings["GetContractExpiryCount"];
                string authKey = ConfigurationManager.AppSettings["AuthKey"];
                string apiKey = Session["APIKEY"]?.ToString();

                string finalUrl = $"{apiUrl}?cmprecid={cmprecid}";

                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", apiKey);
                        client.DefaultRequestHeaders.Add("Authorization", authKey);
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await client.GetAsync(finalUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var content = JsonConvert.DeserializeObject<ContractMasterObject>(jsonString);

                            ViewBag.ExpiredContracts = content.ExpiredContracts;
                            ViewBag.YetToExpireContracts = content.YetToExpireContracts;
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            return Json(new
                            {
                                Status = "N",
                                Message = response.ReasonPhrase
                            }, JsonRequestBehavior.AllowGet);
                        }

                        var json = await response.Content.ReadAsStringAsync();
                        return Content(json, "application/json");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "N",
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<ActionResult> SendContractExpiryMails()
        {
            try
            {
                int cmprecid = Convert.ToInt32(Session["CompanyID"]);

                if (cmprecid == 0)
                {
                    return Json(new
                    {
                        Status = "N",
                        Message = "Invalid Company ID"
                    }, JsonRequestBehavior.AllowGet);
                }

                string apiUrl = ConfigurationManager.AppSettings["SendContractExpiryMails"];
                string authKey = ConfigurationManager.AppSettings["AuthKey"];
                string apiKey = Session["APIKEY"]?.ToString();

                string finalUrl = $"{apiUrl}?cmprecid={cmprecid}";

                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", apiKey);
                        client.DefaultRequestHeaders.Add("Authorization", authKey);
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await client.GetAsync(finalUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var content = JsonConvert.DeserializeObject<ContractMasterObject>(jsonString);
                        }
                        if (!response.IsSuccessStatusCode)
                        {
                            return Json(new
                            {
                                Status = "N",
                                Message = response.ReasonPhrase
                            }, JsonRequestBehavior.AllowGet);
                        }

                        var json = await response.Content.ReadAsStringAsync();
                        return Content(json, "application/json");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "N",
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<ActionResult> SendFollowupReminderMails()
        {
            try
            {
                int cmprecid = Convert.ToInt32(Session["CompanyID"]);

                if (cmprecid == 0)
                {
                    return Json(new
                    {
                        Status = "N",
                        Message = "Invalid Company ID"
                    }, JsonRequestBehavior.AllowGet);
                }

                string apiUrl = ConfigurationManager.AppSettings["SendFollowupReminderMails"];
                string authKey = ConfigurationManager.AppSettings["AuthKey"];
                string apiKey = Session["APIKEY"]?.ToString();

                string finalUrl = $"{apiUrl}?cmprecid={cmprecid}";

                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", apiKey);
                        client.DefaultRequestHeaders.Add("Authorization", authKey);
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await client.GetAsync(finalUrl);

                        if (!response.IsSuccessStatusCode)
                        {
                            return Json(new
                            {
                                Status = "N",
                                Message = response.ReasonPhrase
                            }, JsonRequestBehavior.AllowGet);
                        }

                        var json = await response.Content.ReadAsStringAsync();
                        return Content(json, "application/json");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "N",
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }

}