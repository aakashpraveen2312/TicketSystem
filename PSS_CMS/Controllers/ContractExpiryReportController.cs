using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    [ApiKeyAuthorize]
    public class ContractExpiryReportController : Controller
    {
        // GET: ContractExpiryReport
        public ActionResult ContractexpiryReport()
        {
            return View();
        }

        //   public async Task<ActionResult> ContractexpiryReport(Prioritywise prioritywise, DateTime? FromDate, DateTime? ToDate, string Type,string ActionType, string PastExpiry)
        //   {
        //       bool hasDateRange = FromDate.HasValue && ToDate.HasValue;
        //       bool hasType = !string.IsNullOrWhiteSpace(Type);

        //       //if (!hasDateRange && !hasType)
        //       //{
        //       //    TempData["ErrorMessage"] = "Please select either a Date Range or a Type";
        //       //    return RedirectToAction("ContractexpiryReport");
        //       //}
        //       // 2️⃣ Check ToDate >= FromDate
        //       if (FromDate.HasValue && ToDate.HasValue && ToDate < FromDate)
        //       {
        //           TempData["ErrorMessage"] = "To Date must be greater than or equal to From Date";
        //           return RedirectToAction("ContractexpiryReport");
        //       }

        //       List<Prioritywise> list = new List<Prioritywise>();

        //       string Weburl = ConfigurationManager.AppSettings[
        //ActionType == "PDF" ? "CONTRACTEXPIRYREPORT" : "CONTRACTEXPIRYREPORTLISTVIEW"];


        //       //string Weburl = ConfigurationManager.AppSettings["CONTRACTEXPIRYREPORT"];
        //       string AuthKey = ConfigurationManager.AppSettings["Authkey"];
        //       string APIKey = Session["APIKEY"]?.ToString();


        //       string url = $"{Weburl}?companyRecID={Session["CompanyId"]}&fromDate={FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}&type={Type}&PastExpiry={PastExpiry}";

        //       try
        //       {
        //           using (HttpClientHandler handler = new HttpClientHandler())
        //           using (HttpClient client = new HttpClient(handler))
        //           {
        //               handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        //               client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //               client.DefaultRequestHeaders.Add("Authorization", AuthKey);
        //               client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //               var response = await client.GetAsync(url);

        //               var responseContent = await response.Content.ReadAsStringAsync();

        //               if (!response.IsSuccessStatusCode)
        //               {
        //                   return Content($"Status: {response.StatusCode}<br/>{responseContent}");
        //               }
        //               if (!response.IsSuccessStatusCode)
        //                   return Content("Error fetching data: " + response.ReasonPhrase);

        //               var jsonString = await response.Content.ReadAsStringAsync();
        //               var rootObjects = JsonConvert.DeserializeObject<Prioritywisepdfobjects>(jsonString);

        //               if (rootObjects == null || rootObjects.Status != "Y")
        //                   return Content(rootObjects?.Message ?? "No data found for the selected criteria.");

        //               if (ActionType == "Filter")
        //               {
        //                   if (rootObjects != null && rootObjects.Status == "Y")
        //                   {
        //                       list = rootObjects.Data;
        //                   }
        //                   return View(list);
        //               }
        //               else
        //               {
        //                   // The API already returns a PDF URL
        //                   string pdfUrl = rootObjects.fileUrl;
        //                   var fileBytes = await client.GetByteArrayAsync(pdfUrl);
        //                   var fileName = Path.GetFileName(pdfUrl); // GstInReport_20250924052413.pdf

        //                   // Download
        //                   return File(fileBytes, "application/pdf", fileName);


        //               }

        //           }
        //       }
        //       catch (Exception ex)
        //       {
        //           return Content("Exception occurred: " + ex.Message);
        //       }
        //   }

        [HttpPost]
        public async Task<ActionResult> ContractexpiryReport(DateTime? FromDate, DateTime? ToDate, string Type, string ActionType, string PastExpiry)
        {
            // Format once, reuse everywhere (also matches the yyyy-MM-dd format
            // required by <input type="date">)
            string fromDateStr = FromDate.HasValue ? FromDate.Value.ToString("yyyy-MM-dd") : "";
            string toDateStr = ToDate.HasValue ? ToDate.Value.ToString("yyyy-MM-dd") : "";

            // ── Validate date range ──────────────────────────────────────
            if (FromDate.HasValue && ToDate.HasValue && ToDate < FromDate)
            {
                TempData["ErrorMessage"] = "To Date must be greater than or equal to From Date";

                // FIX: TempData (not ViewBag) survives a redirect, so the filter
                // values are still available on the next GET and the form won't
                // appear cleared to the user.
                TempData["FromDate"] = fromDateStr;
                TempData["ToDate"] = toDateStr;
                TempData["Type"] = Type;
                TempData["PastExpiry"] = PastExpiry;

                return RedirectToAction("ContractexpiryReport");
            }

            // ── Expose current filter values to the view ─────────────────
            // Prefer values carried over from a redirect (TempData) if present,
            // otherwise use whatever was just submitted on this request.
            ViewBag.FromDate = TempData["FromDate"] as string ?? fromDateStr;
            ViewBag.ToDate = TempData["ToDate"] as string ?? toDateStr;
            ViewBag.Type = TempData["Type"] as string ?? Type;
            ViewBag.PastExpiry = TempData["PastExpiry"] as string ?? PastExpiry;

            List<Prioritywise> list = new List<Prioritywise>();

            string Weburl = ConfigurationManager.AppSettings[
                ActionType == "PDF" ? "CONTRACTEXPIRYREPORT" : "CONTRACTEXPIRYREPORTLISTVIEW"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"]?.ToString();

            string url = $"{Weburl}?companyRecID={Session["CompanyId"]}&fromDate={fromDateStr}&toDate={toDateStr}&type={Type}&PastExpiry={PastExpiry}";

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                using (HttpClient client = new HttpClient(handler))
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(url);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        // FIX: show the error via the normal view + TempData instead of
                        // Content(), which wipes out the whole page (layout, filter form,
                        // and all input values) and replaces it with plain text.
                        TempData["ErrorMessage"] = $"Status: {response.StatusCode} — {responseContent}";
                        return View(list);
                    }

                    var rootObjects = JsonConvert.DeserializeObject<Prioritywisepdfobjects>(responseContent);

                    if (rootObjects == null || rootObjects.Status != "Y")
                    {
                        TempData["ErrorMessage"] = rootObjects?.Message ?? "No data found for the selected criteria.";
                        return View(list);
                    }

                    // FIX: treat anything that is NOT explicitly "PDF" as the list/filter
                    // view. Previously this only matched ActionType == "Filter", so the
                    // very first page load (ActionType is null) fell through to the PDF
                    // branch and threw trying to download a non-existent file URL —
                    // which was silently swallowed by the outer catch.
                    if (ActionType != "PDF")
                    {
                        list = rootObjects.Data ?? new List<Prioritywise>();
                        return View(list);
                    }
                    else
                    {
                        string pdfUrl = rootObjects.fileUrl;

                        if (string.IsNullOrWhiteSpace(pdfUrl))
                        {
                            TempData["ErrorMessage"] = "PDF could not be generated for the selected criteria.";
                            return View(list);
                        }

                        var fileBytes = await client.GetByteArrayAsync(pdfUrl);
                        var fileName = Path.GetFileName(pdfUrl); // e.g. GstInReport_20250924052413.pdf
                        return File(fileBytes, "application/pdf", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                // FIX: same reasoning — keep the user on the real view with their
                // filters intact instead of a bare exception message replacing the page.
                TempData["ErrorMessage"] = "Exception occurred: " + ex.Message;
                return View(list);
            }
        }
        public async Task<ActionResult> ProductFinanceReport()
        {
            await ProductCombo();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ProductFinanceReport(Prioritywise prioritywise,string Type,string Productrecid,string contractrecid,string ActionType)
        {
            List<Prioritywise> list = new List<Prioritywise>();

            string Weburl = ConfigurationManager.AppSettings[
     ActionType == "PDF" ? "PRODUCTFINANCEREPORT" : "PRODUCTFINANCEREPORTLISTVIEW"];

            //string Weburl = ConfigurationManager.AppSettings["PRODUCTFINANCEREPORT"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"]?.ToString();


            string url = $"{Weburl}?companyRecID={Session["CompanyId"]}&productRecid={Productrecid}&contractRecid={contractrecid}&type={Type}";


            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                using (HttpClient client = new HttpClient(handler))
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                        return Content("Error fetching data: " + response.ReasonPhrase);

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var rootObjects = JsonConvert.DeserializeObject<Prioritywisepdfobjects>(jsonString);

                    if (rootObjects == null || rootObjects.Status != "Y")
                        return Content(rootObjects?.Message ?? "No data found for the selected criteria.");

                    if (ActionType == "Filter")
                    {
                        if (rootObjects != null && rootObjects.Status == "Y")
                        {
                            list = rootObjects.Data;
                        }
                        return View(list);
                    }
                    else
                    {
                        // The API already returns a PDF URL
                        string pdfUrl = rootObjects.fileUrl;
                        var fileBytes = await client.GetByteArrayAsync(pdfUrl);
                        var fileName = Path.GetFileName(pdfUrl); // GstInReport_20250924052413.pdf

                        // Download
                        return File(fileBytes, "application/pdf", fileName);


                    }

                }
            }
            catch (Exception ex)
            {
                return Content("Exception occurred: " + ex.Message);
            }
        }
        public async Task<ActionResult> CustomerFinanceReport()
        {
            await ContractCombo();
            await ProductCombo();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CustomerFinanceReport(Prioritywise prioritywise, string Type, string CustomerRecid, string ContractRecid,string ProductRecid,string ActionType)
        {
            List<Prioritywise> list = new List<Prioritywise>();
            string Weburl = ConfigurationManager.AppSettings[
     ActionType == "PDF" ? "CUSTOMERFINANCEREPORT" : "CUSTOMERFINANCEREPORTLISTVIEW"];

            //string Weburl = ConfigurationManager.AppSettings["CUSTOMERFINANCEREPORT"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"]?.ToString();


            string url = $"{Weburl}?companyRecID={Session["CompanyId"]}&customerRecid={CustomerRecid}&contractRecid={ContractRecid}&type={Type}&ProductRecid={ProductRecid}";


            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                using (HttpClient client = new HttpClient(handler))
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                        return Content("Error fetching data: " + response.ReasonPhrase);

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var rootObjects = JsonConvert.DeserializeObject<Prioritywisepdfobjects>(jsonString);

                    if (rootObjects == null || rootObjects.Status != "Y")
                        return Content(rootObjects?.Message ?? "No data found for the selected criteria.");

                    if (ActionType == "Filter")
                    {
                        if (rootObjects != null && rootObjects.Status == "Y")
                        {
                            list = rootObjects.Data;
                        }
                        return View(list);
                    }
                    else
                    {
                        // The API already returns a PDF URL
                        string pdfUrl = rootObjects.fileUrl;
                        var fileBytes = await client.GetByteArrayAsync(pdfUrl);
                        var fileName = Path.GetFileName(pdfUrl); // GstInReport_20250924052413.pdf

                        // Download
                        return File(fileBytes, "application/pdf", fileName);


                    }

                }
            }
            catch (Exception ex)
            {
                return Content("Exception occurred: " + ex.Message);
            }
        }


        //ProductCombo
        public async Task<ActionResult> ProductCombo()
        {
            List<SelectListItem> Product = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["PRODUCTGETCOMBO"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();
            string strParams = "cmprecid=" + Session["CompanyID"];
            string finalUrl = $"{webUrlGet}?{strParams}";

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
                            var rootObjects = JsonConvert.DeserializeObject<ProductMasterRootObject>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Product = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.P_RECID.ToString(),
                                    Text = t.P_NAME,
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            ViewBag.Product = Product;
          
            return View();
        }

        public async Task<JsonResult> ProductBasedContract(string ProductRecid)
        {
            List<SelectListItem> Contractdata = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["PRODUCTGETCONTRACT"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();
            string strParams = $"Recid={ProductRecid}&CompanyRecID={Session["CompanyID"]}";
            string finalUrl = $"{webUrlGet}?{strParams}";

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
                            var rootObjects = JsonConvert.DeserializeObject<RootObjectsContract>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Contractdata = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.CT_RECID.ToString(),
                                    Text = t.CT_CONTRACTREFERENCENUMBER
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return Json(new { data = Contractdata }, JsonRequestBehavior.AllowGet);
        }

        //Contract Combo
        public async Task<ActionResult> ContractCombo()
        {
            List<SelectListItem> Contractnumber = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["CONTRACTGETCOMBO"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();
            string strParams = "CompanyRecID=" + Session["CompanyID"];
            string finalUrl = $"{webUrlGet}?{strParams}";

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
                            var rootObjects = JsonConvert.DeserializeObject<RootObjectsContract>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Contractnumber = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.CT_RECID.ToString(),
                                    Text = t.CT_CONTRACTREFERENCENUMBER,
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            ViewBag.Contractnumber = Contractnumber;

            return View();
        }


        public async Task<JsonResult>CustomerBasedContract(string Productrecid)
        {
            List<SelectListItem> Customerdata = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["CUSTOMERGETCONTRACT"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();
            string strParams = $"ContractRecID={Productrecid}&CompanyRecID={Session["CompanyID"]}";
            string finalUrl = $"{webUrlGet}?{strParams}";

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
                            var rootObjects = JsonConvert.DeserializeObject<ProjectMasterRootObject>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Customerdata = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.CU_RECID.ToString(),
                                    Text = t.CU_NAME
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return Json(new { data = Customerdata }, JsonRequestBehavior.AllowGet);
        }

        //Customer Combo
        public async Task<ActionResult> CustomerCombo()
        {
            List<SelectListItem> Customer = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["CUSTOMERGETCOMBO"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();
            string strParams = "CompanyRecID=" + Session["CompanyID"];
            string finalUrl = $"{webUrlGet}?{strParams}";

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
                            var rootObjects = JsonConvert.DeserializeObject<ApiResponseUserObjects>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Customer = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.U_RECID.ToString(),
                                    Text = t.U_USERNAME,
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            ViewBag.Customer = Customer;

            return View();
        }

        public async Task<JsonResult> CustomerBasedProduct(string Productrecid)
        {
            List<SelectListItem> Productdata = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["CUSTOMERGETPRODUCT"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();
            string strParams = $"CustomerRecid={Productrecid}&CompanyRecID={Session["CompanyID"]}";
            string finalUrl = $"{webUrlGet}?{strParams}";

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
                            var rootObjects = JsonConvert.DeserializeObject<ProjectMasterRootObject>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Productdata = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.P_RECID.ToString(),
                                    Text = t.P_NAME
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return Json(new { data = Productdata }, JsonRequestBehavior.AllowGet);
        }

        //Material Combo
        public async Task<ActionResult> MaterialCombo()
        {
            List<SelectListItem> Material = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["GETMATERIALSCOMBO"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();
            string strParams = "CompanyRecID=" + Session["CompanyID"];
            string finalUrl = $"{webUrlGet}?{strParams}";

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
                            var rootObjects = JsonConvert.DeserializeObject<IRootObjects>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Material = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.I_RECID.ToString(),
                                    Text = t.I_DESCRIPTION,
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            ViewBag.Material = Material;

            return View();
        }
        public async Task<ActionResult> ServiceProductReport()
        {
            await CustomerCombo();
            await MaterialCombo();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ServiceProductReport(Prioritywise prioritywise, string Type, string Productrecid, string contractrecid,string Materialrecid, string ActionType)
        {

            List<Prioritywise> list = new List<Prioritywise>();

            string Weburl = ConfigurationManager.AppSettings[
     ActionType == "PDF" ? "SERVICEPRODUCTREPORT" : "SERVICEPRODUCTREPORTLISTVIEW"];

            //string Weburl = ConfigurationManager.AppSettings["SERVICEPRODUCTREPORT"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"]?.ToString();


            string url = $"{Weburl}?companyRecID={Session["CompanyId"]}&customerRecid={Productrecid}&productRecid={contractrecid}&type={Type}&MaterialRecid={Materialrecid}";


            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                using (HttpClient client = new HttpClient(handler))
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                        return Content("Error fetching data: " + response.ReasonPhrase);

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var rootObjects = JsonConvert.DeserializeObject<Prioritywisepdfobjects>(jsonString);

                    if (rootObjects == null || rootObjects.Status != "Y")
                        return Content(rootObjects?.Message ?? "No data found for the selected criteria.");

                    if (ActionType == "Filter")
                    {
                        if (rootObjects != null && rootObjects.Status == "Y")
                        {
                            list = rootObjects.Data;
                        }
                        await CustomerCombo();
                        await MaterialCombo();
                        return View(list);
                    }
                    else
                    {
                        // The API already returns a PDF URL
                        string pdfUrl = rootObjects.fileUrl;
                        var fileBytes = await client.GetByteArrayAsync(pdfUrl);
                        var fileName = Path.GetFileName(pdfUrl); // GstInReport_20250924052413.pdf

                        // Download
                        return File(fileBytes, "application/pdf", fileName);


                    }

                }
            }
            catch (Exception ex)
            {
                return Content("Exception occurred: " + ex.Message);
            }
        }


    }
}