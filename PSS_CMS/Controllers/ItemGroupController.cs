using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class ItemGroupController : Controller
    {
        // GET: ItemGroup

        public async Task<ActionResult> List( string searchphrase)
        {
            int serialNo = 1;

            // Initialize necessary configurations
            string webUrlGet = ConfigurationManager.AppSettings["GETITEMGROUP"];
            string authKey = ConfigurationManager.AppSettings["Authkey"];



            string apiKey = Session["APIKEY"].ToString();
           
            string strParams = "companyId=" + Session["CompanyId"];
            string finalUrl = $"{webUrlGet}?{strParams}";

            DataTable dt = Session["ItemGroupListTable"] as DataTable;
            List<ItemGroup> itemGroupList = new List<ItemGroup>();

           
                dt = new DataTable();

                try
                {
                    using (HttpClientHandler handler = new HttpClientHandler())
                    {
                        handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("ApiKey", apiKey);
                            client.DefaultRequestHeaders.Add("Authorization", authKey);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                            var response = await client.GetAsync(finalUrl);

                            if (response.IsSuccessStatusCode)
                            {
                                var jsonString = await response.Content.ReadAsStringAsync();
                                var rootObjects = JsonConvert.DeserializeObject<IGRootObjects>(jsonString);
                                itemGroupList = rootObjects?.Data ?? new List<ItemGroup>();

                                // Set SerialNumber for each ItemGroup
                                foreach (var item in itemGroupList)
                                {
                                    item.SerialNumber = serialNo++;
                                }

                                // Define DataTable columns
                                dt.Columns.Add("SerialNumber", typeof(int));
                                dt.Columns.Add("IGCode", typeof(string));
                                dt.Columns.Add("IGName", typeof(string));
                                dt.Columns.Add("CRecID", typeof(string));
                                dt.Columns.Add("IG_Recid", typeof(int));
                                dt.Columns.Add("SortOrder", typeof(int));
                                // Additional fields as required

                                // Populate DataTable
                                foreach (var itemgroup in itemGroupList)
                                {
                                    DataRow row = dt.NewRow();
                                    row["SerialNumber"] = itemgroup.SerialNumber;
                                    row["IGCode"] = itemgroup.IG_CODE;
                                    row["IGName"] = itemgroup.IG_DESCRIPTION;
                                    row["CRecID"] = itemgroup.IG_CRECID;
                                    row["SortOrder"] = itemgroup.IG_SORTORDER;
                                    row["IG_Recid"] = itemgroup.IG_RECID;
                                    dt.Rows.Add(row);
                                }

                                Session["ItemGroupListTable"] = dt;
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
            

            // Convert DataTable to List<ItemGroup>
            itemGroupList = dt.AsEnumerable().Select(row => new ItemGroup
            {

                SerialNumber = row.Field<int>("SerialNumber"),
                IG_CODE = row.Field<string>("IGCode"),
                IG_DESCRIPTION = row.Field<string>("IGName"),
                IG_CRECID = row.Field<string>("CRecID"),
                IG_SORTORDER = row.Field<int>("SortOrder"),
                IG_RECID = row.Field<int>("IG_Recid")
                // Add additional mappings as required
            }).ToList();

            // Perform search if searchPhrase is provided
            if (!string.IsNullOrEmpty(searchphrase))
            {
                DataTable filteredDt = dt.Clone();
                string escapedSearchPhrase = searchphrase.Replace("'", "''");
                foreach (DataRow dr in dt.Select($"CONVERT(SerialNumber, 'System.String') LIKE '%{escapedSearchPhrase}%' OR IGCode LIKE '%{escapedSearchPhrase}%' OR IGName LIKE '%{escapedSearchPhrase}%'"))
                {
                    filteredDt.ImportRow(dr);
                }

                itemGroupList = filteredDt.AsEnumerable().Select(row => new ItemGroup
                {
                    SerialNumber = row.Field<int>("SerialNumber"),
                    IG_CODE = row.Field<string>("IGCode"),
                    IG_DESCRIPTION = row.Field<string>("IGName"),
                    IG_CRECID = row.Field<string>("CRecID"),
                    IG_SORTORDER = row.Field<int>("SortOrder"),
                    IG_RECID = row.Field<int>("IG_Recid")
                    // Add additional mappings as required
                }).ToList();
            }

            return View(itemGroupList);
        }

    }
}