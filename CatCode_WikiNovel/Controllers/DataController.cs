using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using WowCommon;
using WowCore;
using WowSQL;

namespace CatCode_WikiNovel.Controllers
{
    [AllowAnonymous]
    public class DataController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DataController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        [Route("ExecuteQuery")]
        [Route("data/ExecuteQuery")]
        public ActionResult ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                if (query == null)
                {
                    query = "Select ID, Description  from tblLog Order By ID DESC";
                }
                if (!string.IsNullOrEmpty(query))
                {
                    // dt = SqlHelper.ExecuteDataTable(query);
                }
            }
            catch (Exception ex)
            {
            }

            return View(new object[] { query, dt });
        }

        [Route("data/exec")]
        [Route("exec")]
#if !DEBUG
        [HttpPost]
#endif
        public string EXEC(string query, string jParam = "")
        {

            DataSet dsResult;
            bool formatJson = false;
            try
            {
                if (query.Length > 200 || query.Contains(" "))
                {
                    return string.Empty;
                }
                int uid = -1;
                string uidPhone = "";
                string phone = "";
                int fVersion = -1;

                string db = "";
                string uidBackup = "";
                bool kDebugMode = false;
                List<object> lst = new List<object>();
                bool existsParaIsWeb = false;
                bool existsLanguage = false;
                if (!string.IsNullOrEmpty(jParam))
                {
                    Dictionary<string, object> param = string.IsNullOrEmpty(jParam) ? new Dictionary<string, object>() : JsonConvert.DeserializeObject<Dictionary<string, object>>(jParam);
                    foreach (var item in param)
                    {
                        string paraName = item.Key.StartsWith("@") ? item.Key : "@" + item.Key;
                        lst.Add(paraName);
                        if (paraName == "@password" && item.Value != null)
                        {
                            lst.Add(EnDe.EncryptText(item.Value.ToString()));
                        }
                        else
                        {
                            lst.Add(item.Value);
                        }
                        try
                        {
                            if ((paraName == "@uid") && !string.IsNullOrEmpty(item.Value?.ToString()))
                            {
                                int.TryParse(item.Value.ToString(), out uid);
                            }
                            else if ((paraName == "@kDebugMode") && !string.IsNullOrEmpty(item.Value?.ToString()))
                            {
                                kDebugMode = Convert.ToBoolean(item.Value?.ToString());
                            }
                            else if (paraName == "@isWeb")
                            {
                                existsParaIsWeb = true;
                            }
                            else if (paraName == "@languageId")
                            {
                                existsLanguage = true;
                            }
                            else if (paraName == "@uidPhone")
                            {
                                uidPhone = item.Value?.ToString();
                            }
                            else if (paraName == "@fVersion")
                            {
                                int.TryParse(item.Value.ToString(), out fVersion);
                            }
                            else if (paraName == "@uidBackup")
                            {
                                uidBackup = item.Value?.ToString();
                            }
                            else if (paraName == "@phone")
                            {
                                phone = item.Value?.ToString();
                            }
                            else if (paraName == "@formatJson")
                            {
                                formatJson = Convert.ToBoolean(item.Value?.ToString());
                            }
                            else if (paraName == "@db")
                            {
                                db = item.Value?.ToString();
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                Common.Mode = ModeLog.API;

                if (!existsParaIsWeb)
                {
                    lst.Add("@isWeb");
                    lst.Add(false);
                }

                if (!existsLanguage)
                {
                    lst.Add("@languageId");
                    lst.Add(1);
                }
                EConnection dbBAK = EConnection.None;
                if (!string.IsNullOrEmpty(db) && int.TryParse(db, out int edb))
                {
                    dbBAK = Common.EConnection;
                    //SqlHelper.ConnectDB((EConnection)edb, HttpContext.Server.MapPath("bin/" + ((EConnection)edb).ToString() + ".catcode.db"));

                }
                dsResult = SqlHelper.ExecuteDataset(query, lst.ToArray());
                if (dbBAK != EConnection.None)
                {
                    //SqlHelper.ConnectDB(dbBAK, HttpContext.Server.MapPath("bin/" + dbBAK.ToString() + ".catcode.db"));
                }
                if (dsResult.Tables.Count > 1)
                {
                    var dtTableName = dsResult.Tables[dsResult.Tables.Count - 1];
                    if (dtTableName.Columns.Contains("tableName"))
                    {
                        for (int i = 0; i < dsResult.Tables.Count - 1; i++)
                        {
                            if (dtTableName.Rows.Count > i)
                            {
                                dsResult.Tables[i].TableName = dtTableName.Rows[i]["tableName"].ToString();
                            }
                        }
                        dsResult.Tables[dsResult.Tables.Count - 1].TableName = "tableName";
                    }
                }
                else if (dsResult.Tables.Count == 1)
                {
                    dsResult.Tables[0].TableName = query;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                Common.Mode = ModeLog.Web;
            }

            Dictionary<string, object> dictResult = new Dictionary<string, object>();
            try
            {
                DataTable dtTableNameMap = null;
                if (dsResult.Tables.Count > 1 && dsResult.Tables[dsResult.Tables.Count - 1].Columns.Contains("tableName"))
                {
                    if (formatJson)
                    {
                        dtTableNameMap = dsResult.Tables[dsResult.Tables.Count - 1].Copy();
                    }
                    dsResult.Tables.RemoveAt(dsResult.Tables.Count - 1);
                }
                dictResult = dsResult.ToDictionaryDataSet(dtTableNameMap: dtTableNameMap);
                if (!dictResult.Any()) return null;
            }
            catch (Exception ex)
            {
                JsonConvert.SerializeObject(new Dictionary<string, object>() {
                    { "methods", "Data.EXEC" }
                    , { "statusCode", -3 }
                    , { "message", ex.Message } }
                , Formatting.Indented);
            }


            return JsonConvert.SerializeObject(dictResult, Formatting.Indented);
        }

        [HttpPost]
        [Route("data/CreateAccount")]
        public void CreateAccount(string phoneNumber, string uidPhone)
        {
            //nothing
            try
            {
                SqlHelper.ExecuteNonQuery("sp_CreateAccount"
                    , "@phone", phoneNumber
                    , "@uidPhone", uidPhone);
            }
            catch (Exception ex)
            {
                SqlHelper.ExecuteNonQuery("sp_SystemLog", "@title", "CreateAccount Exception: " + ex.Message
                   , "@machineName", Environment.MachineName
                   , "@callAt", "CreateAccount"
                   , "@mode", ModeLog.API);
            }
            finally
            {
                Common.Mode = ModeLog.Web;
            }
        }

#if !DEBUG
        [HttpPost]
#endif
        [Route("CheckConnectionInfo")]
        [Route("Data/CheckConnectionInfo")]
        public int CheckConnectionInfo()
        {

            try
            {
                string mgs;
                if (SqlHelper.CheckConnectionInfo(out mgs))
                {
                    return 3;
                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
            }


        }
#if !DEBUG
        [HttpPost]
#endif
        [Route("CheckConnectionInfo2")]
        [Route("Data/CheckConnectionInfo2")]
        public string CheckConnectionInfo2()
        {
            try
            {
                SqlHelper.SetConnection();
                using (SqlConnection sql = new SqlConnection(SqlHelper.connectionString))
                {
                    sql.Open();
                    sql.Close();
                }
                return "Connection success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        [HttpGet]
        public virtual ActionResult Download(string fileName)
        {
            try
            {
                if (!fileName.EndsWith(".dart")) return null;
                //fileName should be like "photo.jpg" 
                string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, fileName);
                return File(fullPath, "application/octet-stream", fileName);
            }
            catch (Exception)
            {
            }
            return null;
        }

        [Route("empty")]
        public string Empty()
        {
            return string.Empty;
        }


        [HttpPost]
        [Route("api/EncryptText")]
        public string EncryptText(string text)
        {
            return EnDe.EncryptText(text);
        }
    }
}
