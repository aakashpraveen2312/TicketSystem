using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class LocationController : Controller
    {
        // GET: Location
        public ActionResult LocationList()
        {
            List<Locations> StorageList = new List<Locations>();
            string connectionString = ConfigurationManager.ConnectionStrings["Mystring"].ConnectionString;

            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();

                string cmd1 = @"
        SELECT 
            ROW_NUMBER() OVER(ORDER BY SP.SP_CRECID) AS SerialNumber,
            SP.SP_CRECID,
            SP.SP_RECID,
            SP.SP_Code,
            SP.SP_Name,
            SP.SP_Sortorder,
            SP.SP_LOCATIONTYPE,
            SP.SP_PartnerName,
            SP.SP_EmailID,
            SP.SP_ContactNumber,
            SP.SP_GSTNumber,
            LT.LT_NAME AS LocationTypeName
        FROM IM_StoragePoint SP
        LEFT JOIN LOCATIONTYPE LT ON SP.SP_LOCATIONTYPE = LT.LT_RECID
        WHERE SP.SP_CRECID = @CompanyId";

                using (SqlCommand sqlCdm = new SqlCommand(cmd1, sqlcon))
                {
                    sqlCdm.Parameters.AddWithValue("@CompanyId", Session["CompanyID"]);

                    using (SqlDataReader sqlread = sqlCdm.ExecuteReader())
                    {
                        while (sqlread.Read())
                        {
                            var SPList = new Locations
                            {
                                L_CRECID = (int)sqlread["SP_CRECID"],
                                L_RECID = (int)sqlread["SP_RECID"],
                                L_CODE = sqlread["SP_Code"].ToString(),
                                L_NAME = sqlread["SP_Name"].ToString(),
                                L_SORTORDER = (int)sqlread["SP_Sortorder"],
                                LT_RECID = sqlread["SP_LOCATIONTYPE"] != DBNull.Value ? (int)sqlread["SP_LOCATIONTYPE"] : 0,
                                LocationTypeName = sqlread["LocationTypeName"].ToString(),

                                // ✅ New fields
                                L_PartnerName = sqlread["SP_PartnerName"] != DBNull.Value ? sqlread["SP_PartnerName"].ToString() : string.Empty,
                                L_EmailID = sqlread["SP_EmailID"] != DBNull.Value ? sqlread["SP_EmailID"].ToString() : string.Empty,
                                L_ContactNumber = sqlread["SP_ContactNumber"] != DBNull.Value ? sqlread["SP_ContactNumber"].ToString() : string.Empty,
                                L_GSTNumber = sqlread["SP_GSTNumber"] != DBNull.Value ? sqlread["SP_GSTNumber"].ToString() : string.Empty
                            };

                            StorageList.Add(SPList);
                        }
                    }
                }
                sqlcon.Close();
            }

            return View(StorageList);
        }

        // GET: Location

        //public async Task<ActionResult> LocationList()
        //{

        //    string Weburl = ConfigurationManager.AppSettings["LOCATIONS"];

        //    string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
        //    string APIKey = Session["APIKEY"].ToString();

        //    List<Locations> Locationslist = new List<Locations>();

        //    string strparams = "cmprecid=" + Session["CompanyID"];
        //    string url = Weburl + "?" + strparams;

        //    try
        //    {
        //        using (HttpClientHandler handler = new HttpClientHandler())
        //        {
        //            handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        //            using (HttpClient client = new HttpClient(handler))
        //            {
        //                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //                var response = await client.GetAsync(url);

        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var jsonString = await response.Content.ReadAsStringAsync();
        //                    var rootObjects = JsonConvert.DeserializeObject<LocationsObjects>(jsonString);
        //                    Locationslist = rootObjects.Data;
        //                    ViewBag.LocationList = Locationslist;
        //                    //if (!string.IsNullOrEmpty(searchPharse))
        //                    //{
        //                    //    projectmasterlist = projectmasterlist
        //                    //        .Where(r => r.CU_CODE.ToLower().Contains(searchPharse.ToLower()) ||
        //                    //                    r.CU_EMAIL.ToString().Contains(searchPharse.ToLower()) ||
        //                    //                    r.CU_NAME.ToString().Contains(searchPharse.ToLower()) ||
        //                    //                    r.CU_MOBILENO.ToString().Contains(searchPharse.ToLower()) ||
        //                    //                    r.CU_INVOICENO.ToString().Contains(searchPharse.ToLower()) ||
        //                    //                    r.CU_WARRANTYFREECALLS.ToString().Contains(searchPharse.ToLower()) ||
        //                    //                    r.CU_WARRANTYUPTO.ToString().Contains(searchPharse.ToLower()) ||
        //                    //                    r.CU_SORTORDER.ToString().Contains(searchPharse.ToLower()))
        //                    //        .ToList();
        //                    //}

        //                }
        //                else
        //                {
        //                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
        //    }
        //    return View(Locationslist);
        //}

        //public ActionResult Create()
        //{
        //    return View();
        //}

        //public ActionResult LocationList()
        //{
        //    Locations objstoragePoint = new Locations();
        //    List<Locations> StorageList = new List<Locations>();
        //    string connectionString = ConfigurationManager.ConnectionStrings["Mystring"].ConnectionString;
        //    using (SqlConnection sqlcon = new SqlConnection(connectionString))
        //    {
        //        sqlcon.Open();

        //        string cmd1 = "SELECT ROW_NUMBER() OVER(ORDER BY SP_CRECID) AS SerialNumber,SP_CRECID,SP_RECID,SP_Code,SP_Name,SP_Type,SP_Sortorder FROM IM_StoragePoint where SP_CRECID =" + Session["CompanyID"] + " ";

        //        SqlCommand sqlCdm = new SqlCommand(cmd1, sqlcon); // take the values from the DB
        //        //sqlCdm.Parameters.AddWithValue("@C_RECID", id);
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

        //        }
        //        sqlcon.Close();

        //        //sqlcon.Open();
        //        //string cmd2 = "SELECT C_RECID, C_NAME FROM IM_COMPANIES WHERE C_RECID = " + id + "";
        //        //using (MySqlCommand command = new MySqlCommand(cmd2, sqlcon))
        //        //{
        //        //    MySqlDataReader reader = command.ExecuteReader();

        //        //    while (reader.Read())
        //        //    {
        //        //        string C_NAME = reader["C_NAME"].ToString();
        //        //        string C_RECID = reader["C_RECID"].ToString();
        //        //        Session["CMPID"] = C_RECID;
        //        //        Session["CMPNAME"] = C_NAME;
        //        //    }
        //        //}

        //        //sqlcon.Close();

        //    }


        //    return View(StorageList);
        //}



        private void PopulateLocationTypeList(int? selectedId = null)
        {
            List<LocationType> locationTypes = new List<LocationType>();
            string connectionString = ConfigurationManager.ConnectionStrings["Mystring"].ConnectionString;

            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                string cmdText = "SELECT LT_RECID, LT_NAME FROM LOCATIONTYPE ORDER BY LT_SORTORDER";

                using (SqlCommand cmd = new SqlCommand(cmdText, sqlcon))
                {

                    using (SqlDataAdapter sqlda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sqlda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            locationTypes.Add(new LocationType
                            {
                                LT_RECID = Convert.ToInt32(row["LT_RECID"]),
                                LT_NAME = row["LT_NAME"].ToString()
                            });
                        }
                    }
                }
            }

            ViewBag.LocationTypeList = new SelectList(locationTypes, "LT_RECID", "LT_NAME", selectedId);
        }

        public ActionResult Create()
        {
            PopulateLocationTypeList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Locations objStorage)
        {
            // ==========================================
            // Basic Null Check
            // ==========================================
            if (objStorage == null)
            {
                return Content(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "N",
                        Message = "Invalid location data."
                    }),
                    "application/json"
                );
            }

            // ==========================================
            // Location Code
            // ==========================================
            if (string.IsNullOrWhiteSpace(objStorage.L_CODE))
            {
                return Content(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "N",
                        Message = "Please enter the Code"
                    }),
                    "application/json"
                );
            }

            // ==========================================
            // Partner Name
            // ==========================================
            if (string.IsNullOrWhiteSpace(objStorage.L_PartnerName))
            {
                return Content(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "N",
                        Message = "Please enter the Partner Name"
                    }),
                    "application/json"
                );
            }


            // ==========================================
            // Contact Number
            // ==========================================
            if (string.IsNullOrWhiteSpace(objStorage.L_ContactNumber))
            {
                return Content(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "N",
                        Message = "Please enter the Contact Number"
                    }),
                    "application/json"
                );
            }

            // Contact Number - 10 digit validation
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                    objStorage.L_ContactNumber.Trim(),
                    @"^[0-9]{10}$"))
            {
                return Content(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "N",
                        Message = "Please enter a valid 10 digit Contact Number"
                    }),
                    "application/json"
                );
            }

            // ==========================================
            // Email
            // ==========================================
            if (string.IsNullOrWhiteSpace(objStorage.L_EmailID))
            {
                return Content(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "N",
                        Message = "Please enter the Email ID"
                    }),
                    "application/json"
                );
            }

            objStorage.L_EmailID = objStorage.L_EmailID.Trim();

            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (!System.Text.RegularExpressions.Regex.IsMatch(
                    objStorage.L_EmailID,
                    emailPattern))
            {
                return Content(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "N",
                        Message = "Please enter a valid Email ID"
                    }),
                    "application/json"
                );
            }


            // ==========================================
            // GST Number
            // ==========================================
            if (!string.IsNullOrWhiteSpace(objStorage.L_GSTNumber))
            {
                objStorage.L_GSTNumber = objStorage.L_GSTNumber.Trim().ToUpper();

                if (!System.Text.RegularExpressions.Regex.IsMatch(
                        objStorage.L_GSTNumber,
                        @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$"))
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Please enter a valid GST Number"
                        }),
                        "application/json"
                    );
                }
            }

            // ==========================================
            // Location Name
            // ==========================================
            if (string.IsNullOrWhiteSpace(objStorage.L_NAME))
            {
                return Content(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "N",
                        Message = "Please enter the Location Name"
                    }),
                    "application/json"
                );
            }

            // ==========================================
            // Location Type
            // ==========================================
            if (objStorage.LT_RECID <= 0)
            {
                return Content(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "N",
                        Message = "Please select the Location Type"
                    }),
                    "application/json"
                );
            }

            // ==========================================
            // Company ID
            // ==========================================
            if (Session["CompanyID"] == null)
            {
                return Content(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "N",
                        Message = "Company information is missing. Please login again."
                    }),
                    "application/json"
                );
            }




            objStorage.L_CRECID = (int)Session["CompanyID"];

           
                string connectionString = ConfigurationManager.ConnectionStrings["Mystring"].ConnectionString;
                using (SqlConnection sqlcon = new SqlConnection(connectionString))
                {
                    sqlcon.Open();
                    string cmd3 = @"INSERT INTO IM_StoragePoint 
               (SP_CODE, SP_NAME, SP_CRECID, SP_SORTORDER, SP_DISABLE, SP_LOCATIONTYPE,
                SP_PartnerName, SP_EmailID, SP_ContactNumber, SP_GSTNumber)
               VALUES 
               (@SP_CODE, @SP_NAME, @SP_CRECID, @SP_SORTORDER, @SP_DISABLE, @SP_LOCATIONTYPE,
                @SP_PartnerName, @SP_EmailID, @SP_ContactNumber, @SP_GSTNumber)";

                    using (SqlCommand com2 = new SqlCommand(cmd3, sqlcon))
                    {
                        com2.Parameters.AddWithValue("@SP_CRECID", objStorage.L_CRECID);
                        com2.Parameters.AddWithValue("@SP_CODE", objStorage.L_CODE);
                        com2.Parameters.AddWithValue("@SP_NAME", objStorage.L_NAME);
                        com2.Parameters.AddWithValue("@SP_SORTORDER", objStorage.L_SORTORDER);

                        string disable = objStorage.Disable ? "Y" : "N";
                        com2.Parameters.AddWithValue("@SP_DISABLE", disable);

                        com2.Parameters.AddWithValue("@SP_LOCATIONTYPE", objStorage.LT_RECID);

                        com2.Parameters.AddWithValue("@SP_PartnerName", objStorage.L_PartnerName ?? (object)DBNull.Value);
                        com2.Parameters.AddWithValue("@SP_EmailID", objStorage.L_EmailID ?? (object)DBNull.Value);
                        com2.Parameters.AddWithValue("@SP_ContactNumber", objStorage.L_ContactNumber);
                        com2.Parameters.AddWithValue("@SP_GSTNumber", objStorage.L_GSTNumber ?? (object)DBNull.Value);

                        com2.ExecuteNonQuery();
                    }
                }

                return Content(
             JsonConvert.SerializeObject(new
                             {
                                 Status = "Y",
                                 Message = "Created Successfully",
                                 RedirectUrl = Url.Action(
                                     "LocationList",
                                     "Location",
                                     new
                                     {
                                         id = Session["CompanyId"]
                                     }
                                 )
                             }),
                             "application/json"
                         );
         
            // Re-populate dropdown list for the view if validation fails
            PopulateLocationTypeList(objStorage.LT_RECID);

            return View(objStorage);
        }





        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Session["SPRecID"] = id;

            Locations strEdit = new Locations();

            string connectionString = ConfigurationManager.ConnectionStrings["Mystring"].ConnectionString;

            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                // 1️⃣ Fetch the StoragePoint record including new fields
                string cmd3 = "SELECT * FROM IM_StoragePoint WHERE SP_RECID = @SP_RECID";
                using (SqlCommand cmd = new SqlCommand(cmd3, sqlcon))
                {
                    cmd.Parameters.AddWithValue("@SP_RECID", id);

                    using (SqlDataAdapter sqlda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sqlda.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];

                            strEdit.L_CODE = row["SP_CODE"].ToString();
                            strEdit.L_NAME = row["SP_NAME"].ToString();
                            strEdit.L_SORTORDER = Convert.ToInt32(row["SP_SORTORDER"]);
                            strEdit.L_DISABLE = row["SP_DISABLE"].ToString();
                            strEdit.Disable = row["SP_DISABLE"].ToString() != "N";

                            strEdit.LT_RECID = row["SP_LOCATIONTYPE"] != DBNull.Value
                                ? Convert.ToInt32(row["SP_LOCATIONTYPE"])
                                : 0;

                            // ✅ Load new fields
                            strEdit.L_PartnerName = row["SP_PartnerName"] != DBNull.Value
     ? row["SP_PartnerName"].ToString()
     : string.Empty;
                            strEdit.L_EmailID = row["SP_EmailID"] != DBNull.Value
     ? row["SP_EmailID"].ToString()
     : string.Empty;
                            strEdit.L_ContactNumber = row["SP_ContactNumber"].ToString();
                            strEdit.L_GSTNumber = row["SP_GSTNumber"].ToString();
                        }
                        else
                        {
                            return HttpNotFound("Storage Point not found.");
                        }
                    }
                }

                // 2️⃣ Load Location Types for dropdown
                string cmdLoc = "SELECT LT_RECID, LT_NAME FROM LOCATIONTYPE  ORDER BY LT_SORTORDER";
                using (SqlCommand cmdLocTypes = new SqlCommand(cmdLoc, sqlcon))
                {


                    using (SqlDataAdapter da = new SqlDataAdapter(cmdLocTypes))
                    {
                        DataTable dtLoc = new DataTable();
                        da.Fill(dtLoc);

                        List<LocationType> locationTypes = new List<LocationType>();
                        foreach (DataRow locRow in dtLoc.Rows)
                        {
                            locationTypes.Add(new LocationType
                            {
                                LT_RECID = Convert.ToInt32(locRow["LT_RECID"]),
                                LT_NAME = locRow["LT_NAME"].ToString()
                            });
                        }

                        ViewBag.LocationTypeList = new SelectList(locationTypes, "LT_RECID", "LT_NAME", strEdit.LT_RECID);
                    }
                }
            }

            return View(strEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Locations objstr)
        {
            try
            {
                // ==========================================
                // Basic Null Check
                // ==========================================
                if (objstr == null)
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Invalid location data."
                        }),
                        "application/json"
                    );
                }

                // ==========================================
                // Record ID
                // ==========================================
                if (Session["SPRecID"] == null)
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Location information is missing. Please try again."
                        }),
                        "application/json"
                    );
                }

                objstr.L_RECID = (int)Session["SPRecID"];

                if (objstr.L_RECID <= 0)
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Invalid location record."
                        }),
                        "application/json"
                    );
                }

                // ==========================================
                // Location Code
                // ==========================================
                if (string.IsNullOrWhiteSpace(objstr.L_CODE))
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Please enter the Code"
                        }),
                        "application/json"
                    );
                }

                objstr.L_CODE = objstr.L_CODE.Trim();

                // ==========================================
                // Partner Name
                // ==========================================
                if (string.IsNullOrWhiteSpace(objstr.L_PartnerName))
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Please enter the Partner Name"
                        }),
                        "application/json"
                    );
                }

                objstr.L_PartnerName = objstr.L_PartnerName.Trim();

                // ==========================================
                // Contact Number
                // ==========================================
                if (string.IsNullOrWhiteSpace(objstr.L_ContactNumber))
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Please enter the Contact Number"
                        }),
                        "application/json"
                    );
                }

                objstr.L_ContactNumber = objstr.L_ContactNumber.Trim();

                if (!System.Text.RegularExpressions.Regex.IsMatch(
                        objstr.L_ContactNumber,
                        @"^[0-9]{10}$"))
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Please enter a valid 10 digit Contact Number"
                        }),
                        "application/json"
                    );
                }

                // ==========================================
                // Email
                // ==========================================
                if (string.IsNullOrWhiteSpace(objstr.L_EmailID))
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Please enter the Email ID"
                        }),
                        "application/json"
                    );
                }

                objstr.L_EmailID = objstr.L_EmailID.Trim();

                string emailPattern =
                    @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

                if (!System.Text.RegularExpressions.Regex.IsMatch(
                        objstr.L_EmailID,
                        emailPattern))
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Please enter a valid Email ID"
                        }),
                        "application/json"
                    );
                }

                

                // ==========================================
                // Location Name
                // ==========================================
                if (string.IsNullOrWhiteSpace(objstr.L_NAME))
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Please enter the Location Name"
                        }),
                        "application/json"
                    );
                }

                objstr.L_NAME = objstr.L_NAME.Trim();

                // ==========================================
                // Location Type
                // ==========================================
                if (objstr.LT_RECID <= 0)
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Please select the Location Type"
                        }),
                        "application/json"
                    );
                }

                // ==========================================
                // Company ID
                // ==========================================
                if (Session["CompanyID"] == null)
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Company information is missing. Please login again."
                        }),
                        "application/json"
                    );
                }

                objstr.L_CRECID = (int)Session["CompanyID"];

                // ==========================================
                // Model State
                // ==========================================
                if (!ModelState.IsValid)
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Please enter valid location details."
                        }),
                        "application/json"
                    );
                }

                // ==========================================
                // Database Update
                // ==========================================
                string connectionString =
                    ConfigurationManager.ConnectionStrings["Mystring"].ConnectionString;

                using (SqlConnection sqlcon =
                       new SqlConnection(connectionString))
                {
                    sqlcon.Open();

                    string cmd4 = @"
                UPDATE IM_StoragePoint SET
                    SP_CODE = @SP_CODE,
                    SP_NAME = @SP_NAME,
                    SP_SORTORDER = @SP_SORTORDER,
                    SP_DISABLE = @SP_DISABLE,
                    SP_LOCATIONTYPE = @SP_LOCATIONTYPE,
                    SP_PartnerName = @SP_PartnerName,
                    SP_EmailID = @SP_EmailID,
                    SP_ContactNumber = @SP_ContactNumber,
                    SP_GSTNumber = @SP_GSTNumber
                WHERE SP_RECID = @SP_RECID";

                    using (SqlCommand com4 =
                           new SqlCommand(cmd4, sqlcon))
                    {
                        com4.Parameters.AddWithValue(
                            "@SP_RECID",
                            objstr.L_RECID
                        );

                        com4.Parameters.AddWithValue(
                            "@SP_CODE",
                            objstr.L_CODE
                        );

                        com4.Parameters.AddWithValue(
                            "@SP_NAME",
                            objstr.L_NAME
                        );

                        com4.Parameters.AddWithValue(
                            "@SP_SORTORDER",
                            objstr.L_SORTORDER
                        );

                        string disable =
                            objstr.Disable ? "Y" : "N";

                        com4.Parameters.AddWithValue(
                            "@SP_DISABLE",
                            disable
                        );

                        com4.Parameters.AddWithValue(
                            "@SP_LOCATIONTYPE",
                            objstr.LT_RECID
                        );

                        com4.Parameters.AddWithValue(
                            "@SP_PartnerName",
                            objstr.L_PartnerName ??
                            (object)DBNull.Value
                        );

                        com4.Parameters.AddWithValue(
                            "@SP_EmailID",
                            objstr.L_EmailID ??
                            (object)DBNull.Value
                        );

                        com4.Parameters.AddWithValue(
                            "@SP_ContactNumber",
                            objstr.L_ContactNumber ??
                            (object)DBNull.Value
                        );

                        com4.Parameters.AddWithValue(
                            "@SP_GSTNumber",
                            objstr.L_GSTNumber ??
                            (object)DBNull.Value
                        );

                        int rowsAffected =
                            com4.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return Content(
                                JsonConvert.SerializeObject(new
                                {
                                    Status = "N",
                                    Message = "Location not found."
                                }),
                                "application/json"
                            );
                        }
                    }
                }

                // ==========================================
                // SUCCESS
                // ==========================================
                return Content(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "Y",
                        Message = "Updated Successfully",
                        RedirectUrl = Url.Action(
                            "LocationList",
                            "Location",
                            new
                            {
                                id = Session["CompanyID"]
                            }
                        )
                    }),
                    "application/json"
                );
            }
            catch (Exception)
            {
                return Content(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "N",
                        Message = "Unable to update the location. Please try again."
                    }),
                    "application/json"
                );
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Content(
                        JsonConvert.SerializeObject(new
                        {
                            Status = "N",
                            Message = "Invalid location."
                        }),
                        "application/json"
                    );
                }

                string connectionString =
                    ConfigurationManager.ConnectionStrings["Mystring"].ConnectionString;

                using (SqlConnection sqlcon = new SqlConnection(connectionString))
                {
                    sqlcon.Open();

                    string cmd5 = @"
                DELETE FROM IM_StoragePoint
                WHERE SP_RECID = @SP_RECID";

                    using (SqlCommand com5 = new SqlCommand(cmd5, sqlcon))
                    {
                        com5.Parameters.AddWithValue("@SP_RECID", id);

                        int rowsAffected = com5.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return Content(
                                JsonConvert.SerializeObject(new
                                {
                                    Status = "N",
                                    Message = "Location not found."
                                }),
                                "application/json"
                            );
                        }
                    }
                }

                return Content(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "Y",
                        Message = "Deleted Successfully",
                        RedirectUrl = Url.Action(
                            "LocationList",
                            "Location",
                            new
                            {
                                id = Session["CompanyID"]
                            }
                        )
                    }),
                    "application/json"
                );
            }
            catch (Exception)
            {
                return Content(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "N",
                        Message = "Unable to delete the location. Please try again."
                    }),
                    "application/json"
                );
            }
        }

        [HttpGet]
        public ActionResult LocationType()
        {
            LocationType strEdit = new LocationType();

            string connectionString = ConfigurationManager.ConnectionStrings["Mystring"].ConnectionString;

            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                string cmdText = "SELECT * FROM LOCATIONTYPE";

                using (SqlCommand cmd = new SqlCommand(cmdText, sqlcon))
                {


                    using (SqlDataAdapter sqlda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sqlda.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            strEdit.LT_RECID = Convert.ToInt32(row["LT_RECID"]);
                            strEdit.LT_NAME = row["LT_NAME"].ToString();
                            strEdit.LT_SORTORDER = row["LT_SORTORDER"] != DBNull.Value
                                ? Convert.ToInt32(row["LT_SORTORDER"])
                                : (int?)null;
                            strEdit.LT_DISABLE = row["LT_DISABLE"].ToString();
                            strEdit.LT_CRECID = Convert.ToInt32(row["LT_CRECID"]);
                        }
                        else
                        {
                            return HttpNotFound("Location Type not found.");
                        }
                    }
                }
            }

            return View(strEdit);
        }


    }
}