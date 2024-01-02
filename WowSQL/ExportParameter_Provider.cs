
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WowCommon;


namespace WowSQL
{
  public static class ExportParameter_Provider
  {
    public static Hashtable ParamCache { get; set; } = Hashtable.Synchronized(new Hashtable());

    private static SqlParameter[] CloneParameters(IList<SqlParameter> originalParameters)
    {
      SqlParameter[] sqlParameterArray = new SqlParameter[originalParameters.Count];
      int index = 0;
      for (int count = originalParameters.Count; index < count; ++index)
        sqlParameterArray[index] = (SqlParameter) ((ICloneable) originalParameters[index]).Clone();
      return sqlParameterArray;
    }

    public static SqlParameter[] GetSpParameterSet(string spName)
    {
      spName = !string.IsNullOrWhiteSpace(spName) ? spName.ToLower() : throw new Exception("spName is IsNullOrWhiteSpace");
      string str = spName;
      if (!(ExportParameter_Provider.ParamCache[(object) str] is SqlParameter[] arraySqlParameter))
      {
        arraySqlParameter = ExportParameter_Provider.IEnumerableListToArraySqlParameter((IEnumerable<DataRow>) ExportParameter_Provider.GetExportParameter(str).AsEnumerable());
        if (SqlHelper.IsCacheParam)
          ExportParameter_Provider.ParamCache[(object) str] = (object) arraySqlParameter;
      }
      return ExportParameter_Provider.CloneParameters((IList<SqlParameter>) arraySqlParameter);
    }

    public static void Reset()
    {
    }

    public static void Clear()
    {
      ExportParameter_Provider.DicData.Clear();
      ExportParameter_Provider.ParamCache.Clear();
    }

    public static OneString_ReturnDataTable GetExportParameter { get; set; } = new OneString_ReturnDataTable(ExportParameter_Provider.GetExportParameterNeed);

    public static Hashtable DicData { get; set; } = Hashtable.Synchronized(new Hashtable());

    private static bool GetExportParameterMulti(string ProcedureName, out DataTable outTableParam)
    {
      if (ProcedureName.Contains("|"))
      {
        List<DataTable> list = ((IEnumerable<string>) ProcedureName.Split(new char[1]
        {
          '|'
        }, StringSplitOptions.RemoveEmptyEntries)).Select<string, DataTable>((System.Func<string, DataTable>) (p => ExportParameter_Provider.GetExportParameter(p))).Where<DataTable>((System.Func<DataTable, bool>) (c => c != null)).ToList<DataTable>();
        if (list != null)
        {
          DataTable dtTmp = list.First<DataTable>().Clone();
          dtTmp.PrimaryKey = new DataColumn[2]
          {
            dtTmp.Columns["Specific_name"],
            dtTmp.Columns["Name"]
          };
          list.ForEach((Action<DataTable>) (p => dtTmp.Merge(p, true, MissingSchemaAction.Error)));
          outTableParam = dtTmp;
          return true;
        }
      }
      outTableParam = (DataTable) null;
      return false;
    }

    private static DataTable GetExportParameterCore(string ProcedureName, bool Innit = false)
    {
      if (!Innit && string.IsNullOrWhiteSpace(ProcedureName))
        return (DataTable) null;
      DataTable dataTable = new DataTable();
      try
      {
        using (SqlConnection connection = new SqlConnection(SqlHelper.connectionString))
        {
          connection.Open();
          SqlCommand sqlCommand = new SqlCommand("sp_Export_GetParameter", connection);
          sqlCommand.CommandType = CommandType.StoredProcedure;
          sqlCommand.CommandTimeout = 7200;
          using (SqlCommand selectCommand = sqlCommand)
          {
            selectCommand.Parameters.AddWithValue("@ProcedureName", (object) ProcedureName);
            selectCommand.Parameters.AddWithValue("@LanguageId", (object) "VN");
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand))
            {
              if (sqlDataAdapter.SelectCommand.Connection == null)
                sqlDataAdapter.SelectCommand.Connection = connection;
              if (selectCommand.Connection.State == ConnectionState.Closed)
                selectCommand.Connection = connection;
              sqlDataAdapter.Fill(dataTable);
            }
          }
          connection.Close();
        }
      }
      catch (Exception ex)
      {
        ex.Log();
      }
      return dataTable;
    }

    public static DataTable GetExportParameterNeed(string ProcedureName)
    {
      ProcedureName = ProcedureName.ToLower();
      DataTable outTableParam;
      if (ExportParameter_Provider.GetExportParameterMulti(ProcedureName, out outTableParam))
        return outTableParam;
      if (!(ExportParameter_Provider.DicData[(object) ProcedureName] is DataTable exportParameterCore))
      {
        exportParameterCore = ExportParameter_Provider.GetExportParameterCore(ProcedureName);
        if (SqlHelper.IsCacheParam)
          ExportParameter_Provider.DicData[(object) ProcedureName] = (object) exportParameterCore;
      }
      return exportParameterCore;
    }

    private static DataTable GetExportParameterRAM(string ProcedureName)
    {
      ProcedureName = ProcedureName.ToLower();
      DataTable outTableParam;
      if (ExportParameter_Provider.GetExportParameterMulti(ProcedureName, out outTableParam))
        return outTableParam;
      if (!(ExportParameter_Provider.DicData[(object) ProcedureName] is DataTable exportParameterNeed))
        exportParameterNeed = ExportParameter_Provider.GetExportParameterNeed(ProcedureName);
      return exportParameterNeed;
    }

    public static void Init()
    {
      ExportParameter_Provider.GetExportParameter = new OneString_ReturnDataTable(ExportParameter_Provider.GetExportParameterNeed);
      using (DataTable exportParameterCore = ExportParameter_Provider.GetExportParameterCore("", true))
      {
        IEnumerable<IGrouping<string, DataRow>> source = exportParameterCore.Rows.Cast<DataRow>().GroupBy<DataRow, string>((System.Func<DataRow, string>) (c => c["Specific_name"].ToString()));
        ExportParameter_Provider.DicData = Hashtable.Synchronized(new Hashtable((IDictionary) source.ToDictionary<IGrouping<string, DataRow>, string, DataTable>((System.Func<IGrouping<string, DataRow>, string>) (c => c.Key.ToLower()), (System.Func<IGrouping<string, DataRow>, DataTable>) (c => c.CopyToDataTable<DataRow>()))));
        ExportParameter_Provider.ParamCache = Hashtable.Synchronized(new Hashtable((IDictionary) source.ToDictionary<IGrouping<string, DataRow>, string, SqlParameter[]>((System.Func<IGrouping<string, DataRow>, string>) (c => c.Key.ToLower()), (System.Func<IGrouping<string, DataRow>, SqlParameter[]>) (c => ExportParameter_Provider.IEnumerableListToArraySqlParameter((IEnumerable<DataRow>) c)))));
      }
    }

    private static SqlParameter[] IEnumerableListToArraySqlParameter(IEnumerable<DataRow> pa)
    {
      try
      {
        return pa.Select<DataRow, SqlParameter>((System.Func<DataRow, SqlParameter>) (c => !(c["DB_DataType"].ToString() == "table type") && !(c["DB_DataType"].ToString() == "sql_variant") ? new SqlParameter(c["Name"].ToString(), (SqlDbType) Enum.Parse(typeof (SqlDbType), c["DB_DataType"].ToString(), true), Convert.ToInt32(c["length"])) : new SqlParameter(c["Name"].ToString(), (object) null))).ToArray<SqlParameter>();
      }
      catch
      {
        return (SqlParameter[]) null;
      }
    }

    public static bool IsProcedure(string ProcedureName)
    {
      if (!string.IsNullOrEmpty(ProcedureName))
      {
        ProcedureName = ProcedureName.ToLower();
        if (ExportParameter_Provider.DicData.Count > 0 && ExportParameter_Provider.DicData[(object) ProcedureName] is DataTable)
          return true;
        using (DataTable dataTable = SqlHelper.ExecuteDataTable("select top 1 1 from sys.procedures where name = @Name", (object) "@Name", (object) ProcedureName))
        {
          if (dataTable != null)
          {
            if (dataTable.Rows.Count > 0)
            {
              ExportParameter_Provider.DicData[(object) ProcedureName] = (object) new DataTable();
              return true;
            }
          }
        }
      }
      return false;
    }
  }
}
