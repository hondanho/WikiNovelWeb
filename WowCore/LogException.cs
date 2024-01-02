using System;
using System.Collections.Generic;
using System.Data;


namespace WowCore
{
  public static class LogException
  {
    public static string ResultForClient(this Exception ex)
    {
      return Logger.ResultForClient(-1, "Dữ liệu không đúng cấu trúc");
    }

    public static string ResultForClient(this Exception ex, Dictionary<string, object> dict)
    {
      return Logger.ResultForClient(-1, ex.Message, dict);
    }

    public static void Log(this Exception ex, bool isLogSQL = true)
    {
      Logger.Log("[Exception]:" + ex.ToString() + "\nMessage:" + ex.Message + "\nStackTrace:" + ex.StackTrace, "Exception", isLogSQL);
    }

    public static DataTable ToDataTable(this Exception ex)
    {
      DataTable dataTable = new DataTable();
      dataTable.Columns.Add("Message");
      dataTable.Columns.Add("StackTrace");
      DataRow row = dataTable.NewRow();
      row["Message"] = (object) ex.Message;
      row["StackTrace"] = (object) ex.StackTrace;
      dataTable.Rows.Add(row);
      return dataTable;
    }
  }
}
