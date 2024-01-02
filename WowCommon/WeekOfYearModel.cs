
using System;


namespace WowCommon
{
  public class WeekOfYearModel
  {
    public int WeekNum { get; }

    public DateTime DFrom { get; }

    public DateTime DTo { get; }

    public WeekOfYearModel(int week, DateTime f, DateTime t)
    {
      this.WeekNum = week;
      this.DFrom = f;
      this.DTo = t;
    }
  }
}
