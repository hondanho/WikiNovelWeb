
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using WowCommon.Model;


namespace WowCommon
{
  public static class Common
  {
    public static string DATETIME_FORTMAT_STRING_JSON = "yyyy-MM-ddThh:mm:ss.000Z";
    public const string ORDER_BY_ID = "";
    public const bool DEV_DEBUG_MODE_JAVA_SCRIPT = false;
    public const int DEMO_ACCOUNT_TEMPLATE_ID = 21;
    public const string DEMO_ACCOUNT_KEY = ".AUTH_ACCOUNT_KEY";
    public const bool DEMO_CLEAR_ALL_ON_CREATE = true;
    public static bool HIEN_THI_ID_WEB = true;
    public static bool HIEN_THI_ID_DATA = false;
    public static bool LoadThumbnailImageGridControl = false;
    public static bool MOVE_FILE_AFTER_UPLOAD = false;
    public static bool USE_ValidFireBase = false;
    public static bool WRITE_LOG_SQL = true;
    public static string LanguageCode = "languageCode";
    public static string LanguageId = "languageId";
    public static string DEFAULT_IMAGE_URL = "images/default_image/default.jpg";
    public static bool HOI_KHI_THEM_COLUMN = false;
    public static int MAX_LENGTH_REQUEST = 10485760;
    public static string MESSAGE_MAX_LENGTH_REQUEST = "File quá lớn, Giới hạn 10Mb";
    public static bool IS_WEB = false;
    public static bool USE_DEFAULT_IMAGE = true;
    public static bool SU_DUNG_BO_LOC_NANG_CAO_WEB = false;
    public static bool UsingCompareUI = true;
    public static bool ShowSTTInTableData = false;
    public const string SETTING_TABLE = "tblFormSetting";
    public const string SETTING_TABLE_DS_CONTROL = "tblFormSetting_dsControl";
    public const string MainPath_Client = "WowWinForm.exe";
    public const int CommandTimeout = 120;
    public static bool Using_MultipleLanguage = false;
    public const LOAD_FORM_MODE LOAD_MODE = LOAD_FORM_MODE.OnlyParent;
    public static EConnection EConnection = EConnection.None;
    public static string HOST_URL_API = "https://wowit.xyz/";
    public const int META_TITLE = 270;
    public const int META_TAGS = 37510;
    public const int META_LONG = 4;
    public const int META_LAT = 2;
    public static string HOST_PHOTO_SUB = "images/Upload";
    public const double DonViTienTe = 1000000.0;
    public const double DonViTienTe_Trieu = 1000000.0;
    public const string DonViTienTe_Text_Nhap = "(Đồng)";
    public const string DonViTienTe_Text_Xem = "(Triệu đồng)";
    public const int ROUND_NUM = 1;
    public static string DEFAULT_LANGUAGE_CODE = "cn";
    public static int DEFAULT_LANGUAGE_ID = 1;
    public static string LOG_FOLDER = "Log";
    public static string APPLCATION_STATUS_PATH = "";
    public static Dictionary<int, string> DICT_LANUGAE_CODE = new Dictionary<int, string>()
    {
      {
        1,
        "vn"
      },
      {
        2,
        "jp"
      },
      {
        3,
        "en"
      },
      {
        4,
        "kr"
      },
      {
        5,
        "cn"
      }
    };
    public static Dictionary<string, Dictionary<string, string>> DICT_MESSAGE = new Dictionary<string, Dictionary<string, string>>();
    public static Dictionary<int, string> DICT_IMAGE_PROJECT = new Dictionary<int, string>();
    public static Dictionary<string, DataTable> DICT_TABLE_SETTING = new Dictionary<string, DataTable>();
    public static Dictionary<string, DataSet> DICT_TABLE_SETTING_DATA_SET = new Dictionary<string, DataSet>();
    public static Dictionary<string, object> DICT_BASE_SETTING_LAYOUT = new Dictionary<string, object>();
    public static Dictionary<string, DataTable> DICT_INFORMATION_SCHEMA_TABLE = new Dictionary<string, DataTable>();
    public static Dictionary<string, List<Dictionary<string, object>>> DICT_FORM_SETTING = new Dictionary<string, List<Dictionary<string, object>>>();
    public static Dictionary<string, string> DictURL = (Dictionary<string, string>) null;
    public static string EntityDBName = "";
    public static string EntityNameSpace = "";
    public static Dictionary<string, List<string>> DictControlType = new Dictionary<string, List<string>>()
    {
      {
        "number",
        new List<string>() { "int", "numeric", "float", "double" }
      },
      {
        "text",
        new List<string>() { "nvarchar", "varchar", "char" }
      },
      {
        "checkbox",
        new List<string>() { "bit" }
      },
      {
        "date",
        new List<string>() { "date" }
      },
      {
        "datetime-local",
        new List<string>() { "datetime" }
      }
    };
    public static Dictionary<EConnection, string> DICT_MAP_URL_WEBTYPE = new Dictionary<EConnection, string>()
    {
      {
        EConnection.None,
        ""
      },
      {
        EConnection.API,
        "http://103.98.160.55"
      },
      {
        EConnection.Land8x,
        "https://8xland.com"
      },
      {
        EConnection.PhanRang,
        "https://phanrangland.com"
      },
      {
        EConnection.TBT,
        "https://8xlandapi.xyz"
      },
      {
        EConnection.Spa,
        "https://spa.catcode.net"
      },
      {
        EConnection.FivePlans,
        "https://5plans.catcode.net"
      },
      {
        EConnection.QLKHO_LENA,
        "https://dichvuvantainguyenthiut.com"
      },
      {
        EConnection.TCCN,
        "https://finmap.vn"
      },
      {
        EConnection.TruyenFree,
        "https://truyenfree.net"
      }
    };
    public static Dictionary<EConnection, string> DICT_MAP_ROOT_FOLDER = new Dictionary<EConnection, string>()
    {
      {
        EConnection.None,
        ""
      },
      {
        EConnection.API,
        "api_8xland"
      },
      {
        EConnection.Land8x,
        "8xland"
      },
      {
        EConnection.PhanRang,
        "phanrangland"
      },
      {
        EConnection.TBT,
        "tbt"
      },
      {
        EConnection.TCCN,
        "tccn.catcode.net"
      },
      {
        EConnection.Spa,
        "spa.catcode.net"
      },
      {
        EConnection.FivePlans,
        "fiveplans"
      },
      {
        EConnection.QLKHO_LENA,
        "qlkho_lena"
      },
      {
        EConnection.TruyenFree,
        "TruyenFree"
      }
    };
    public static List<Type> ListBaseTypeToDict = new List<Type>()
    {
      typeof (int),
      typeof (int?),
      typeof (int),
      typeof (int?),
      typeof (float),
      typeof (float?),
      typeof (double),
      typeof (double?),
      typeof (string),
      typeof (DateTime),
      typeof (DateTime?),
      typeof (byte[]),
      typeof (bool),
      typeof (bool?)
    };
    public static List<string> ListBaseField = new List<string>()
    {
      "uidCreate",
      "uidModify",
      "uidDelete",
      "createDate",
      "modifyDate",
      "deleteDate",
      "visibleHOT",
      "visibleHome",
      "visibleHightlight"
    };
    public static List<string> ListExtendField = new List<string>()
    {
      "image_name",
      "image_title",
      "image_tag",
      "image_size",
      "image_width",
      "image_height"
    };
    public const string EXT_PHOTO_FILTER = "Image files (*.jpg, *.png, *.jpeg) | *.jpg; *.png; *.jpeg";
    public const string EXT_DOCUMENT_FILTER = "PDF, Excel, Word, Power Point, Video MP4|*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.txt;*.html;*.mp4;;*.mp4;*.ppt;*.pptx;";
    public static List<string> EXT_PHOTO = new List<string>()
    {
      ".jpg",
      ".png",
      ".jpeg"
    };
    public static List<string> EXT_DOCUMENT_FILE = new List<string>()
    {
      ".doc",
      ".docx",
      ".xls",
      ".xlsx",
      ".pdf",
      ".svg",
      ".ppt",
      ".pptx",
      ".mp4"
    };
    public const string URL_QUY_TAC_DAT_TEN_FILE = "https://vietnoidungquangcao.wordpress.com/2016/10/15/de-dat-ten-hinh-anh-chuan-seo/";
    public static string URL_NO_PHOTO = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/No_image_available.svg/600px-No_image_available.svg.png";
    public static string URL_NO_PHOTO_USER = "https://8xland.com/images/user.png";
    public static List<LanguageItem> ListLanguage = new List<LanguageItem>();
    public static List<CoutryPhoneNumber> ListCoutryPhoneNumber = new List<CoutryPhoneNumber>()
    {
      new CoutryPhoneNumber()
      {
        CountryCode = "vn",
        PhoneHeaderCode = "+84",
        ListStartPhoneHeader = new List<string>()
        {
          "059",
          "081",
          "082",
          "083",
          "059",
          "079",
          "096",
          "097",
          "098",
          "032",
          "033",
          "034",
          "035",
          "036",
          "037",
          "038",
          "039",
          "090",
          "093",
          "070",
          "071",
          "072",
          "076",
          "077",
          "078",
          "091",
          "094",
          "083",
          "084",
          "085",
          "087",
          "089",
          "099",
          "092",
          "056",
          "058",
          "095",
          "088",
          "086"
        }
      }
    };
    public static Dictionary<int, string> DicLanguageSupport = new Dictionary<int, string>()
    {
      {
        2,
        "jp"
      },
      {
        3,
        "en"
      },
      {
        4,
        "kr"
      },
      {
        5,
        "cn"
      }
    };
    public const int ID_BANNER_TrangChu = 1;
    public const int ID_BANNER_HOC_ONLINE = 2;
    public const int ID_BANNER_UuDaiDacBiet = 3;
    public const int ID_BANNER_TrangChu_Web = 4;
    public const int ID_BANNER_TrangChu_GioiThieu = 5;
    public const int ID_BANNER_TrangChu_BDS_NuocNgoai = 6;
    public const int NhomBDS_AuMy = 1;
    public const int NhomBDS_ChauA = 2;
    public const long NHA_PHO_DANG_BOI_CA_NHAN = 1;
    public const long NHA_PHO_DANG_BOI_MOI_GIOI = 2;
    public const long TrangThaiThongBao_KhoiTao = 1;
    public const long TrangThaiThongBao_GuiLoiChoGuiLai = 2;
    public const long TrangThaiThongBao_DaGuiThanhCong = 10;
    public const long TrangThaiThongBao_Loi = 99;
    private static Logger _Log = LogManager.GetLogger("");
    private static Dictionary<int, List<string>> dictFormatTemplate = new Dictionary<int, List<string>>()
    {
      {
        8,
        new List<string>() { "yyyyMMdd" }
      },
      {
        10,
        new List<string>()
        {
          "yyyy-MM-dd",
          "dd/MM/yyyy",
          "dd-MM-yyyy"
        }
      },
      {
        17,
        new List<string>()
        {
          "HH:mm:ss yyyyMMdd",
          "yyyyMMdd HH:mm:ss"
        }
      },
      {
        19,
        new List<string>()
        {
          "HH:mm:ss yyyy/MM/dd",
          "HH:mm:ss yyyy-MM-dd",
          "yyyy-MM-dd HH:mm:ss",
          "yyyy/MM/dd HH:mm:ss"
        }
      },
      {
        22,
        new List<string>()
        {
          "dd/MM/yyyy HH:mm:ss tt",
          "MM/dd/yyyy HH:mm:ss tt"
        }
      },
      {
        16,
        new List<string>() { "dd/MM/yyyy HH:mm" }
      }
    };
    public const string yaxis_labels_style = "style: {   fontSize: '10px',   fontFamily: 'Roboto',   fontWeight: 'normal',},";
    public const string xaxis_labels_style = "style: {   fontSize: '10px',   fontFamily: 'Roboto',   fontWeight: 'normal',},";
    public const string yaxis_title_style = "style: {   fontSize: '10px',   fontFamily: 'Roboto',   fontWeight: 'normal',},";
    public const string xaxis_title_style = "style: {   fontSize: '10px',   fontFamily: 'Roboto',   fontWeight: 'normal',},";
    public const string dataLabels_style = "style: {   fontSize: '10px',   fontFamily: 'Roboto',   fontWeight: 'normal',},";
    public const string yaxis_series_style = "style: {   fontSize: '10px',   fontFamily: 'Roboto',   fontWeight: 'normal',},";
    public const string xaxis_series_style = "style: {   fontSize: '10px',   fontFamily: 'Roboto',   fontWeight: 'normal',},";

    public static string ERROR_EVENT_IAMGE
    {
      get
      {
        return "onerror=\"if (this.src != '" + Common.HOST_URL + "/" + Common.DEFAULT_IMAGE_URL + "') this.src = '" + Common.HOST_URL + "/" + Common.DEFAULT_IMAGE_URL + "';\"";
      }
    }

    public static string HOST_URL => Common.DICT_MAP_URL_WEBTYPE[Common.EConnection];

    public static ModeLog Mode { get; set; } = ModeLog.Undefine;

    public static string MapLink(string page, params object[] param)
    {
      string str = "/" + page;
      string empty = string.Empty;
      if (param != null && ((IEnumerable<object>) param).Any<object>())
      {
        str += "?";
        for (int index = 0; index < param.Length; index += 2)
        {
          if (!string.IsNullOrEmpty(empty))
            empty += "&";
          empty += string.Format("{0}={1}", param[index], (object) HttpUtility.UrlEncode(param[index + 1].ToString()));
        }
      }
      return str + empty;
    }

    public static string FILE_NAME_TRANSACTION
    {
      get
      {
        if (string.IsNullOrEmpty(Common.APPLCATION_STATUS_PATH))
          Common.APPLCATION_STATUS_PATH = Application.StartupPath;
        return Common.APPLCATION_STATUS_PATH + Common.LOG_FOLDER + "/Logs_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
      }
    }

    public static string GetFullURLFileHostingSQL(
      string parentTable,
      string tableName,
      int id,
      string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
        return string.Empty;
      if (fileName.Contains(":"))
        return fileName;
      if (!string.IsNullOrEmpty(parentTable))
      {
        string str1;
        if (string.IsNullOrEmpty(parentTable))
          str1 = tableName;
        else
          str1 = parentTable + "/" + id.ToString() + "/" + tableName;
        string str2 = str1;
        return string.Format("{0}/{1}/{2}", (object) Common.HOST_PHOTO_SUB, (object) str2, (object) fileName);
      }
      return string.Format("{0}/{1}/{2}/{3}", (object) Common.HOST_PHOTO_SUB, (object) tableName, (object) id.ToString(), (object) fileName);
    }

    public static void CreateDirectory(string fullPath, bool isFilePath)
    {
      try
      {
        string[] strArray = fullPath.Split('\\', '/');
        string path = string.Empty;
        for (int index = 0; index < strArray.Length && (!isFilePath || strArray.Length - 1 != index); ++index)
        {
          string str = strArray[index];
          path = path + str + "\\";
          if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        }
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static DataRow AddNewRow(DataTable dt, params object[] arrFields)
    {
      try
      {
        if (dt == null)
          return (DataRow) null;
        DataRow row = dt.NewRow();
        int index = 0;
        if (arrFields != null)
        {
          for (; index < arrFields.Length; index += 2)
          {
            if (row.Table.Columns.Contains(arrFields[index].ToString()))
            {
              object UInt32 = arrFields[index + 1] ?? (object) DBNull.Value;
              if (row.Table.Columns[arrFields[index].ToString()].DataType.IsNumeric())
                UInt32 = UInt32.ToInt32((object) DBNull.Value);
              row[arrFields[index].ToString()] = UInt32;
            }
          }
        }
        dt.Rows.InsertAt(row, 0);
        return row;
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public static object ToInt32(this object UInt32, object Default)
    {
      return !UInt32.NotNullNotDBNull() || UInt32 is string str && string.IsNullOrEmpty(str) ? Default : (object) Convert.ToInt32(UInt32);
    }

    public static bool NotNullNotDBNull(this object objectData)
    {
      return objectData != DBNull.Value && objectData != null;
    }

    public static bool IsNumeric(this Type dataType)
    {
      if (dataType == (Type) null)
        throw new ArgumentNullException(nameof (dataType));
      return dataType == typeof (int) || dataType == typeof (double) || dataType == typeof (long) || dataType == typeof (short) || dataType == typeof (float) || dataType == typeof (short) || dataType == typeof (int) || dataType == typeof (long) || dataType == typeof (uint) || dataType == typeof (ushort) || dataType == typeof (uint) || dataType == typeof (ulong) || dataType == typeof (sbyte) || dataType == typeof (float);
    }

    public static async Task<HttpResponseMessage> UploadImage(
      byte[] ImageData,
      string parentTable,
      string tableName,
      int id,
      string fileName,
      string fieldName,
      int uid)
    {
      string requestUri = Common.HOST_URL + "/File/UploadImage";
      HttpResponseMessage httpResponseMessage;
      using (HttpClient client = new HttpClient())
      {
        MultipartFormDataContent content1 = new MultipartFormDataContent();
        ByteArrayContent content2 = new ByteArrayContent(ImageData);
        content2.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content1.Add((HttpContent) content2, "file", fileName);
        content1.Add((HttpContent) new StringContent(parentTable), nameof (parentTable));
        content1.Add((HttpContent) new StringContent(tableName), nameof (tableName));
        content1.Add((HttpContent) new StringContent(id.ToString()), nameof (id));
        content1.Add((HttpContent) new StringContent(fileName), nameof (fileName));
        content1.Add((HttpContent) new StringContent(fieldName), nameof (fieldName));
        content1.Add((HttpContent) new StringContent(uid.ToString()), nameof (uid));
        httpResponseMessage = await client.PostAsync(requestUri, (HttpContent) content1);
      }
      return httpResponseMessage;
    }

    private static async Task<string> UploadImage_Zip(
      string url,
      string fullPathLocal,
      string parentTable,
      string tableName,
      int id,
      string fileName,
      string fieldName,
      int deleteId,
      int uid)
    {
      if (parentTable == null)
        parentTable = "";
      string str1;
      using (HttpClient client = new HttpClient())
      {
        MultipartFormDataContent requestContent = new MultipartFormDataContent();
        ByteArrayContent imageContent = new ByteArrayContent(Common.PathFileToByteArray(fullPathLocal));
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        requestContent.Add((HttpContent) imageContent, "file", fileName);
        requestContent.Add((HttpContent) new StringContent(parentTable), nameof (parentTable));
        requestContent.Add((HttpContent) new StringContent(tableName), nameof (tableName));
        requestContent.Add((HttpContent) new StringContent(id.ToString()), nameof (id));
        requestContent.Add((HttpContent) new StringContent(fileName), nameof (fileName));
        requestContent.Add((HttpContent) new StringContent(fieldName), nameof (fieldName));
        requestContent.Add((HttpContent) new StringContent(deleteId.ToString()), nameof (deleteId));
        requestContent.Add((HttpContent) new StringContent(uid.ToString()), nameof (uid));
        HttpResponseMessage reponse = await client.PostAsync(url, (HttpContent) requestContent);
        string str2 = await reponse.Content.ReadAsStringAsync();
        imageContent.Dispose();
        requestContent.Dispose();
        reponse.Dispose();
        client.Dispose();
        str1 = str2;
      }
      return str1;
    }

    private static async Task<string> TEST(
      string url,
      string fullPathLocal,
      string parentTable,
      string tableName,
      int id,
      string fileName,
      string fieldName,
      int deleteId,
      int uid)
    {
      url = "https://8xland.com/File/JoinImage";
      if (parentTable == null)
        parentTable = "";
      string str1;
      using (HttpClient client = new HttpClient())
      {
        MultipartFormDataContent requestContent = new MultipartFormDataContent();
        ByteArrayContent imageContent = new ByteArrayContent(Common.PathFileToByteArray(fullPathLocal));
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        requestContent.Add((HttpContent) imageContent, "file", fileName);
        requestContent.Add((HttpContent) new StringContent(uid.ToString()), nameof (uid));
        requestContent.Add((HttpContent) new StringContent(100.ToString()), "extendSize");
        HttpResponseMessage reponse = await client.PostAsync(url, (HttpContent) requestContent);
        string str2 = await reponse.Content.ReadAsStringAsync();
        if (!str2.StartsWith("[Success]"))
          throw new Exception("Upload Image Fail");
        imageContent.Dispose();
        requestContent.Dispose();
        reponse.Dispose();
        client.Dispose();
        str1 = str2;
      }
      return str1;
    }

    public static async Task<string> DeleteFile(string filePath, int uid)
    {
      string str;
      using (HttpClient client = new HttpClient())
        str = await (await client.PostAsync(Common.HOST_URL + "/File/DeleteFile", (HttpContent) new MultipartFormDataContent()
        {
          {
            (HttpContent) new StringContent(filePath),
            nameof (filePath)
          },
          {
            (HttpContent) new StringContent(uid.ToString()),
            nameof (uid)
          }
        })).Content.ReadAsStringAsync();
      return str;
    }

    public static async Task<string> UploadImage_Zip(
      string fullPathLocal,
      string parentTable,
      string tableName,
      int id,
      string fileName,
      string fieldName,
      int deleteId,
      int uid)
    {
      return await Common.UploadImage_Zip(Common.HOST_URL + "/File/UploadImage_Zip", fullPathLocal, parentTable, tableName, id, fileName, fieldName, deleteId, uid);
    }

    public static async Task<string> UploadImage(
      string fullPathLocal,
      string parentTable,
      string tableName,
      int id,
      string fileName,
      string fieldName,
      int deleteId,
      int uid)
    {
      return await Common.UploadImage_Zip(Common.HOST_URL + "/File/UploadImage", fullPathLocal, parentTable, tableName, id, fileName, fieldName, deleteId, uid);
    }

    public static async Task<HttpResponseMessage> DeletePhoto_Hosting(
      string parentTable,
      string tableName,
      int id,
      string fileName)
    {
      string requestUri = Common.HOST_URL + "/File/DeleteImage";
      HttpResponseMessage httpResponseMessage;
      using (HttpClient client = new HttpClient())
        httpResponseMessage = await client.PostAsync(requestUri, (HttpContent) new MultipartFormDataContent()
        {
          {
            (HttpContent) new StringContent(parentTable),
            nameof (parentTable)
          },
          {
            (HttpContent) new StringContent(tableName),
            nameof (tableName)
          },
          {
            (HttpContent) new StringContent(id.ToString()),
            nameof (id)
          },
          {
            (HttpContent) new StringContent(fileName),
            nameof (fileName)
          }
        });
      return httpResponseMessage;
    }

    public static async Task<string> UploadZipFile(string fileName, string folder, string host)
    {
      string requestUri = host + "/File/UploadZipFile";
      string str;
      using (HttpClient client = new HttpClient())
      {
        MultipartFormDataContent content1 = new MultipartFormDataContent();
        ByteArrayContent content2 = new ByteArrayContent(Common.PathFileToByteArray(fileName));
        content2.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        content1.Add((HttpContent) content2, "file", fileName);
        content1.Add((HttpContent) new StringContent(folder), nameof (folder));
        content1.Add((HttpContent) new StringContent(new FileInfo(fileName).Name), nameof (fileName));
        str = await (await client.PostAsync(requestUri, (HttpContent) content1)).Content.ReadAsStringAsync();
      }
      return str;
    }

    public static byte[] PathFileToByteArray(string fileName)
    {
      using (FileStream input = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        return new BinaryReader((Stream) input).ReadBytes((int) new FileInfo(fileName).Length);
    }

    public static bool IsValidEmail(string email)
    {
      try
      {
        return new MailAddress(email).Address == email;
      }
      catch
      {
        return false;
      }
    }

    public static bool ValidPhoneNumber(string phoneNumber, string countryCode = "vn")
    {
      if (countryCode == "vn")
        return Common.ValidPhoneNumber_VI(phoneNumber);
      throw new Exception("Invalid Country code = " + countryCode);
    }

    public static bool ValidPhoneNumber_VI(string nationalNumber, string countryCode = "vn")
    {
      if (nationalNumber.Trim().StartsWith("1"))
        return true;
      string checkPhone = nationalNumber;
      if (checkPhone.StartsWith("+84"))
        checkPhone = checkPhone.Replace("+84", "0");
      if (!checkPhone.StartsWith("0") && countryCode == "vn")
        checkPhone = "0" + checkPhone;
      return checkPhone.Length == 10 && Common.ListCoutryPhoneNumber.Where<CoutryPhoneNumber>((System.Func<CoutryPhoneNumber, bool>) (s => s.CountryCode == countryCode)).First<CoutryPhoneNumber>().ListStartPhoneHeader.Where<string>((System.Func<string, bool>) (s => checkPhone.StartsWith(s))).Any<string>() && new Regex("[0-9]{10}$").IsMatch(checkPhone);
    }

    public static void Log(this Exception ex)
    {
      try
      {
        Common._Log.Error<Exception>(ex);
      }
      catch (Exception ex1)
      {
        throw ex1;
      }
    }

    public static void Log(this string mgs)
    {
      try
      {
        Common._Log.Info(mgs);
      }
      catch
      {
      }
    }

    public static List<string> GetListSeletedColor(int id, int num)
    {
      List<string> listColor = Common.GetListColor(id);
      if (id > 2 && listColor.Count > num)
        return listColor.Take<string>(num).ToList<string>();
      if (num <= 0 || listColor == null || !listColor.Any<string>())
        return (List<string>) null;
      int num1 = listColor.Count / num;
      if (num1 == 0)
        num1 = 1;
      List<string> listSeletedColor = new List<string>();
      for (int index = 0; index <= Math.Max(num, listColor.Count); index += num1)
      {
        if (listColor.Count > index)
          listSeletedColor.Add(listColor[index]);
        else
          listSeletedColor.Add(listColor[listColor.Count - 1]);
      }
      return listSeletedColor;
    }

    public static List<string> GetListColor(int id)
    {
      switch (id)
      {
        case 1:
          return new List<string>()
          {
            "#447b1d",
            "#467e1e",
            "#488120",
            "#4a8421",
            "#4c8722",
            "#4e8a23",
            "#508d25",
            "#529126",
            "#549427",
            "#569729",
            "#589a2a",
            "#5a9d2b",
            "#5ca02c",
            "#5ea32e",
            "#60a62f",
            "#62a930",
            "#64ac32",
            "#66af33",
            "#68b234",
            "#6ab535",
            "#6eb83a",
            "#74ba43",
            "#7abc4b",
            "#7fbe54",
            "#85c05d",
            "#8bc265",
            "#91c46e",
            "#97c676",
            "#9dc87f",
            "#a3ca87",
            "#a8cb90",
            "#aecd98",
            "#b4cfa1",
            "#bad1aa",
            "#c0d3b2",
            "#c6d5bb",
            "#cbd7c3",
            "#d1d9cc",
            "#d7dbd4",
            "#dddddd"
          };
        case 2:
          return new List<string>()
          {
            "#dba12d",
            "#dca330",
            "#dea533",
            "#dfa735",
            "#e1a938",
            "#e2ab3b",
            "#e3ad3e",
            "#e5af40",
            "#e6b143",
            "#e7b346",
            "#e9b449",
            "#eab64b",
            "#ecb84e",
            "#edba51",
            "#eebc54",
            "#f0be57",
            "#f1c059",
            "#f3c25c",
            "#f4c45f",
            "#f5c662",
            "#f5c866",
            "#f4c96c",
            "#f3ca73",
            "#f2cb79",
            "#f0cc7f",
            "#efcd85",
            "#eece8c",
            "#eccf92",
            "#ebd198",
            "#ead29e",
            "#e9d3a5",
            "#e7d4ab",
            "#e6d5b1",
            "#e5d6b7",
            "#e3d7be",
            "#e2d8c4",
            "#e1daca",
            "#e0dbd0",
            "#dedcd7",
            "#dddddd"
          };
        case 4:
          return new List<string>()
          {
            "#3D7C17",
            "#F4A442",
            "#63B92E",
            "#F6BF16",
            "#88D657",
            "#FFDB6E"
          };
        default:
          return new List<string>()
          {
            "#447b1d",
            "#4d8028",
            "#558531",
            "#5c893a",
            "#648d42",
            "#6b914a",
            "#729551",
            "#799958",
            "#809d5e",
            "#86a064",
            "#8ca369",
            "#92a66e",
            "#98a973",
            "#9eab77",
            "#a4ae7b",
            "#a9b07e",
            "#aeb281",
            "#b3b484",
            "#b8b586",
            "#bdb688",
            "#c1b889",
            "#c6b88a",
            "#cab98a",
            "#ceba8a",
            "#d2ba8a",
            "#d5ba89",
            "#d9ba87",
            "#dcba86",
            "#deb983",
            "#e1b981",
            "#e3b87d",
            "#e6b77a",
            "#e8b576",
            "#e9b471",
            "#ebb26c",
            "#ecb067",
            "#ecae61",
            "#edac5a",
            "#edaa53",
            "#eda74b"
          };
      }
    }

    public static void GetMocThoiGianTruoc(
      string mocThoiGian_truoc,
      DateTime dtNow,
      out int idMocThoiGianThangTruoc,
      out DateTime thoiGianThangTruoc)
    {
      switch (mocThoiGian_truoc)
      {
        case "ngày":
          thoiGianThangTruoc = new DateTime(dtNow.Year, dtNow.Month, 1).AddMonths(-1);
          idMocThoiGianThangTruoc = 8;
          break;
        case "tháng":
          thoiGianThangTruoc = dtNow.AddMonths(-1);
          idMocThoiGianThangTruoc = 4;
          break;
        case "tuần":
          thoiGianThangTruoc = dtNow.AddDays(-7.0);
          idMocThoiGianThangTruoc = 6;
          break;
        case "năm":
          thoiGianThangTruoc = dtNow.AddYears(-1);
          idMocThoiGianThangTruoc = 2;
          break;
        default:
          thoiGianThangTruoc = new DateTime(1, 1, 1);
          idMocThoiGianThangTruoc = -1;
          break;
      }
    }

    public static DateTime ToDateTimeCommon(this object obj, DateTime? defaultValue = null)
    {
      try
      {
        if (obj == null || string.IsNullOrEmpty(obj?.ToString()))
          return new DateTime(1, 1, 1);
        if (obj is DateTime dateTimeCommon1)
          return dateTimeCommon1;
        string str = obj.ToString();
        if (!Common.dictFormatTemplate.ContainsKey(str.Length))
          return Convert.ToDateTime(obj);
        foreach (string format in Common.dictFormatTemplate[str.Length])
        {
          DateTime? dateTimeCommon2 = obj.ToDateTimeCommon(format);
          if (dateTimeCommon2.HasValue)
            return dateTimeCommon2.Value;
        }
      }
      catch
      {
      }
      return !defaultValue.HasValue ? new DateTime(1, 1, 1) : defaultValue.Value;
    }

    private static DateTime? ToDateTimeCommon(this object obj, string format)
    {
      try
      {
        return obj == null || obj.ToString().Length < 8 ? new DateTime?() : new DateTime?(DateTime.ParseExact(obj.ToString(), format, (IFormatProvider) CultureInfo.InvariantCulture));
      }
      catch
      {
        return new DateTime?();
      }
    }

    public enum TextAlignment
    {
      LEFT,
      CENTER,
      RIGHT,
    }
  }
}
