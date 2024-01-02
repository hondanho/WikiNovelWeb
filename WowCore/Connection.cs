
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WowCommon;
using WowCommon.Model;


namespace WowCore
{
  public static class Connection
  {
    public static string URL_HOST = "https://8xland.com";
    private static FirebaseApp _app = (FirebaseApp) null;
    private static FirebaseMessaging _messaging = (FirebaseMessaging) null;
    public static Dictionary<string, DataTable> CacheData = new Dictionary<string, DataTable>();

    public static string KEY_DOCUMENT_ID => "ID";

    public static void InitLanguage(string pathFile)
    {
      if (!File.Exists(pathFile))
        return;
      try
      {
        string format = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0};Extended Properties=\"Excel 8.0;HDR=yes;\"";
        if (pathFile.EndsWith(".xlsx"))
          format = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=YES;\";";
        using (OleDbConnection connection = new OleDbConnection(string.Format(format, (object) pathFile)))
        {
          connection.Open();
          using (OleDbDataReader oleDbDataReader = new OleDbCommand("select * from [Sheet1$]", connection).ExecuteReader())
          {
            while (oleDbDataReader.Read())
            {
              string str1 = oleDbDataReader[1]?.ToString();
              if (!string.IsNullOrEmpty(str1))
              {
                LanguageItem item = new LanguageItem();
                int result;
                int.TryParse(oleDbDataReader[0]?.ToString() ?? "-1", out result);
                item.index = result;
                item.vi = str1;
                foreach (KeyValuePair<int, string> keyValuePair in Common.DicLanguageSupport)
                {
                  string str2 = oleDbDataReader[keyValuePair.Key * 2 - 1]?.ToString();
                  if (!string.IsNullOrEmpty(str2))
                  {
                    switch (keyValuePair.Value)
                    {
                      case "en":
                        item.en = str2;
                        break;
                      case "jp":
                        item.jp = str2;
                        break;
                      case "kr":
                        item.kr = str2;
                        break;
                      case "cn":
                        item.cn = str2;
                        break;
                    }
                    if (!Common.ListLanguage.Any<LanguageItem>((System.Func<LanguageItem, bool>) (i => i.vi == item.vi)))
                      Common.ListLanguage.Add(item);
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        ex.Log();
      }
    }

    public static FirebaseApp app
    {
      get
      {
        if (Connection._app == null)
          Connection.ConnectFireBaseAdmin();
        return Connection._app;
      }
      set => Connection._app = value;
    }

    public static FirebaseMessaging messaging
    {
      get
      {
        if (Connection._messaging == null)
          Connection.ConnectFireBaseAdmin();
        return Connection._messaging;
      }
      set => Connection._messaging = value;
    }

    private static void setCacheData(DataTable dt, string key)
    {
      if (Connection.CacheData.ContainsKey(key))
        Connection.CacheData[key] = dt;
      else
        Connection.CacheData.Add(key, dt);
    }

    private static DataTable getCacheData(string key)
    {
      return Connection.CacheData.ContainsKey(key) ? Connection.CacheData[key] : (DataTable) null;
    }

    private static string GetPathConfig(string name)
    {
      string path = AppDomain.CurrentDomain.BaseDirectory + "bin\\" + name;
      return File.Exists(path) ? path : AppDomain.CurrentDomain.BaseDirectory + name;
    }

    public static void ConnectFireBaseAdmin()
    {
      string name;
      switch (Common.EConnection)
      {
        case EConnection.Land8x:
          name = "wowlandgroup.json";
          break;
        case EConnection.WowBook:
          name = "wowbook.json";
          break;
        case EConnection.TBT:
          name = "wowtbt.json";
          break;
        default:
          throw new Exception(Common.EConnection.ToString() + " isn't support");
      }
      if (Connection._app != null && Connection.messaging != null)
        return;
      string pathConfig = Connection.GetPathConfig(name);
      Connection._app = File.Exists(pathConfig) ? FirebaseApp.Create(new AppOptions()
      {
        Credential = GoogleCredential.FromFile(pathConfig).CreateScoped(new string[1]
        {
          "https://www.googleapis.com/auth/firebase.messaging"
        })
      }) : throw new Exception("Find not found json config file: " + pathConfig);
      Connection._messaging = FirebaseMessaging.GetMessaging(Connection.app);
    }

    public static async Task<string> GetSeverVersion() => "-1";

    public static int getPadding(double percenWidth)
    {
      if (percenWidth <= 0.25)
        return 12;
      if (percenWidth <= 0.5)
        return 18;
      return percenWidth <= 0.75 ? 24 : 30;
    }
  }
}
