using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
    [ApiKeyAuthorize]
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


                            // Report Images

                            // =====================================================
                            // REPORT IMAGES
                            // =====================================================

                            ViewBag.imageHeader =
                                companyinfo.C_HEADERIMAGE;

                            ViewBag.imageFooter =
                                companyinfo.C_FOOTERIMAGE;

                            ViewBag.imageSign =
                                companyinfo.C_AUTHORIZESIGNATURE;

                            ViewBag.imageQR =
                                companyinfo.C_QRIMAGE;
                            //if (logoBytes != null)
                            //{
                            //    string base64Logo = Convert.ToBase64String(logoBytes);
                            //    ViewBag.Logo = base64Logo;
                            //}
                            //await LocationList();
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
            //await LocationList();
            return View();
        }

        private string GetImageSrc(string base64Image)
        {
            if (string.IsNullOrWhiteSpace(base64Image))
            {
                return "";
            }

            base64Image = base64Image.Trim();

            // Already a data URI
            if (base64Image.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
            {
                return base64Image;
            }

            // Remove possible existing prefix
            if (base64Image.Contains(","))
            {
                base64Image =
                    base64Image.Substring(base64Image.IndexOf(",") + 1);
            }

            try
            {
                // Validate Base64
                Convert.FromBase64String(base64Image);

                return "data:image/png;base64," + base64Image;
            }
            catch
            {
                return "";
            }
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


        [HttpPost]
        public async Task<ActionResult> UpdateCompaniesBank(CompanyInfo ObjCompaniesinfo)
        {

            string companyId = "";
            companyId = Session["CompanyId"].ToString();
            var MaterialcatPostURL = ConfigurationManager.AppSettings["UpdateCompaniesBankDetail"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            bool blresult = true;




            if (blresult == true)
            {
                try
                {
                    var content = $@"{{
    ""C_RECID"": {Session["companyId"]},
    ""c_BANKNAME"": ""{ObjCompaniesinfo.C_BANKNAME}"",
    ""c_BRANCHNAME"": ""{ObjCompaniesinfo.C_BRANCHNAME}"",
    ""c_ACCOUNTNAME"": ""{ObjCompaniesinfo.C_ACCOUNTNAME}"",
    ""c_ACCOUNTNO"": ""{ObjCompaniesinfo.C_ACCOUNTNO}"",
    ""c_ACCOUNTTYPE"": ""{ObjCompaniesinfo.C_ACCOUNTTYPE}"",
    ""c_IFSCCODE"": ""{ObjCompaniesinfo.C_IFSCCODE}"",
    ""c_BANKLOCATION"": ""{ObjCompaniesinfo.C_BANKLOCATION}"",
    ""c_BANKADDRESS"": ""{ObjCompaniesinfo.C_BANKADDRESS}""
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
                            return Json(new { status = "success", message = "Bank Details Updated successfully" });
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
                            return RedirectToAction("List", "Party", new { id = companyId });

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


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> UpdatePartyReportS(
    CompanyInfo CompanyInfo,
    HttpPostedFileBase Signimg,
    HttpPostedFileBase Headerimg,
    HttpPostedFileBase Footerimg,
    HttpPostedFileBase QRimg,
    HttpPostedFileBase CompanyLogoimg)
        {
            string companyId = Session["CompanyId"]?.ToString();

            if (string.IsNullOrEmpty(companyId))
            {
                return Json(new
                {
                    status = "error",
                    message = "Company session expired. Please login again."
                });
            }

            try
            {
                // =========================================================
                // EXISTING BASE64 VALUES FROM HIDDEN FIELDS
                // =========================================================

                string existingSign =
                    Request.Form["ExistingSignImage"];

                string existingHeader =
                    Request.Form["ExistingHeaderImage"];

                string existingFooter =
                    Request.Form["ExistingFooterImage"];

                string existingQR =
                    Request.Form["ExistingQrImage"];

                string existingLogo =
                    Request.Form["ExistingLogoImage"];


                // =========================================================
                // SIGNATURE IMAGE
                // =========================================================

                string signBase64 = null;

                if (Signimg != null && Signimg.ContentLength > 0)
                {
                    byte[] signBytes = ConvertToByteArray(Signimg);

                    if (signBytes != null && signBytes.Length > 0)
                    {
                        signBase64 = Convert.ToBase64String(signBytes);
                    }
                }
                else
                {
                    signBase64 = existingSign;

                    // Fallback to model value
                    if (string.IsNullOrWhiteSpace(signBase64))
                    {
                        signBase64 = CompanyInfo.C_AUTHORIZESIGNATURE;
                    }
                }


                // =========================================================
                // HEADER IMAGE
                // =========================================================

                string headerBase64 = null;

                if (Headerimg != null && Headerimg.ContentLength > 0)
                {
                    byte[] headerBytes = ConvertToByteArray(Headerimg);

                    if (headerBytes != null && headerBytes.Length > 0)
                    {
                        headerBase64 = Convert.ToBase64String(headerBytes);
                    }
                }
                else
                {
                    headerBase64 = existingHeader;

                    // Fallback to model value
                    if (string.IsNullOrWhiteSpace(headerBase64))
                    {
                        headerBase64 = CompanyInfo.C_HEADERIMAGE;
                    }
                }


                // =========================================================
                // FOOTER IMAGE
                // =========================================================

                string footerBase64 = null;

                if (Footerimg != null && Footerimg.ContentLength > 0)
                {
                    byte[] footerBytes = ConvertToByteArray(Footerimg);

                    if (footerBytes != null && footerBytes.Length > 0)
                    {
                        footerBase64 = Convert.ToBase64String(footerBytes);
                    }
                }
                else
                {
                    footerBase64 = existingFooter;

                    // Fallback to model value
                    if (string.IsNullOrWhiteSpace(footerBase64))
                    {
                        footerBase64 = CompanyInfo.C_FOOTERIMAGE;
                    }
                }


                // =========================================================
                // QR IMAGE
                // =========================================================

                string qrBase64 = null;

                if (QRimg != null && QRimg.ContentLength > 0)
                {
                    byte[] qrBytes = ConvertToByteArray(QRimg);

                    if (qrBytes != null && qrBytes.Length > 0)
                    {
                        qrBase64 = Convert.ToBase64String(qrBytes);
                    }
                }
                else
                {
                    qrBase64 = existingQR;

                    // Fallback to model value
                    if (string.IsNullOrWhiteSpace(qrBase64))
                    {
                        qrBase64 = CompanyInfo.C_QRIMAGE;
                    }
                }


                // =========================================================
                // COMPANY LOGO
                // =========================================================

                string logoBase64 = null;

                if (CompanyLogoimg != null && CompanyLogoimg.ContentLength > 0)
                {
                    byte[] logoBytes = ConvertToByteArray(CompanyLogoimg);

                    if (logoBytes != null && logoBytes.Length > 0)
                    {
                        logoBase64 = Convert.ToBase64String(logoBytes);
                    }
                }
                else
                {
                    logoBase64 = existingLogo;

                    // Fallback to model value
                    if (string.IsNullOrWhiteSpace(logoBase64))
                    {
                        logoBase64 = CompanyInfo.C_LOGO;
                    }
                }


                // =========================================================
                // VALIDATION
                // =========================================================

                if (string.IsNullOrWhiteSpace(signBase64))
                {
                    return Json(new
                    {
                        status = "error",
                        message = "Please insert Signature Image"
                    });
                }

                if (string.IsNullOrWhiteSpace(headerBase64))
                {
                    return Json(new
                    {
                        status = "error",
                        message = "Please insert Header Image"
                    });
                }

                if (string.IsNullOrWhiteSpace(footerBase64))
                {
                    return Json(new
                    {
                        status = "error",
                        message = "Please insert Footer Image"
                    });
                }

                if (string.IsNullOrWhiteSpace(logoBase64))
                {
                    return Json(new
                    {
                        status = "error",
                        message = "Please insert Company Logo"
                    });
                }


                // =========================================================
                // API URL / AUTHENTICATION
                // =========================================================

                string reportInfoUpdateURL =
                    ConfigurationManager.AppSettings["ReportInfoUpdate"];

                string authKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string apiKey =
                    Session["APIKEY"]?.ToString();


                if (string.IsNullOrEmpty(apiKey))
                {
                    return Json(new
                    {
                        status = "error",
                        message = "API Key is missing. Please login again."
                    });
                }


                // =========================================================
                // REQUEST OBJECT
                // =========================================================

                var requestData = new
                {
                    c_RECID = Convert.ToInt32(companyId),

                    c_AUTHSIGNNAME =
                        CompanyInfo.C_AUTHSIGNNAME ?? "",

                    c_AUTHPOSITION =
                        CompanyInfo.C_AUTHPOSITION ?? "",

                    c_AUTHORIZESIGNATURE =
                        signBase64,

                    c_HEADERIMAGE =
                        headerBase64,

                    c_FOOTERIMAGE =
                        footerBase64,

                    c_QRIMAGE =
                        qrBase64 ?? "",

                    c_LOGO =
                        logoBase64
                };


                // =========================================================
                // SERIALIZE JSON
                // =========================================================

                string content =
                    JsonConvert.SerializeObject(requestData);


                // =========================================================
                // HTTP REQUEST
                // =========================================================

                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add(
                            "ApiKey",
                            apiKey
                        );

                        client.DefaultRequestHeaders.Add(
                            "Authorization",
                            authKey
                        );

                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue(
                                "application/json"
                            )
                        );


                        using (HttpRequestMessage request =
                            new HttpRequestMessage())
                        {
                            request.RequestUri =
                                new Uri(reportInfoUpdateURL);

                            request.Method =
                                HttpMethod.Put;

                            request.Headers.Add(
                                "X-Version",
                                "1"
                            );

                            request.Content =
                                new StringContent(
                                    content,
                                    System.Text.Encoding.UTF8,
                                    "application/json"
                                );


                            // =================================================
                            // SEND REQUEST
                            // =================================================

                            HttpResponseMessage response =
                                await client.SendAsync(request);


                            string responseBody =
                                await response.Content.ReadAsStringAsync();


                            // =================================================
                            // API RESPONSE
                            // =================================================

                            if (response.IsSuccessStatusCode)
                            {
                                ApiResponseInfo apiResponse =
                                    JsonConvert.DeserializeObject<ApiResponseInfo>(
                                        responseBody
                                    );

                                if (apiResponse != null)
                                {
                                    if (apiResponse.Status == "Y")
                                    {
                                        return Json(new
                                        {
                                            status = "success",
                                            message =
                                                "Company Info Reported updated successfully"
                                        });
                                    }

                                    if (apiResponse.Status == "U")
                                    {
                                        return Json(new
                                        {
                                            status = "error",
                                            message =
                                                apiResponse.Message
                                        });
                                    }

                                    if (apiResponse.Status == "N")
                                    {
                                        return Json(new
                                        {
                                            status = "error",
                                            message =
                                                apiResponse.Message
                                        });
                                    }

                                    return Json(new
                                    {
                                        status = "error",
                                        message =
                                            apiResponse.Message ??
                                            "Unable to update company information."
                                    });
                                }

                                return Json(new
                                {
                                    status = "error",
                                    message = "Invalid response received from API."
                                });
                            }


                            // =================================================
                            // API HTTP ERROR
                            // =================================================

                            return Json(new
                            {
                                status = "error",
                                message =
                                    "Error: " +
                                    response.ReasonPhrase +
                                    " | " +
                                    responseBody
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "error",
                    message =
                        "Exception occurred: " +
                        ex.Message
                });
            }
        }

       

        //public async Task<ActionResult> LocationList()
        //{
        //    Locations objstoragePoint = new Locations();
        //    List<Locations> StorageList = new List<Locations>();
        //    string connectionString = ConfigurationManager.ConnectionStrings["Mystring"].ConnectionString;
        //    using (SqlConnection sqlcon = new SqlConnection(connectionString))
        //    {
        //        sqlcon.Open();

        //        string cmd1 = "SELECT ROW_NUMBER() OVER(ORDER BY SP_CRECID) AS SerialNumber,SP_CRECID,SP_RECID,SP_Code,SP_Name,SP_Type,SP_Sortorder FROM IM_StoragePoint where SP_CRECID =" + Session["CompanyID"] + " ";

        //        SqlCommand sqlCdm = new SqlCommand(cmd1, sqlcon); // take the values from the DB
        //        SqlDataReader sqlread = sqlCdm.ExecuteReader(); // to read the value reader class called
        //        while (sqlread.Read()) // each value to read create function
        //        {
        //            var SPList = new Locations();

        //            SPList.L_CRECID = (int)sqlread["SP_CRECID"];
        //            SPList.L_RECID = (int)sqlread["SP_RECID"];
        //            SPList.L_CODE = sqlread["SP_Code"].ToString();
        //            SPList.L_NAME = sqlread["SP_Name"].ToString();
        //            SPList.L_SORTORDER = (int)sqlread["SP_Sortorder"];

        //            StorageList.Add(SPList);
        //            ViewBag.LocationList = StorageList;
        //        }
        //        sqlcon.Close();           
        //    }
        //    return View(StorageList);
        //}
    }
}