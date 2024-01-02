
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json.Linq;
using System.Net;
using WowSQL;


namespace WowCore
{
  public static class FireShapeAdmin
  {
    private static IFirebaseClient _client;

    private static IFirebaseClient Client
    {
      get
      {
        if (FireShapeAdmin._client == null)
          FireShapeAdmin._client = (IFirebaseClient) new FirebaseClient((IFirebaseConfig) new FirebaseConfig()
          {
            AuthSecret = SqlHelper.AuthSecret,
            BasePath = SqlHelper.BasePath
          });
        return FireShapeAdmin._client;
      }
    }

    public static bool ValidFireBase(string uidPhone, string phone)
    {
      if (FireShapeAdmin.Client == null)
        return false;
      FirebaseResponse firebaseResponse = FireShapeAdmin.Client.Get(QueryBuilder.New("/tblAccount/" + uidPhone).ToQueryString());
      if (firebaseResponse.StatusCode == HttpStatusCode.OK)
      {
        if (!string.IsNullOrEmpty(firebaseResponse.Body))
        {
          try
          {
            JObject jobject = JObject.Parse(firebaseResponse.Body);
            if (jobject.ContainsKey(nameof (phone)))
            {
              if (jobject.ContainsKey(nameof (uidPhone)))
              {
                if (jobject[nameof (phone)].ToString() == phone)
                {
                  if (jobject[nameof (uidPhone)].ToString() == uidPhone)
                    return true;
                }
              }
            }
          }
          catch
          {
            return false;
          }
        }
      }
      return false;
    }
  }
}
