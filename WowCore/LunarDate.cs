
using System;


namespace WowCore
{
  public class LunarDate
  {
    public int Day { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public bool IsLeapYear { get; set; }

    public LunarDate()
    {
    }

    public LunarDate(int day, int month, int year, bool leap)
    {
      this.Day = day;
      this.Month = month;
      this.Year = year;
      this.IsLeapYear = leap;
    }

    public override string ToString()
    {
      int num = this.Year;
      string str1 = num.ToString();
      num = this.Month;
      string str2 = num.ToString();
      num = this.Day;
      string str3 = num.ToString();
      return str1 + str2 + str3;
    }

    public DateTime ToSolarDate(int timeZone) => LunarYearTools.LunarToSolar(this);

    public DateTime ToSolarDate() => this.ToSolarDate(7);
  }
}
