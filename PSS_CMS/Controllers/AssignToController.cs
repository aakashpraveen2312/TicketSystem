using PSS_CMS.Fillter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSS_CMS.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Configuration;
using System.Net;

namespace PSS_CMS.Controllers
{
    [ApiKeyAuthorize]
    public class AssignToController : Controller
    {
        public async Task<ActionResult> Assignto(int? Recid)
        {
            Session["RECORDID"] = Recid;
            await AdminsCombo();
            return View();
        }
        // GET: AssignTo
        [HttpPost]
        public async Task<ActionResult> Assignto(AssignTo assignTo)
        {
            
            try
            {
                var UpdateAssignUrl = ConfigurationManager.AppSettings["UpdateAssignTo"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                // Construct the JSON content for the API request
                var content = $@"{{           
                ""tC_EXPECTEDDATETIME"": ""{ assignTo.A_EXPECTEDDATEANDTIME}"",           
                ""tC_ASSIGNBY"": ""{Session["UserRECID"]}"",           
                ""tC_ASSIGNETODATETIME"": ""{assignTo.A_ASSIGNEDDATEANDTIME}"",                                     
                ""tC_ASSIGNTOURECID"": ""{assignTo.SelectedAdmin}"",                                     
                ""tC_COMMENTS"": ""{assignTo.A_COMMENTS}"",                                     
                ""tC_CRECID"": ""{Session["CompanyID"]}"",                                     
                ""tC_RECID"": ""{Session["RECORDID"]}"" ,          
                ""tC_ASSIGNFLAG"": ""{"Y"}""          
            }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(UpdateAssignUrl),
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
                    var apiResponse = JsonConvert.DeserializeObject<AssignToObject>(responseBody);

                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { success = true, message = "Updated successfully!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed " });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Error: " + response.ReasonPhrase });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception: " + ex.Message });
            }
           
           
        }

        public async Task<ActionResult> AdminsCombo()
        {

            List<SelectListItem> admin = new List<SelectListItem>();

            string Weburl = ConfigurationManager.AppSettings["AdminCombo1"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            string strparams = "cmprecid=" + Session["CompanyID"] + "&ticketRecId=" + Session["RECORDID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<TicketTypeModels>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                admin = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.U_RECID.ToString(), // or the appropriate value field
                                    Text = t.U_USERNAME // or the appropriate text field
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

            // Assuming you are passing ticketTypes to the view
            ViewBag.Admin = admin;

            return View();
        }


        public async Task<ActionResult> PickTicket(Reviewtickets tickets, string recid2)
        {
            try
            {
                Session["RECORDID"] = recid2;
              

                var UpdateTicketPostURL = ConfigurationManager.AppSettings["PICKTICKET"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                // Construct the JSON content for the API request
                var content = $@"{{           
                                             
                ""tC_CRECID"": ""{Session["CompanyID"]}"",                                     
                ""tC_RECID"": ""{Session["RECORDID"]}"" ,          
                ""tC_PICKFLAG"": ""{"Y"}""          
            }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(UpdateTicketPostURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseTicketsResponse>(responseBody);

                    // Return the appropriate result based on the API response
                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { success = true, message = "Ticket picked successfully." });
                    }
                    else if (apiResponse.Status == "U" || apiResponse.Status == "N")
                    {
                        return Json(new { success = false, message = apiResponse.Message });
                    }
                }

                return Json(new { success = false, message = "Error occurred while closing the ticket." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception: " + ex.Message });
            }

        }


    }
}