using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    public class UserGroupController : Controller
    {
        // GET: UserGroup
        public ActionResult List()
        {
            Tickethistory objRecents = new Tickethistory();

            string Weburl = ConfigurationManager.AppSettings["ClientTicketURL"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Tickethistory> RecentTicketListall = new List<Tickethistory>();

            string strparams = "TC_USERID=" + Session["UserID"] + "&StrUsertype=" + Session["UserRole"] + "&cmprecid=" + Session["CompanyID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<ApiResponseTicketsHistoryResponse>(jsonString);
                            RecentTicketListall = rootObjects.Data;


                            if (
                                 string.IsNullOrWhiteSpace(ticketType) &&
                                 string.IsNullOrWhiteSpace(status) &&
                                 string.IsNullOrWhiteSpace(projectType) &&
                                 string.IsNullOrWhiteSpace(StartDate) &&
                                 string.IsNullOrWhiteSpace(EndDate) &&
                                 string.IsNullOrWhiteSpace(searchPharse))
                            {
                                // Exclude Closed tickets on the first load if no filters are applied
                                RecentTicketListall = RecentTicketListall.Where(t => t.TC_STATUS != "C").ToList();
                            }

                            if (!string.IsNullOrEmpty(ticketType))
                            {
                                RecentTicketListall = RecentTicketListall.Where(t => t.TC_TICKETTYPE == ticketType).ToList();
                            }


                            if (!string.IsNullOrEmpty(status))
                            {
                                RecentTicketListall = RecentTicketListall.Where(t => t.TC_STATUS == status).ToList();
                            }
                            if (!string.IsNullOrEmpty(projectType))
                            {
                                RecentTicketListall = RecentTicketListall.Where(t => t.TC_PROJECTID == projectType).ToList();
                            }
                            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
                            {
                                //DateTime fromDate = DateTime.Parse(StartDate);//parse it is used to convert the string to datetime object
                                //DateTime toDate = DateTime.Parse(EndDate);


                                RecentTicketListall = RecentTicketListall
          .Where(t => string.Compare(t.TC_TICKETDATE, StartDate) >= 0 &&
                      string.Compare(t.TC_TICKETDATE, EndDate) <= 0)
          .ToList();
                            }
                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                RecentTicketListall = RecentTicketListall
                                    .Where(r => r.TC_PROJECTID.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.TC_SUBJECT.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.TC_PRIORITYTYPE.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.TC_STATUS.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.TC_TICKETTYPE.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.TC_TICKETDATES.ToLower().Contains(searchPharse.ToLower()))
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

            await ComboBoxTicketHistory();
            await ComboBoxTicketHistoryProjectType();
            return View(RecentTicketListall);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Usergroup usergroup)
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Edit(Usergroup usergroup)
        {
            return View();
        }
        public ActionResult Delete()
        {
            return View();
        }
    }
}