
using System;


namespace WowCore
{
  public static class LunarYearTools
  {
    private const double PI = 3.1415926535897931;

    public static long INT(double d) => (long) Math.Floor(d);

    public static long jdFromDate(int dd, int mm, int yy)
    {
      long num1 = LunarYearTools.INT((double) ((14 - mm) / 12));
      long num2 = (long) (yy + 4800) - num1;
      long num3 = (long) mm + 12L * num1 - 3L;
      long num4 = (long) dd + LunarYearTools.INT((double) ((153L * num3 + 2L) / 5L)) + 365L * num2 + LunarYearTools.INT((double) (num2 / 4L)) - LunarYearTools.INT((double) (num2 / 100L)) + LunarYearTools.INT((double) (num2 / 400L)) - 32045L;
      if (num4 < 2299161L)
        num4 = (long) dd + LunarYearTools.INT((double) ((153L * num3 + 2L) / 5L)) + 365L * num2 + LunarYearTools.INT((double) (num2 / 4L)) - 32083L;
      return num4;
    }

    public static DateTime jdToDate(long jd)
    {
      long num1;
      long num2;
      if (jd > 2299160L)
      {
        long num3 = jd + 32044L;
        num1 = LunarYearTools.INT((double) ((4L * num3 + 3L) / 146097L));
        num2 = num3 - LunarYearTools.INT((double) (num1 * 146097L / 4L));
      }
      else
      {
        num1 = 0L;
        num2 = jd + 32082L;
      }
      long num4 = LunarYearTools.INT((double) ((4L * num2 + 3L) / 1461L));
      long num5 = num2 - LunarYearTools.INT((double) (1461L * num4 / 4L));
      long num6 = LunarYearTools.INT((double) ((5L * num5 + 2L) / 153L));
      long day = num5 - LunarYearTools.INT((double) ((153L * num6 + 2L) / 5L)) + 1L;
      long month = num6 + 3L - 12L * LunarYearTools.INT((double) (num6 / 10L));
      return new DateTime((int) (num1 * 100L + num4 - 4800L + LunarYearTools.INT((double) (num6 / 10L))), (int) month, (int) day);
    }

    public static long NewMoon(long k)
    {
      double num1 = (double) k / 1236.85;
      double num2 = num1 * num1;
      double num3 = num2 * num1;
      double num4 = Math.PI / 180.0;
      double num5 = 2415020.75933 + 29.53058868 * (double) k + 0.0001178 * num2 - 1.55E-07 * num3 + 0.00033 * Math.Sin((166.56 + 132.87 * num1 - 0.009173 * num2) * num4);
      double num6 = 359.2242 + 29.10535608 * (double) k - 3.33E-05 * num2 - 3.47E-06 * num3;
      double num7 = 306.0253 + 385.81691806 * (double) k + 0.0107306 * num2 + 1.236E-05 * num3;
      double num8 = 21.2964 + 390.67050646 * (double) k - 0.0016528 * num2 - 2.39E-06 * num3;
      double num9 = (0.1734 - 0.000393 * num1) * Math.Sin(num6 * num4) + 0.0021 * Math.Sin(2.0 * num4 * num6) - 0.4068 * Math.Sin(num7 * num4) + 0.0161 * Math.Sin(num4 * 2.0 * num7) - 0.0004 * Math.Sin(num4 * 3.0 * num7) + 0.0104 * Math.Sin(num4 * 2.0 * num8) - 0.0051 * Math.Sin(num4 * (num6 + num7)) - 0.0074 * Math.Sin(num4 * (num6 - num7)) + 0.0004 * Math.Sin(num4 * (2.0 * num8 + num6)) - 0.0004 * Math.Sin(num4 * (2.0 * num8 - num6)) - 0.0006 * Math.Sin(num4 * (2.0 * num8 + num7)) + 0.001 * Math.Sin(num4 * (2.0 * num8 - num7)) + 0.0005 * Math.Sin(num4 * (2.0 * num7 + num6));
      double num10 = num1 >= -11.0 ? 0.000265 * num1 - 0.000278 + 0.000262 * num2 : 0.001 + 0.000839 * num1 + 0.0002261 * num2 - 8.45E-06 * num3 - 8.1E-08 * num1 * num3;
      double num11 = num9;
      return (long) Math.Round(num5 + num11 - num10);
    }

    public static double SunLongitude(double jdn)
    {
      double num1 = (jdn - 2451545.0) / 36525.0;
      double num2 = num1 * num1;
      double num3 = Math.PI / 180.0;
      double num4 = 357.5291 + 35999.0503 * num1 - 0.0001559 * num2 - 4.8E-07 * num1 * num2;
      double num5 = (280.46645 + 36000.76983 * num1 + 0.0003032 * num2 + ((1.9146 - 0.004817 * num1 - 1.4E-05 * num2) * Math.Sin(num3 * num4) + (0.019993 - 0.000101 * num1) * Math.Sin(num3 * 2.0 * num4) + 0.00029 * Math.Sin(num3 * 3.0 * num4))) * num3;
      return num5 - 2.0 * Math.PI * (double) LunarYearTools.INT(num5 / (2.0 * Math.PI));
    }

    public static long getSunLongitude(long dayNumber, int timeZone)
    {
      return LunarYearTools.INT(LunarYearTools.SunLongitude((double) dayNumber - 0.5 - (double) (timeZone / 24)) / Math.PI * 6.0);
    }

    public static long getNewMoonDay(long k, int timeZone)
    {
      return LunarYearTools.INT((double) LunarYearTools.NewMoon(k) + 0.5 + (double) (timeZone / 24));
    }

    public static long getLunarMonth11(int yy, int timeZone)
    {
      long k = LunarYearTools.INT((double) (LunarYearTools.jdFromDate(31, 12, yy) - 2415021L) / 29.530588853);
      long newMoonDay = LunarYearTools.getNewMoonDay(k, timeZone);
      if (LunarYearTools.getSunLongitude(newMoonDay, timeZone) >= 9L)
        newMoonDay = LunarYearTools.getNewMoonDay(k - 1L, timeZone);
      return newMoonDay;
    }

    public static long getLeapMonthOffset(long a11, int timeZone)
    {
      long num1 = LunarYearTools.INT(((double) a11 - 2415021.0769986948) / 29.530588853 + 0.5);
      long num2 = 1;
      long sunLongitude = LunarYearTools.getSunLongitude(LunarYearTools.getNewMoonDay(num1 + num2, timeZone), timeZone);
      long num3;
      do
      {
        num3 = sunLongitude;
        ++num2;
        sunLongitude = LunarYearTools.getSunLongitude(LunarYearTools.getNewMoonDay(num1 + num2, timeZone), timeZone);
      }
      while (sunLongitude != num3 && num2 < 14L);
      return num2 - 1L;
    }

    public static LunarDate SolarToLunar(DateTime date) => LunarYearTools.SolarToLunar(date, 7);

    public static LunarDate SolarToLunar(DateTime date, int timeZone)
    {
      long num1 = LunarYearTools.jdFromDate(date.Day, date.Month, date.Year);
      long k = LunarYearTools.INT(((double) num1 - 2415021.0769986948) / 29.530588853);
      long newMoonDay = LunarYearTools.getNewMoonDay(k + 1L, timeZone);
      if (newMoonDay > num1)
        newMoonDay = LunarYearTools.getNewMoonDay(k, timeZone);
      long lunarMonth11 = LunarYearTools.getLunarMonth11(date.Year, timeZone);
      long num2 = lunarMonth11;
      long year;
      if (lunarMonth11 >= newMoonDay)
      {
        year = (long) date.Year;
        lunarMonth11 = LunarYearTools.getLunarMonth11(date.Year - 1, timeZone);
      }
      else
      {
        year = (long) (date.Year + 1);
        num2 = LunarYearTools.getLunarMonth11(date.Year + 1, timeZone);
      }
      long day = num1 - newMoonDay + 1L;
      long num3 = LunarYearTools.INT((double) ((newMoonDay - lunarMonth11) / 29L));
      bool leap = false;
      long month = num3 + 11L;
      if (num2 - lunarMonth11 > 365L)
      {
        long leapMonthOffset = LunarYearTools.getLeapMonthOffset(lunarMonth11, timeZone);
        if (num3 >= leapMonthOffset)
        {
          month = num3 + 10L;
          if (num3 == leapMonthOffset)
            leap = true;
        }
      }
      if (month > 12L)
        month -= 12L;
      if (month >= 11L && num3 < 4L)
        --year;
      return new LunarDate((int) day, (int) month, (int) year, leap);
    }

    public static DateTime LunarToSolar(LunarDate ld) => LunarYearTools.LunarToSolar(ld, 7);

    public static DateTime LunarToSolar(LunarDate ld, int timeZone)
    {
      long lunarMonth11_1;
      long lunarMonth11_2;
      if (ld.Month < 11)
      {
        lunarMonth11_1 = LunarYearTools.getLunarMonth11(ld.Year - 1, timeZone);
        lunarMonth11_2 = LunarYearTools.getLunarMonth11(ld.Year, timeZone);
      }
      else
      {
        lunarMonth11_1 = LunarYearTools.getLunarMonth11(ld.Year, timeZone);
        lunarMonth11_2 = LunarYearTools.getLunarMonth11(ld.Year + 1, timeZone);
      }
      long num1 = LunarYearTools.INT(0.5 + ((double) lunarMonth11_1 - 2415021.0769986948) / 29.530588853);
      long num2 = (long) (ld.Month - 11);
      if (num2 < 0L)
        num2 += 12L;
      if (lunarMonth11_2 - lunarMonth11_1 > 365L)
      {
        long leapMonthOffset = LunarYearTools.getLeapMonthOffset(lunarMonth11_1, timeZone);
        long num3 = leapMonthOffset - 2L;
        if (num3 < 0L)
          num3 += 12L;
        if (ld.IsLeapYear && (long) ld.Month != num3)
          return DateTime.MinValue;
        if (ld.IsLeapYear || num2 >= leapMonthOffset)
          ++num2;
      }
      return LunarYearTools.jdToDate(LunarYearTools.getNewMoonDay(num1 + num2, timeZone) + (long) ld.Day - 1L);
    }
  }
}
