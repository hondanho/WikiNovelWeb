
using System;


namespace WowCore
{
  public static class LunarDateExt
  {
    public static LunarDate ToLunarDate(this DateTime d, int timeZone)
    {
      return LunarYearTools.SolarToLunar(d, timeZone);
    }

    public static DateTime ToLunarDate(this DateTime d)
    {
      int[] numArray = new Solar2Lunar().convertSolar2Lunar(d.Day, d.Month, d.Year, 7);
      return new DateTime(numArray[2], numArray[1], numArray[0]);
    }
  }
}
