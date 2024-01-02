
using System;


namespace WowCore
{
  public class Solar2Lunar
  {
    public int[] convertSolar2Lunar(int dd, int mm, int yy, int timeZone)
    {
      long num1 = this.jdFromDate(dd, mm, yy);
      long k = this.INT(((double) num1 - 2415021.0769986948) / 29.530588853);
      long newMoonDay = this.getNewMoonDay(k + 1L, (long) timeZone);
      if (newMoonDay > num1)
        newMoonDay = this.getNewMoonDay(k, (long) timeZone);
      long lunarMonth11 = this.getLunarMonth11(yy, timeZone);
      long num2 = lunarMonth11;
      int num3;
      if (lunarMonth11 >= newMoonDay)
      {
        num3 = yy;
        lunarMonth11 = this.getLunarMonth11(yy - 1, timeZone);
      }
      else
      {
        num3 = yy + 1;
        num2 = this.getLunarMonth11(yy + 1, timeZone);
      }
      long num4 = num1 - newMoonDay + 1L;
      long num5 = this.INT((double) ((newMoonDay - lunarMonth11) / 29L));
      int num6 = 0;
      long num7 = num5 + 11L;
      if (num2 - lunarMonth11 > 365L)
      {
        int leapMonthOffset = this.getLeapMonthOffset(lunarMonth11, timeZone);
        if (num5 >= (long) leapMonthOffset)
        {
          num7 = num5 + 10L;
          if (num5 == (long) leapMonthOffset)
            num6 = 1;
        }
      }
      if (num7 > 12L)
        num7 -= 12L;
      if (num7 >= 11L && num5 < 4L)
        --num3;
      return new int[4]
      {
        (int) num4,
        (int) num7,
        num3,
        num6
      };
    }

    private long INT(double d) => (long) Math.Floor(d);

    private long jdFromDate(int dd, int mm, int yy)
    {
      long num1 = this.INT((double) ((14 - mm) / 12));
      long num2 = (long) (yy + 4800) - num1;
      long num3 = (long) mm + 12L * num1 - 3L;
      long num4 = (long) dd + this.INT((double) ((153L * num3 + 2L) / 5L)) + 365L * num2 + this.INT((double) (num2 / 4L)) - this.INT((double) (num2 / 100L)) + this.INT((double) (num2 / 400L)) - 32045L;
      if (num4 < 2299161L)
        num4 = (long) dd + this.INT((double) ((153L * num3 + 2L) / 5L)) + 365L * num2 + this.INT((double) (num2 / 4L)) - 32083L;
      return num4;
    }

    private long getNewMoonDay(long k, long timeZone)
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
      return this.INT(num5 + num11 - num10 + 0.5 + (double) timeZone / 24.0);
    }

    private long getSunLongitude(long jdn, int timeZone)
    {
      double num1 = ((double) jdn - 2451545.5 - (double) (timeZone / 24)) / 36525.0;
      double num2 = num1 * num1;
      double num3 = Math.PI / 180.0;
      double num4 = 357.5291 + 35999.0503 * num1 - 0.0001559 * num2 - 4.8E-07 * num1 * num2;
      double num5 = (280.46645 + 36000.76983 * num1 + 0.0003032 * num2 + ((1.9146 - 0.004817 * num1 - 1.4E-05 * num2) * Math.Sin(num3 * num4) + (0.019993 - 0.000101 * num1) * Math.Sin(num3 * 2.0 * num4) + 0.00029 * Math.Sin(num3 * 3.0 * num4)) - 0.00569 - 0.00478 * Math.Sin((125.04 - 1934.136 * num1) * num3)) * num3;
      return this.INT((num5 - 2.0 * Math.PI * (double) this.INT(num5 / (2.0 * Math.PI))) / Math.PI * 6.0);
    }

    private long getLunarMonth11(int yy, int timeZone)
    {
      long k = this.INT((double) (this.jdFromDate(31, 12, yy) - 2415021L) / 29.530588853);
      long newMoonDay = this.getNewMoonDay(k, (long) timeZone);
      if (this.getSunLongitude(newMoonDay, timeZone) >= 9L)
        newMoonDay = this.getNewMoonDay(k - 1L, (long) timeZone);
      return newMoonDay;
    }

    private int getLeapMonthOffset(long a11, int timeZone)
    {
      long num1 = this.INT(((double) a11 - 2415021.0769986948) / 29.530588853 + 0.5);
      int num2 = 1;
      long sunLongitude = this.getSunLongitude(this.getNewMoonDay(num1 + (long) num2, (long) timeZone), timeZone);
      long num3;
      do
      {
        num3 = sunLongitude;
        ++num2;
        sunLongitude = this.getSunLongitude(this.getNewMoonDay(num1 + (long) num2, (long) timeZone), timeZone);
      }
      while (sunLongitude != num3 && num2 < 14);
      return num2 - 1;
    }
  }
}
