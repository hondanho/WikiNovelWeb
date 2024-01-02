
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WowCommon;


namespace WowSQL
{
  public static class SqlHelper
  {
    private static DataRow _drInfo = (DataRow) null;
    public static DateTime lastRanProcessPendingData = DateTime.Now;

    public static bool PauseProcessNotify { get; set; } = true;

    public static string ApplicationName { get; set; } = "CatCode INC";

    public static string AuthSecret { get; set; }

    public static string BasePath { get; set; }

    public static bool IsCacheParam { get; set; } = false;

    public static string m_strServer { get; set; }

    public static string m_strDatabase { get; set; }

    public static string m_strUser { get; set; }

    public static string m_strPassword { get; set; }

    public static string m_strEncrypted { get; set; }

    public static DataRow Row_Infor
    {
      get => SqlHelper._drInfo;
      set
      {
        SqlHelper._drInfo = value;
        WowCommon.Common.HIEN_THI_ID_DATA = SqlHelper._drInfo != null && SqlHelper._drInfo.Table.Columns.Contains("hienThiIDData") && SqlHelper._drInfo["hienThiIDData"] != DBNull.Value && Convert.ToBoolean(SqlHelper._drInfo["hienThiIDData"]);
      }
    }

    public static int UserID { get; set; } = -1;

    public static bool IS_SUPPER_ADMIN { get; set; } = false;

    public static string UserName { set; get; }

    public static int LanguageID { get; set; } = 1;

    public static string GetSQLDBType(string name)
    {
      switch (name)
      {
        case "Boolean":
          return "bit";
        case "Byte[]":
          return "varbinary";
        case "DateTime":
          return "datetime";
        case "Decimal":
          return "money";
        case "Double":
          return "float";
        case "Int16":
        case "Int32":
        case "Int64":
        case "UInt16":
        case "UInt32":
        case "UInt64":
          return "int";
        case "String":
          return "nvarchar";
        case "TimeSpan":
          return "time";
        default:
          return "nvarchar";
      }
    }

    public static Type GetClrType(SqlDbType sqlType)
    {
      switch (sqlType)
      {
        case SqlDbType.BigInt:
          return typeof (long?);
        case SqlDbType.Binary:
        case SqlDbType.Image:
        case SqlDbType.Timestamp:
        case SqlDbType.VarBinary:
          return typeof (byte[]);
        case SqlDbType.Bit:
          return typeof (bool?);
        case SqlDbType.Char:
        case SqlDbType.NChar:
        case SqlDbType.NText:
        case SqlDbType.NVarChar:
        case SqlDbType.Text:
        case SqlDbType.VarChar:
        case SqlDbType.Xml:
          return typeof (string);
        case SqlDbType.DateTime:
        case SqlDbType.SmallDateTime:
        case SqlDbType.Date:
        case SqlDbType.Time:
        case SqlDbType.DateTime2:
          return typeof (DateTime?);
        case SqlDbType.Decimal:
        case SqlDbType.Money:
        case SqlDbType.SmallMoney:
          return typeof (Decimal?);
        case SqlDbType.Float:
          return typeof (double?);
        case SqlDbType.Int:
          return typeof (int?);
        case SqlDbType.Real:
          return typeof (float?);
        case SqlDbType.UniqueIdentifier:
          return typeof (Guid?);
        case SqlDbType.SmallInt:
          return typeof (short?);
        case SqlDbType.TinyInt:
          return typeof (byte?);
        case SqlDbType.Variant:
        case SqlDbType.Udt:
          return typeof (object);
        case SqlDbType.Structured:
          return typeof (DataTable);
        case SqlDbType.DateTimeOffset:
          return typeof (DateTimeOffset?);
        default:
          throw new ArgumentOutOfRangeException(nameof (sqlType));
      }
    }

    public static void ConnectDB(EConnection eConnection, string filepath = "")
    {
      if (string.IsNullOrEmpty(filepath))
        filepath = string.Format("{0}\\{1}", (object) Application.StartupPath, (object) (eConnection.ToString() + ".catcode.db"));
      if (!File.Exists(filepath))
        throw new Exception("Không tìm thấy file cấu hình kết nối: " + filepath);
      try
      {
        string strServer;
        string strDatabase;
        string strUserName;
        string strPassword;
        string _AuthSecret;
        string _BasePath;
        EnDeConnection.GetDBConnectionInfo(filepath, out strServer, out strDatabase, out strUserName, out strPassword, out _AuthSecret, out _BasePath, out string _, out string _);
        SqlHelper.SetDBConnectionInfo(strServer, strDatabase, strUserName, strPassword);
        SqlHelper.AuthSecret = _AuthSecret;
        SqlHelper.BasePath = _BasePath;
        WowCommon.Common.EConnection = eConnection;
      }
      catch (Exception ex)
      {
        ex.Log();
        throw new Exception("ConnectDB: " + ex.Message);
      }
    }

    public static void ConnectDBCore(EConnection eConnection, string filepath = "")
    {
      if (!File.Exists(filepath))
        throw new Exception("Không tìm thấy file cấu hình kết nối: " + filepath);
      try
      {
        string strServer;
        string strDatabase;
        string strUserName;
        string strPassword;
        string _AuthSecret;
        string _BasePath;
        EnDeConnection.GetDBConnectionInfo(filepath, out strServer, out strDatabase, out strUserName, out strPassword, out _AuthSecret, out _BasePath, out string _, out string _);
        SqlHelper.SetDBConnectionInfo(strServer, strDatabase, strUserName, strPassword);
        SqlHelper.AuthSecret = _AuthSecret;
        SqlHelper.BasePath = _BasePath;
        WowCommon.Common.EConnection = eConnection;
      }
      catch (Exception ex)
      {
        ex.Log();
        throw new Exception("ConnectDB: " + ex.Message);
      }
    }

    public static bool SetDBConnectionInfo(
      string strServer,
      string strDatabase,
      string strUserName,
      string strPassword)
    {
      try
      {
        SqlHelper.m_strServer = strServer;
        SqlHelper.m_strDatabase = strDatabase;
        SqlHelper.m_strUser = strUserName;
        SqlHelper.m_strPassword = strPassword;
        SqlHelper.SetConnection(strServer, strDatabase, strUserName, strPassword);
        return true;
      }
      catch (Exception ex)
      {
        ex.Log();
        return false;
      }
    }

    private static string GetParameter(string key, string defaultValue)
    {
      try
      {
        DataTable dataTable = SqlHelper.ExecuteDataTable("SELECT VALUE FROM tblParameter where CODE = '" + key + "'");
        if (dataTable != null)
        {
          if (dataTable.Rows.Count > 0)
          {
            if (dataTable.Rows[0][0] != DBNull.Value)
              return dataTable.Rows[0][0].ToString();
          }
        }
      }
      catch
      {
      }
      return defaultValue;
    }

    public static void SetConnection()
    {
      SqlHelper.SetConnection(SqlHelper.m_strServer, SqlHelper.m_strDatabase, SqlHelper.m_strUser, SqlHelper.m_strPassword);
    }

    private static void SetConnection(
      string strServer,
      string strDatabase,
      string strUserName,
      string strPassword)
    {
      if (strServer.ToLower().StartsWith("(localdb)"))
        SqlHelper.connectionString = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True;", (object) strServer, (object) strDatabase);
      else
        SqlHelper.connectionString = string.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3};Application Name={4};", (object) strServer, (object) strDatabase, (object) strUserName, (object) strPassword, (object) SqlHelper.ApplicationName);
    }

    public static bool IsConnected { get; set; } = false;

    public static bool CheckConnectionInfo(out string mgs)
    {
      try
      {
        SqlHelper.SetConnection();
        using (SqlConnection sqlConnection = new SqlConnection(SqlHelper.connectionString))
        {
          sqlConnection.Open();
          sqlConnection.Close();
        }
        SqlHelper.IsConnected = true;
        mgs = "CONNECT SUCCESS";
      }
      catch (Exception ex)
      {
        ex.Log();
        mgs = ex.Message;
        mgs = mgs + "\n" + ex.StackTrace;
        SqlHelper.IsConnected = false;
      }
      return SqlHelper.IsConnected;
    }

    public static bool CheckConnectionInfo(string connectionString)
    {
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          sqlConnection.Open();
          sqlConnection.Close();
        }
        return true;
      }
      catch
      {
        return false;
      }
    }

    public static string connectionString { get; set; } = string.Empty;

    public static object ProcessNotifyDataObject { get; set; } = new object();

    private static void AttachParameters(
      SqlCommand command,
      IEnumerable<SqlParameter> commandParameters)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      if (commandParameters == null)
        return;
      foreach (SqlParameter sqlParameter in commandParameters.Where<SqlParameter>((System.Func<SqlParameter, bool>) (p => p != null)))
      {
        if ((sqlParameter.Direction == ParameterDirection.InputOutput || sqlParameter.Direction == ParameterDirection.Input) && sqlParameter.Value == null)
          sqlParameter.Value = (object) DBNull.Value;
        command.Parameters.Add(sqlParameter);
      }
    }

    public static DataTable ExecuteDataTable(
      SqlConnection conn,
      string commandText,
      object[] parameterValues)
    {
      DataSet dataSet;
      if (commandText.Contains(" "))
      {
        if (parameterValues == null || parameterValues.Length == 0)
        {
          dataSet = SqlHelper.ExecuteDataset(conn, CommandType.Text, commandText);
        }
        else
        {
          SqlParameter[] array = ((IEnumerable<object>) parameterValues).Select((val, ind) => new
          {
            v = val,
            i = ind
          }).Where(c => c.i % 2 == 0).Select(c => new SqlParameter(c.v.ToString(), parameterValues[c.i + 1])).ToArray<SqlParameter>();
          dataSet = SqlHelper.ExecuteDataset(conn, CommandType.Text, commandText, array);
        }
      }
      else
        dataSet = SqlHelper.ExecuteDataset(conn, commandText, parameterValues);
      return dataSet != null && dataSet.Tables.Count != 0 ? dataSet.Tables[0] : (DataTable) null;
    }

    private static void AssignParameterValues(
      IEnumerable<SqlParameter> commandParameters,
      DataRow dataRow)
    {
      if (commandParameters == null || dataRow == null)
        return;
      int num = 0;
      foreach (SqlParameter commandParameter in commandParameters)
      {
        if (commandParameter.ParameterName == null || commandParameter.ParameterName.Length <= 1)
          throw new Exception(string.Format("Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: '{1}'.", (object) num, (object) commandParameter.ParameterName));
        if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
          commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
        ++num;
      }
    }

    private static void AssignParameterValues(
      ref SqlParameter[] commandParametersbase,
      object[] parameterValues)
    {
      SqlParameter[] commandParameters = commandParametersbase;
      if (commandParameters == null || parameterValues == null)
        return;
      if (parameterValues.Length % 2 != 0)
        throw new ArgumentException("Parameter count does not match Parameter Value count.");
      List<string> inputPrNames = ((IEnumerable<object>) parameterValues).Select((value, index) => new
      {
        value = value,
        index = index
      }).Where(dc => dc.index % 2 == 0 && dc.value != null).Select(dc => dc.value.ToString().ToLower()).ToList<string>();
      if (!inputPrNames.Contains("@uid") && SqlHelper.UserID > 0)
      {
        parameterValues = ((IEnumerable<object>) parameterValues).Concat<object>((IEnumerable<object>) new object[2]
        {
          (object) "@uid",
          (object) SqlHelper.UserID
        }).ToArray<object>();
        inputPrNames.Add("@uid");
      }
      if (!inputPrNames.Contains("@languageid"))
      {
        parameterValues = ((IEnumerable<object>) parameterValues).Concat<object>((IEnumerable<object>) new object[2]
        {
          (object) "@languageid",
          (object) SqlHelper.LanguageID
        }).ToArray<object>();
        inputPrNames.Add("@languageid");
      }
      if (!inputPrNames.Contains("@isweb"))
      {
        parameterValues = ((IEnumerable<object>) parameterValues).Concat<object>((IEnumerable<object>) new object[2]
        {
          (object) "@isweb",
          (object) WowCommon.Common.IS_WEB
        }).ToArray<object>();
        inputPrNames.Add("@isweb");
      }
      if (!inputPrNames.Contains("@mode"))
      {
        parameterValues = ((IEnumerable<object>) parameterValues).Concat<object>((IEnumerable<object>) new object[2]
        {
          (object) "@mode",
          (object) WowCommon.Common.Mode.ToString()
        }).ToArray<object>();
        inputPrNames.Add("@mode");
      }
      commandParameters = ((IEnumerable<SqlParameter>) commandParameters).Where<SqlParameter>((System.Func<SqlParameter, bool>) (dc => inputPrNames.Contains(dc.ParameterName.ToLower()))).ToArray<SqlParameter>();
      ((IEnumerable<object>) parameterValues).Select((value, index) => new
      {
        value = value,
        index = index
      }).Where(dc => dc.index % 2 == 0 && dc.value != null && SqlHelper.TruelyParamName(dc.value.ToString())).ToList().ForEach(dc =>
      {
        SqlParameter sqlParameter = ((IEnumerable<SqlParameter>) commandParameters).FirstOrDefault<SqlParameter>((System.Func<SqlParameter, bool>) (pr => string.Compare(pr.ParameterName, dc.value.ToString(), true) == 0));
        if (sqlParameter == null)
          return;
        object obj = parameterValues[dc.index + 1];
        if (sqlParameter.DbType == DbType.Time && obj is DateTime dateTime2)
          obj = (object) dateTime2.TimeOfDay;
        if (obj != null)
          sqlParameter.Value = obj is string str2 ? (object) str2.Trim() : obj;
        else if (obj == null)
        {
          if (sqlParameter.DbType == DbType.Boolean)
            sqlParameter.Value = (object) false;
          else
            sqlParameter.Value = (object) DBNull.Value;
        }
        else
          sqlParameter.Value = obj;
      });
      commandParametersbase = commandParameters;
    }

    private static bool TruelyParamName(string v)
    {
      if (v.StartsWith("@"))
        return true;
      throw new ArgumentException("Parameter name imputed is not correct: " + v);
    }

    private static void PrepareCommand(
      SqlCommand command,
      SqlConnection connection,
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      IEnumerable<SqlParameter> commandParameters,
      out bool mustCloseConnection)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      if (string.IsNullOrEmpty(commandText))
        throw new ArgumentNullException(nameof (commandText));
      if (connection.State != ConnectionState.Open)
      {
        mustCloseConnection = true;
        connection.Open();
      }
      else
        mustCloseConnection = false;
      command.Connection = connection;
      command.CommandText = commandText;
      if (transaction != null)
        command.Transaction = transaction.Connection != null ? transaction : throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      command.CommandType = commandType;
      if (commandParameters == null)
        return;
      SqlHelper.AttachParameters(command, commandParameters);
    }

    private static async Task<bool> PrepareCommandAsync(
      SqlCommand command,
      SqlConnection connection,
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      IEnumerable<SqlParameter> commandParameters)
    {
      if (command == null)
        throw new ArgumentNullException(nameof (command));
      if (string.IsNullOrEmpty(commandText))
        throw new ArgumentNullException(nameof (commandText));
      bool mustCloseConnection = false;
      if (connection.State != ConnectionState.Open)
      {
        mustCloseConnection = true;
        await connection.OpenAsync().ConfigureAwait(false);
      }
      command.Connection = connection;
      command.CommandText = commandText;
      if (transaction != null)
        command.Transaction = transaction.Connection != null ? transaction : throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      command.CommandType = commandType;
      if (commandParameters != null)
        SqlHelper.AttachParameters(command, commandParameters);
      return mustCloseConnection;
    }

    public static int ExecuteNonQuery(
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      try
      {
        using (SqlConnection connection = new SqlConnection(SqlHelper.connectionString))
        {
          connection.Open();
          return SqlHelper.ExecuteNonQuery(connection, commandType, commandText, commandParameters);
        }
      }
      catch (Exception ex)
      {
        ex.Log();
        throw ex;
      }
    }

    private static void HandleException(string message)
    {
      string lower = message.ToLower();
      if (!lower.Contains(" is not a parameter") && !lower.Contains("expects p") && !lower.Contains("too man") && !lower.Contains("incompatible") && !lower.Contains("to convert"))
        return;
      ExportParameter_Provider.Clear();
    }

    public static SqlTransaction BeginTransaction(string transactionName)
    {
      transactionName = transactionName.Length > 32 ? transactionName.Substring(transactionName.Length - 32) : transactionName;
      SqlConnection sqlConnection = new SqlConnection(SqlHelper.connectionString);
      sqlConnection.Open();
      return sqlConnection.BeginTransaction(transactionName);
    }

    public static int ExecuteNonQuery(string commandText, params object[] parameterValues)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(commandText))
          return 0;
        if (commandText.Contains(" "))
        {
          if (parameterValues == null || parameterValues.Length == 0)
            return SqlHelper.ExecuteNonQuery(CommandType.Text, commandText);
          SqlParameter[] array = ((IEnumerable<object>) parameterValues).Select((val, ind) => new
          {
            v = val,
            i = ind
          }).Where(c => c.i % 2 == 0).Select(c => new SqlParameter(c.v.ToString(), parameterValues[c.i + 1])).ToArray<SqlParameter>();
          return SqlHelper.ExecuteNonQuery(CommandType.Text, commandText, array);
        }
        SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(commandText);
        if (spParameterSet.Length == 0)
          return SqlHelper.ExecuteNonQuery(CommandType.StoredProcedure, commandText);
        SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
        return SqlHelper.ExecuteNonQuery(CommandType.StoredProcedure, commandText, spParameterSet);
      }
      catch (Exception ex)
      {
        ex.Log();
        throw ex;
      }
    }

    public static int ExecuteNonQuery(
      SqlConnection conn,
      string commandText,
      params object[] parameterValues)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(commandText))
          return 0;
        if (commandText.Contains(" "))
        {
          if (parameterValues == null || parameterValues.Length == 0)
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, commandText);
          SqlParameter[] array = ((IEnumerable<object>) parameterValues).Select((val, ind) => new
          {
            v = val,
            i = ind
          }).Where(c => c.i % 2 == 0).Select(c => new SqlParameter(c.v.ToString(), parameterValues[c.i + 1])).ToArray<SqlParameter>();
          return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, commandText, array);
        }
        SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(commandText);
        if (spParameterSet.Length == 0)
          return SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, commandText);
        SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
        return SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, commandText, spParameterSet);
      }
      catch (Exception ex)
      {
        ex.Log();
        throw ex;
      }
    }

    public static bool ExistsColumn(this DataRow dr, string colName)
    {
      return dr.Table.Columns.Contains(colName);
    }

    public static void Log(ModeLog mode, string title, int uid)
    {
      SqlHelper.Log(mode, title, (string) null, -1, (Dictionary<string, object>) null, (Dictionary<string, object>) null, (string) null, uid);
    }

    public static void Log(ModeLog mode, string title, string tableName, int uid)
    {
      SqlHelper.Log(mode, title, tableName, -1, (Dictionary<string, object>) null, (Dictionary<string, object>) null, (string) null, uid);
    }

    public static void Log(
      ModeLog mode,
      string title,
      string tableName,
      int dataID,
      Dictionary<string, object> dictOldData,
      Dictionary<string, object> dictNewData,
      string callAt,
      int uid,
      SqlTransaction trans = null)
    {
      try
      {
        string str1 = (string) null;
        if (dictOldData != null && dictOldData.Any<KeyValuePair<string, object>>())
          str1 = JsonConvert.SerializeObject((object) dictOldData);
        string str2 = (string) null;
        if (dictNewData != null && dictNewData.Any<KeyValuePair<string, object>>())
          str2 = JsonConvert.SerializeObject((object) dictNewData);
        SqlHelper.ExecuteNonQuery(trans, "sp_SystemLog", (object) "@title", (object) title, (object) "@tableName", (object) tableName, (object) "@dataID", (object) dataID, (object) "@uid", (object) uid, (object) "@oldData", (object) str1, (object) "@newData", (object) str2, (object) "@machineName", (object) Environment.MachineName, (object) "@callAt", (object) callAt, (object) "@mode", (object) mode.ToString());
      }
      catch (Exception ex)
      {
        ex.Log();
      }
    }

    private static Dictionary<string, object> GetDict(List<object> parameter)
    {
      Dictionary<string, object> dict = new Dictionary<string, object>();
      try
      {
        if (parameter == null || parameter.Count == 0)
          return dict;
        for (int index = 0; index < parameter.Count; index += 2)
        {
          if (parameter[index] != null && !dict.ContainsKey(parameter[index].ToString()) && parameter.Count > index + 1)
          {
            string key = parameter[index].ToString();
            if (!key.StartsWith("@"))
              key = "@" + key;
            dict.Add(key, parameter[index + 1]);
          }
        }
      }
      catch
      {
      }
      return dict;
    }

    public static void SaveData(DataTable dtData, int uid)
    {
      SqlHelper.SaveData((SqlTransaction) null, dtData, "", new Dictionary<string, CtrlTyp>(), out List<ImageUpload> _, out List<string> _, uid, true);
    }

    public static DataTable Get_INFORMATION_SCHEMA_Table(string tableName)
    {
      try
      {
        tableName = tableName.ToLower();
        if (WowCommon.Common.DICT_INFORMATION_SCHEMA_TABLE.ContainsKey(tableName))
          return WowCommon.Common.DICT_INFORMATION_SCHEMA_TABLE[tableName];
        WowCommon.Common.DICT_INFORMATION_SCHEMA_TABLE.Remove(tableName);
        WowCommon.Common.DICT_INFORMATION_SCHEMA_TABLE.Add(tableName, SqlHelper.ExecuteDataTable("SELECT COLUMN_NAME , DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TABLE_NAME", (object) "@TABLE_NAME", (object) tableName));
        return WowCommon.Common.DICT_INFORMATION_SCHEMA_TABLE[tableName];
      }
      catch
      {
        return (DataTable) null;
      }
    }

    public static void SaveData(
      SqlTransaction trans,
      DataTable dtData,
      string parentTableName,
      Dictionary<string, CtrlTyp> dictType,
      out List<ImageUpload> listImageUpload,
      out List<string> lstFilepathDelete,
      int uid,
      bool isAPI = false)
    {
      try
      {
        SqlHelper.BaseSaveData(trans, dtData, parentTableName, dictType, out listImageUpload, out lstFilepathDelete, uid, isAPI);
      }
      catch (Exception ex)
      {
        SqlHelperException sqlHelperException = new SqlHelperException(SQLHelperStatusCode.UNDEFINE, ex);
        if (ex.Message.Contains("Violation of PRIMARY KEY") || ex.Message.Contains("Cannot insert duplicate key in object"))
          sqlHelperException = new SqlHelperException(SQLHelperStatusCode.DUPLICATE_PRIMARY_KEY, ex);
        else if (ex.Message.Contains("Cannot insert the value NULL into column") || ex.Message.Contains("column does not allow nulls"))
          sqlHelperException = new SqlHelperException(SQLHelperStatusCode.NOT_ALLOW_NULL, ex);
        throw sqlHelperException;
      }
    }

    private static void BaseSaveData(
      SqlTransaction trans,
      DataTable dtData,
      string parentTableName,
      Dictionary<string, CtrlTyp> dictType,
      out List<ImageUpload> listImageUpload,
      out List<string> lstFilepathDelete,
      int uid,
      bool isAPI = false)
    {
      listImageUpload = new List<ImageUpload>();
      lstFilepathDelete = new List<string>();
      DataTable dataTable1 = dtData;
      if (!dataTable1.Columns.Contains("ID"))
        dataTable1.Columns.Add("ID", typeof (int));
      DataTable informationSchemaTable = SqlHelper.Get_INFORMATION_SCHEMA_Table(dataTable1.TableName);
      if (dataTable1 == null || dataTable1.Rows.Count == 0)
        return;
      foreach (DataRow row in (InternalDataCollectionBase) dataTable1.Rows)
      {
        string format;
        switch (row.RowState)
        {
          case DataRowState.Detached:
          case DataRowState.Deleted:
            format = "DELETE FROM [" + dataTable1.TableName + "] WHERE ID = @ID";
            break;
          case DataRowState.Added:
            format = "INSERT INTO [" + dataTable1.TableName + "]({0}) \nVALUES({1});";
            break;
          case DataRowState.Modified:
            format = "UPDATE [" + dataTable1.TableName + "] \nSET  {0} \nWHERE ID = @ID";
            break;
          default:
            continue;
        }
        List<string> values1 = new List<string>();
        List<string> values2 = new List<string>();
        Dictionary<string, object> dictOldData = new Dictionary<string, object>();
        Dictionary<string, object> dictNewData = new Dictionary<string, object>();
        List<object> source = new List<object>();
        if (row.RowState == DataRowState.Added)
        {
          source.Add((object) "@uidCreate");
          if (isAPI)
          {
            if (uid > 0)
              source.Add((object) uid);
            else if (row.Table.Columns.Contains("_uid"))
              source.Add(row["_uid"]);
            else
              source.Add((object) SqlHelper.UserID);
          }
          else
            source.Add((object) SqlHelper.UserID);
          values1.Add("uidCreate");
          values2.Add("@uidCreate");
          source.Add((object) "@createDate");
          source.Add((object) DateTime.Now);
          values1.Add("createDate");
          values2.Add("@createDate");
        }
        else if (row.RowState == DataRowState.Modified)
        {
          source.Add((object) "@uidModify");
          if (isAPI)
          {
            if (row.Table.Columns.Contains("_uid"))
              source.Add(row["_uid"]);
            else
              source.Add((object) SqlHelper.UserID);
          }
          else
            source.Add((object) SqlHelper.UserID);
          values1.Add("uidModify = @uidModify");
          source.Add((object) "@modifyDate");
          source.Add((object) DateTime.Now);
          values1.Add("modifyDate = @modifyDate");
          source.Add((object) "@ID");
          source.Add(row["ID"]);
        }
        if (isAPI && (row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Detached))
        {
          source.Add((object) "@ID");
          source.Add(row["ID", DataRowVersion.Original]);
        }
        foreach (DataColumn dataColumn in dataTable1.Columns.Cast<DataColumn>().Where<DataColumn>((System.Func<DataColumn, bool>) (col => col.ColumnName != "ID" && !col.ColumnName.EndsWith("_image_tmp_local_path"))))
        {
          DataColumn col = dataColumn;
          DataRow dataRow = informationSchemaTable.AsEnumerable().FirstOrDefault<DataRow>((System.Func<DataRow, bool>) (s => s["COLUMN_NAME"].ToString().ToLower() == col.ColumnName.ToLower()));
          if (dataRow != null)
          {
            string str1 = dataRow["DATA_TYPE"].ToString();
            int int32 = dataRow["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value ? Convert.ToInt32(dataRow["CHARACTER_MAXIMUM_LENGTH"]) : 0;
            object cellValue = row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached || !row.HasVersion(DataRowVersion.Original) ? row[col.ColumnName] : row[col.ColumnName, DataRowVersion.Original];
            if (cellValue == DBNull.Value || cellValue == null || string.IsNullOrEmpty(cellValue.ToString().Trim()))
              cellValue = !(str1 == "nvarchar") || !(dataRow["IS_NULLABLE"].ToString() == "NO") ? (object) DBNull.Value : (object) string.Empty;
            else if (cellValue.GetType() != typeof (DateTime) && (str1 == "datetime" || str1 == "date"))
              cellValue = !cellValue.ToString().Contains("T") ? (object) cellValue.ToDateTimeCommon() : (object) DateTime.ParseExact(cellValue.ToString(), "yyyy-MM-ddTHH:mm", (IFormatProvider) CultureInfo.InvariantCulture);
            else if (str1 != "nvarchar")
            {
              switch (str1)
              {
                case "int":
                  int result1;
                  if (int.TryParse(cellValue.ToString(), out result1))
                  {
                    cellValue = (object) result1;
                    break;
                  }
                  break;
                case "float":
                  double result2;
                  if (double.TryParse(cellValue.ToString(), out result2))
                  {
                    cellValue = (object) result2;
                    break;
                  }
                  break;
              }
            }
            else if (str1 == "nvarchar" && int32 > 0 && cellValue.ToString().Length > int32)
              cellValue = (object) cellValue.ToString().Substring(0, int32 - 1);
            string str2 = col.ColumnName + "_image_name";
            if (row.RowState == DataRowState.Added)
            {
              if (cellValue != DBNull.Value)
              {
                if (dictType.Any<KeyValuePair<string, CtrlTyp>>((System.Func<KeyValuePair<string, CtrlTyp>, bool>) (t =>
                {
                  if (!(t.Key == col.ColumnName))
                    return false;
                  return t.Value == CtrlTyp.Picture || t.Value == CtrlTyp.UploadFile;
                })))
                  listImageUpload.Add(new ImageUpload(row, col.ColumnName));
                else if (!source.Any<object>((System.Func<object, bool>) (o => o != null && o.ToString() == "@" + col.ColumnName)))
                {
                  source.Add((object) ("@" + col.ColumnName));
                  if (str1 == "nvarchar")
                    cellValue = SqlHelper.Custome_Valid(cellValue, col.ColumnName.ToLower());
                  source.Add(cellValue);
                  dictNewData.Add(col.ColumnName, cellValue);
                  values1.Add("[" + col.ColumnName + "]");
                  values2.Add("@" + col.ColumnName);
                }
              }
            }
            else if (row.RowState == DataRowState.Modified)
            {
              if (dictType.Any<KeyValuePair<string, CtrlTyp>>((System.Func<KeyValuePair<string, CtrlTyp>, bool>) (t =>
              {
                if (!(t.Key == col.ColumnName))
                  return false;
                return t.Value == CtrlTyp.Picture || t.Value == CtrlTyp.UploadFile;
              })))
              {
                bool flag = isAPI || !cellValue.Equals(row[col.ColumnName, DataRowVersion.Original]);
                if (!flag)
                {
                  string str3 = col.ColumnName + "_file_tmp_modifyDate";
                  if (row.Table.Columns.Contains(str3) && row[str3] != DBNull.Value && (row[str3, DataRowVersion.Original] == DBNull.Value || Convert.ToDateTime(row[str3, DataRowVersion.Original]) != Convert.ToDateTime(row[str3])))
                    flag = true;
                }
                if (!flag)
                {
                  string str4 = col.ColumnName + "_image_name";
                  if (row.Table.Columns.Contains(str4) && row[str4] != DBNull.Value && (row[str4, DataRowVersion.Original] == DBNull.Value || Convert.ToString(row[str4, DataRowVersion.Original]) != Convert.ToString(row[str4])))
                    flag = true;
                }
                if (flag)
                  listImageUpload.Add(new ImageUpload(row, col.ColumnName));
              }
              else if ((isAPI || !cellValue.Equals(row[col.ColumnName, DataRowVersion.Original])) && !source.Any<object>((System.Func<object, bool>) (o => o != null && o.ToString() == "@" + col.ColumnName)))
              {
                source.Add((object) ("@" + col.ColumnName));
                if (str1 == "nvarchar")
                  cellValue = SqlHelper.Custome_Valid(cellValue, col.ColumnName.ToLower());
                source.Add(cellValue);
                dictOldData.Add(col.ColumnName, row[col.ColumnName, DataRowVersion.Original]);
                dictNewData.Add(col.ColumnName, cellValue);
                values1.Add("[" + col.ColumnName + "] = @" + col.ColumnName);
              }
            }
            else if ((row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Detached) && dictType.Any<KeyValuePair<string, CtrlTyp>>((System.Func<KeyValuePair<string, CtrlTyp>, bool>) (t =>
            {
              if (!(t.Key == col.ColumnName))
                return false;
              return t.Value == CtrlTyp.Picture || t.Value == CtrlTyp.UploadFile;
            })) && row[col.ColumnName, DataRowVersion.Original] != DBNull.Value && row[col.ColumnName, DataRowVersion.Original].ToString() != string.Empty)
            {
              string str5 = col.ColumnName + "_image_path";
              if (row.Table.Columns.Contains(str5))
                lstFilepathDelete.Add(row[str5, DataRowVersion.Original].ToString());
              string columnName = col.ColumnName + "_small_path";
              if (row.Table.Columns.Contains(str5))
                lstFilepathDelete.Add(row[columnName, DataRowVersion.Original].ToString());
            }
          }
        }
        DataTable dataTable2 = (DataTable) null;
        if (source.Count > 1)
        {
          if (!source.Contains((object) "@ID") && row.RowState != DataRowState.Added)
            throw new Exception("@ID is not exists in query");
          if (row.RowState != DataRowState.Modified || (source.Contains((object) "@ID") || source.Contains((object) "@uidModify") || source.Contains((object) "@modifyDate")) && source.Count >= 7)
          {
            string commandText = string.Format(format, (object) string.Join(", ", (IEnumerable<string>) values1), (object) string.Join(", ", (IEnumerable<string>) values2));
            if (trans != null)
              SqlHelper.ExecuteNonQuery(trans, commandText, source.ToArray());
            else
              SqlHelper.ExecuteNonQuery(commandText, source.ToArray());
            if (row.RowState == DataRowState.Added)
            {
              string spName = "SELECT TOP(1) ID, refID FROM " + dataTable1.TableName + " ORDER BY ID DESC";
              dataTable2 = trans == null ? SqlHelper.ExecuteDataTable(spName) : SqlHelper.ExecuteDataTable(trans, spName);
            }
          }
          else
            continue;
        }
        int dataID = 0;
        if (row.RowState == DataRowState.Added)
          dataID = int.Parse(dataTable2.Rows[0]["ID"].ToString());
        else if (row.Table.Columns.Contains("ID") && row.RowState != DataRowState.Detached && row.RowState != DataRowState.Deleted)
          dataID = int.Parse(row["ID"].ToString());
        if (row.RowState == DataRowState.Added && row.Table.Columns.Contains("ID"))
          row["ID"] = (object) dataID;
        SqlHelper.Log(ModeLog.Winform, row.RowState.ToString(), dataTable1.TableName, dataID, dictOldData, dictNewData, "onSaveData", uid, trans);
      }
    }

    private static object Custome_Valid(object cellValue, string fieldNameValid)
    {
      if (cellValue == null || string.IsNullOrEmpty(cellValue.ToString()))
        return cellValue;
      if (fieldNameValid.Contains("password"))
        cellValue = (object) EnDe.EncryptText(cellValue.ToString());
      else if (fieldNameValid.Contains("phone") || fieldNameValid.Contains("sdt"))
      {
        if (!WowCommon.Common.ValidPhoneNumber(cellValue.ToString()))
          throw new Exception("Số điện thoại [" + cellValue.ToString() + "] không đúng định dạng.");
      }
      else if (fieldNameValid.Contains("email") && !WowCommon.Common.IsValidEmail(cellValue.ToString()))
        throw new Exception("Email [" + cellValue.ToString() + "] không đúng định dạng.");
      return cellValue;
    }

    public static bool DeleteFile(string fullpath)
    {
      try
      {
        if (File.Exists(fullpath))
          File.Delete(fullpath);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public static bool CreateDirectory(string dir)
    {
      try
      {
        if (!Directory.Exists(dir))
          Directory.CreateDirectory(dir);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public static async Task UploadImage(
      List<ImageUpload> listImageUpload,
      string tableName,
      string parentTableName,
      int uid)
    {
      foreach (ImageUpload imageUpload in listImageUpload)
      {
        ImageUpload item = imageUpload;
        string fieldName = item.ColumnName;
        DataRow dataRow = item.DataRow;
        int id = int.Parse(dataRow["ID"].ToString());
        int result = -1;
        int.TryParse(dataRow["refID"].ToString(), out result);
        string tempPathLocal = dataRow[fieldName + "_image_tmp_local_path"]?.ToString();
        string fileName = dataRow[fieldName + "_image_name"]?.ToString();
        if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(tempPathLocal))
        {
          if (!dataRow.HasVersion(DataRowVersion.Original))
            break;
          string filePath = dataRow[fieldName + "_image_path", DataRowVersion.Original]?.ToString();
          string small_pathOgn = dataRow[fieldName + "_small_path", DataRowVersion.Original]?.ToString();
          if (string.IsNullOrEmpty(filePath))
            break;
          string str1 = await WowCommon.Common.DeleteFile(filePath, uid);
          if (!string.IsNullOrEmpty(small_pathOgn))
          {
            string str2 = await WowCommon.Common.DeleteFile(small_pathOgn, uid);
          }
          SqlHelper.ExecuteNonQuery(string.Format("UPDATE [" + tableName + "] SET [" + fieldName + "_image_path] = null, [" + fieldName + "_small_path] = null WHERE [ID] =  " + id.ToString(), (object) tableName));
          break;
        }
        if (string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(tempPathLocal))
          fileName = new FileInfo(tempPathLocal).Name;
        int upID = string.IsNullOrEmpty(parentTableName) ? id : result;
        string image_path = WowCommon.Common.GetFullURLFileHostingSQL(parentTableName, tableName, upID, fileName);
        FileInfo fileInfo = new FileInfo(fileName);
        dataRow[fieldName + "_image_path"] = (object) image_path;
        if (dataRow.HasVersion(DataRowVersion.Original))
        {
          string path = dataRow[fieldName + "_image_path", DataRowVersion.Original]?.ToString();
          if (!string.IsNullOrEmpty(path) && File.Exists(path))
            SqlHelper.DeleteFile(Application.StartupPath + "\\" + path);
        }
        string rs;
        if (WowCommon.Common.EXT_PHOTO.Any<string>((System.Func<string, bool>) (ext => fileName.ToLower().EndsWith(ext))))
        {
          rs = await WowCommon.Common.UploadImage_Zip(tempPathLocal, parentTableName, tableName, upID, fileName, fieldName + "_image_path", id, uid);
          await SqlHelper.UploadImage_SmallPath(item, tableName, parentTableName, uid);
        }
        else
        {
          rs = await WowCommon.Common.UploadImage(tempPathLocal, parentTableName, tableName, upID, fileName, fieldName + "_image_path", id, uid);
          if (!rs.StartsWith("[Success]"))
            throw new Exception(rs);
        }
        SqlHelper.ExecuteNonQuery(string.Format("UPDATE [" + tableName + "] SET [" + fieldName + "_image_path] = @image_path  WHERE [ID] =  " + id.ToString(), (object) tableName), (object) "@image_path", (object) image_path);
        if (WowCommon.Common.MOVE_FILE_AFTER_UPLOAD)
        {
          string str = Application.StartupPath + (image_path.StartsWith("/") ? "" : "/") + image_path;
          if (!File.Exists(str))
          {
            WowCommon.Common.CreateDirectory(str, true);
            File.Copy(tempPathLocal, str);
          }
        }
        string tableName1 = tableName;
        int dataID = upID;
        Dictionary<string, object> dictNewData = new Dictionary<string, object>();
        dictNewData.Add("image_path", (object) image_path);
        dictNewData.Add("reponse", (object) rs);
        int uid1 = uid;
        SqlHelper.Log(ModeLog.Winform, "Upload File", tableName1, dataID, (Dictionary<string, object>) null, dictNewData, nameof (UploadImage), uid1);
        fieldName = (string) null;
        tempPathLocal = (string) null;
        image_path = (string) null;
        rs = (string) null;
        item = (ImageUpload) null;
      }
    }

    public static async Task UploadImage_SmallPath(
      ImageUpload item,
      string tableName,
      string parentTableName,
      int uid,
      string extFieldName = "small_path",
      int width = 75,
      int height = 75)
    {
      string fieldName = item.ColumnName;
      if (fieldName.ToLower().Contains("chuky"))
      {
        width = 300;
        height = 100;
      }
      DataRow dataRow = item.DataRow;
      int id = int.Parse(dataRow["ID"].ToString());
      int result = -1;
      int.TryParse(dataRow["refID"].ToString(), out result);
      string tempPathImage = dataRow[fieldName + "_image_tmp_local_path"].ToString();
      string fileName = dataRow[fieldName + "_image_name"].ToString();
      if (string.IsNullOrEmpty(fileName))
        fileName = new FileInfo(dataRow[fieldName + "_image_tmp_local_path"].ToString()).Name;
      string hostingURL;
      string tempPathLocal_small;
      if (!WowCommon.Common.EXT_PHOTO.Any<string>((System.Func<string, bool>) (ext => fileName.ToLower().EndsWith(ext))))
      {
        fieldName = (string) null;
        hostingURL = (string) null;
        tempPathLocal_small = (string) null;
      }
      else
      {
        int upID = string.IsNullOrEmpty(parentTableName) ? id : result;
        FileInfo fileInfo = new FileInfo(fileName);
        hostingURL = WowCommon.Common.GetFullURLFileHostingSQL(parentTableName, tableName, upID, fileInfo.Name.Replace(fileInfo.Extension, "_" + extFieldName.Replace("_path", "") + fileInfo.Extension));
        string name = new FileInfo(hostingURL).Name;
        dataRow[fieldName + "_" + extFieldName] = (object) hostingURL;
        tempPathLocal_small = SqlHelper.ResizeImage(tempPathImage, width, height);
        if (string.IsNullOrEmpty(tempPathLocal_small))
        {
          fieldName = (string) null;
          hostingURL = (string) null;
          tempPathLocal_small = (string) null;
        }
        else if (!File.Exists(tempPathLocal_small))
        {
          fieldName = (string) null;
          hostingURL = (string) null;
          tempPathLocal_small = (string) null;
        }
        else
        {
          string str = await WowCommon.Common.UploadImage_Zip(tempPathLocal_small, parentTableName, tableName, upID, name, fieldName + "_" + extFieldName, id, uid);
          SqlHelper.DeleteFile(tempPathLocal_small);
          SqlHelper.ExecuteNonQuery("UPDATE [" + tableName + "] SET  [" + fieldName + "_" + extFieldName + "] = @newURL  WHERE [ID] = " + id.ToString(), (object) "@newURL", (object) hostingURL);
          string title = "Upload SmallFile :" + extFieldName;
          string tableName1 = tableName;
          int dataID = upID;
          Dictionary<string, object> dictNewData = new Dictionary<string, object>();
          dictNewData.Add(fieldName + "_" + extFieldName, (object) hostingURL);
          dictNewData.Add("reponse", (object) str);
          int uid1 = uid;
          SqlHelper.Log(ModeLog.Winform, title, tableName1, dataID, (Dictionary<string, object>) null, dictNewData, "UploadImage", uid1);
          fieldName = (string) null;
          hostingURL = (string) null;
          tempPathLocal_small = (string) null;
        }
      }
    }

    public static string ResizeImage(string tempPathImage, int width, int height, string direc = "")
    {
      FileInfo fileInfo = new FileInfo(tempPathImage);
      string str = (string.IsNullOrEmpty(direc) ? "tmp_Photo/" : direc) + fileInfo.Name.Replace(fileInfo.Extension, "_" + width.ToString() + "x" + height.ToString() + fileInfo.Extension);
      SqlHelper.DeleteFile(str);
      SqlHelper.CreateDirectory(new FileInfo(str).DirectoryName);
      using (Bitmap bitmap1 = new Bitmap(tempPathImage))
      {
        using (Bitmap bitmap2 = new Bitmap(width, height))
        {
          bitmap2.SetResolution(bitmap1.HorizontalResolution, bitmap1.VerticalResolution);
          using (Graphics graphics = Graphics.FromImage((Image) bitmap2))
          {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            using (ImageAttributes imageAttr = new ImageAttributes())
            {
              imageAttr.SetWrapMode(WrapMode.TileFlipXY);
              graphics.DrawImage((Image) bitmap1, new Rectangle(0, 0, width, height), 0, 0, bitmap1.Width, bitmap1.Height, GraphicsUnit.Pixel, imageAttr);
            }
          }
          bitmap2.Save(str);
          bitmap2.Dispose();
        }
      }
      return str;
    }

    public static async Task DeleteImage(List<string> lstFilepathDelete, int uid)
    {
      foreach (string filePath in lstFilepathDelete)
      {
        string str = await WowCommon.Common.DeleteFile(filePath, uid);
      }
    }

    public static void ReMapDataBase(
      string strServer,
      string strDatabase,
      string strUserName,
      string strPassword)
    {
      SqlHelper.m_strServer = strServer;
      SqlHelper.m_strDatabase = strDatabase;
      SqlHelper.m_strUser = strUserName;
      SqlHelper.m_strPassword = strPassword;
      SqlHelper.SetConnection(strServer, strDatabase, strUserName, strPassword);
    }

    public static int ExecuteNonQuery(
      SqlConnection connection,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[]) null);
    }

    public static int ExecuteNonQuery(
      SqlConnection connection,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      try
      {
        SqlCommand sqlCommand1 = new SqlCommand();
        sqlCommand1.CommandTimeout = 120;
        SqlCommand sqlCommand2 = sqlCommand1;
        bool mustCloseConnection;
        SqlHelper.PrepareCommand(sqlCommand2, connection, (SqlTransaction) null, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, out mustCloseConnection);
        int num = sqlCommand2.ExecuteNonQuery();
        SqlHelper.SystemTraceLog(connection, sqlCommand2);
        sqlCommand2.Parameters.Clear();
        if (mustCloseConnection)
          connection.Close();
        SqlHelper.ProcessNotifyData(sqlCommand2);
        return num;
      }
      catch (Exception ex)
      {
        ex.Log();
        throw ex;
      }
    }

    private static void SystemTraceLog(SqlConnection connection, SqlCommand cmdLog)
    {
      try
      {
        if (!WowCommon.Common.WRITE_LOG_SQL || cmdLog.CommandText.ToLower() == "sp_systemlog")
          return;
        using (SqlConnection connection1 = new SqlConnection(SqlHelper.connectionString))
        {
          if (connection1.State != ConnectionState.Open)
            connection1.Open();
          SqlCommand sqlCommand = new SqlCommand("sp_SystemLog", connection1);
          sqlCommand.CommandType = CommandType.StoredProcedure;
          SqlCommand cmd = sqlCommand;
          int num = SqlHelper.UserID;
          List<string> values = new List<string>();
          if (cmdLog.Parameters != null && cmdLog.Parameters.Count > 0)
          {
            foreach (SqlParameter sqlParameter in cmdLog.Parameters.Cast<SqlParameter>())
            {
              object obj1 = sqlParameter.Value;
              if (obj1 != null && obj1 != DBNull.Value)
              {
                bool flag = false;
                object obj2;
                switch (sqlParameter.SqlDbType)
                {
                  case SqlDbType.Bit:
                    obj2 = (object) (Convert.ToBoolean(obj1) ? 1 : 0);
                    break;
                  case SqlDbType.DateTime:
                  case SqlDbType.SmallDateTime:
                  case SqlDbType.DateTime2:
                    obj2 = (object) Convert.ToDateTime(obj1).ToString("yyyyMMdd HH:mm:ss");
                    break;
                  case SqlDbType.Decimal:
                  case SqlDbType.Float:
                  case SqlDbType.Money:
                  case SqlDbType.Real:
                    flag = true;
                    obj2 = (object) Convert.ToDecimal(obj1);
                    break;
                  case SqlDbType.Int:
                  case SqlDbType.SmallInt:
                  case SqlDbType.SmallMoney:
                  case SqlDbType.TinyInt:
                    flag = true;
                    int int32 = Convert.ToInt32(obj1);
                    if (sqlParameter.ParameterName == "@uid")
                      num = int32;
                    obj2 = (object) int32;
                    break;
                  case SqlDbType.Date:
                    obj2 = (object) Convert.ToDateTime(obj1).ToString("yyyyMMdd");
                    break;
                  default:
                    obj2 = obj1;
                    break;
                }
                if (flag)
                  values.Add(sqlParameter.ParameterName + " = " + obj2.ToString());
                else
                  values.Add(sqlParameter.ParameterName + " = N'" + obj2?.ToString() + "'");
              }
              else
                values.Add(sqlParameter.ParameterName + " = null");
            }
          }
          Dictionary<string, object> pars = new Dictionary<string, object>()
          {
            {
              "@title",
              (object) "SQLHelper.Log"
            },
            {
              "@uid",
              (object) (WowCommon.Common.Mode == ModeLog.Winform ? SqlHelper.UserID : num)
            },
            {
              "@machineName",
              (object) Environment.MachineName
            },
            {
              "@callAt",
              (object) nameof (SystemTraceLog)
            },
            {
              "@parameter",
              cmdLog.CommandText.ToLower().StartsWith("select ") ? (object) cmdLog.CommandText : (object) ("EXEC  " + cmdLog.CommandText + " " + string.Join(",", (IEnumerable<string>) values))
            },
            {
              "@mode",
              (object) WowCommon.Common.Mode.ToString()
            }
          };
          pars.Keys.All<string>((System.Func<string, bool>) (key =>
          {
            cmd.Parameters.Add(new SqlParameter(key, pars[key]));
            return true;
          }));
          cmd.ExecuteNonQuery();
          cmd.Parameters.Clear();
          if (connection1.State != ConnectionState.Closed)
            connection1.Close();
          connection1.Dispose();
        }
      }
      catch (Exception ex)
      {
        ex.Log();
      }
    }

    public static void ProcessNotifyData(SqlCommand cmd)
    {
    }

    public static int ExecuteNonQuery(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteNonQuery(transaction, commandType, commandText, (SqlParameter[]) null);
    }

    public static int ExecuteNonQuery(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      try
      {
        if (transaction == null)
          throw new ArgumentNullException(nameof (transaction));
        if (transaction != null && transaction.Connection == null)
          throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
        SqlCommand sqlCommand1 = new SqlCommand();
        sqlCommand1.CommandTimeout = 120;
        SqlCommand sqlCommand2 = sqlCommand1;
        SqlHelper.PrepareCommand(sqlCommand2, transaction.Connection, transaction, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, out bool _);
        int num = sqlCommand2.ExecuteNonQuery();
        SqlHelper.SystemTraceLog(transaction.Connection, sqlCommand2);
        sqlCommand2.Parameters.Clear();
        return num;
      }
      catch (Exception ex)
      {
        ex.Log();
        throw ex;
      }
    }

    public static int ExecuteNonQuery(
      SqlTransaction transaction,
      string commandText,
      params object[] parameterValues)
    {
      if (transaction == null)
        return SqlHelper.ExecuteNonQuery(commandText, parameterValues);
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      try
      {
        if (string.IsNullOrWhiteSpace(commandText))
          return 0;
        if (commandText.Contains(" "))
        {
          if (parameterValues == null || parameterValues.Length == 0)
            return SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, commandText);
          SqlParameter[] array = ((IEnumerable<object>) parameterValues).Select((val, ind) => new
          {
            v = val,
            i = ind
          }).Where(c => c.i % 2 == 0).Select(c => new SqlParameter(c.v.ToString(), parameterValues[c.i + 1])).ToArray<SqlParameter>();
          return SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, commandText, array);
        }
        SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(commandText);
        if (spParameterSet.Length == 0)
          return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, commandText);
        SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
        return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, commandText, spParameterSet);
      }
      catch (Exception ex)
      {
        ex.Log();
        throw ex;
      }
    }

    public static async Task<int> ExecuteNonQueryAsync(
      string connectionString,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      int num;
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync().ConfigureAwait(false);
        num = await SqlHelper.ExecuteNonQueryAsync(connection, commandType, commandText, commandParameters).ConfigureAwait(false);
      }
      return num;
    }

    public static Task<int> ExecuteNonQueryAsync(string spName, params object[] parameterValues)
    {
      if (spName.Contains(" ") && parameterValues.Length == 0)
        return SqlHelper.ExecuteNonQueryAsync(SqlHelper.connectionString, CommandType.Text, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteNonQueryAsync(SqlHelper.connectionString, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteNonQueryAsync(SqlHelper.connectionString, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<int> ExecuteNonQueryAsync(
      SqlConnection connection,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteNonQueryAsync(connection, commandType, commandText, (SqlParameter[]) null);
    }

    public static async Task<int> ExecuteNonQueryAsync(
      SqlConnection connection,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      int num1;
      try
      {
        SqlCommand sqlCommand = new SqlCommand();
        sqlCommand.CommandTimeout = 120;
        SqlCommand cmd = sqlCommand;
        bool mustCloseConnection = await SqlHelper.PrepareCommandAsync(cmd, connection, (SqlTransaction) null, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters).ConfigureAwait(false);
        int num2 = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
        SqlHelper.SystemTraceLog(connection, cmd);
        cmd.Parameters.Clear();
        if (mustCloseConnection)
          connection.Close();
        num1 = num2;
      }
      catch (Exception ex)
      {
        ex.Log();
        throw ex;
      }
      return num1;
    }

    public static Task<int> ExecuteNonQueryAsync(
      SqlConnection connection,
      string spName,
      params object[] parameterValues)
    {
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteNonQueryAsync(connection, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteNonQueryAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<int> ExecuteNonQueryAsync(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteNonQueryAsync(transaction, commandType, commandText, (SqlParameter[]) null);
    }

    public static async Task<int> ExecuteNonQueryAsync(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      int num1;
      try
      {
        if (transaction == null)
          throw new ArgumentNullException(nameof (transaction));
        if (transaction != null && transaction.Connection == null)
          throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
        SqlCommand sqlCommand = new SqlCommand();
        sqlCommand.CommandTimeout = 120;
        SqlCommand cmd = sqlCommand;
        int num2 = await SqlHelper.PrepareCommandAsync(cmd, transaction.Connection, transaction, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters).ConfigureAwait(false) ? 1 : 0;
        int num3 = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
        SqlHelper.SystemTraceLog(transaction.Connection, cmd);
        cmd.Parameters.Clear();
        num1 = num3;
      }
      catch (Exception ex)
      {
        ex.Log();
        throw ex;
      }
      return num1;
    }

    public static Task<int> ExecuteNonQueryAsync(
      SqlTransaction transaction,
      string spName,
      params object[] parameterValues)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteNonQueryAsync(transaction, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteNonQueryAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static DataSet ExecuteDataset(
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      using (SqlConnection connection = new SqlConnection(SqlHelper.connectionString))
      {
        connection.Open();
        return SqlHelper.ExecuteDataset(connection, commandType, commandText, commandParameters);
      }
    }

    public static DataSet ExecuteDataset(string spName, params object[] parameterValues)
    {
      if (spName.Contains(" ") && parameterValues.Length == 0)
      {
        if (parameterValues == null || parameterValues.Length == 0)
          return SqlHelper.ExecuteDataset(CommandType.Text, spName);
        SqlParameter[] array = ((IEnumerable<object>) parameterValues).Select((val, ind) => new
        {
          v = val,
          i = ind
        }).Where(c => c.i % 2 == 0).Select(c => new SqlParameter(c.v.ToString(), parameterValues[c.i + 1])).ToArray<SqlParameter>();
        return SqlHelper.ExecuteDataset(CommandType.Text, spName, array);
      }
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteDataset(CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteDataset(CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<DataSet> ExecuteDatasetAsync(CommandType commandType, string commandText)
    {
      return SqlHelper.ExecuteDatasetAsync(SqlHelper.connectionString, commandType, commandText, (SqlParameter[]) null);
    }

    public static DataTable ExecuteDataTable(string spName, params object[] parameterValues)
    {
      if (spName != null && spName.Contains(" "))
      {
        if (parameterValues == null || parameterValues.Length == 0)
        {
          DataSet dataSet = SqlHelper.ExecuteDataset(CommandType.Text, spName);
          return dataSet != null && dataSet.Tables.Count != 0 ? dataSet.Tables[0] : (DataTable) null;
        }
        SqlParameter[] array = ((IEnumerable<object>) parameterValues).Select((val, ind) => new
        {
          v = val,
          i = ind
        }).Where(c => c.i % 2 == 0).Select(c => new SqlParameter(c.v.ToString(), parameterValues[c.i + 1])).ToArray<SqlParameter>();
        return SqlHelper.ExecuteDataset(CommandType.Text, spName, array).Tables[0];
      }
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length != 0)
      {
        SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
        using (DataSet dataSet = SqlHelper.ExecuteDataset(CommandType.StoredProcedure, spName, spParameterSet))
          return dataSet == null || dataSet.Tables.Count <= 0 ? (DataTable) null : dataSet.Tables[0];
      }
      else
      {
        using (DataSet dataSet = SqlHelper.ExecuteDataset(CommandType.StoredProcedure, spName))
          return dataSet == null || dataSet.Tables.Count <= 0 ? (DataTable) null : dataSet.Tables[0];
      }
    }

    public static DataTable ExecuteDataTable_CORE(string spName, params object[] parameterValues)
    {
      if (spName != null && spName.Contains(" "))
      {
        if (parameterValues == null || parameterValues.Length == 0)
        {
          DataSet dataSet = SqlHelper.ExecuteDataset(CommandType.Text, spName);
          return dataSet != null && dataSet.Tables.Count != 0 ? dataSet.Tables[0] : (DataTable) null;
        }
        SqlParameter[] array = ((IEnumerable<object>) parameterValues).Select((val, ind) => new
        {
          v = val,
          i = ind
        }).Where(c => c.i % 2 == 0).Select(c => new SqlParameter(c.v.ToString(), parameterValues[c.i + 1])).ToArray<SqlParameter>();
        return SqlHelper.ExecuteDataset(CommandType.Text, spName, array).Tables[0];
      }
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length != 0)
      {
        SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
        using (DataSet dataSet = SqlHelper.ExecuteDataset(CommandType.StoredProcedure, spName, spParameterSet))
          return dataSet == null || dataSet.Tables.Count <= 0 ? (DataTable) null : dataSet.Tables[0];
      }
      else
      {
        using (DataSet dataSet = SqlHelper.ExecuteDataset(CommandType.StoredProcedure, spName))
          return dataSet == null || dataSet.Tables.Count <= 0 ? (DataTable) null : dataSet.Tables[0];
      }
    }

    public static bool CheckExistsProcedureName(string procedureName)
    {
      DataTable dataTable = SqlHelper.ExecuteDataTable("select 1 from sys.objects where name =  @Procname and type_desc like '%proce%'", (object) "@Procname", (object) procedureName);
      return dataTable != null && dataTable.Rows.Count > 0;
    }

    public static DataTable ExecuteDataTable(
      SqlTransaction transaction,
      string spName,
      params object[] parameterValues)
    {
      if (spName.Contains(" ") && parameterValues.Length == 0)
        return SqlHelper.ExecuteDataset(transaction, CommandType.Text, spName).Tables[0];
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length != 0)
      {
        SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
        using (DataSet dataSet = SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameterSet))
          return dataSet == null || dataSet.Tables.Count <= 0 ? (DataTable) null : dataSet.Tables[0];
      }
      else
      {
        using (DataSet dataSet = SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName))
          return dataSet == null || dataSet.Tables.Count <= 0 ? (DataTable) null : dataSet.Tables[0];
      }
    }

    public static DataSet ExecuteDataset(
      SqlConnection connection,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteDataset(connection, commandType, commandText, (SqlParameter[]) null);
    }

    public static DataSet ExecuteDataset(
      SqlConnection connection,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      try
      {
        SqlCommand sqlCommand1 = new SqlCommand();
        sqlCommand1.CommandTimeout = 120;
        SqlCommand sqlCommand2 = sqlCommand1;
        bool mustCloseConnection;
        SqlHelper.PrepareCommand(sqlCommand2, connection, (SqlTransaction) null, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, out mustCloseConnection);
        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand2))
        {
          DataSet dataSet = new DataSet();
          sqlDataAdapter.Fill(dataSet);
          SqlHelper.SystemTraceLog(connection, sqlCommand2);
          sqlCommand2.Parameters.Clear();
          if (mustCloseConnection)
            connection.Close();
          return dataSet;
        }
      }
      catch (Exception ex)
      {
        ex.Log();
        throw new Exception(ex.Message + "\nCommandText: " + commandText);
      }
    }

    public static DataSet ExecuteDataset(
      SqlConnection connection,
      string spName,
      params object[] parameterValues)
    {
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static DataSet ExecuteDataset(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteDataset(transaction, commandType, commandText, (SqlParameter[]) null);
    }

    public static DataSet ExecuteDataset(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      try
      {
        if (transaction == null)
          throw new ArgumentNullException(nameof (transaction));
        if (transaction != null && transaction.Connection == null)
          throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
        SqlCommand sqlCommand1 = new SqlCommand();
        sqlCommand1.CommandTimeout = 120;
        SqlCommand sqlCommand2 = sqlCommand1;
        SqlHelper.PrepareCommand(sqlCommand2, transaction.Connection, transaction, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, out bool _);
        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand2))
        {
          DataSet dataSet = new DataSet();
          sqlDataAdapter.Fill(dataSet);
          SqlHelper.SystemTraceLog(transaction.Connection, sqlCommand2);
          sqlCommand2.Parameters.Clear();
          return dataSet;
        }
      }
      catch (Exception ex)
      {
        ex.Log();
        throw ex;
      }
    }

    public static DataSet ExecuteDataset(
      SqlTransaction transaction,
      string spName,
      params object[] parameterValues)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<DataSet> ExecuteDatasetAsync(
      string connectionString,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteDatasetAsync(connectionString, commandType, commandText, (SqlParameter[]) null);
    }

    public static async Task<DataSet> ExecuteDatasetAsync(
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      return await SqlHelper.ExecuteDatasetAsync(SqlHelper.connectionString, commandType, commandText, commandParameters);
    }

    public static async Task<DataSet> ExecuteDatasetAsync(
      string connectionString,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      SqlConnection connection;
      int num;
      Exception ex1;

      try
      {
        connection = new SqlConnection(connectionString);
        try
        {
          await connection.OpenAsync().ConfigureAwait(false);
          return await SqlHelper.ExecuteDatasetAsync(connection, commandType, commandText, commandParameters).ConfigureAwait(false);
        }
        finally
        {
          connection?.Dispose();
        }
      }
      catch (Exception ex)
      {
        num = 1;
        ex1 = ex;
      }
      if (num != 1)
      {
        DataSet dataSet = new DataSet();
        return dataSet;
      }
      
      ex1.Log();
      if (!ex1.Message.Contains(" is not a parameter") && !ex1.Message.Contains("expects p"))
        throw ex1;
      connection = new SqlConnection(connectionString);
      try
      {
        await connection.OpenAsync().ConfigureAwait(false);
        return await SqlHelper.ExecuteDatasetAsync(connection, commandType, commandText, commandParameters).ConfigureAwait(false);
      }
      finally
      {
        connection?.Dispose();
      }
    }

    public static Task<DataSet> ExecuteDatasetAsync(string spName, params object[] parameterValues)
    {
      Thread.Sleep(10000);
      if (spName.Contains(" ") && parameterValues.Length == 0)
        return SqlHelper.ExecuteDatasetAsync(SqlHelper.connectionString, CommandType.Text, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteDatasetAsync(SqlHelper.connectionString, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteDatasetAsync(SqlHelper.connectionString, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<DataSet> ExecuteDatasetAsync(
      SqlConnection connection,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteDatasetAsync(connection, commandType, commandText, (SqlParameter[]) null);
    }

    public static async Task<DataSet> ExecuteDatasetAsync(
      SqlConnection connection,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      DataSet dataSet1;
      try
      {
        SqlCommand sqlCommand = new SqlCommand();
        sqlCommand.CommandTimeout = 120;
        SqlCommand cmd = sqlCommand;
        bool flag = await SqlHelper.PrepareCommandAsync(cmd, connection, (SqlTransaction) null, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters).ConfigureAwait(false);
        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
        {
          DataSet dataSet2 = new DataSet();
          sqlDataAdapter.Fill(dataSet2);
          SqlHelper.SystemTraceLog(connection, cmd);
          cmd.Parameters.Clear();
          if (flag)
            connection.Close();
          dataSet1 = dataSet2;
        }
      }
      catch (Exception ex)
      {
        ex.Log();
        throw ex;
      }
      return dataSet1;
    }

    public static Task<DataSet> ExecuteDatasetAsync(
      SqlConnection connection,
      string spName,
      params object[] parameterValues)
    {
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteDatasetAsync(connection, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteDatasetAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<DataSet> ExecuteDatasetAsync(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteDatasetAsync(transaction, commandType, commandText, (SqlParameter[]) null);
    }

    public static async Task<DataSet> ExecuteDatasetAsync(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      DataSet dataSet1;
      try
      {
        if (transaction == null)
          throw new ArgumentNullException(nameof (transaction));
        if (transaction != null && transaction.Connection == null)
          throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
        SqlCommand sqlCommand = new SqlCommand();
        sqlCommand.CommandTimeout = 120;
        SqlCommand cmd = sqlCommand;
        int num = await SqlHelper.PrepareCommandAsync(cmd, transaction.Connection, transaction, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters).ConfigureAwait(false) ? 1 : 0;
        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
        {
          DataSet dataSet2 = new DataSet();
          sqlDataAdapter.Fill(dataSet2);
          SqlHelper.SystemTraceLog(transaction.Connection, cmd);
          cmd.Parameters.Clear();
          dataSet1 = dataSet2;
        }
      }
      catch (Exception ex)
      {
        ex.Log();
        throw ex;
      }
      return dataSet1;
    }

    public static Task<DataSet> ExecuteDatasetAsync(
      SqlTransaction transaction,
      string spName,
      params object[] parameterValues)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteDatasetAsync(transaction, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteDatasetAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    private static SqlDataReader ExecuteReader(
      SqlConnection connection,
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      IEnumerable<SqlParameter> commandParameters,
      SqlHelper.SqlConnectionOwnership connectionOwnership)
    {
      bool mustCloseConnection = false;
      SqlCommand sqlCommand1 = new SqlCommand();
      sqlCommand1.CommandTimeout = 120;
      SqlCommand sqlCommand2 = sqlCommand1;
      try
      {
        SqlHelper.PrepareCommand(sqlCommand2, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
        SqlDataReader sqlDataReader = connectionOwnership == SqlHelper.SqlConnectionOwnership.External ? sqlCommand2.ExecuteReader() : sqlCommand2.ExecuteReader(CommandBehavior.CloseConnection);
        SqlHelper.SystemTraceLog(connection, sqlCommand2);
        bool flag = true;
        foreach (DbParameter parameter in (DbParameterCollection) sqlCommand2.Parameters)
        {
          if (parameter.Direction != ParameterDirection.Input)
            flag = false;
        }
        if (flag)
          sqlCommand2.Parameters.Clear();
        return sqlDataReader;
      }
      catch
      {
        if (mustCloseConnection)
          connection.Close();
        throw;
      }
    }

    public static SqlDataReader ExecuteReader(
      string connectionString,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteReader(connectionString, commandType, commandText, (SqlParameter[]) null);
    }

    public static SqlDataReader ExecuteReader(
      string connectionString,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      SqlConnection connection = (SqlConnection) null;
      try
      {
        connection = new SqlConnection(connectionString);
        connection.Open();
        return SqlHelper.ExecuteReader(connection, (SqlTransaction) null, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, SqlHelper.SqlConnectionOwnership.Internal);
      }
      catch
      {
        connection?.Close();
        throw;
      }
    }

    public static SqlDataReader ExecuteReader(
      string connectionString,
      string spName,
      params object[] parameterValues)
    {
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static SqlDataReader ExecuteReader(
      SqlConnection connection,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteReader(connection, commandType, commandText, (SqlParameter[]) null);
    }

    public static SqlDataReader ExecuteReader(
      SqlConnection connection,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      return SqlHelper.ExecuteReader(connection, (SqlTransaction) null, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, SqlHelper.SqlConnectionOwnership.External);
    }

    public static SqlDataReader ExecuteReader(
      SqlConnection connection,
      string spName,
      params object[] parameterValues)
    {
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static SqlDataReader ExecuteReader(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteReader(transaction, commandType, commandText, (SqlParameter[]) null);
    }

    public static SqlDataReader ExecuteReader(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      return SqlHelper.ExecuteReader(transaction.Connection, transaction, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, SqlHelper.SqlConnectionOwnership.External);
    }

    public static SqlDataReader ExecuteReader(
      SqlTransaction transaction,
      string spName,
      params object[] parameterValues)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    private static async Task<SqlDataReader> ExecuteReaderAsync(
      SqlConnection connection,
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      IEnumerable<SqlParameter> commandParameters,
      SqlHelper.SqlConnectionOwnership connectionOwnership)
    {
      bool mustCloseConnection = false;
      SqlCommand sqlCommand = new SqlCommand();
      sqlCommand.CommandTimeout = 120;
      SqlCommand cmd = sqlCommand;
      SqlDataReader sqlDataReader1;
      try
      {
        mustCloseConnection = await SqlHelper.PrepareCommandAsync(cmd, connection, transaction, commandType, commandText, commandParameters).ConfigureAwait(false);
        SqlDataReader sqlDataReader2;
        if (connectionOwnership == SqlHelper.SqlConnectionOwnership.External)
          sqlDataReader2 = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
        else
          sqlDataReader2 = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection).ConfigureAwait(false);
        SqlHelper.SystemTraceLog(connection, cmd);
        bool flag = true;
        foreach (DbParameter parameter in (DbParameterCollection) cmd.Parameters)
        {
          if (parameter.Direction != ParameterDirection.Input)
            flag = false;
        }
        if (flag)
          cmd.Parameters.Clear();
        sqlDataReader1 = sqlDataReader2;
      }
      catch
      {
        if (mustCloseConnection)
          connection.Close();
        throw;
      }
      cmd = (SqlCommand) null;
      return sqlDataReader1;
    }

    public static Task<SqlDataReader> ExecuteReaderAsync(
      string connectionString,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteReaderAsync(connectionString, commandType, commandText, (SqlParameter[]) null);
    }

    public static async Task<SqlDataReader> ExecuteReaderAsync(
      string connectionString,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      SqlConnection connection = (SqlConnection) null;
      SqlDataReader sqlDataReader;
      try
      {
        connection = new SqlConnection(connectionString);
        await connection.OpenAsync().ConfigureAwait(false);
        sqlDataReader = await SqlHelper.ExecuteReaderAsync(connection, (SqlTransaction) null, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, SqlHelper.SqlConnectionOwnership.Internal).ConfigureAwait(false);
      }
      catch
      {
        connection?.Close();
        throw;
      }
      connection = (SqlConnection) null;
      return sqlDataReader;
    }

    public static Task<SqlDataReader> ExecuteReaderAsync(
      string connectionString,
      string spName,
      params object[] parameterValues)
    {
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteReaderAsync(connectionString, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteReaderAsync(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<SqlDataReader> ExecuteReaderAsync(
      SqlConnection connection,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteReaderAsync(connection, commandType, commandText, (SqlParameter[]) null);
    }

    public static Task<SqlDataReader> ExecuteReaderAsync(
      SqlConnection connection,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      return SqlHelper.ExecuteReaderAsync(connection, (SqlTransaction) null, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, SqlHelper.SqlConnectionOwnership.External);
    }

    public static Task<SqlDataReader> ExecuteReaderAsync(
      SqlConnection connection,
      string spName,
      params object[] parameterValues)
    {
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteReaderAsync(connection, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteReaderAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<SqlDataReader> ExecuteReaderAsync(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteReaderAsync(transaction, commandType, commandText, (SqlParameter[]) null);
    }

    public static Task<SqlDataReader> ExecuteReaderAsync(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      return SqlHelper.ExecuteReaderAsync(transaction.Connection, transaction, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, SqlHelper.SqlConnectionOwnership.External);
    }

    public static Task<SqlDataReader> ExecuteReaderAsync(
      SqlTransaction transaction,
      string spName,
      params object[] parameterValues)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteReaderAsync(transaction, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteReaderAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static object ExecuteScalar(
      string connectionString,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[]) null);
    }

    public static object ExecuteScalar(
      string connectionString,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        return SqlHelper.ExecuteScalar(connection, commandType, commandText, commandParameters);
      }
    }

    public static object ExecuteScalar(
      string connectionString,
      string spName,
      params object[] parameterValues)
    {
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static object ExecuteScalar(
      SqlConnection connection,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteScalar(connection, commandType, commandText, (SqlParameter[]) null);
    }

    public static object ExecuteScalar(
      SqlConnection connection,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      SqlCommand sqlCommand1 = new SqlCommand();
      sqlCommand1.CommandTimeout = 120;
      SqlCommand sqlCommand2 = sqlCommand1;
      bool mustCloseConnection;
      SqlHelper.PrepareCommand(sqlCommand2, connection, (SqlTransaction) null, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, out mustCloseConnection);
      object obj = sqlCommand2.ExecuteScalar();
      SqlHelper.SystemTraceLog(connection, sqlCommand2);
      sqlCommand2.Parameters.Clear();
      if (!mustCloseConnection)
        return obj;
      connection.Close();
      return obj;
    }

    public static object ExecuteScalar(
      SqlConnection connection,
      string spName,
      params object[] parameterValues)
    {
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static object ExecuteScalar(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteScalar(transaction, commandType, commandText, (SqlParameter[]) null);
    }

    public static object ExecuteScalar(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      SqlCommand sqlCommand1 = new SqlCommand();
      sqlCommand1.CommandTimeout = 120;
      SqlCommand sqlCommand2 = sqlCommand1;
      SqlHelper.PrepareCommand(sqlCommand2, transaction.Connection, transaction, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, out bool _);
      object obj = sqlCommand2.ExecuteScalar();
      SqlHelper.SystemTraceLog(transaction.Connection, sqlCommand2);
      sqlCommand2.Parameters.Clear();
      return obj;
    }

    public static object ExecuteScalar(
      SqlTransaction transaction,
      string spName,
      params object[] parameterValues)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<object> ExecuteScalarAsync(
      string connectionString,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteScalarAsync(connectionString, commandType, commandText, (SqlParameter[]) null);
    }

    public static async Task<object> ExecuteScalarAsync(
      string connectionString,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      object obj;
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync().ConfigureAwait(false);
        obj = await SqlHelper.ExecuteScalarAsync(connection, commandType, commandText, commandParameters).ConfigureAwait(false);
      }
      return obj;
    }

    public static Task<object> ExecuteScalarAsync(
      string connectionString,
      string spName,
      params object[] parameterValues)
    {
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteScalarAsync(connectionString, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteScalarAsync(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<object> ExecuteScalarAsync(
      SqlConnection connection,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteScalarAsync(connection, commandType, commandText, (SqlParameter[]) null);
    }

    public static async Task<object> ExecuteScalarAsync(
      SqlConnection connection,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      SqlCommand sqlCommand = new SqlCommand();
      sqlCommand.CommandTimeout = 120;
      SqlCommand cmd = sqlCommand;
      bool mustCloseConnection = await SqlHelper.PrepareCommandAsync(cmd, connection, (SqlTransaction) null, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters).ConfigureAwait(false);
      object obj1 = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
      SqlHelper.SystemTraceLog(connection, cmd);
      cmd.Parameters.Clear();
      if (mustCloseConnection)
        connection.Close();
      object obj2 = obj1;
      cmd = (SqlCommand) null;
      return obj2;
    }

    public static Task<object> ExecuteScalarAsync(
      SqlConnection connection,
      string spName,
      params object[] parameterValues)
    {
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteScalarAsync(connection, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteScalarAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<object> ExecuteScalarAsync(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteScalarAsync(transaction, commandType, commandText, (SqlParameter[]) null);
    }

    public static async Task<object> ExecuteScalarAsync(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      SqlCommand sqlCommand = new SqlCommand();
      sqlCommand.CommandTimeout = 120;
      SqlCommand cmd = sqlCommand;
      int num = await SqlHelper.PrepareCommandAsync(cmd, transaction.Connection, transaction, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters).ConfigureAwait(false) ? 1 : 0;
      object obj1 = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
      SqlHelper.SystemTraceLog(transaction.Connection, cmd);
      cmd.Parameters.Clear();
      object obj2 = obj1;
      cmd = (SqlCommand) null;
      return obj2;
    }

    public static Task<object> ExecuteScalarAsync(
      SqlTransaction transaction,
      string spName,
      params object[] parameterValues)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteScalarAsync(transaction, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteScalarAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static XmlReader ExecuteXmlReader(
      SqlConnection connection,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteXmlReader(connection, commandType, commandText, (SqlParameter[]) null);
    }

    public static XmlReader ExecuteXmlReader(
      SqlConnection connection,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      bool mustCloseConnection = false;
      SqlCommand sqlCommand1 = new SqlCommand();
      sqlCommand1.CommandTimeout = 120;
      SqlCommand sqlCommand2 = sqlCommand1;
      try
      {
        SqlHelper.PrepareCommand(sqlCommand2, connection, (SqlTransaction) null, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, out mustCloseConnection);
        XmlReader xmlReader = sqlCommand2.ExecuteXmlReader();
        SqlHelper.SystemTraceLog(connection, sqlCommand2);
        sqlCommand2.Parameters.Clear();
        return xmlReader;
      }
      catch
      {
        if (mustCloseConnection)
          connection.Close();
        throw;
      }
    }

    public static XmlReader ExecuteXmlReader(
      SqlConnection connection,
      string spName,
      params object[] parameterValues)
    {
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static XmlReader ExecuteXmlReader(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteXmlReader(transaction, commandType, commandText, (SqlParameter[]) null);
    }

    public static XmlReader ExecuteXmlReader(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      SqlCommand sqlCommand1 = new SqlCommand();
      sqlCommand1.CommandTimeout = 120;
      SqlCommand sqlCommand2 = sqlCommand1;
      SqlHelper.PrepareCommand(sqlCommand2, transaction.Connection, transaction, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, out bool _);
      XmlReader xmlReader = sqlCommand2.ExecuteXmlReader();
      SqlHelper.SystemTraceLog(transaction.Connection, sqlCommand2);
      sqlCommand2.Parameters.Clear();
      return xmlReader;
    }

    public static XmlReader ExecuteXmlReader(
      SqlTransaction transaction,
      string spName,
      params object[] parameterValues)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<XmlReader> ExecuteXmlReaderAsync(
      SqlConnection connection,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteXmlReaderAsync(connection, commandType, commandText, (SqlParameter[]) null);
    }

    public static async Task<XmlReader> ExecuteXmlReaderAsync(
      SqlConnection connection,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      bool mustCloseConnection = false;
      SqlCommand sqlCommand = new SqlCommand();
      sqlCommand.CommandTimeout = 120;
      SqlCommand cmd = sqlCommand;
      XmlReader xmlReader1;
      try
      {
        mustCloseConnection = await SqlHelper.PrepareCommandAsync(cmd, connection, (SqlTransaction) null, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters).ConfigureAwait(false);
        XmlReader xmlReader2 = await cmd.ExecuteXmlReaderAsync().ConfigureAwait(false);
        SqlHelper.SystemTraceLog(connection, cmd);
        cmd.Parameters.Clear();
        xmlReader1 = xmlReader2;
      }
      catch
      {
        if (mustCloseConnection)
          connection.Close();
        throw;
      }
      cmd = (SqlCommand) null;
      return xmlReader1;
    }

    public static Task<XmlReader> ExecuteXmlReaderAsync(
      SqlConnection connection,
      string spName,
      params object[] parameterValues)
    {
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteXmlReaderAsync(connection, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteXmlReaderAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<XmlReader> ExecuteXmlReaderAsync(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText)
    {
      return SqlHelper.ExecuteXmlReaderAsync(transaction, commandType, commandText, (SqlParameter[]) null);
    }

    public static async Task<XmlReader> ExecuteXmlReaderAsync(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      params SqlParameter[] commandParameters)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      SqlCommand sqlCommand = new SqlCommand();
      sqlCommand.CommandTimeout = 120;
      SqlCommand cmd = sqlCommand;
      int num = await SqlHelper.PrepareCommandAsync(cmd, transaction.Connection, transaction, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters).ConfigureAwait(false) ? 1 : 0;
      XmlReader xmlReader1 = await cmd.ExecuteXmlReaderAsync().ConfigureAwait(false);
      SqlHelper.SystemTraceLog(transaction.Connection, cmd);
      cmd.Parameters.Clear();
      XmlReader xmlReader2 = xmlReader1;
      cmd = (SqlCommand) null;
      return xmlReader2;
    }

    public static Task<XmlReader> ExecuteXmlReaderAsync(
      SqlTransaction transaction,
      string spName,
      params object[] parameterValues)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.ExecuteXmlReaderAsync(transaction, CommandType.StoredProcedure, spName);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.ExecuteXmlReaderAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static void FillDataset(
      string connectionString,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames)
    {
      if (dataSet == null)
        throw new ArgumentNullException(nameof (dataSet));
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        SqlHelper.FillDataset(connection, commandType, commandText, dataSet, tableNames);
      }
    }

    public static void FillDataset(
      string connectionString,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames,
      params SqlParameter[] commandParameters)
    {
      if (dataSet == null)
        throw new ArgumentNullException(nameof (dataSet));
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        SqlHelper.FillDataset(connection, commandType, commandText, dataSet, tableNames, commandParameters);
      }
    }

    public static void FillDataset(
      string connectionString,
      string spName,
      DataSet dataSet,
      string[] tableNames,
      params object[] parameterValues)
    {
      if (dataSet == null)
        throw new ArgumentNullException(nameof (dataSet));
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        SqlHelper.FillDataset(connection, spName, dataSet, tableNames, parameterValues);
      }
    }

    public static void FillDataset(
      SqlConnection connection,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames)
    {
      SqlHelper.FillDataset(connection, commandType, commandText, dataSet, tableNames, (SqlParameter[]) null);
    }

    public static void FillDataset(
      SqlConnection connection,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames,
      params SqlParameter[] commandParameters)
    {
      SqlHelper.FillDataset(connection, (SqlTransaction) null, commandType, commandText, dataSet, tableNames, commandParameters);
    }

    public static void FillDataset(
      SqlConnection connection,
      string spName,
      DataSet dataSet,
      string[] tableNames,
      params object[] parameterValues)
    {
      if (dataSet == null)
        throw new ArgumentNullException(nameof (dataSet));
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length != 0)
      {
        SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
        SqlHelper.FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, spParameterSet);
      }
      else
        SqlHelper.FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
    }

    public static void FillDataset(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames)
    {
      SqlHelper.FillDataset(transaction, commandType, commandText, dataSet, tableNames, (SqlParameter[]) null);
    }

    public static void FillDataset(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames,
      params SqlParameter[] commandParameters)
    {
      SqlHelper.FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
    }

    public static void FillDataset(
      SqlTransaction transaction,
      string spName,
      DataSet dataSet,
      string[] tableNames,
      params object[] parameterValues)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      if (dataSet == null)
        throw new ArgumentNullException(nameof (dataSet));
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length != 0)
      {
        SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
        SqlHelper.FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, spParameterSet);
      }
      else
        SqlHelper.FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
    }

    private static void FillDataset(
      SqlConnection connection,
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames,
      params SqlParameter[] commandParameters)
    {
      if (dataSet == null)
        throw new ArgumentNullException(nameof (dataSet));
      SqlCommand sqlCommand1 = new SqlCommand();
      sqlCommand1.CommandTimeout = 120;
      SqlCommand sqlCommand2 = sqlCommand1;
      bool mustCloseConnection;
      SqlHelper.PrepareCommand(sqlCommand2, connection, transaction, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters, out mustCloseConnection);
      using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand2))
      {
        if (tableNames != null && tableNames.Length != 0)
        {
          string sourceTable = "Table";
          for (int index = 0; index < tableNames.Length; ++index)
          {
            if (tableNames[index] == null || tableNames[index].Length == 0)
              throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", nameof (tableNames));
            sqlDataAdapter.TableMappings.Add(sourceTable, tableNames[index]);
            sourceTable += (index + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
          }
        }
        sqlDataAdapter.Fill(dataSet);
        SqlHelper.SystemTraceLog(connection, sqlCommand2);
        sqlCommand2.Parameters.Clear();
      }
      if (!mustCloseConnection)
        return;
      connection.Close();
    }

    public static async Task FillDatasetAsync(
      string connectionString,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames)
    {
      if (dataSet == null)
        throw new ArgumentNullException(nameof (dataSet));
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync().ConfigureAwait(false);
        await SqlHelper.FillDatasetAsync(connection, commandType, commandText, dataSet, tableNames).ConfigureAwait(false);
      }
    }

    public static async Task FillDatasetAsync(
      string connectionString,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames,
      params SqlParameter[] commandParameters)
    {
      if (dataSet == null)
        throw new ArgumentNullException(nameof (dataSet));
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync().ConfigureAwait(false);
        await SqlHelper.FillDatasetAsync(connection, commandType, commandText, dataSet, tableNames, commandParameters).ConfigureAwait(false);
      }
    }

    public static async Task FillDatasetAsync(
      string connectionString,
      string spName,
      DataSet dataSet,
      string[] tableNames,
      params object[] parameterValues)
    {
      if (dataSet == null)
        throw new ArgumentNullException(nameof (dataSet));
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync().ConfigureAwait(false);
        await SqlHelper.FillDatasetAsync(connection, spName, dataSet, tableNames, parameterValues).ConfigureAwait(false);
      }
    }

    public static Task FillDatasetAsync(
      SqlConnection connection,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames)
    {
      return SqlHelper.FillDatasetAsync(connection, commandType, commandText, dataSet, tableNames, (SqlParameter[]) null);
    }

    public static Task FillDatasetAsync(
      SqlConnection connection,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames,
      params SqlParameter[] commandParameters)
    {
      return SqlHelper.FillDatasetAsync(connection, (SqlTransaction) null, commandType, commandText, dataSet, tableNames, commandParameters);
    }

    public static Task FillDatasetAsync(
      SqlConnection connection,
      string spName,
      DataSet dataSet,
      string[] tableNames,
      params object[] parameterValues)
    {
      if (dataSet == null)
        throw new ArgumentNullException(nameof (dataSet));
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.FillDatasetAsync(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.FillDatasetAsync(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, spParameterSet);
    }

    public static Task FillDatasetAsync(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames)
    {
      return SqlHelper.FillDatasetAsync(transaction, commandType, commandText, dataSet, tableNames, (SqlParameter[]) null);
    }

    public static Task FillDatasetAsync(
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames,
      params SqlParameter[] commandParameters)
    {
      return SqlHelper.FillDatasetAsync(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
    }

    public static Task FillDatasetAsync(
      SqlTransaction transaction,
      string spName,
      DataSet dataSet,
      string[] tableNames,
      params object[] parameterValues)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      if (dataSet == null)
        throw new ArgumentNullException(nameof (dataSet));
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      if (spParameterSet.Length == 0)
        return SqlHelper.FillDatasetAsync(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
      SqlHelper.AssignParameterValues(ref spParameterSet, parameterValues);
      return SqlHelper.FillDatasetAsync(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, spParameterSet);
    }

    private static async Task FillDatasetAsync(
      SqlConnection connection,
      SqlTransaction transaction,
      CommandType commandType,
      string commandText,
      DataSet dataSet,
      string[] tableNames,
      params SqlParameter[] commandParameters)
    {
      if (dataSet == null)
        throw new ArgumentNullException(nameof (dataSet));
      SqlCommand sqlCommand = new SqlCommand();
      sqlCommand.CommandTimeout = 120;
      SqlCommand command = sqlCommand;
      bool flag = await SqlHelper.PrepareCommandAsync(command, connection, transaction, commandType, commandText, (IEnumerable<SqlParameter>) commandParameters).ConfigureAwait(false);
      using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command))
      {
        if (tableNames != null && tableNames.Length != 0)
        {
          string sourceTable = "Table";
          for (int index = 0; index < tableNames.Length; ++index)
          {
            if (tableNames[index] == null || tableNames[index].Length == 0)
              throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", nameof (tableNames));
            sqlDataAdapter.TableMappings.Add(sourceTable, tableNames[index]);
            sourceTable += (index + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
          }
        }
        sqlDataAdapter.Fill(dataSet);
        SqlHelper.SystemTraceLog(connection, command);
        command.Parameters.Clear();
      }
      if (!flag)
      {
        command = (SqlCommand) null;
      }
      else
      {
        connection.Close();
        command = (SqlCommand) null;
      }
    }

    public static void UpdateDataset(
      SqlCommand insertCommand,
      SqlCommand deleteCommand,
      SqlCommand updateCommand,
      DataSet dataSet,
      string tableName)
    {
      if (insertCommand == null)
        throw new ArgumentNullException(nameof (insertCommand));
      if (deleteCommand == null)
        throw new ArgumentNullException(nameof (deleteCommand));
      if (updateCommand == null)
        throw new ArgumentNullException(nameof (updateCommand));
      if (string.IsNullOrEmpty(tableName))
        throw new ArgumentNullException(nameof (tableName));
      using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
      {
        sqlDataAdapter.UpdateCommand = updateCommand;
        sqlDataAdapter.InsertCommand = insertCommand;
        sqlDataAdapter.DeleteCommand = deleteCommand;
        sqlDataAdapter.Update(dataSet, tableName);
        dataSet.AcceptChanges();
      }
    }

    public static SqlCommand CreateCommand(
      SqlConnection connection,
      string spName,
      params string[] sourceColumns)
    {
      SqlCommand sqlCommand = new SqlCommand(spName, connection);
      sqlCommand.CommandType = CommandType.StoredProcedure;
      SqlCommand command = sqlCommand;
      if (sourceColumns != null && sourceColumns.Length != 0)
      {
        SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
        for (int index = 0; index < sourceColumns.Length; ++index)
          spParameterSet[index].SourceColumn = sourceColumns[index];
        SqlHelper.AttachParameters(command, (IEnumerable<SqlParameter>) spParameterSet);
      }
      return command;
    }

    public static int ExecuteNonQueryTypedParams(
      string connectionString,
      string spName,
      DataRow dataRow)
    {
      if (dataRow != null && dataRow.ItemArray.Length != 0)
      {
        SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
        SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
        return SqlHelper.ExecuteNonQuery(connectionString, (object) CommandType.StoredProcedure, (object) spName, (object) spParameterSet);
      }
      return SqlHelper.ExecuteNonQuery(connectionString, (object) CommandType.StoredProcedure, (object) spName);
    }

    public static int ExecuteNonQueryTypedParams(
      SqlConnection connection,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static int ExecuteNonQueryTypedParams(
      SqlTransaction transaction,
      string spName,
      DataRow dataRow)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<int> ExecuteNonQueryTypedParamsAsync(
      string connectionString,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteNonQueryAsync(connectionString, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteNonQueryAsync(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<int> ExecuteNonQueryTypedParamsAsync(
      SqlConnection connection,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteNonQueryAsync(connection, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteNonQueryAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<int> ExecuteNonQueryTypedParamsAsync(
      SqlTransaction transaction,
      string spName,
      DataRow dataRow)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteNonQueryAsync(transaction, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteNonQueryAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static DataSet ExecuteDatasetTypedParams(
      string connectionString,
      string spName,
      DataRow dataRow)
    {
      if (dataRow != null && dataRow.ItemArray.Length != 0)
      {
        SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
        SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
        return SqlHelper.ExecuteDataset(connectionString, (object) CommandType.StoredProcedure, (object) spName, (object) spParameterSet);
      }
      return SqlHelper.ExecuteDataset(connectionString, (object) CommandType.StoredProcedure, (object) spName);
    }

    public static DataSet ExecuteDatasetTypedParams(
      SqlConnection connection,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static DataSet ExecuteDatasetTypedParams(
      SqlTransaction transaction,
      string spName,
      DataRow dataRow)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<DataSet> ExecuteDatasetTypedParamsAsync(
      string connectionString,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<DataSet> ExecuteDatasetTypedParamsAsync(
      SqlConnection connection,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteDatasetAsync(connection, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteDatasetAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<DataSet> ExecuteDatasetTypedParamsAsync(
      SqlTransaction transaction,
      string spName,
      DataRow dataRow)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteDatasetAsync(transaction, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteDatasetAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static SqlDataReader ExecuteReaderTypedParams(
      string connectionString,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static SqlDataReader ExecuteReaderTypedParams(
      SqlConnection connection,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static SqlDataReader ExecuteReaderTypedParams(
      SqlTransaction transaction,
      string spName,
      DataRow dataRow)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<SqlDataReader> ExecuteReaderTypedParamsAsync(
      string connectionString,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteReaderAsync(connectionString, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteReaderAsync(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<SqlDataReader> ExecuteReaderTypedParamsAsync(
      SqlConnection connection,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteReaderAsync(connection, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteReaderAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<SqlDataReader> ExecuteReaderTypedParamsAsync(
      SqlTransaction transaction,
      string spName,
      DataRow dataRow)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteReaderAsync(transaction, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteReaderAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static object ExecuteScalarTypedParams(
      string connectionString,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static object ExecuteScalarTypedParams(
      SqlConnection connection,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static object ExecuteScalarTypedParams(
      SqlTransaction transaction,
      string spName,
      DataRow dataRow)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<object> ExecuteScalarTypedParamsAsync(
      string connectionString,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteScalarAsync(connectionString, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteScalarAsync(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<object> ExecuteScalarTypedParamsAsync(
      SqlConnection connection,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteScalarAsync(connection, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteScalarAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<object> ExecuteScalarTypedParamsAsync(
      SqlTransaction transaction,
      string spName,
      DataRow dataRow)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteScalarAsync(transaction, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteScalarAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static XmlReader ExecuteXmlReaderTypedParams(
      SqlConnection connection,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static XmlReader ExecuteXmlReaderTypedParams(
      SqlTransaction transaction,
      string spName,
      DataRow dataRow)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<XmlReader> ExecuteXmlReaderTypedParamsAsync(
      SqlConnection connection,
      string spName,
      DataRow dataRow)
    {
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteXmlReaderAsync(connection, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteXmlReaderAsync(connection, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static Task<XmlReader> ExecuteXmlReaderTypedParamsAsync(
      SqlTransaction transaction,
      string spName,
      DataRow dataRow)
    {
      if (transaction == null)
        throw new ArgumentNullException(nameof (transaction));
      if (transaction != null && transaction.Connection == null)
        throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof (transaction));
      if (dataRow == null || dataRow.ItemArray.Length == 0)
        return SqlHelper.ExecuteXmlReaderAsync(transaction, CommandType.StoredProcedure, spName);
      SqlParameter[] spParameterSet = ExportParameter_Provider.GetSpParameterSet(spName);
      SqlHelper.AssignParameterValues((IEnumerable<SqlParameter>) spParameterSet, dataRow);
      return SqlHelper.ExecuteXmlReaderAsync(transaction, CommandType.StoredProcedure, spName, spParameterSet);
    }

    public static void ExecStoreWithDataRow(
      string StoreName,
      DataRow drUpdate,
      params object[] param)
    {
      List<object> paramFromDataRow = SqlHelper.GetParamFromDataRow(StoreName, drUpdate, param);
      if (paramFromDataRow.Count > 0)
        SqlHelper.ExecuteNonQuery(StoreName, paramFromDataRow.ToArray());
      else
        SqlHelper.ExecuteNonQuery(StoreName);
    }

    public static List<object> GetParamFromDataRow(
      string StoreName,
      DataRow drUpdate,
      params object[] param)
    {
      if (drUpdate == null)
        return ((IEnumerable<object>) param).ToList<object>();
      try
      {
        DataTable dataTable = ExportParameter_Provider.GetExportParameter(StoreName);
        List<object> paramFromDataRow = new List<object>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
        {
          string str = row["Name"].ToString();
          if (!((IEnumerable<object>) param).Contains<object>((object) str) && drUpdate.Table.Columns.Contains(str.Substring(1)))
          {
            paramFromDataRow.Add((object) str);
            paramFromDataRow.Add(drUpdate[str.Substring(1)]);
          }
        }
        paramFromDataRow.AddRange((IEnumerable<object>) param);
        return paramFromDataRow;
      }
      catch (Exception ex)
      {
        ex.Log();
        throw ex;
      }
    }

    public static void AlterColumn(
      string tableName,
      string columnName,
      string type,
      SqlTransaction transaction,
      bool ignnorCath = true)
    {
      try
      {
        SqlHelper.ExecuteNonQuery(transaction, string.Format("ALTER TABLE [{0}] ALTER COLUMN [{1}] {2} ", (object) tableName, (object) columnName, (object) type));
      }
      catch (Exception ex)
      {
        ex.Log();
        if (!ignnorCath)
          throw ex;
      }
    }

    public static void CreateColumn(
      string tableName,
      string columnName,
      string type,
      SqlTransaction transaction,
      bool ignnorCath = true)
    {
      try
      {
        SqlHelper.ExecuteNonQuery(transaction, string.Format("ALTER TABLE [{0}] ADD  [{1}] {2} ", (object) tableName, (object) columnName, (object) type));
      }
      catch (Exception ex)
      {
        ex.Log();
        if (!ignnorCath)
          throw ex;
      }
    }

    public static void CreateFKStatusData(
      string tableName,
      SqlTransaction transaction,
      bool ignnorCath = true)
    {
      string commandText = string.Format(" ALTER TABLE [dbo].[{0}]  WITH CHECK ADD  CONSTRAINT [FK_{0}_cbb_TrangThaiDuLieu] FOREIGN KEY([statusData]) REFERENCES [dbo].[cbb_TrangThaiDuLieu] ([ID]) \r\n                                     GO\r\n                                     ALTER TABLE [dbo].[{0}] CHECK CONSTRAINT [FK_{0}_cbb_TrangThaiDuLieu] ", (object) tableName);
      try
      {
        SqlHelper.ExecuteNonQuery(transaction, commandText);
      }
      catch (Exception ex)
      {
        ex.Log();
      }
    }

    public static void CreateTrigger(string tableName, SqlTransaction transaction, bool ignnorCath = true)
    {
      string commandText1 = string.Format(" CREATE TRIGGER [dbo].[trg_delete_{0}] ON [dbo].[{0}] FOR DELETE AS  \n BEGIN  \n  UPDATE [{0}]  \n   SET  deleteDate = GETDATE()   \n   from [{0}]  \n   where exists  (select ID from deleted where [{0}].ID =  deleted.ID)  \n  END ", (object) tableName);
      try
      {
        SqlHelper.ExecuteNonQuery(transaction, commandText1);
      }
      catch (Exception ex)
      {
        ex.Log();
        if (!ignnorCath)
          throw ex;
      }
      string commandText2 = string.Format(" CREATE TRIGGER [dbo].[trg_insert_{0}] ON [dbo].[{0}] AFTER INSERT AS \n  BEGIN  \n UPDATE [{0}]  \n  SET  createDate = GETDATE()   \n   from [{0}]  \n   where exists (select ID from inserted where [{0}].ID =  inserted.ID)  \n END ", (object) tableName);
      try
      {
        SqlHelper.ExecuteNonQuery(transaction, commandText2);
      }
      catch (Exception ex)
      {
        ex.Log();
        if (!ignnorCath)
          throw ex;
      }
      string commandText3 = string.Format(" CREATE TRIGGER [dbo].[trg_update_{0}] on [dbo].[{0}] after update AS  \n BEGIN  \n UPDATE [{0}]  \n SET  modifyDate = GETDATE()   \n from [{0}]  \n where exists (select ID from deleted where [{0}].ID =  deleted.ID AND  DATEDIFF(SECOND, deleted.createDate ,GETDATE()  ) > 30)  \n end ", (object) tableName);
      try
      {
        SqlHelper.ExecuteNonQuery(transaction, commandText3);
      }
      catch (Exception ex)
      {
        ex.Log();
        if (!ignnorCath)
          throw ex;
      }
    }

    public static void CreateTable(
      bool idIdentity,
      string tableName,
      bool childTable,
      SqlTransaction transaction,
      bool ignnorCath = true)
    {
      string format;
      if (tableName.StartsWith("EXPORT_"))
        format = "CREATE TABLE [dbo].[{0}]( \n [ID] [int] IDENTITY(1,1) NOT NULL, \n  CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED  \n ( \n \t[ID] ASC \n ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] \n ) ON [PRIMARY] \n ";
      else
        format = "CREATE TABLE [dbo].[{0}]( \n [ID] [int] " + (idIdentity ? "IDENTITY(1,1)" : "") + " NOT NULL, \n [refID] [int]" + (childTable ? " NOT " : "") + " NULL, \n [title] nvarchar(255)  NULL, \n [statusData] [int] NULL, \n [visible] [bit]  NULL, \n [priority] [float] NULL, \n [uidCreate] [int] NULL, \n [createDate] [datetime] NULL, \n [uidModify] [int] NULL, \n [modifyDate] [datetime] NULL, \n  CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED  \n ( \n \t[ID] ASC \n ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] \n ) ON [PRIMARY] \n ";
      string commandText = string.Format(format, (object) tableName);
      try
      {
        SqlHelper.ExecuteNonQuery(transaction, commandText);
      }
      catch (Exception ex)
      {
        ex.Log();
        if (!ignnorCath)
          throw ex;
      }
    }

    private enum SqlConnectionOwnership
    {
      Internal,
      External,
    }
  }
}
