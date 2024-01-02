
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using WowCommon;


namespace WowSQL
{
  public class DBConnection
  {
    private const byte DELTA_KEY = 71;
    private const string SEPARATE_CHAR = ";";
    public const string ENCRYPTED = "1";
    public const string NO_ENCRYPTED = "0";
    private string m_strServer;
    private string m_strDatabase;
    private string m_strUser;
    private string m_strPassword;
    private string m_strEncrypted;

    public DBConnection()
    {
      this.m_strServer = "";
      this.m_strDatabase = "";
      this.m_strUser = "";
      this.m_strPassword = "";
      this.m_strEncrypted = "0";
    }

    public DBConnection(
      string strServer,
      string strDatabase,
      string strUser,
      string strPassword,
      string strEncrypted)
    {
      this.m_strServer = strServer;
      this.m_strDatabase = strDatabase;
      this.m_strUser = strUser;
      this.m_strPassword = strPassword;
      this.m_strEncrypted = strEncrypted;
    }

    public DBConnection(string strServer, string strDatabase, string strUser, string strPassword)
    {
      this.m_strServer = strServer;
      this.m_strDatabase = strDatabase;
      this.m_strUser = strUser;
      this.m_strPassword = strPassword;
    }

    public static string FILE_NAME { get; set; } = "Paradise.vts";

    public static string FILE_FULLNAME { get; set; } = "";

    public static int CountDate(DateTime ValueFrom, DateTime ValueTo)
    {
      return Convert.ToInt32((ValueTo - ValueFrom).TotalDays);
    }

    public bool GetDBConnectionInfo(
      out string strServer,
      out string strDatabase,
      out string strUserName,
      out string strPassword,
      out string strEncrypted,
      string FileFullName = "")
    {
      strServer = "";
      strDatabase = "";
      strUserName = "";
      strPassword = "";
      strEncrypted = "0";
      try
      {
        string str = "";
        using (FileStream fileStream = new FileStream(!string.IsNullOrEmpty(FileFullName) ? FileFullName : (string.IsNullOrEmpty(DBConnection.FILE_FULLNAME) ? string.Format("{0}\\{1}", (object) Application.StartupPath, (object) DBConnection.FILE_NAME) : DBConnection.FILE_FULLNAME), FileMode.Open))
        {
          for (int index = fileStream.ReadByte(); index != -1; index = fileStream.ReadByte())
          {
            int num = index - 71;
            if (num < 0)
              num += 256;
            str += ((char) num).ToString();
          }
        }
        if (string.IsNullOrWhiteSpace(str))
          return false;
        string[] strArray = str.Split(";".ToCharArray(), 5);
        strServer = strArray[0];
        strDatabase = strArray[1];
        strUserName = strArray[2];
        strPassword = strArray[3];
        strEncrypted = strArray[4];
        return true;
      }
      catch (Exception ex)
      {
        ex.Log();
        return false;
      }
      finally
      {
        this.m_strServer = strServer;
        this.m_strDatabase = strDatabase;
        this.m_strUser = strUserName;
        this.m_strPassword = strPassword;
        this.m_strEncrypted = strEncrypted;
      }
    }

    public bool GetDBConnectionInfo(
      out string strServer,
      out string strDatabase,
      out string strUserName,
      out string strPassword)
    {
      return this.GetDBConnectionInfo(out strServer, out strDatabase, out strUserName, out strPassword, out string _);
    }

    public bool CheckConnectionInfo()
    {
      return this.m_strServer.ToLower().StartsWith("(localdb)") ? this.CheckLocalDBConnectionInfo(this.m_strServer, this.m_strDatabase) : this.CheckConnectionInfo(this.m_strServer, this.m_strDatabase, this.m_strUser, this.m_strPassword);
    }

    private bool CheckLocalDBConnectionInfo(string strServer, string strDatabase)
    {
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True;", (object) strServer, (object) strDatabase)))
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

    public bool CheckConnectionInfo(
      string strServer,
      string strDatabase,
      string strUserName,
      string strPassword)
    {
      string connectionString = string.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3}", (object) strServer, (object) strDatabase, (object) strUserName, (object) strPassword);
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

    public bool SaveConnectionInfo()
    {
      return this.SaveConnectionInfo(this.m_strServer, this.m_strDatabase, this.m_strUser, this.m_strPassword, "0");
    }

    public bool SaveConnectionInfo(
      string strServer,
      string strDatabase,
      string strUserName,
      string strPassword,
      string strEncrypted)
    {
      string startupPath = Application.StartupPath;
      return this.SaveConnectionInfo(strServer, strDatabase, strUserName, strPassword, strEncrypted, startupPath);
    }

    public bool SaveConnectionInfo(
      string strServer,
      string strDatabase,
      string strUserName,
      string strPassword,
      string strEncrypted,
      string strPhycicalPath)
    {
      if (strServer == string.Empty || strDatabase == string.Empty || strUserName == string.Empty)
        return false;
      FileStream fileStream = (FileStream) null;
      try
      {
        string str = strServer + ";" + strDatabase + ";" + strUserName + ";" + strPassword + ";" + strEncrypted;
        fileStream = new FileStream(string.IsNullOrEmpty(DBConnection.FILE_FULLNAME) ? string.Format("{0}\\{1}", (object) strPhycicalPath, (object) DBConnection.FILE_NAME) : DBConnection.FILE_FULLNAME, FileMode.Create);
        if (strEncrypted == "1")
        {
          for (int index = 0; index < str.Length; ++index)
            fileStream.WriteByte((byte) (((int) (byte) str[index] + 71) % 256));
        }
        fileStream.Flush();
        fileStream.Close();
        return true;
      }
      catch (Exception ex)
      {
        ex.Log();
        fileStream?.Close();
        throw ex;
      }
    }
  }
}
