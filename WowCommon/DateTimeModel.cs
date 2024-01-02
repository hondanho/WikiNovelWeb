
using System;
using System.Collections.Generic;
using System.Linq;


namespace WowCommon
{
  public class DateTimeModel
  {
    public static List<WeekOfYearModel> GetWeekOfYear(int year = -1)
    {
      if (year == -1)
        year = DateTime.Today.Year;
      DateTime jan1 = new DateTime(year, 1, 1);
      DateTime startOfFirstWeek = jan1.AddDays((double) (1 - jan1.DayOfWeek));
      return Enumerable.Range(0, 54).Select(i => new
      {
        weekStart = startOfFirstWeek.AddDays((double) (i * 7))
      }).TakeWhile(x => x.weekStart.Year <= jan1.Year).Select(x => new
      {
        weekStart = x.weekStart,
        weekFinish = x.weekStart.AddDays(6.0)
      }).SkipWhile(x => x.weekFinish < jan1.AddDays(1.0)).Select((x, i) => new WeekOfYearModel(i + 1, x.weekStart, x.weekFinish)).ToList<WeekOfYearModel>();
    }
  }
}
