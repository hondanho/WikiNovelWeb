
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WowSQL;


namespace WowModel
{
  public class ProcedureSQL
  {
    public string StoreProcedureName { set; get; }

    public Dictionary<string, SqlDbType> Parameters { set; get; }

    public ProcedureSQL(string spName)
    {
      this.StoreProcedureName = spName;
      this.Parameters = new Dictionary<string, SqlDbType>();
      foreach (SqlParameter spParameter in ExportParameter_Provider.GetSpParameterSet(this.StoreProcedureName))
        this.Parameters.Add(spParameter.ParameterName, spParameter.SqlDbType);
    }
  }
}
