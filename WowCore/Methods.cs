using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using FirebaseAdmin.Messaging;
using Newtonsoft.Json;
using WowCommon;
using WowCommon.Model;
using WowSQL;

namespace WowCore
{
    public static class Methods
    {
        public static List<string> ListDefaultColumTemplate = new List<string>
        {
            Connection.KEY_DOCUMENT_ID,
            "uid",
            "statusData",
            "createDate",
            "createMachineName"
        };

        public const string SearchColumn = "SearchColumn_SearchColumn";

        public const string pattent = "__";

        private static Dictionary<int, List<string>> dictFormatTemplate = new Dictionary<int, List<string>>
        {
            {
                8,
                new List<string> { "yyyyMMdd" }
            },
            {
                10,
                new List<string> { "yyyy-MM-dd", "dd/MM/yyyy" }
            },
            {
                17,
                new List<string> { "HH:mm:ss yyyyMMdd", "yyyyMMdd HH:mm:ss" }
            },
            {
                19,
                new List<string> { "HH:mm:ss yyyy/MM/dd", "HH:mm:ss yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss" }
            },
            {
                22,
                new List<string> { "dd/MM/yyyy HH:mm:ss tt", "MM/dd/yyyy HH:mm:ss tt" }
            },
            {
                16,
                new List<string> { "dd/MM/yyyy HH:mm" }
            }
        };

        private static Dictionary<string, object> LocalSetting = null;

        public static string lstKyTuDatTenFile = "abcdefghijklmnopqrstuvwxyz_-ABCDEFGHIJKLKMNOPQRSTUVWXYZ1234567890";

        private static DataTable tmpUserList = null;

        public static Dictionary<string, string> foreign_characters = new Dictionary<string, string>
        {
            { "ÀÁÂÃÄÅǺĀĂĄǍΑΆẢẠẦẪẨẬẰẮẴẲẶА", "A" },
            { "àáâãåǻāăąǎªαάảạầấẫẩậằắẵẳặа", "a" },
            { "ÈÉÊËĒĔĖĘĚΕΈẼẺẸỀẾỄỂỆЕЭ", "E" },
            { "èéêëēĕėęěέεẽẻẹềếễểệеэ", "e" },
            { "ÌÍÎÏĨĪĬǏĮİΗΉΊΙΪỈỊИЫ", "I" },
            { "ìíîïĩīĭǐįıηήίιϊỉịиыї", "i" },
            { "ÒÓÔÕŌŎǑŐƠØǾΟΌΩΏỎỌỒỐỖỔỘỜỚỠỞỢО", "O" },
            { "òóôõōŏǒőơøǿºοόωώỏọồốỗổộờớỡởợо", "o" },
            { "ÙÚÛŨŪŬŮŰŲƯǓǕǗǙǛŨỦỤỪỨỮỬỰУ", "U" },
            { "ùúûũūŭůűųưǔǖǘǚǜυύϋủụừứữửựу", "u" },
            { "ÝŸŶΥΎΫỲỸỶỴЙ", "Y" },
            { "ýÿŷỳỹỷỵй", "y" },
            { "đ", "d" },
            { "Đ", "D" }
        };

        public static string lstCharDocNum = "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLKMNOPQRSTUVWXYZ1234567890_";

        private static readonly string[] VietnameseSigns = new string[15]
        {
            "aAeEoOuUiIdDyY", "áàạảãâấầậẩẫăắằặẳẵ", "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ", "éèẹẻẽêếềệểễ", "ÉÈẸẺẼÊẾỀỆỂỄ", "óòọỏõôốồộổỗơớờợởỡ", "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ", "úùụủũưứừựửữ", "ÚÙỤỦŨƯỨỪỰỬỮ", "íìịỉĩ",
            "ÍÌỊỈĨ", "đ", "Đ", "ýỳỵỷỹ", "ÝỲỴỶỸ"
        };

        public static List<string> lstError = new List<string>();

        private const string highlight_text = "highlight-text";

        public static Dictionary<int, string> DictImageProject
        {
            get
            {
                if (Common.DICT_IMAGE_PROJECT == null || !Common.DICT_IMAGE_PROJECT.Any())
                {
                    foreach (DataRow row in SqlHelper.ExecuteDataTable("sp_IconsList").Rows)
                    {
                        int key = int.Parse(row["ID"].ToString());
                        if (!Common.DICT_IMAGE_PROJECT.ContainsKey(key))
                        {
                            Common.DICT_IMAGE_PROJECT.Add(key, row["hinhAnh_image_path"].ToString());
                        }
                    }
                }

                return Common.DICT_IMAGE_PROJECT;
            }
        }

        public static void ResetCached()
        {
            Common.DICT_MESSAGE.Clear();
            Common.DICT_IMAGE_PROJECT.Clear();
            Common.DICT_TABLE_SETTING.Clear();
            Common.DICT_FORM_SETTING.Clear();
            Common.DICT_INFORMATION_SCHEMA_TABLE.Clear();
            Common.DICT_TABLE_SETTING_DATA_SET.Clear();
            Common.DICT_BASE_SETTING_LAYOUT.Clear();
        }

        public static string GetMessage(string messageId, string languageCode = "vn")
        {
            return GetMessage(messageId, GetLanguageId(languageCode));
        }

        public static void InitMessage(DataTable dt)
        {
            Common.DICT_MESSAGE.Clear();
            foreach (KeyValuePair<int, string> item in Common.DICT_LANUGAE_CODE)
            {
                Common.DICT_MESSAGE[item.Value] = new Dictionary<string, string>();
            }

            foreach (DataRow row in dt.Rows)
            {
                foreach (KeyValuePair<int, string> item2 in Common.DICT_LANUGAE_CODE)
                {
                    string text = ((item2.Value == "vn") ? "title" : ("title_" + item2.Value));
                    if (dt.Columns.Contains(text) && row[text] != DBNull.Value && !string.IsNullOrEmpty(row[text].ToString()))
                    {
                        try
                        {
                            Common.DICT_MESSAGE[item2.Value][row.GetValue("messageId")] = row[text].ToString();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        public static T GetParameter<T>(string CODE, T defaultValue)
        {
            try
            {
                return ConvertValueToType(GetSettingTable("tblParameter").AsEnumerable().FirstOrDefault((DataRow dr) => dr["code"].ToString() == CODE)?["value"]?.ToString(), defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string GetDebugMessage(string messageId, int languageId = 1)
        {
            try
            {
                DataTable dataTable = SqlHelper.ExecuteDataTable("sp_GetMessage", "@messageId", messageId, "@languageId", languageId);
                if (dataTable != null)
                {
                    if (dataTable.Rows.Count > 0)
                    {
                        if (dataTable.Rows[0][0] != DBNull.Value)
                        {
                            return dataTable.Rows[0].GetValue("message", messageId).ToString();
                        }

                        return messageId;
                    }

                    return messageId;
                }

                return messageId;
            }
            catch
            {
                return messageId;
            }
        }

        public static string GetMessage(string messageId, int languageId = 1)
        {
            try
            {
                messageId = messageId.ToUpper();
                if (!Common.DICT_MESSAGE.Any())
                {
                    InitMessage(SqlHelper.ExecuteDataTable("sp_GetAllMessage", "@languageId", languageId));
                }

                if (Common.DICT_MESSAGE[GetLanguageCode(languageId)].ContainsKey(messageId))
                {
                    return Common.DICT_MESSAGE[GetLanguageCode(languageId)][messageId];
                }

                return messageId;
            }
            catch
            {
                return messageId;
            }
        }

        public static string GetSettingCompany(object objCode, string languageCode = "vn")
        {
            return GetSettingCompany(objCode, GetLanguageId(languageCode));
        }

        public static string GetSettingCompany(object objCode, int languageId = 1)
        {
            string code = objCode?.ToString();
            if (string.IsNullOrEmpty(code))
            {
                return "code is null";
            }

            try
            {
                return GetSettingTable("tblThietLapCongTy").AsEnumerable().FirstOrDefault((DataRow dr) => dr["Code"]?.ToString() == code)?["title"]?.ToString();
            }
            catch
            {
            }

            return code;
        }

        public static string GetLanguageCode(int languageId)
        {
            if (Common.DICT_LANUGAE_CODE.ContainsKey(languageId))
            {
                return Common.DICT_LANUGAE_CODE[languageId];
            }

            return Common.DEFAULT_LANGUAGE_CODE;
        }

        public static int GetLanguageId(string languageCode)
        {
            if (Common.DICT_LANUGAE_CODE.ContainsValue(languageCode))
            {
                return Common.DICT_LANUGAE_CODE.AsEnumerable().FirstOrDefault((KeyValuePair<int, string> d) => d.Value.ToLower() == languageCode).Key;
            }

            return Common.DEFAULT_LANGUAGE_ID;
        }

        public static string GetLunarDate(string data)
        {
            try
            {
                if (string.IsNullOrEmpty(data))
                {
                    data = "";
                }

                ("GetLunnarDate(data: " + data + ")").Log();
                if (string.IsNullOrEmpty(data))
                {
                    return string.Empty;
                }

                DateTime dateTime = new DateTime(1990, 1, 1);
                DateTime dateTime2 = data.ToDateTime(dateTime);
                if (dateTime2 == dateTime)
                {
                    return string.Empty;
                }

                int[] array = new Solar2Lunar().convertSolar2Lunar(dateTime2.Day, dateTime2.Month, dateTime2.Year, 7);
                return new DateTime(array[2], array[1], array[0]).ToString("yyyyMMdd HH:mm:ss");
            }
            catch (Exception ex)
            {
                ex.Log();
                return data;
            }
        }

        public static DataTable DictToDataTable(List<Dictionary<string, object>> list)
        {
            DataTable dataTable = new DataTable();
            if (list.Count == 0)
            {
                return dataTable;
            }

            IEnumerable<string> source = list.SelectMany((Dictionary<string, object> dict) => dict.Keys).Distinct();
            dataTable.Columns.AddRange(source.Select((string c) => new DataColumn(c)).ToArray());
            foreach (Dictionary<string, object> item in list)
            {
                DataRow dataRow = dataTable.NewRow();
                foreach (string key in item.Keys)
                {
                    dataRow[key] = item[key];
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        public static bool RemoteFileExists(string url)
        {
            try
            {
                HttpWebRequest obj = WebRequest.Create(url) as HttpWebRequest;
                obj.Method = "HEAD";
                HttpWebResponse obj2 = obj.GetResponse() as HttpWebResponse;
                obj2.Close();
                return obj2.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                ex.Log();
                return false;
            }
        }

        public static void CreateDirectory(string folder)
        {
            try
            {
                string text = "";
                string[] array = folder.Split(new string[2] { "/", "\\" }, StringSplitOptions.None);
                string[] array2 = array;
                foreach (string text2 in array2)
                {
                    if (text2 == array[1] && text2.Contains("."))
                    {
                        break;
                    }

                    text = ((!(text != "")) ? text2 : (text + "/" + text2));
                    if (!Directory.Exists(text))
                    {
                        Directory.CreateDirectory(text);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void ADDDateTimeAgo(this Dictionary<string, object> objItem, string fieldName, Dictionary<string, object> dictItem)
        {
            string valueTemplate = dictItem.getValueTemplate(fieldName, "");
            objItem.Add(fieldName, valueTemplate);
            if (!string.IsNullOrEmpty(valueTemplate))
            {
                objItem.Add("txt_" + fieldName, TimeAgoString(valueTemplate.ToDateTime()));
            }
        }

        public static void AddDisplayMember(this Dictionary<string, object> objItem, string fieldName, List<KeyValue> datasource, Dictionary<string, object> dictItem)
        {
            long val = dictItem.getValueTemplate(fieldName, -1L);
            objItem.Merge(fieldName, val);
            objItem.Merge("text_" + fieldName, datasource.Where((KeyValue t) => t.ValueMember == val)?.FirstOrDefault()?.DisplayMember);
        }

        public static List<object> StringToListLong(object obj)
        {
            if (obj == null)
            {
                return new List<object>();
            }

            string[] array = obj.ToString()?.Split(new char[1] { ',' });
            List<object> list = new List<object>();
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                if (long.TryParse(array2[i].Trim(), out var result))
                {
                    list.Add(result);
                }
            }

            return list;
        }

        public static string PreparePhoneNumber(string phoneNumer, string countryCode = "vn")
        {
            try
            {
                if (phoneNumer.StartsWith("+") || string.IsNullOrEmpty(phoneNumer))
                {
                    return phoneNumer;
                }

                if (phoneNumer[0] != '0')
                {
                    return phoneNumer;
                }

                CoutryPhoneNumber coutryPhoneNumber = Common.ListCoutryPhoneNumber.Where((CoutryPhoneNumber p) => p.CountryCode == countryCode).FirstOrDefault();
                if (coutryPhoneNumber == null)
                {
                    return phoneNumer;
                }

                if (coutryPhoneNumber.ListStartPhoneHeader.Any((string p) => phoneNumer.StartsWith(p)))
                {
                    return coutryPhoneNumber.PhoneHeaderCode + phoneNumer.Substring(1, phoneNumer.Length - 1);
                }

                if (phoneNumer[0] == '0')
                {
                    phoneNumer = "+84" + phoneNumer.Substring(1, phoneNumer.Length - 1);
                }

                return phoneNumer;
            }
            catch (Exception ex)
            {
                ex.Log();
            }

            return phoneNumer;
        }

        public static bool ValidPhoneNumber(string phoneNumer)
        {
            MatchCollection matchCollection = Regex.Matches(phoneNumer, "(84|0[3|5|7|8|9])+([0-9]{8})|(\\+84|0[3|5|7|8|9])+([0-9]{9})\\b");
            if (matchCollection.Count > 0 && matchCollection[0].Success && matchCollection[0].Value == phoneNumer)
            {
                return true;
            }

            return false;
        }

        public static bool ValidPassword(string pw)
        {
            MatchCollection matchCollection = Regex.Matches(pw, "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            if (matchCollection.Count > 0 && matchCollection[0].Success && matchCollection[0].Value == pw)
            {
                return true;
            }

            return false;
        }

        public static bool DeleteFile(string fullpath)
        {
            try
            {
                if (File.Exists(fullpath))
                {
                    File.Delete(fullpath);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string get(this List<string> lst, int index)
        {
            try
            {
                if (lst == null || lst.Count <= index)
                {
                    return "";
                }

                return lst[index];
            }
            catch (Exception ex)
            {
                ex.Log();
                return "";
            }
        }

        public static List<string> GetListString(this Dictionary<string, object> dictSource, string key)
        {
            if (dictSource != null && dictSource.ContainsKey(key))
            {
                List<object> list = dictSource[key] as List<object>;
                if (list != null)
                {
                    return list.Select((object i) => i.ToString()).ToList();
                }
            }

            return new List<string>();
        }

        public static object getValue(this Dictionary<string, object> dict, string key, object defaultValue = null)
        {
            try
            {
                object obj = dict.clone_local();
                string[] array = key.Split(new string[1] { "__" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string key2 in array)
                {
                    Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
                    if (dictionary != null && dictionary.ContainsKey(key2))
                    {
                        obj = dictionary[key2];
                        continue;
                    }

                    return defaultValue ?? "";
                }

                return (obj == null) ? (defaultValue ?? "") : ((string.IsNullOrEmpty(obj.ToString()) && defaultValue != null) ? defaultValue : obj);
            }
            catch (Exception ex)
            {
                string text = "getValue(key:" + key + ")";
                text = text + "\n[Exception]:" + ex.ToString() + "\nMessage:" + ex.Message + "\nStackTrace:" + ex.StackTrace;
                text.Log();
                return defaultValue ?? "";
            }
        }

        public static T getValueTemplate<T>(this Dictionary<string, object> dict, string key, T defaultValue)
        {
            try
            {
                if (string.IsNullOrEmpty(key) || (dict != null && dict.Count == 0))
                {
                    return defaultValue;
                }

                object obj = dict.clone_local();
                string[] array = key.Split(new string[1] { "__" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string key2 in array)
                {
                    Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
                    if (dictionary != null && dictionary.ContainsKey(key2))
                    {
                        obj = dictionary[key2];
                        continue;
                    }

                    return defaultValue;
                }

                return ConvertValueToType(obj, defaultValue);
            }
            catch (Exception ex)
            {
                ex.Log();
                return defaultValue;
            }
        }

        public static T ConvertValueToType<T>(object val, T defaultValue)
        {
            try
            {
                if (val != null)
                {
                    if (typeof(T) == typeof(string))
                    {
                        return (T)(object)val.ToString();
                    }

                    if (typeof(T) == typeof(long) && long.TryParse(val.ToString(), out var result))
                    {
                        return (T)(object)result;
                    }

                    if (typeof(T) == typeof(int) && int.TryParse(val.ToString(), out var result2))
                    {
                        return (T)(object)result2;
                    }

                    if (typeof(T) == typeof(double) && double.TryParse(val.ToString(), out var result3))
                    {
                        return (T)(object)result3;
                    }

                    if (typeof(T) == typeof(float) && float.TryParse(val.ToString(), out var result4))
                    {
                        return (T)(object)result4;
                    }

                    if (typeof(T) == typeof(bool))
                    {
                        return (T)(object)(!string.IsNullOrEmpty(val.ToString()) && "true1".Contains(val.ToString()?.ToLower()));
                    }

                    if (typeof(T) == typeof(List<int>))
                    {
                        List<object> list = val as List<object>;
                        if (list != null)
                        {
                            List<int> list2 = new List<int>();
                            foreach (object item in list)
                            {
                                if (int.TryParse(item.ToString(), out var result5))
                                {
                                    list2.Add(result5);
                                }
                            }

                            return (T)(object)list2;
                        }
                    }
                    else
                    {
                        if (typeof(T) == typeof(DateTime))
                        {
                            if (val is DateTime)
                            {
                                DateTime dateTime = (DateTime)val;
                                return (T)(object)dateTime;
                            }

                            DateTime value = Convert.ToDateTime(defaultValue);
                            return (T)(object)val.ToDateTime(value);
                        }

                        if (typeof(T) == typeof(Dictionary<string, object>))
                        {
                            Dictionary<string, object> dictionary = val as Dictionary<string, object>;
                            if (dictionary != null)
                            {
                                return (T)(object)dictionary;
                            }

                            string value2 = val.ToString();
                            if (string.IsNullOrEmpty(value2))
                            {
                                return default(T);
                            }

                            return (T)(object)JsonConvert.DeserializeObject<Dictionary<string, object>>(value2);
                        }
                    }
                }

                return (val == null) ? defaultValue : ((string.IsNullOrEmpty(val.ToString()) && defaultValue != null) ? defaultValue : ((T)val));
            }
            catch (Exception ex)
            {
                ex.Log();
                return default(T);
            }
        }

        public static Dictionary<string, object> AddRecordInArrayList(Dictionary<string, object> dictData, string fieldName, string newFileName)
        {
            try
            {
                if (!fieldName.Contains("."))
                {
                    throw new Exception("Field name không hơp lệ");
                }

                string[] array = fieldName.Split(new char[1] { '.' });
                List<object> valueTemplate = dictData.getValueTemplate<List<object>>(array[0], null);
                valueTemplate.Add(new Dictionary<string, object> { { fieldName, newFileName } });
                return new Dictionary<string, object> {
                {
                    array[0],
                    valueTemplate
                } };
            }
            catch (Exception ex)
            {
                string text = "AddRecordInArrayList(fieldName:" + fieldName + ", fileName: " + newFileName + " )";
                text = text + "\n[Exception]:" + ex.ToString() + "\nMessage:" + ex.Message + "\nStackTrace:" + ex.StackTrace;
                text.Log();
            }

            return null;
        }

        public static string ToDateTime_String(this object obj)
        {
            try
            {
                return obj.ToDateTime().ToString("yyyyMMdd HH:mm:ss");
            }
            catch (Exception ex)
            {
                ex.Log();
                return "";
            }
        }

        public static DateTime ToDateTime(this object obj, DateTime? defaultValue = null)
        {
            return obj.ToDateTimeCommon(defaultValue);
        }

        public static string GetFile_Version(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return "-1";
                }

                return FileVersionInfo.GetVersionInfo(path).ProductVersion;
            }
            catch (Exception ex)
            {
                ex.Log();
                return "-1";
            }
        }

        public static string GetLanguage(string key, string langId)
        {
            if (string.IsNullOrEmpty(langId) || key.EndsWith(":" + langId) || langId == "vi")
            {
                return key;
            }

            LanguageItem languageItem = Common.ListLanguage.Where((LanguageItem i) => i.vi == key).FirstOrDefault();
            if (languageItem != null)
            {
                switch (langId)
                {
                    case "en": 
                        return languageItem.en;
                    case "jp":
                        return languageItem.jp;
                    case "kr":
                        return languageItem.kr;
                    case "cn":
                        return languageItem.cn;
                    default:
                        return languageItem.vi;
                }
            }

            WriteNewLanguageItem(key);
            return key + ":" + langId;
        }

        private static void WriteNewLanguageItem(string key)
        {
        }

        public static string GetValueInURL(string tokenId, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(tokenId))
                {
                    return string.Empty;
                }

                if (tokenId.StartsWith("tokenId="))
                {
                    tokenId = tokenId.Substring("tokenId=".Length);
                }

                string[] array = tokenId.Split(new string[1] { "__" }, StringSplitOptions.None);
                foreach (string text in array)
                {
                    if (text.StartsWith(key + "="))
                    {
                        return text.Split(new char[1] { '=' })[1].ToString();
                    }
                }
            }
            catch (Exception)
            {
            }

            return string.Empty;
        }

        public static async Task<string> GetServer_Version()
        {
            string requestUri = "https://8xland.com/File/GetCurrentVersion";
            string result = "";
            using (HttpClient client = new HttpClient())
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                result = await (await client.PostAsync(requestUri, content)).Content.ReadAsStringAsync();
            }

            return result;
        }

        public static void SaveData_SQL(string collectionName, DataRow drChange)
        {
            throw new NotImplementedException();
        }

        public static T GetLocalSetting<T>(string k, T defauval)
        {
            if (LocalSetting == null)
            {
                InitLocatlSetting();
            }

            try
            {
                if (!LocalSetting.ContainsKey(k) || LocalSetting[k] == null)
                {
                    return defauval;
                }

                return (T)LocalSetting.getValue(k);
            }
            catch (Exception ex)
            {
                ex.Log();
                return defauval;
            }
        }

        public static int GetLocalSetting(string k, int defauval)
        {
            if (LocalSetting == null)
            {
                InitLocatlSetting();
            }

            try
            {
                if (!LocalSetting.ContainsKey(k) || LocalSetting[k] == null)
                {
                    return defauval;
                }

                return Convert.ToInt32(LocalSetting.getValue(k));
            }
            catch (Exception ex)
            {
                ex.Log();
                return defauval;
            }
        }

        private static void InitLocatlSetting()
        {
            try
            {
                if (!File.Exists("LocalSetting.json"))
                {
                    LocalSetting = new Dictionary<string, object>();
                    return;
                }

                using (StreamReader streamReader = new StreamReader("LocalSetting.json"))
                {
                    LocalSetting = JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd());
                }

                if (LocalSetting == null)
                {
                    LocalSetting = new Dictionary<string, object>();
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }

        public static void SetLocalSetting(string k, object v)
        {
            if (LocalSetting == null)
            {
                InitLocatlSetting();
            }

            try
            {
                LocalSetting.Merge(k, v);
                string contents = JsonConvert.SerializeObject(LocalSetting);
                File.WriteAllText("LocalSetting.json", contents);
            }
            catch
            {
            }
        }

        public static bool ValidFileName(string openFile, ref string mgs)
        {
            FileInfo fileInfo = new FileInfo(openFile);
            string text = fileInfo.Name.Replace(fileInfo.Extension, "");
            bool result = true;
            string text2 = text;
            for (int i = 0; i < text2.Length; i++)
            {
                char c = text2[i];
                if (!Enumerable.Contains(lstKyTuDatTenFile, c))
                {
                    mgs += $"<backcolor=red><color=white>{c}</color></backcolor>";
                    result = false;
                }
                else
                {
                    mgs += c;
                }
            }

            return result;
        }

        public static DataTable GetListUser()
        {
            if (tmpUserList != null)
            {
                return tmpUserList;
            }

            return ReloadListUser();
        }

        public static DataTable ReloadListUser()
        {
            tmpUserList = SqlHelper.ExecuteDataTable(" SELECT * FROM tblAccount");
            return tmpUserList;
        }

        public static void setValue(this Dictionary<string, object> dict, string key, object Value)
        {
            string[] array = key.Split(new string[1] { "__" }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Count() == 1)
            {
                if (dict.ContainsKey(array[0]))
                {
                    dict[array[0]] = Value;
                }
                else
                {
                    dict.Add(array[0], Value);
                }

                return;
            }

            if (!dict.ContainsKey(array[0]))
            {
                dict.Add(array[0], new Dictionary<string, object>());
            }

            (dict[array[0]] as Dictionary<string, object>).setValue(key.Substring(key.IndexOf("__") + "__".Length), Value);
        }

        public static string CreateZipFile(string fileName, List<string> files)
        {
            string startupPath = Application.StartupPath;
            string text = startupPath + "/" + fileName;
            fileName = fileName.Replace(".zip", "");
            string text2 = startupPath + "\\" + fileName + ".zip";
            if (Directory.Exists(text))
            {
                Directory.Delete(text, recursive: true);
            }

            Directory.CreateDirectory(text);
            foreach (string file in files)
            {
                File.Copy(file, text + "/" + new FileInfo(file).Name);
            }

            if (File.Exists(text2))
            {
                File.Delete(text2);
            }

            ZipFile.CreateFromDirectory(text, text2);
            return text2;
        }

        public static byte[] CompressStringToFile(List<string> filestream)
        {
            MemoryStream memoryStream = new MemoryStream();
            GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: false);
            foreach (string item in filestream)
            {
                byte[] array = Common.PathFileToByteArray(item);
                gZipStream.Write(array, 0, array.Length);
            }

            return memoryStream.ToArray();
        }

        public static string ToJsonString(this Dictionary<string, object> dict)
        {
            return JsonConvert.SerializeObject(dict);
        }

        public static void Merge(this Dictionary<string, object> dict, string key, object val)
        {
            if (dict != null)
            {
                if (dict.ContainsKey(key))
                {
                    dict[key] = val;
                }
                else
                {
                    dict.Add(key, val);
                }
            }
        }

        public static void Merge(this Dictionary<string, object> dictLeft, Dictionary<string, object> dictRight, bool isOverride = true)
        {
            if (dictLeft == null)
            {
                dictLeft = new Dictionary<string, object>();
            }

            if (dictRight == null)
            {
                return;
            }

            foreach (KeyValuePair<string, object> item in dictRight)
            {
                if (dictLeft.ContainsKey(item.Key))
                {
                    if (isOverride)
                    {
                        dictLeft.Merge(item.Key, item.Value);
                    }
                }
                else
                {
                    dictLeft.Add(item.Key, item.Value);
                }
            }
        }

        public static void Merge(this Dictionary<string, string> dict, string key, string val)
        {
            if (dict != null)
            {
                if (dict.ContainsKey(key))
                {
                    dict[key] = val;
                }
                else
                {
                    dict.Add(key, val);
                }
            }
        }

        public static T ToObject<T>(this Dictionary<string, object> dict)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(dict));
        }

        public static List<T> GetListControl<T>(this Control parent)
        {
            //IL_001a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0020: Expected O, but got Unknown
            List<T> list = new List<T>();
            foreach (Control item2 in (ArrangedElementCollection)parent.Controls)
            {
                Control val = item2;
                if (val is T)
                {
                    T item = (T)(object)((val is T) ? val : null);
                    list.Add(item);
                }

                list.AddRange(val.GetListControl<T>());
            }

            return list;
        }

        public static bool IsNumberic_Decimal(this object s)
        {
            if (string.IsNullOrEmpty(s?.ToString()))
            {
                return false;
            }

            if (!new Regex("^-?[0-9]*\\.?[0-9]+$").IsMatch(s.ToString()))
            {
                return false;
            }

            return true;
        }

        public static string DocSoThanhChu(object v, int languageId = 1, int round = 2)
        {
            try
            {
                if (!v.IsNumberic_Decimal())
                {
                    return v?.ToString();
                }

                return DocTienBangChu(Convert.ToDouble(v), languageId, round);
            }
            catch (Exception)
            {
                return v?.ToString();
            }
        }

        private static string DocTienBangChu(double SoTien, int languageId = 1, int round = 2, bool lienHe = false)
        {
            if (SoTien > 8999999999999999.0)
            {
                return SoTien.ToString();
            }

            List<string> list = new List<string>
            {
                GetMessage("DON_VI_TIEN_TRIEU_TY", languageId),
                GetMessage("DON_VI_TIEN_NGIN_TY", languageId),
                GetMessage("DON_VI_TIEN_TY", languageId),
                GetMessage("DON_VI_TIEN_TRIEU", languageId),
                GetMessage("DON_VI_TIEN_NGIN", languageId),
                GetMessage("DON_VI_TIEN_DONG", languageId)
            };
            List<double> list2 = new List<double> { 1E+15, 1000000000000.0, 1000000000.0, 1000000.0, 1000.0, 1.0 };
            for (int i = 0; i < list2.Count; i++)
            {
                if (SoTien / list2[i] > double.MaxValue || SoTien / list2[i] < double.MinValue)
                {
                    return SoTien.ToString();
                }

                if (!(SoTien >= list2[i]))
                {
                    continue;
                }

                if (SoTien % list2[i] > 0.0)
                {
                    if (round > 0)
                    {
                        return Math.Round(Convert.ToDouble(SoTien / list2[i]), round) + " " + list[i];
                    }

                    double num = Convert.ToDouble(SoTien / list2[i]);
                    if (num == 0.0 && lienHe)
                    {
                        return GetMessage("WEB_GIA_LIEN_HE", languageId);
                    }

                    return num + " " + list[i];
                }

                return Convert.ToDouble(SoTien / list2[i]) + " " + list[i];
            }

            if (SoTien == 0.0 && lienHe)
            {
                return GetMessage("WEB_GIA_LIEN_HE", languageId);
            }

            return SoTien.ToString();
        }

        public static Dictionary<string, object> clone(this Dictionary<string, object> dict)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> item in dict)
            {
                Dictionary<string, object> dictionary2 = item.Value as Dictionary<string, object>;
                if (dictionary2 != null)
                {
                    dictionary.Add(item.Key, dictionary2.clone_local());
                }
                else
                {
                    dictionary.Add(item.Key, item.Value);
                }
            }

            return dictionary;
        }

        public static Dictionary<string, object> clone_local(this Dictionary<string, object> dict)
        {
            if (dict == null)
            {
                return null;
            }

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> item in dict)
            {
                Dictionary<string, object> dictionary2 = item.Value as Dictionary<string, object>;
                if (dictionary2 != null)
                {
                    dictionary.Add(item.Key, dictionary2.clone_local());
                }
                else
                {
                    dictionary.Add(item.Key, item.Value);
                }
            }

            return dictionary;
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        public static string RemoveToneMark(this string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                {
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
                }
            }

            return str;
        }

        private static string removeChar(char c)
        {
            foreach (KeyValuePair<string, string> foreign_character in foreign_characters)
            {
                if (Enumerable.Contains(foreign_character.Key, c))
                {
                    return foreign_character.Value;
                }
            }

            if (!Enumerable.Contains(lstCharDocNum, c))
            {
                return "";
            }

            return c.ToString();
        }

        public static void InsearchColumn(this DataTable dt)
        {
            if (!dt.Columns.Contains("SearchColumn_SearchColumn"))
            {
                dt.Columns.Add("SearchColumn_SearchColumn", typeof(string));
            }

            foreach (DataRow row in dt.Rows)
            {
                string text = "";
                foreach (DataColumn column in dt.Columns)
                {
                    if (row[column.ColumnName] != DBNull.Value)
                    {
                        string text2 = row[column.ColumnName].ToString();
                        if (text2.Length < 150)
                        {
                            text = text + "|" + text2.RemoveToneMark().ToLower();
                        }
                    }
                }

                row["SearchColumn_SearchColumn"] = text;
            }

            dt.AcceptChanges();
        }

        public static byte[] GetBytes(this string txt, string charsetName = "")
        {
            return Encoding.UTF8.GetBytes(txt);
        }

        public static Stream ToStream(this Image image)
        {
            if (image == null)
            {
                return null;
            }

            ImageFormat png = ImageFormat.Png;
            MemoryStream memoryStream = new MemoryStream();
            image.Save((Stream)memoryStream, png);
            memoryStream.Position = 0L;
            return memoryStream;
        }

        public static byte[] ToByteArray(this Image image)
        {
            if (image == null)
            {
                return null;
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save((Stream)memoryStream, image.RawFormat);
                return memoryStream.ToArray();
            }
        }

        public static string GetFullPathLocal(string path, string folder_image_path = null)
        {
            if (!string.IsNullOrEmpty(folder_image_path))
            {
                folder_image_path = folder_image_path + "/" + Common.DICT_MAP_ROOT_FOLDER[Common.EConnection];
            }

            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            if (path.Contains(":"))
            {
                return path;
            }

            string text = (path.ToString().StartsWith("/") ? "" : "/");
            if (string.IsNullOrEmpty(folder_image_path))
            {
                folder_image_path = Application.StartupPath;
            }

            return folder_image_path + text + path;
        }

        public static string GetDisplayIamgeDocument(string ext, string defaultPath)
        {
            if (string.IsNullOrEmpty(ext))
            {
                return defaultPath;
            }

            ext = ext.ToLower();
            switch (ext)
            {
                case ".doc":
                case ".docx":
                    return "/images/default_image/word.png";
                case ".xls":
                case ".xlsx":
                    return "/images/default_image/excel.png";
                case ".pdf":
                    return "/images/default_image/pdf.png";
                case ".mp4":
                    return "/images/default_image/mp4.png";
                case ".txt":
                    return "/images/default_image/txt.png";
                case ".html":
                    return "/images/default_image/html.png";
                case ".ppt":
                case ".pptx":
                    return "/images/default_image/power_point.png";
                default:
                    return defaultPath;
            }
        }

        public static string GetFullURL(string url, string default_url = "")
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    if (!string.IsNullOrEmpty(default_url))
                    {
                        return GetFullURL(default_url);
                    }

                    return url;
                }

                if (url.StartsWith("http"))
                {
                    return url;
                }

                string text = ((url.StartsWith("/") || url.StartsWith("?")) ? "" : "/");
                return (Common.HOST_URL + text + url).Replace("\\", "/");
            }
            catch (Exception)
            {
                return url;
            }
        }

        public static string GetFullURL_API(string uri)
        {
            try
            {
                if (uri.StartsWith("http"))
                {
                    return uri;
                }

                string text = Common.HOST_URL_API;
                if (!text.EndsWith("/"))
                {
                    text += "/";
                }

                if (uri.StartsWith("/"))
                {
                    uri = uri.Substring(1);
                }

                if (!uri.StartsWith("api"))
                {
                    uri = "api/" + uri;
                }

                return text + uri;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetFullURL(int ImageId)
        {
            string url;
            if (DictImageProject.ContainsKey(ImageId))
            {
                url = DictImageProject[ImageId];
            }
            else
            {
                if (Common.USE_DEFAULT_IMAGE)
                {
                    return Common.DEFAULT_IMAGE_URL;
                }

                url = "/images/favicon.ico";
            }

            return GetFullURL(url);
        }

        public static string ResizeImage(string path, string outFolder)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string result = ResizeImage(0.5, path, outFolder);
            stopwatch.Stop();
            return result;
        }

        public static string ResizeImage(double limiteMb, string fileName, string outFolder, int Width = 0, int Height = 0)
        {
            //IL_008f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0096: Expected O, but got Unknown
            double num = sizeByte(fileName);
            if (num < limiteMb)
            {
                return fileName;
            }

            double lowerPercent = GetLowerPercent(num);
            Image val = Image.FromFile(fileName);
            if (Width == 0 || Height == 0)
            {
                Width = val.Width;
                Height = val.Height;
            }

            int num2 = (int)((double)Width * lowerPercent);
            int num3 = (int)((double)Height * lowerPercent);
            Width -= num2;
            Height -= num3;
            string format = outFolder + "\\ResizeImage_small_{0}x{1}_" + DateTime.Now.Ticks + ".png";
            format = string.Format(format, Width, Height);
            Bitmap val2 = new Bitmap(val, Width, Height);
            val.Dispose();
            ((Image)val2).Save(format);
            ((Image)val2).Dispose();
            return ResizeImage(limiteMb, format, outFolder, Width, Height);
        }

        public static void DeleteTempResizeImage(string folder)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folder);
            foreach (DirectoryInfo item in directoryInfo.EnumerateDirectories())
            {
                DeleteTempResizeImage(item.FullName);
            }

            FileInfo[] files = directoryInfo.GetFiles();
            foreach (FileInfo fileInfo in files)
            {
                try
                {
                    if (fileInfo.Name.Contains("ResizeImage_small"))
                    {
                        fileInfo.Delete();
                    }
                }
                catch (Exception ex)
                {
                    ex.Log();
                }
            }
        }

        public static double GetLowerPercent(double sizeMb)
        {
            if (sizeMb > 10.0)
            {
                return 0.8;
            }

            if (sizeMb > 5.0)
            {
                return 0.6;
            }

            if (sizeMb > 2.0)
            {
                return 0.4;
            }

            return 0.1;
        }

        public static double sizeByte(string path)
        {
            try
            {
                return (double)new FileInfo(path).Length / 1048576.0;
            }
            catch (Exception ex)
            {
                ex.Log();
                return 0.0;
            }
        }

        public static void DownloadPhotoAsync(string url, string savePath)
        {
            try
            {
                Common.CreateDirectory(savePath, isFilePath: true);
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFileAsync(new Uri(url), savePath);
                }
            }
            catch (Exception)
            {
            }
        }

        private static FtpWebRequest CreateFtpWebRequest(string ftpDirectoryPath, string userName, string password, bool keepAlive = false)
        {
            FtpWebRequest obj = (FtpWebRequest)WebRequest.Create(new Uri(ftpDirectoryPath));
            obj.Proxy = null;
            obj.UsePassive = true;
            obj.UseBinary = true;
            obj.KeepAlive = keepAlive;
            obj.Credentials = new NetworkCredential(userName, password);
            return obj;
        }

        public static void DownloadPhotoFtp(string uri, string savePath)
        {
            try
            {
                DownloadPhoto(GetFullURL("/Data/DownLoad?filename=") + uri, savePath);
            }
            catch (Exception)
            {
            }
        }

        public static void DownloadPhoto(string url, string savePath)
        {
            try
            {
                Common.CreateDirectory(savePath, isFilePath: true);
                DeleteFile(savePath);
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(new Uri(url), savePath);
                }
            }
            catch
            {
            }
        }

        public static string DownloadPhoto(string url)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(url.Replace(Common.HOST_URL, ""));
                string text = "tmp_Photo/" + fileInfo.Name;
                Common.CreateDirectory(text, isFilePath: true);
                DeleteFile(text);
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(new Uri(url), text);
                }

                return text;
            }
            catch (Exception)
            {
            }

            return null;
        }

        public static void SavePhotoLocal(Stream stream, string fullPath)
        {
            Image val = Image.FromStream(stream);
            try
            {
                val.Save(fullPath);
            }
            finally
            {
                ((IDisposable)val)?.Dispose();
            }
        }

        public static string GetPropertiesImage(Image im, int id)
        {
            ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
            PropertyItem val = ((IEnumerable<PropertyItem>)im.PropertyItems).FirstOrDefault((Func<PropertyItem, bool>)((PropertyItem x) => x.Id == id));
            if (val == null)
            {
                return string.Empty;
            }

            return aSCIIEncoding.GetString(val.Value)?.Replace("\0", string.Empty);
        }

        public static string MoveFile(string fromFile, string toFile)
        {
            string text = "";
            string text2 = Application.StartupPath + "/tmp";
            if (!Directory.Exists(text2))
            {
                Directory.CreateDirectory(text2);
            }

            text = text2 + "/" + toFile;
            if (File.Exists(text))
            {
                File.Delete(text);
            }

            File.Copy(fromFile, text);
            return text;
        }

        public static async Task<string> ErrorMessage(string mgs)
        {
            await Task.Delay(100);
            return "[\"error message\":\"invalid parameter : " + mgs + "\"]";
        }

        public static DataTable ToDataTable(this List<Dictionary<string, object>> lstDict)
        {
            DataTable dataTable = new DataTable();
            if (lstDict == null || !lstDict.Any())
            {
                return dataTable;
            }

            foreach (KeyValuePair<string, object> item in lstDict[0])
            {
                if (!dataTable.Columns.Contains(item.Key))
                {
                    dataTable.Columns.Add(item.Key, typeof(object));
                }
            }

            foreach (Dictionary<string, object> item2 in lstDict)
            {
                DataRow dataRow = dataTable.NewRow();
                foreach (KeyValuePair<string, object> item3 in item2)
                {
                    if (dataTable.Columns.Contains(item3.Key))
                    {
                        dataRow[item3.Key] = item3.Value;
                    }
                }

                dataTable.Rows.Add(dataRow);
            }

            dataTable.AcceptChanges();
            return dataTable;
        }

        public static string TimeAgoString(DateTime date)
        {
            double totalSeconds = (DateTime.Now - date).TotalSeconds;
            if (totalSeconds < 10.0)
            {
                return "vài giây trước";
            }

            if (totalSeconds < 60.0)
            {
                return totalSeconds + " giây trước";
            }

            double num = Math.Round(totalSeconds / 60.0, 1);
            if (num < 60.0)
            {
                return num + " phút trước";
            }

            double num2 = Math.Round(num / 60.0, 1);
            if (num2 < 24.0)
            {
                return num2 + " giờ trước";
            }

            double num3 = Math.Round(num2 / 60.0, 1);
            if (num3 < 30.0)
            {
                return num3 + " ngày trước";
            }

            double num4 = Math.Round(num3 / 30.0, 1);
            if (num4 < 12.0)
            {
                return num4 + " tháng trước";
            }

            return Math.Round(num4 / 12.0, 1) + " năm trước";
        }

        public static void SendEmail(string toEmail, string subject, string body)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(toEmail);
            mailMessage.From = new MailAddress("from gmail address", "Email head", Encoding.UTF8);
            mailMessage.Subject = subject;
            mailMessage.SubjectEncoding = Encoding.UTF8;
            mailMessage.Body = body;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.IsBodyHtml = true;
            mailMessage.Priority = MailPriority.High;
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Credentials = new NetworkCredential("from gmail address", "your gmail account password");
            smtpClient.Port = 587;
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.EnableSsl = true;
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception innerException)
            {
                string text = string.Empty;
                while (innerException != null)
                {
                    text += innerException.ToString();
                    innerException = innerException.InnerException;
                }
            }
        }

        public static bool ValidPhoneNumber(string phoneNumber, string countryCode = "vn")
        {
            if (countryCode == "vn")
            {
                return Common.ValidPhoneNumber_VI(phoneNumber);
            }

            throw new Exception("Invalid Country code = " + countryCode);
        }

        public static List<Dictionary<string, object>> GetListDictObject(Dictionary<string, object> dict, string name)
        {
            if (dict == null)
            {
                return new List<Dictionary<string, object>>();
            }

            List<Dictionary<string, object>> valueTemplate = dict.getValueTemplate<List<Dictionary<string, object>>>(name, null);
            if (valueTemplate != null && valueTemplate.Any())
            {
                return valueTemplate ?? new List<Dictionary<string, object>>();
            }

            List<object> valueTemplate2 = dict.getValueTemplate<List<object>>(name, null);
            if (valueTemplate2 == null || !valueTemplate2.Any())
            {
                return new List<Dictionary<string, object>>();
            }

            valueTemplate = new List<Dictionary<string, object>>();
            foreach (object item in valueTemplate2)
            {
                Dictionary<string, object> dictionary = item as Dictionary<string, object>;
                if (dictionary != null)
                {
                    valueTemplate.Add(dictionary);
                }
            }

            return valueTemplate;
        }

        public static string ToMoney(this double d, bool returnZero = false, bool showComma = false)
        {
            if (d == 0.0)
            {
                if (returnZero)
                {
                    return "0";
                }

                return "-";
            }

            string text = $"{d:###,###,###,###,###,###,###.##}";
            if (text.StartsWith("."))
            {
                text = "0" + text;
            }

            if (text.StartsWith("-."))
            {
                text = text.Replace("-.", "-0.");
            }

            if (!text.Contains("."))
            {
                text += ".0";
            }

            return ((showComma && d > 0.0) ? "+" : "") + text;
        }

        public static string ToPercentSmall(this double d, bool returnZero = false)
        {
            if (d == 0.0)
            {
                if (returnZero)
                {
                    return "0%";
                }

                return "-";
            }

            if (d == 100.0)
            {
                return "100%";
            }

            d = Math.Round(d, 0);
            string text = d.ToString("0.0");
            if (text.StartsWith("."))
            {
                text = "0" + text;
            }

            if (text.StartsWith("-."))
            {
                text = text.Replace("-.", "-0.");
            }

            if (text.EndsWith(".0"))
            {
                text = text.Replace(".0", "");
            }

            return text + "%";
        }

        public static string ToMoneySmall(this double d, bool returnZero = false, bool showComma = false)
        {
            if (d == 0.0)
            {
                if (returnZero)
                {
                    return "0";
                }

                return "-";
            }

            string text = $"{d:###,###,###,###,###,###,###.##}";
            if (text.StartsWith("."))
            {
                text = "0" + text;
            }

            if (text.StartsWith("-."))
            {
                text = text.Replace("-.", "-0.");
            }

            return ((showComma && d > 0.0) ? "+" : "") + text;
        }

        public static string ToMoney(this double d, string formart)
        {
            return d.ToString(formart);
        }

        public static string ToPercent(this double d, string formart)
        {
            if (d == 100.0)
            {
                return "100%";
            }

            return d.ToString(formart) + "%";
        }

        public static string ToPercent(this double d, bool returnZero = false)
        {
            if (d == 0.0)
            {
                if (returnZero)
                {
                    return "0.0%";
                }

                return "-";
            }

            if (d == 100.0)
            {
                return "100%";
            }

            d = Math.Round(d, 1);
            string text = d.ToString("0.0");
            if (text.StartsWith("."))
            {
                text = "0" + text;
            }

            if (text.StartsWith("-."))
            {
                text = text.Replace("-.", "-0.");
            }

            if (!text.Contains("."))
            {
                text += ".0";
            }

            return text + "%";
        }

        public static string ToDoubleHtml(this double d)
        {
            return d.ToString().Replace(",", ".");
        }

        public static string ToMoney(this int i)
        {
            if (i == 0)
            {
                return "0";
            }

            return $"{double.Parse(i.ToString()):###,###,###,###,###,###,###.##}";
        }

        public static string GetString(this string[] arr, int index, string defaulVal)
        {
            try
            {
                if (arr == null || arr.Length <= index || index < 0)
                {
                    return defaulVal;
                }

                return arr[index];
            }
            catch (Exception)
            {
                return defaulVal;
            }
        }

        public static async Task<string> PushNotifyForMobile(string registrationToken, string title, string body)
        {
            FirebaseAdmin.Messaging.Message val = new FirebaseAdmin.Messaging.Message();
            Notification val2 = new Notification();
            val2.Title = title;
            val2.Body = body;
            val.Notification = val2;
            val.Token = registrationToken;
            FirebaseAdmin.Messaging.Message val3 = val;
            return await Connection.messaging.SendAsync(val3);
        }

        public static int GetID(this DataRow dr)
        {
            return Convert.ToInt32(dr["ID"]);
        }

        public static async Task SendNotify(int uid = -1)
        {
            DataTable dataTable = SqlHelper.ExecuteDataTable("sp_GetNotifySending");
            foreach (DataRow row in dataTable.Rows)
            {
                int num = int.Parse(row["uid"].ToString());
                if (uid <= 0 || num == uid)
                {
                    string ID = row["ID"].ToString();
                    string registrationToken = row["tokenId"].ToString();
                    string title = row["title"].ToString();
                    string body = row["body"].ToString();
                    string text;
                    int num2;
                    try
                    {
                        text = await PushNotifyForMobile(registrationToken, title, body);
                        num2 = 2;
                    }
                    catch (Exception ex)
                    {
                        text = ex.Message;
                        ex.Log();
                        num2 = 3;
                    }

                    SqlHelper.ExecuteNonQuery("UPDATE tblAccount_dsThongBao SET idTrangThaiThongBao = " + num2 + ", log = isnull(log + CHAR(13) + CHAR(10), '') + @log WHERE ID = " + ID, "@log", text);
                    SqlHelper.Log(ModeLog.Service, "[FirebaseAdmin] " + text, uid);
                    if (uid > 0)
                    {
                        throw new Exception(text);
                    }
                }
            }
        }

        public static bool ValidCreateTabelColumn(string tableName)
        {
            if (!tableName.StartsWith("tbl"))
            {
                return tableName.StartsWith("cbb");
            }

            return true;
        }

        public static DataSet SendEmailVerifyCode(string jParam)
        {
            List<object> list = new List<object>();
            if (!string.IsNullOrEmpty(jParam))
            {
                foreach (KeyValuePair<string, object> item in string.IsNullOrEmpty(jParam) ? new Dictionary<string, object>() : JsonConvert.DeserializeObject<Dictionary<string, object>>(jParam))
                {
                    if (item.Key.StartsWith("@"))
                    {
                        list.Add(item.Key);
                    }
                    else
                    {
                        list.Add("@" + item.Key);
                    }

                    list.Add(item.Value);
                }
            }

            return SendEmailVerifyCode(list);
        }

        public static DataSet SendEmailVerifyCode(List<object> lst)
        {
            DataSet dataSet;
            try
            {
                dataSet = SqlHelper.ExecuteDataset("sp_CreateVerifyEmailCode", lst.ToArray());
                if (dataSet.Tables[0].Rows.Count == 0)
                {
                    return null;
                }

                if (dataSet.Tables[0].Rows[0]["code"].ToString() == "200")
                {
                    EmailSender emailSender = new EmailSender();
                    string toEmail = dataSet.Tables[0].Rows[0]["email"].ToString();
                    string value = dataSet.Tables[0].Rows[0]["email_code"].ToString();
                    Dictionary<string, object> datamap = new Dictionary<string, object> { { "EMAIL_CODE", value } };
                    emailSender.Send(datamap, toEmail, "TEMPLATE_VERIFY_EMAIL");
                }

                dataSet.Tables[0].TableName = "mgs";
            }
            catch (Exception ex)
            {
                new Exception("SendEmailVerifyCode: " + ex.Message).Log();
                return null;
            }

            if (dataSet.Tables.Count > 1 && dataSet.Tables[dataSet.Tables.Count - 1].Columns.Count == 1)
            {
                dataSet.Tables.RemoveAt(dataSet.Tables.Count - 1);
            }

            return dataSet;
        }

        public static string GetValue(this DataRow dr, string columnName, string defaultValue = "")
        {
            if (dr != null && dr.Table.Columns.Contains(columnName))
            {
                if (dr[columnName] == DBNull.Value)
                {
                    return defaultValue;
                }

                object obj = dr[columnName];
                if (obj is DateTime)
                {
                    return ((DateTime)obj).ToString("yyyyMMdd HH:mm:ss");
                }

                return dr[columnName].ToString();
            }

            return defaultValue;
        }

        public static T Get<T>(this DataRow dr, string column, T defaultValue = default(T))
        {
            try
            {
                if (dr == null || !dr.Table.Columns.Contains(column))
                {
                    return defaultValue;
                }

                object obj = dr[column];
                if (obj == DBNull.Value || obj == null)
                {
                    return defaultValue;
                }

                return ConvertValueToType(dr[column], defaultValue);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static object GetValueObject(this DataRow dr, string columnName, object defaultValue = null)
        {
            if (dr.Table.Columns.Contains(columnName))
            {
                if (dr[columnName] == DBNull.Value)
                {
                    return defaultValue;
                }

                return dr[columnName];
            }

            return defaultValue;
        }

        public static object GetCellValueHtml(this DataRow dr, string columnName, ILayoutItem item, int UID, object defaultValue = null)
        {
            try
            {
                return FormatValueHtml(dr.GetValueObject(columnName, defaultValue), item, UID) ?? "";
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static object FormatValueHtml(object val, ILayoutItem item, int UID)
        {
            if (val == null || val == DBNull.Value)
            {
                return val;
            }

            CtrlTyp type = item.Type;
            if ((type == CtrlTyp.Date || type == CtrlTyp.DateTime) && val != null)
            {
                return Convert.ToDateTime(val).ToString("dd/MM/yyyy");
            }

            if (type == CtrlTyp.Time && val != null && val is TimeSpan)
            {
                if (val != null)
                {
                    return ((TimeSpan)val).ToString("hh\\:mm");
                }
            }
            else
            {
                switch (type)
                {
                    case CtrlTyp.Numberic:
                    case CtrlTyp.Int:
                        if (val == null || val.ToString() == "0")
                        {
                            return "-";
                        }

                        if (item.FieldName.ToLower().Contains("nam") && item.Title.ToLower().Contains("năm"))
                        {
                            return val;
                        }

                        return Convert.ToDouble(val).ToMoney();
                    case CtrlTyp.CheckBox:
                        if (val != null && Convert.ToBoolean(val))
                        {
                            return "checked";
                        }

                        return "";
                    case CtrlTyp.DropDown:
                        {
                            DataTable dropdownData = item.GetDropdownData(item, -1, -1, -1, getAll: true, UID);
                            if (dropdownData != null && dropdownData.Rows.Count > 0)
                            {
                                DataRow dataRow = dropdownData.AsEnumerable().FirstOrDefault((DataRow dr) => dr["ID"].ToString() == val.ToString());
                                if (dataRow != null)
                                {
                                    return dataRow["title"];
                                }
                            }

                            break;
                        }
                }
            }

            return val;
        }

        public static string GetValueDiaChi(this DataRow dr)
        {
            string text = dr.GetValue("diaChi").ToString().Trim();
            if (string.IsNullOrEmpty(text))
            {
                text = string.Join(", ", new List<string>
                {
                    dr.GetValue("soNha").ToString(),
                    dr.GetValue("tenDuong").ToString(),
                    dr.GetValue("title_idXaPhuong").ToString(),
                    dr.GetValue("title_idQuanHuyen").ToString(),
                    dr.GetValue("title_idTinhThanhPho").ToString(),
                    dr.GetValue("title_idQuocGia").ToString()
                }.Where((string i) => !string.IsNullOrEmpty(i)));
            }

            return text;
        }

        public static string GetImage(this DataTable dt, int rowIndex, string columnName = "hinhAnh_image_path", string defaultValue = "")
        {
            try
            {
                if (string.IsNullOrEmpty(defaultValue))
                {
                    defaultValue = Common.DEFAULT_IMAGE_URL;
                }

                if (string.IsNullOrEmpty(defaultValue))
                {
                    defaultValue = Common.DEFAULT_IMAGE_URL;
                }

                if (dt != null && dt.Rows.Count > rowIndex && dt.Columns.Contains(columnName) && dt.Rows[rowIndex][columnName] != DBNull.Value)
                {
                    return GetFullURL(dt.Rows[rowIndex][columnName].ToString());
                }
            }
            catch
            {
            }

            return GetFullURL(defaultValue);
        }

        public static DataTable GetSettingTable(string tableName)
        {
            try
            {
                lock (new object())
                {
                    tableName = tableName.ToLower();
                    if (Common.DICT_TABLE_SETTING.ContainsKey(tableName))
                    {
                        return Common.DICT_TABLE_SETTING[tableName];
                    }

                    Common.DICT_TABLE_SETTING.Remove(tableName);
                    Common.DICT_TABLE_SETTING[tableName] = SqlHelper.ExecuteDataTable("SELECT * FROM [" + tableName + "] order by isnulL( priority, 999 )");
                    return Common.DICT_TABLE_SETTING[tableName];
                }
            }
            catch
            {
                return null;
            }
        }

        public static DataTable GetSettingProcedure(string procName, params object[] parameterValues)
        {
            try
            {
                procName = procName.ToLower();
                if (Common.DICT_TABLE_SETTING.ContainsKey(procName))
                {
                    return Common.DICT_TABLE_SETTING[procName];
                }

                Common.DICT_TABLE_SETTING.Remove(procName);
                Common.DICT_TABLE_SETTING[procName] = SqlHelper.ExecuteDataTable(procName, parameterValues.ToArray());
                return Common.DICT_TABLE_SETTING[procName];
            }
            catch
            {
                return null;
            }
        }

        public static DataSet GetSettingProcedureDataSet(string procName, params object[] parameterValues)
        {
            try
            {
                procName = procName.ToLower();
                if (Common.DICT_TABLE_SETTING_DATA_SET.ContainsKey(procName))
                {
                    return Common.DICT_TABLE_SETTING_DATA_SET[procName];
                }

                Common.DICT_TABLE_SETTING_DATA_SET.Remove(procName);
                Common.DICT_TABLE_SETTING_DATA_SET[procName] = SqlHelper.ExecuteDataset(procName, parameterValues.ToArray());
                return Common.DICT_TABLE_SETTING_DATA_SET[procName];
            }
            catch
            {
                return null;
            }
        }

        public static DataTable GetSettingTable_FormSetting()
        {
            try
            {
                string text = "GetSettingTable_FormSetting";
                text = text.ToLower();
                if (Common.DICT_TABLE_SETTING.ContainsKey(text))
                {
                    return Common.DICT_TABLE_SETTING[text];
                }

                Common.DICT_TABLE_SETTING.Add(text, SqlHelper.ExecuteDataTable("sp_GetListParentTable"));
                return Common.DICT_TABLE_SETTING[text];
            }
            catch
            {
                return null;
            }
        }

        public static void GetColorBackGroundColor(ref string tienDo, ref string color, ref string background_color, DataRow dr, string phan_loai)
        {
            DataTable dataTable = null;
            switch (phan_loai)
            {
                case "du-an":
                case "bds-nuoc-ngoai":
                    dataTable = GetSettingTable("cbb_TienDoDuAnBDS");
                    break;
                case "nha-pho":
                    dataTable = GetSettingTable("cbb_TrangThaiRiengLeBDS");
                    break;
            }

            if (dataTable == null)
            {
                return;
            }

            DataRow dataRow = null;
            if (phan_loai == "du-an" && dataTable.AsEnumerable().Any((DataRow cl) => cl.GetValue("ID").ToString() == dr.GetValue("tienDoDuAn").ToString()))
            {
                dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow cl) => cl.GetValue("ID").ToString() == dr.GetValue("tienDoDuAn").ToString());
            }
            else if (phan_loai == "nha-pho")
            {
                int idDaXacThuc = ((!"true1".Contains(dr.GetValue("daXacThuc", "0").ToString().ToLower())) ? 1 : 2);
                if (dataTable.AsEnumerable().Any((DataRow cl) => cl.GetValue("ID").ToString() == idDaXacThuc.ToString()))
                {
                    dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow cl) => cl.GetValue("ID").ToString() == idDaXacThuc.ToString());
                }
            }
            else if (phan_loai == "bds-nuoc-ngoai")
            {
                dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow cl) => cl.GetValue("ID").ToString() == dr.GetValue("idTienDo").ToString());
            }

            if (dataRow != null)
            {
                color = dataRow["textColor"].ToString();
                background_color = dataRow["backgroundColor"].ToString();
                tienDo = dataRow["title"].ToString();
            }
        }

        public static string GetIconTaiLieu(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }

            fileName = fileName.ToLower();
            string text = "";
            if (fileName.EndsWith(".xls") || fileName.EndsWith(".xlsx"))
            {
                text = "excel.png";
            }
            else if (fileName.EndsWith(".doc") || fileName.EndsWith(".docx"))
            {
                text = "word.png";
            }
            else if (fileName.EndsWith(".pdf"))
            {
                text = "pdf.png";
            }
            else if (fileName.EndsWith(".txt"))
            {
                text = "txt.png";
            }
            else if (fileName.EndsWith(".html"))
            {
                text = "html.png";
            }

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return "/images/default_image/" + text;
        }

        public static string FormatURLPage(string uriPage, string pageName = "page")
        {
            if (Regex.Match(uriPage, "&" + pageName + "=[0-9]*").Success)
            {
                uriPage = Regex.Replace(uriPage, "&" + pageName + "=[0-9]*", "");
            }
            else if (Regex.Match(uriPage, pageName + "=[0-9]*&").Success)
            {
                uriPage = Regex.Replace(uriPage, pageName + "=[0-9]*&", "");
            }

            if (uriPage.EndsWith("#"))
            {
                uriPage = uriPage.Substring(0, uriPage.Length - 1);
            }

            if (uriPage.EndsWith("\\"))
            {
                uriPage = uriPage.Substring(0, uriPage.Length - 1);
            }

            if (!uriPage.Contains("?"))
            {
                uriPage += "?";
            }

            uriPage = uriPage + "&" + pageName + "={0}";
            return uriPage;
        }

        public static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        public static string Base64Decode(string base64EncodedData)
        {
            byte[] bytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(bytes);
        }

        public static Image Base64ToImage(string imgBase64)
        {
            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(imgBase64)))
            {
                return Image.FromStream((Stream)memoryStream);
            }
        }

        public static string ImageToBase64(Image img)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                img.Save((Stream)memoryStream, ImageFormat.Jpeg);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public static Dictionary<string, object> ToDictionaryDataSet(this DataSet dataSet, bool removeDBNull = true, DataTable dtTableNameMap = null)
        {
            try
            {
                if (dataSet == null || dataSet.Tables.Count == 0)
                {
                    return new Dictionary<string, object>();
                }

                if (dtTableNameMap != null)
                {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    foreach (DataRow item in from d in dtTableNameMap.AsEnumerable()
                                             where d["tableOutput"] != DBNull.Value && Convert.ToBoolean(d["tableOutput"])
                                             select d)
                    {
                        string text = item["tableName"]?.ToString();
                        if (!string.IsNullOrEmpty(text) && dataSet.Tables.Contains(text))
                        {
                            List<Dictionary<string, object>> list = dataSet.Tables[text].ToDictionaryDataTable(removeDBNull);
                            PrepareJsonTable(dataSet, removeDBNull, dtTableNameMap, item, text, list);
                            dictionary[text] = list;
                        }
                    }

                    return dictionary;
                }

                return dataSet.Tables.Cast<DataTable>().ToDictionary((Func<DataTable, string>)((DataTable dt) => dt.TableName), (Func<DataTable, object>)((DataTable dt) => dt.ToDictionaryDataTable(removeDBNull)));
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "methods", "Methods.ToDictionaryDataSet" },
                    { "statusCode", -3 },
                    { "message", ex.Message }
                };
            }
        }

        private static void PrepareJsonTable(DataSet dataSet, bool removeDBNull, DataTable dtTableNameMap, DataRow drMap, string outputTableName, List<Dictionary<string, object>> listDictParent)
        {
            try
            {
                foreach (DataRow item in from d in dtTableNameMap.AsEnumerable()
                                         where d["parentTable"]?.ToString() == outputTableName
                                         select d)
                {
                    string text = item["parentField"]?.ToString();
                    string childTableName = item["tableName"]?.ToString();
                    string text2 = item["childField"]?.ToString();
                    if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(childTableName) || string.IsNullOrEmpty(text2))
                    {
                        continue;
                    }

                    foreach (Dictionary<string, object> item2 in listDictParent)
                    {
                        List<Dictionary<string, object>> list = (item2.ContainsKey(text) ? dataSet.Tables[childTableName].ToDictionaryDataTable(removeDBNull, int.Parse(item2[text].ToString()), text2) : new List<Dictionary<string, object>>());
                        foreach (DataRow item3 in from d in dtTableNameMap.AsEnumerable()
                                                  where d["parentTable"]?.ToString() == childTableName
                                                  select d)
                        {
                            string text3 = childTableName;
                            if (dataSet.Tables.Contains(text3))
                            {
                                dataSet.Tables[text3].ToDictionaryDataTable(removeDBNull);
                                PrepareJsonTable(dataSet, removeDBNull, dtTableNameMap, item3, text3, list);
                            }
                        }

                        item2[childTableName] = list;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Methods.PrepareJsonTable: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static List<Dictionary<string, object>> ToDictionaryDataTable(this DataTable dt, List<string> lstColumn)
        {
            return (from dr in dt.AsEnumerable().ToList()
                    select lstColumn.ToDictionary((string k) => k, (string v) => dr[v])).ToList();
        }

        public static List<Dictionary<string, object>> ToDictionaryDataTable(this DataTable dtIn, bool removeDBNull = true, int parentID = -1, string childField = "")
        {
            if (dtIn == null || dtIn.Rows.Count == 0)
            {
                return new List<Dictionary<string, object>>();
            }

            DataTable dt = dtIn;
            if (removeDBNull)
            {
                if (parentID > 0)
                {
                    childField = (string.IsNullOrEmpty(childField) ? "refID" : childField);
                    if (!dt.Columns.Contains(childField))
                    {
                        return null;
                    }

                    return (from dr in dt.AsEnumerable()
                            where dr[childField] != DBNull.Value && int.Parse(dr[childField].ToString()) == parentID
                            select (from DataColumn c in dt.Columns
                                    select c.ColumnName into cName
                                    where dr[cName] != DBNull.Value
                                    select cName).ToDictionary((string cName) => cName, (string cName) => dr[cName])).ToList();
                }

                return (from dr in dt.AsEnumerable()
                        select (from DataColumn c in dt.Columns
                                select c.ColumnName into cName
                                where dr[cName] != DBNull.Value
                                select cName).ToDictionary((string cName) => cName, (string cName) => dr[cName])).ToList();
            }

            return (from dr in dt.AsEnumerable()
                    select (from DataColumn c in dt.Columns
                            select c.ColumnName).ToDictionary((string cName) => cName, (string cName) => dr[cName])).ToList();
        }

        public static DataTable ConvertDatetimeToString(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0 || !dt.Columns.Cast<DataColumn>().Any((DataColumn c) => c.DataType == typeof(DateTime)))
            {
                return dt;
            }

            DataTable dtRs = dt.Copy();
            (from DataColumn c in dt.Columns
             where c.DataType == typeof(DateTime)
             select c).ToList().ForEach(delegate (DataColumn c)
             {
                 dtRs.Columns.Remove(c.ColumnName);
                 dtRs.Columns.Add(c.ColumnName, typeof(string));
                 (from r in dt.AsEnumerable()
                  where r[c.ColumnName] != DBNull.Value
                  select r).ToList().ForEach(delegate (DataRow r)
                  {
                      dtRs.Rows[dt.Rows.IndexOf(r)][c.ColumnName] = ((DateTime)r[c.ColumnName]).ToString("yyyyMMdd HH:mm:ss");
                  });
             });
            dtRs.AcceptChanges();
            return dtRs;
        }

        public static string FormatHighLightResult(string cellValue, string keyWord)
        {
            try
            {
                if (string.IsNullOrEmpty(keyWord))
                {
                    return cellValue;
                }

                string[] separator = new string[2]
                {
                    string.Format("<span class='{0}'>", "highlight-text"),
                    "</span>"
                };
                string text = cellValue.RemoveToneMark().ToLower();
                string[] array = keyWord.RemoveToneMark().ToLower().Split(new char[1] { ' ' });
                foreach (string text2 in array)
                {
                    if (text2 != string.Empty)
                    {
                        text = text.Replace(text2, string.Format("<span class='{0}'>", "highlight-text") + text2 + "</span>");
                    }
                }

                string[] array2 = text.Split(separator, StringSplitOptions.None);
                List<string> list = cellValue.Select((char c) => c.ToString()).ToList();
                if (array2.Length % 2 != 0)
                {
                    array2 = array2.Take(array2.Length - 1).ToArray();
                }

                int num = 0;
                int num2 = 0;
                array = array2;
                foreach (string text3 in array)
                {
                    num2 += text3.Length;
                    if (num2 + num > list.Count)
                    {
                        num2 = list.Count - num;
                    }

                    if (num % 2 == 0)
                    {
                        list.Insert(num2 + num, string.Format("<span class='{0}'>", "highlight-text"));
                    }
                    else
                    {
                        list.Insert(num2 + num, string.Format("</span>", "highlight-text"));
                    }

                    num++;
                }

                return string.Join("", list);
            }
            catch (Exception)
            {
                return cellValue;
            }
        }

        public static T GetPropertiesValue<T>(this object obj, string propertiesName)
        {
            T result = default(T);
            try
            {
                if (obj == null)
                {
                    return result;
                }

                string lProperties = propertiesName;
                string text = string.Empty;
                if (Enumerable.Contains(propertiesName, '.'))
                {
                    lProperties = propertiesName.Substring(0, propertiesName.IndexOf('.'));
                    text = propertiesName.Substring(propertiesName.IndexOf('.') + 1);
                }

                PropertyInfo propertyInfo = obj.GetType().GetProperties().FirstOrDefault((PropertyInfo o) => o.Name == lProperties);
                if (propertyInfo == null)
                {
                    return result;
                }

                obj = propertyInfo.GetValue(obj, null);
                if (obj == null)
                {
                    return result;
                }

                if (text != string.Empty)
                {
                    return obj.GetPropertiesValue<T>(text);
                }

                return (T)obj;
            }
            catch (Exception)
            {
                return result;
            }
        }

        public static void SetPropertiesValue(this object obj, string propertiesName, object val)
        {
            try
            {
                if (obj == null)
                {
                    return;
                }

                string empty = string.Empty;
                string rProperties = propertiesName;
                if (Enumerable.Contains(propertiesName, '.'))
                {
                    empty = propertiesName.Substring(0, propertiesName.LastIndexOf('.'));
                    rProperties = propertiesName.Substring(propertiesName.LastIndexOf('.') + 1);
                    obj = obj.GetPropertiesValue<object>(empty);
                }

                if (obj == null)
                {
                    return;
                }

                PropertyInfo propertyInfo = obj.GetType().GetProperties().FirstOrDefault((PropertyInfo o) => o.Name == rProperties);
                if (propertyInfo != null)
                {
                    if (string.IsNullOrEmpty(val?.ToString()))
                    {
                        propertyInfo.SetValue(obj, null, null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?)) && val != null)
                    {
                        propertyInfo.SetValue(obj, Convert.ToDateTime(val), null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?)) && val != null)
                    {
                        propertyInfo.SetValue(obj, Convert.ToInt32(val), null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(double) || propertyInfo.PropertyType == typeof(double?)) && val != null)
                    {
                        propertyInfo.SetValue(obj, Convert.ToDouble(val), null);
                    }
                    else if ((propertyInfo.PropertyType == typeof(bool) || propertyInfo.PropertyType == typeof(bool?)) && val != null)
                    {
                        propertyInfo.SetValue(obj, val != null && "true1".Contains(val.ToString()?.ToLower()), null);
                    }
                    else
                    {
                        propertyInfo.SetValue(obj, val, null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(propertiesName + " = " + (val?.ToString() ?? "NULL") + ": " + ex.Message);
            }
        }

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();
            foreach (PropertyDescriptor item in properties)
            {
                dataTable.Columns.Add(item.Name, Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType);
            }

            foreach (T datum in data)
            {
                DataRow dataRow = dataTable.NewRow();
                foreach (PropertyDescriptor item2 in properties)
                {
                    dataRow[item2.Name] = item2.GetValue(datum) ?? DBNull.Value;
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        public static DataRow ToDataRow<T>(this T data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();
            foreach (PropertyDescriptor item in properties)
            {
                dataTable.Columns.Add(item.Name, Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType);
            }

            DataRow dataRow = dataTable.NewRow();
            foreach (PropertyDescriptor item2 in properties)
            {
                dataRow[item2.Name] = item2.GetValue(data) ?? DBNull.Value;
            }

            return dataRow;
        }

        public static Dictionary<string, object> ToDictionaryAny<T>(this T data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (PropertyDescriptor item in properties)
            {
                if (item.GetValue(data) != null && item.GetValue(data) != DBNull.Value)
                {
                    dictionary[item.Name] = item.GetValue(data);
                }
            }

            return dictionary;
        }

        public static Dictionary<string, object> ToDictionaryDataRow(this DataRow dr, bool removeNull = false)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (DataColumn column in dr.Table.Columns)
            {
                if (!removeNull || dr[column.ColumnName] != DBNull.Value)
                {
                    dictionary.Add(column.ColumnName, dr[column.ColumnName]);
                }
            }

            return dictionary;
        }

        public static Dictionary<string, object> ToBaseDictionary(this object data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(data.GetType());
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (PropertyDescriptor item in properties)
            {
                if (Common.ListBaseTypeToDict.Contains(item.PropertyType) && item.GetValue(data) != null && item.GetValue(data) != DBNull.Value)
                {
                    dictionary[item.Name] = item.GetValue(data);
                }
            }

            return dictionary;
        }

        public static DataTable SelectData(string tableName, int refId)
        {
            try
            {
                if (tableName.EndsWith("_") || lstError.Contains(tableName))
                {
                    return null;
                }

                return SqlHelper.ExecuteDataTable("SELECT * FROM " + tableName + " WHERE  refID = " + refId);
            }
            catch
            {
                lstError.Add(tableName);
            }

            return null;
        }

        public static T Get<T>(this Dictionary<string, object> data, string key, T defaultValue = default(T))
        {
            try
            {
                if ((data == null || !data.ContainsKey(key)) && !key.Contains("."))
                {
                    return ConvertValueToType(null, defaultValue);
                }

                if (key.Contains("."))
                {
                    string[] array = key.Split(new char[1] { '.' });
                    return (T)(data.Get<Dictionary<string, object>>(array[0])?.Get<object>(array[1]));
                }

                return ConvertValueToType(data[key], defaultValue);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static string ToTitle(string strs)
        {
            if (string.IsNullOrEmpty(strs) || strs == "ID")
            {
                return strs;
            }

            strs = strs.Trim();
            if (strs.StartsWith("id"))
            {
                strs = strs.Substring(2);
            }

            strs = strs.Replace("_image_path", "");
            string text = string.Concat(strs.Select((char x) => (!char.IsUpper(x)) ? x.ToString() : (" " + x))).TrimStart(new char[1] { ' ' });
            if (!string.IsNullOrEmpty(text))
            {
                text = text[0].ToString().ToUpper() + text.Substring(1);
            }

            return text;
        }

        public static int GetInt(this Dictionary<string, object> data, string key)
        {
            if (!data.ContainsKey(key))
            {
                return 0;
            }

            return (int)data[key];
        }

        public static string GetControlType(string dbType)
        {
            if (Common.DictControlType.Any((KeyValuePair<string, List<string>> c) => c.Value.Contains(dbType)))
            {
                return Common.DictControlType.First((KeyValuePair<string, List<string>> c) => c.Value.Contains(dbType)).Key;
            }

            return "text";
        }

        public static T NewEndity<T>(this T entity, Dictionary<string, object> data)
        {
            foreach (KeyValuePair<string, object> datum in data)
            {
                if (!(datum.Key == "ID"))
                {
                    entity.SetPropertiesValue(datum.Key, datum.Value);
                }
            }

            entity.SetPropertiesValue("createDate", DateTime.Now);
            return entity;
        }

        public static T ModifyEndity<T>(this T entity, Dictionary<string, object> data)
        {
            foreach (KeyValuePair<string, object> datum in data)
            {
                if (!(datum.Key == "ID"))
                {
                    object obj = datum.Value;
                    if (string.IsNullOrEmpty(obj?.ToString()))
                    {
                        obj = null;
                    }

                    entity.SetPropertiesValue(datum.Key, obj);
                }
            }

            entity.SetPropertiesValue("modifyDate", DateTime.Now);
            return entity;
        }

        public static void Put(this Dictionary<string, object> data, string key, object val)
        {
            if (data == null)
            {
                data = new Dictionary<string, object>();
            }

            if (data.ContainsKey(key))
            {
                data[key] = val;
            }
            else
            {
                data.Add(key, val);
            }
        }

        public static Dictionary<string, object> RemoveNullData(this Dictionary<string, object> data)
        {
            Dictionary<string, object> dictionary = data.Clone();
            foreach (KeyValuePair<string, object> datum in data)
            {
                if (datum.Value == null || datum.Value == DBNull.Value)
                {
                    dictionary.Remove(datum.Key);
                }
            }

            return dictionary;
        }

        public static Dictionary<string, object> Clone(this Dictionary<string, object> data)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> datum in data)
            {
                dictionary.Add(datum.Key, datum.Value);
            }

            return dictionary;
        }

        public static List<Dictionary<string, object>> RemoveNullData(this List<Dictionary<string, object>> data)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (Dictionary<string, object> datum in data)
            {
                if (datum != null && datum.Any())
                {
                    list.Add(datum.RemoveNullData());
                }
            }

            return list;
        }

        public static Dictionary<string, object> ToDictionaryException(this Exception ex)
        {
            return new Dictionary<string, object> { { "error_description", ex.Message } };
        }

        public static List<Dictionary<string, object>> RequestAPI(string token, string uri, Dictionary<string, object> dictParameter = null)
        {
            dictParameter = dictParameter ?? new Dictionary<string, object>();
            HttpClient httpClient = new HttpClient(new HttpClientHandler());
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            HttpResponseMessage result = httpClient.PostAsync(GetFullURL_API(uri), new FormUrlEncodedContent(dictParameter.ToDictionary((KeyValuePair<string, object> k) => k.Key, (KeyValuePair<string, object> v) => v.Value?.ToString()))).Result;
            string result2 = result.Content.ReadAsStringAsync().Result;
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            list = JsonToListDict(result2);
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (!list.Any())
                {
                    list.Add(new Dictionary<string, object> { { "error_description", "Empty Data" } });
                }
                else
                {
                    list[0]["error_description"] = list[0]["Message"];
                }
            }

            return list;
        }

        private static List<Dictionary<string, object>> JsonToListDict(string jsonContent)
        {
            if (jsonContent.StartsWith("<!DOCTYPE html>"))
            {
                return null;
            }

            if (jsonContent.StartsWith("{"))
            {
                return new List<Dictionary<string, object>> { JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent) };
            }

            return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonContent);
        }

        public static object PrepareValue(Type pType, object val)
        {
            try
            {
                if (pType == typeof(int) || pType == typeof(int?))
                {
                    if (string.IsNullOrEmpty(val?.ToString()))
                    {
                        return null;
                    }

                    return int.Parse(val.ToString());
                }

                if (pType == typeof(double) || pType == typeof(double?) || pType == typeof(float) || pType == typeof(float?))
                {
                    if (string.IsNullOrEmpty(val?.ToString()))
                    {
                        return null;
                    }

                    return double.Parse(val.ToString());
                }

                if (pType == typeof(bool) || pType == typeof(bool?))
                {
                    if (string.IsNullOrEmpty(val?.ToString()))
                    {
                        return null;
                    }

                    return "true1".Contains(val.ToString()?.ToLower());
                }

                if (pType == typeof(DateTime) || pType == typeof(DateTime?))
                {
                    if (string.IsNullOrEmpty(val?.ToString()))
                    {
                        return null;
                    }

                    if (val.ToString().Contains("T"))
                    {
                        return DateTime.ParseExact(val.ToString(), Common.DATETIME_FORTMAT_STRING_JSON, CultureInfo.InvariantCulture);
                    }

                    return Convert.ToDateTime(val);
                }

                return val;
            }
            catch (Exception)
            {
                return val;
            }
        }

        public static string GetBaseLayout()
        {
            return "~/Views/Shared/_BaseLayout.cshtml";
        }

        public static string ToSQLDbType(CtrlTyp ctrlTyp)
        {
            switch (ctrlTyp)
            {
                case CtrlTyp.Text:
                    return "nvarchar(4000)";
                case CtrlTyp.CheckBox:
                    return "bit";
                case CtrlTyp.Date:
                    return "date";
                case CtrlTyp.DateTime:
                    return "datetime";
                case CtrlTyp.DropDown:
                case CtrlTyp.Int:
                    return "int";
                case CtrlTyp.Memo:
                case CtrlTyp.WebQuickView:
                    return "nvarchar(MAX)";
                case CtrlTyp.Numberic:
                case CtrlTyp.NumbericLong:
                    return "float";
                case CtrlTyp.Picture:
                case CtrlTyp.UploadFile:
                    return "varbinary(MAX)";
                case CtrlTyp.YoutubeKey:
                case CtrlTyp.MultiCheck:
                    return "nvarchar(255)";
                case CtrlTyp.Time:
                    return "time(7)";
                case CtrlTyp.TempField:
                    return "nvarchar(4000)";
                default:
                    throw new Exception("Không hỗ trợ tạo kiểu dữ liệu: " + ctrlTyp);
            }
        }

        public static CtrlTyp ToControlType(Type type)
        {
            if (type == typeof(int))
            {
                return CtrlTyp.Int;
            }

            if (type == typeof(bool))
            {
                return CtrlTyp.CheckBox;
            }

            if (type == typeof(DateTime))
            {
                return CtrlTyp.DateTime;
            }

            if (type == typeof(float) || type == typeof(double))
            {
                return CtrlTyp.Numberic;
            }

            return CtrlTyp.Text;
        }

        public static async Task ResizeImageSQL(int uid, string _tableName = "", Func<Dictionary<string, object>, object> funcUpdateUI = null)
        {
            DataTable dataTable = SqlHelper.ExecuteDataTable("select ID, tableName, isnull(parentTableName ,'') as parentTableName from tblFormSetting where  tableName in (select distinct TABLE_NAME from INFORMATION_SCHEMA.COLUMNS  WHERE   DATA_TYPE = 'varbinary' )");
            foreach (DataRow row in dataTable.Rows)
            {
                string TableName = row["tableName"].ToString();
                if (!string.IsNullOrEmpty(_tableName) && _tableName != TableName)
                {
                    continue;
                }

                string ParentTableName = row["parentTableName"].ToString();
                DataTable dtImage = SqlHelper.ExecuteDataTable("sp_GetImageResize", "@TableName", TableName);
                if (dtImage.Rows.Count == 0)
                {
                    continue;
                }

                EnumerableRowCollection<string> source = from d in SqlHelper.ExecuteDataTable(" SELECT  distinct col.COLUMN_NAME  FROM INFORMATION_SCHEMA.COLUMNS col   WHERE  col.DATA_TYPE = 'varbinary' and TABLE_NAME = '" + TableName + "'").AsEnumerable()
                                                         select d["COLUMN_NAME"].ToString();
                foreach (string fieldName in source.Where((string c) => dtImage.Columns.Contains(c)))
                {
                    EnumerableRowCollection<DataRow> lstDR = from d in dtImage.AsEnumerable()
                                                             where d[fieldName + "_image_path"] != DBNull.Value && d[fieldName + "_image_path"].ToString() != ""
                                                             select d;
                    if (!lstDR.Any())
                    {
                        continue;
                    }

                    SqlHelper.ExecuteNonQuery(" IF COL_LENGTH('" + TableName + "','" + fieldName + "_small_path') is null ALTER TABLE " + TableName + " ADD [" + fieldName + "_small_path] nvarchar(255)");
                    if (!dtImage.Columns.Contains(fieldName + "_image_tmp_local_path"))
                    {
                        dtImage.Columns.Add(fieldName + "_image_tmp_local_path", typeof(string));
                    }

                    int i = 0;
                    foreach (DataRow dr in lstDR)
                    {
                        i++;
                        string urlPath = dr[fieldName + "_image_path"].ToString();
                        funcUpdateUI?.Invoke(new Dictionary<string, object> {
                        {
                            "titleForm",
                            "[" + Math.Round((double)i / (double)lstDR.Count() * 100.0, 2) + "%] " + urlPath
                        } });
                        string url = dr[fieldName + "_small_path"].ToString();
                        try
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadFile(new Uri(GetFullURL(url)), "tmp.jpg");
                            }

                            SqlHelper.ExecuteNonQuery("sp_InsertResizeImage", "@TableName", TableName, "@urlPath", urlPath, "@note", "http image", "@idData", int.Parse(dr["ID"].ToString()));
                        }
                        catch
                        {
                            goto IL_0453;
                        }

                        continue;
                    IL_0453:
                        if (urlPath.Contains("http"))
                        {
                            SqlHelper.ExecuteNonQuery("sp_InsertResizeImage", "@TableName", TableName, "@urlPath", urlPath, "@note", "http image", "@idData", int.Parse(dr["ID"].ToString()));
                            continue;
                        }

                        try
                        {
                            string localFile = DownloadPhoto(GetFullURL(urlPath));
                            if (!File.Exists(localFile))
                            {
                                SqlHelper.ExecuteNonQuery("sp_InsertResizeImage", "@TableName", TableName, "@urlPath", urlPath, "@note", "File not found", "@idData", int.Parse(dr["ID"].ToString()));
                            }
                            else
                            {
                                dr[fieldName + "_image_tmp_local_path"] = localFile;
                                await SqlHelper.UploadImage_SmallPath(new ImageUpload(dr, fieldName), TableName, ParentTableName, uid);
                                SqlHelper.ExecuteNonQuery("sp_InsertResizeImage", "@TableName", TableName, "@urlPath", urlPath, "@idData", int.Parse(dr["ID"].ToString()));
                                DeleteFile(localFile);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.Log();
                        }
                    }
                }
            }
        }

        public static bool IsNullOrEmpty(this DataTable dt)
        {
            if (dt != null)
            {
                return dt.Rows.Count == 0;
            }

            return true;
        }

        public static bool AnyRow(this DataTable dt)
        {
            if (dt != null)
            {
                return !dt.IsNullOrEmpty();
            }

            return false;
        }

        public static void PrepareMonthYear(ref int? thang, ref int? nam)
        {
            if (!thang.HasValue || thang < 1 || thang > 12)
            {
                thang = DateTime.Now.Month;
            }

            if (!nam.HasValue || nam < 1 || nam > 2100)
            {
                nam = DateTime.Now.Year;
            }
        }

        public static void PrepareTuNgayDenNgay(DateTime? defaultTuNgay, DateTime? defaultDenNgay, ref DateTime? tuNgay, ref DateTime? denNgay)
        {
            tuNgay = tuNgay ?? defaultTuNgay;
            denNgay = denNgay ?? defaultDenNgay;
            if (!tuNgay.HasValue && !denNgay.HasValue)
            {
                tuNgay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                denNgay = tuNgay.Value.AddMonths(1).AddDays(-1.0);
            }

            if (!tuNgay.HasValue || tuNgay.Value.Year < 200)
            {
                tuNgay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            if (!denNgay.HasValue)
            {
                denNgay = tuNgay;
            }

            if (denNgay < tuNgay)
            {
                denNgay = tuNgay;
            }
        }

        public static void CreateKhoanMucDanhMuc_CuaToi(out DataTable sp_KhoanMuc_CuaToi, out DataTable sp_DanhMuc_CuaToi, out int idKhoanMucDefault, out int idDanhMucDefault, int uid, int idDanhMucThuChi, bool getAll = false)
        {
            sp_KhoanMuc_CuaToi = SqlHelper.ExecuteDataTable("sp_KhoanMuc_CuaToi", "@uid", uid, "@idDanhMucThuChi", idDanhMucThuChi, "@getAll", getAll);
            idKhoanMucDefault = -1;
            if (sp_KhoanMuc_CuaToi.AnyRow())
            {
                idKhoanMucDefault = Convert.ToInt32(sp_KhoanMuc_CuaToi.Rows[0]["ID"]);
            }

            sp_DanhMuc_CuaToi = SqlHelper.ExecuteDataTable("sp_DanhMuc_CuaToi", "@uid", uid, "@idAccount_ThietLapTCCN", idKhoanMucDefault, "@getAll", getAll);
            idDanhMucDefault = -1;
            if (sp_DanhMuc_CuaToi.AnyRow())
            {
                idDanhMucDefault = Convert.ToInt32(sp_DanhMuc_CuaToi.Rows[0]["ID"]);
            }
        }

        public static string GetTuNgayDenNgay(DateTime tuNgay, DateTime denNgay)
        {
            if (tuNgay.Month != denNgay.Month || tuNgay.Year != denNgay.Year)
            {
                return string.Format("<span>Tháng</span>&nbsp;{0}&nbsp;<span>so với Tháng</span>&nbsp;{1}", denNgay.ToString("M/yyyy"), tuNgay.ToString("M/yyyy"));
            }

            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            return string.Format("<span>Tháng {0}</span>", denNgay.ToString("MM/yyyy"));
        }

        public static string GetTuNgayDenNgay_Calendar(DateTime tuNgay, DateTime denNgay)
        {
            if (tuNgay != denNgay)
            {
                return string.Format("<strong>{0}&nbsp;-&nbsp;{1}</strong>", tuNgay.ToString("dd/MM/yyyy"), denNgay.ToString("dd/MM/yyyy"));
            }

            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            return string.Format("<strong>Ngày {0}</strong>", tuNgay.ToString("dd/MM/yyyy"));
        }

        public static void InitDictURL()
        {
            Common.DictURL = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> item in new Dictionary<string, string>
            {
                { "da", "tblDuAn" },
                { "np", "tblNhaPho" },
                { "nn", "tblBDSNN" },
                { "tt", "tblTinTuc" },
                { "dv", "cbb_DichVuBDS" }
            })
            {
                string value = item.Value;
                string code = item.Key;
                SqlHelper.ExecuteDataTable("select ID, isnull(title,'') as title from " + value + " where  visible = 1 and statusData = 3").AsEnumerable().ToList()
                    .ForEach(delegate (DataRow dr)
                    {
                        string[] value2 = dr["title"].ToString().RemoveToneMark().RemoveSpecialCharacters()
                            .Split(new string[2] { " ", "-" }, StringSplitOptions.RemoveEmptyEntries);
                        Common.DictURL[code + dr["ID"].ToString()] = string.Join("-", value2).ToLower();
                    });
            }
        }

        public static string GetURL(string code, int id, string defaultURL)
        {
            string text = code + id;
            if (!Common.DictURL.ContainsKey(text))
            {
                return defaultURL;
            }

            return Common.DictURL[text] + "-" + text;
        }

        public static string ToHtml(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return string.Join("<br/>", str.Split(new string[4] { "<br>", "<br/>", "\r", "\n\"" }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}