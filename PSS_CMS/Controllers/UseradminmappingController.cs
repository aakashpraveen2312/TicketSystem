using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    public class UseradminmappingController : Controller
    {
        // GET: Useradminmapping
        public async Task<ActionResult> List()
        {
            Useradminmap objuseradminmap = new Useradminmap();

            string Weburl = ConfigurationManager.AppSettings["USERADMINGET"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Useradminmap> useradminlist = new List<Useradminmap>();

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
                            var rootObjects = JsonConvert.DeserializeObject<UserAdminRootObject>(jsonString);
                            useradminlist = rootObjects.Data;

                            //if (!string.IsNullOrEmpty(searchPharse))
                            //{
                            //    projectmasterlist = projectmasterlist
                            //        .Where(r => r.P_NAME.ToLower().Contains(searchPharse.ToLower()) ||
                            //                    r.P_SORTORDER.ToString().Contains(searchPharse.ToLower()))
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
            return View(useradminlist);
        }
    }
}