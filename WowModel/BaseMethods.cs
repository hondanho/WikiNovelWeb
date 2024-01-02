
using System;
using System.Web;
using WowCore;


namespace WowModel
{
  public static class BaseMethods
  {
    public static T GetSessionData<T>(this HttpSessionStateBase session, string key)
    {
      try
      {
        object obj = session[key];
        return obj == null ? default (T) : (T) obj;
      }
      catch (Exception ex)
      {
        ex.Log();
        return default (T);
      }
    }
  }
}
