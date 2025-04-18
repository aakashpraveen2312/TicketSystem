﻿using Newtonsoft.Json;
using PSS_CMS.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class CompanyRegistrationController : Controller
    {
        // GET: CompanyRegistration
        public ActionResult Index()
        {
            ViewBag.ToastrSuccessMessage = "Request has been successfully sent";
            ViewBag.ToastrErrorMessage = "Please fill out all required fields.";

            return View();
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Companies cmp)
        {
            string password = "";
            if (ModelState.IsValid)
            {
                var Regurl = ConfigurationManager.AppSettings["Registration"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];


                try
                {
                    var content = $@"{{
                    ""C_CODE"": ""{cmp.C_CODE}"",
                    ""C_NAME"": ""{cmp.C_NAME}"",
                    ""C_ADDRESS"": ""{cmp.C_ADDRESS}"",
                    ""C_COUNTRY"":""{cmp.C_COUNTRY}"",
                    ""C_PINCODE"": ""{cmp.C_PINCODE}"",
                    ""C_PHONE"": ""{cmp.C_PHONE}"",
                    ""C_WEB"": ""{cmp.C_WEB}"",
                    ""C_EMAILID"": ""{cmp.C_EMAILID}"",
                    ""C_RBICODE"": ""{cmp.C_RBICODE}"",
                    ""C_GST"": ""{cmp.C_GST}"",
                    ""C_APPUSERNAME"": ""{cmp.C_APPUSERNAME}"",
                    ""C_DOMAIN"": ""{cmp.C_Domain}"",
                    ""C_SOURCETYPE"": ""{"TICKET"}""
                }}";

                    // Prepare header parameters as per RSGT inputs
                    var requests = new HttpRequestMessage
                    {
                        RequestUri = new Uri(Regurl),
                        Method = HttpMethod.Post,
                        Headers =
                {
                    { "X-Version", "1" },
                    { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" },
                    { HttpRequestHeader.ContentType.ToString(), "application/json" }
                },
                        Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                    };

                    // API call and API process
                    HttpClientHandler handlers = new HttpClientHandler();
                    handlers.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    HttpClient clients = new HttpClient(handlers);

                    clients.DefaultRequestHeaders.Add("Authorization", AuthKey);


                    var responses = await clients.SendAsync(requests);
                    if (responses.IsSuccessStatusCode)
                    {

                        var objOutputTask = await responses.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<Companies>(objOutputTask);
                        password = data.Password;
                        string Status = data.Status;
                        if (Status == "Y")
                        {
                            var url = ConfigurationManager.AppSettings["EmailURL"];
             

                            // Create the payload
                            var payload = new
                            {
                                emailID = cmp.C_EMAILID,
                                name = cmp.C_APPUSERNAME,
                                password = password,
                                domain = cmp.C_Domain
                            };

                            // Serialize payload to JSON
                            var jsonPayload = JsonConvert.SerializeObject(payload);
                            var contents = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                            using (var httpClient = new HttpClient())
                            {
                                httpClient.DefaultRequestHeaders.Add("Authorization", AuthKey);


                                try
                                {
                                    var response = await httpClient.PostAsync(url, contents);

                                    if (response.IsSuccessStatusCode)
                                    {
                                        ViewBag.ToastrSuccessMessage = "Request has been successfully sent";
                                    }
                                    else
                                    {
                                        return new HttpStatusCodeResult((int)response.StatusCode, "Failed to send request.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
                                }
                            }

                            return RedirectToAction("Index", "Login");
                        }

                        else if (Status == "U")
                        {
                            @ViewBag.Errormessage = "Email is already exists!";
                            return View();
                        }
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred: {ex.Message}");
                    return View();
                }
            }
            return View();
        }




    }
}