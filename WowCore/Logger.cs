
using Newtonsoft.Json;
using System.Collections.Generic;
using WowCommon;


namespace WowCore
{
  public static class Logger
  {
    public static string ResultForClient(int statusCode, string message)
    {
      return JsonConvert.SerializeObject((object) new
      {
        statusCode = statusCode,
        message = message
      });
    }

    public static string ResultForClient(
      int statusCode,
      string message,
      Dictionary<string, object> dict)
    {
      dict.Merge(nameof (statusCode), (object) statusCode);
      dict.Merge(nameof (message), (object) message);
      return JsonConvert.SerializeObject((object) dict);
    }

    public static void Log(string mgs, bool isLogSQL = true)
    {
      Logger.Log(mgs, nameof (Log), isLogSQL);
    }

    public static void Log(string mgs, string Kind, bool isLogSQL) => mgs.Log();
  }
}
