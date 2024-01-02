
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WowCommon;


namespace WowSQL
{
  public class SQL_Bulk_Insert_Helper
  {
    public SqlBulkCopy bulk;
    private SqlCommand command;
    public SqlConnection conn;

    public SqlCommand cmd
    {
      get
      {
        if (this.command == null)
        {
          this.command = new SqlCommand()
          {
            Connection = this.conn
          };
          this.command.CommandTimeout = 60;
        }
        return this.command;
      }
      set => this.command = value;
    }

    public SQL_Bulk_Insert_Helper BulkCopy(
      DataTable dtSource,
      SqlConnection _conn,
      string DestinationTable,
      IEnumerable<string> WithInitCommand,
      IEnumerable<string> WithFinalCommand,
      bool WithClose,
      bool ignoreFailureOnFinalCommand,
      Action<double> _ProgressReceiver,
      SqlTransaction trans,
      params object[] otherPRS)
    {
      if (_conn == null)
      {
        _conn = new SqlConnection(SqlHelper.connectionString);
        _conn.Open();
      }
      this.conn = _conn;
      if (this.bulk == null)
        this.bulk = new SqlBulkCopy(this.conn, SqlBulkCopyOptions.Default, trans);
      if (WithInitCommand != null && WithInitCommand.Count<string>() > 0)
      {
        foreach (string str in WithInitCommand.Where<string>((System.Func<string, bool>) (c => c.Length > 0)))
        {
          try
          {
            this.cmd.CommandType = CommandType.Text;
            this.command.Transaction = trans;
            this.cmd.CommandText = str;
            this.cmd.ExecuteNonQuery();
            if (_ProgressReceiver != null)
              _ProgressReceiver(48.0);
          }
          catch (Exception ex)
          {
            ex.Log();
            if (!ignoreFailureOnFinalCommand)
              throw ex;
          }
        }
      }
      this.bulk.BulkCopyTimeout = 60;
      this.bulk.DestinationTableName = DestinationTable;
      this.bulk.WriteToServer(dtSource);
      if (_ProgressReceiver != null)
        _ProgressReceiver(52.0);
      if (WithFinalCommand != null && WithFinalCommand.Count<string>() > 0)
      {
        foreach (string str in WithFinalCommand.Where<string>((System.Func<string, bool>) (c => c.Length > 0)))
        {
          try
          {
            if (!str.Contains(" ") && ExportParameter_Provider.IsProcedure(str))
            {
              SqlHelper.ExecuteNonQuery(this.conn, str, otherPRS);
            }
            else
            {
              this.command.Transaction = trans;
              this.cmd.CommandType = CommandType.Text;
              this.cmd.CommandText = str;
              this.cmd.ExecuteNonQuery();
            }
          }
          catch (Exception ex)
          {
            ex.Log();
            if (!ignoreFailureOnFinalCommand)
              throw ex;
          }
        }
      }
      if (WithClose)
      {
        this.CloseBulk();
        this.conn.Close();
      }
      SqlHelper.ProcessNotifyData(this.cmd);
      return this;
    }

    public void CloseBulk()
    {
      this.bulk.Close();
      this.command.Dispose();
    }

    public void RunCommand(string commandString, SqlConnection connection, SqlTransaction trans)
    {
      if (this.conn == null)
        this.conn = connection;
      this.cmd.CommandText = commandString;
      this.cmd.Transaction = trans;
      this.cmd.ExecuteNonQuery();
    }

    public SQL_Bulk_Insert_Helper BulkCopy_BulkForProcedures(
      DataTable dtSource,
      SqlConnection _conn,
      IEnumerable<string> WithFinalCommand,
      bool WithClose,
      bool ignoreFailureOnFinalCommand,
      Action<double> _ProgressReceiver,
      SqlTransaction trans,
      params object[] otherPRS)
    {
      List<string> WithInitCommand = new List<string>();
      string str1 = string.Join(",", dtSource.Columns.Cast<DataColumn>().Select(c => new
      {
        ColumnName = c.ColumnName,
        dbtype = SqlHelper.GetSQLDBType(c.DataType.Name).ToLower()
      }).Select(c => string.Format("[{0}] {1}", (object) c.ColumnName, c.dbtype.Contains("var") ? (object) string.Format("{0}(max)", (object) c.dbtype) : (object) c.dbtype)));
      string DestinationTable = "#TempTableData";
      string str2 = string.Format("create table {1}({0})", (object) str1, (object) DestinationTable);
      WithInitCommand.Add(str2);
      return this.BulkCopy(dtSource, _conn, DestinationTable, (IEnumerable<string>) WithInitCommand, WithFinalCommand, WithClose, ignoreFailureOnFinalCommand, _ProgressReceiver, trans, otherPRS);
    }
  }
}
