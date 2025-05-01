using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class CompanyRegistrationController : Controller
    {
        // GET: CompanyRegistration
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Companies cmp)
        {
            if (!ModelState.IsValid)
                return View();

            string registrationUrl = ConfigurationManager.AppSettings["COMPANYREGISTRATION"];
            string authKey = ConfigurationManager.AppSettings["AuthKey"];
            string emailUrl = ConfigurationManager.AppSettings["EMAILURL"];

            var companyPayload = new
            {
                c_CODE = cmp.C_CODE,
                c_NAME = cmp.C_NAME,
                c_ADDRESS = cmp.C_ADDRESS,
                c_COUNTRY = cmp.C_COUNTRY,
                c_PINCODE = cmp.C_PINCODE,
                c_PHONE = cmp.C_PHONE,
                c_WEB = cmp.C_WEB,
                c_EMAILID = cmp.C_EMAILID,
                c_RBICODE = cmp.C_RBICODE,
                c_GST = cmp.C_GST,
                c_APPUSERNAME = cmp.C_APPUSERNAME,
                c_DOMAIN = cmp.C_Domain,
                c_SOURCETYPE = "TICKET"
            };

            try
            {
                var requestContent = new StringContent(JsonConvert.SerializeObject(companyPayload), Encoding.UTF8, "application/json");

                var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true };
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("Authorization", authKey);

                    var response = await client.PostAsync(registrationUrl, requestContent);
                    if (!response.IsSuccessStatusCode)
                        return Json(new { success = false, message = "Registration failed." });

                    var responseData = JsonConvert.DeserializeObject<Companies>(await response.Content.ReadAsStringAsync());

                    if (responseData.Status == "Y")
                    {
                        var emailPayload = new
                        {
                            emailID = cmp.C_EMAILID,
                            name = cmp.C_APPUSERNAME,
                            password = responseData.Password,
                            Usercode = responseData.Usercode,
                            domain = cmp.C_Domain
                        };

                        var emailContent = new StringContent(JsonConvert.SerializeObject(emailPayload), Encoding.UTF8, "application/json");

                        using (var emailClient = new HttpClient())
                        {
                            emailClient.DefaultRequestHeaders.Add("Authorization", authKey);
                            var emailResponse = await emailClient.PostAsync(emailUrl, emailContent);

                            bool success = emailResponse.IsSuccessStatusCode;
                            return Json(new { success, message = responseData.Message });
                        }
                    }

                    return Json(new { success = false, message = responseData.Message });
                }
            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError,"An error occurred while processing your request.");
            }
        }
    }
}