
using System;
using System.IO;
using System.Windows.Forms;


namespace WowCommon
{
  public static class EnDeConnection
  {
    private static byte DELTA_KEY = 85;
    private static string SEPARATE_CHAR = ";";
    public static string ENCRYPTED = "1";
    public static string NO_ENCRYPTED = "0";
    public static string m_strServer;
    public static string m_strDatabase;
    public static string m_strUser;
    public static string m_strPassword;
    public static string m_strEncrypted;

    public static bool SaveConnectionInfo(
      string fullPath,
      string strServer,
      string strDatabase,
      string strUserName,
      string strPassword,
      string _AuthSecret,
      string _BasePath,
      string userId,
      string strEncrypted = "1")
    {
      if (strServer == string.Empty || strDatabase == string.Empty || strUserName == string.Empty)
      {
        int num = (int) MessageBox.Show("Invalid data!", "Paradise", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return false;
      }
      FileStream fileStream = (FileStream) null;
      try
      {
        string str = strServer + EnDeConnection.SEPARATE_CHAR + strDatabase + EnDeConnection.SEPARATE_CHAR + strUserName + EnDeConnection.SEPARATE_CHAR + strPassword + EnDeConnection.SEPARATE_CHAR + _AuthSecret + EnDeConnection.SEPARATE_CHAR + _BasePath + EnDeConnection.SEPARATE_CHAR + userId + EnDeConnection.SEPARATE_CHAR + strEncrypted + EnDeConnection.SEPARATE_CHAR;
        for (int index = 0; index < 99; ++index)
          str = str + Guid.NewGuid().ToString() + EnDeConnection.SEPARATE_CHAR;
        fileStream = new FileStream(fullPath, FileMode.Create);
        if (strEncrypted == EnDeConnection.ENCRYPTED)
        {
          for (int index = 0; index < str.Length; ++index)
            fileStream.WriteByte((byte) (((int) (byte) str[index] + (int) EnDeConnection.DELTA_KEY) % 256));
        }
        fileStream.Flush();
        fileStream.Close();
        return true;
      }
      catch
      {
        fileStream?.Close();
        throw;
      }
    }

    public static byte[] ReadFile(string sPath)
    {
      try
      {
        long length = new FileInfo(sPath).Length;
        FileStream input = new FileStream(sPath, FileMode.Open, FileAccess.Read);
        byte[] numArray = new BinaryReader((Stream) input).ReadBytes((int) length);
        input.Close();
        return numArray;
      }
      catch
      {
        return (byte[]) null;
      }
    }

    public static bool GetDBConnectionInfo(
      string fullPath,
      out string strServer,
      out string strDatabase,
      out string strUserName,
      out string strPassword,
      out string _AuthSecret,
      out string _BasePath,
      out string userId,
      out string strEncrypted)
    {
      strServer = "";
      strDatabase = "";
      strUserName = "";
      strPassword = "";
      _AuthSecret = "";
      _BasePath = "";
      userId = "";
      strEncrypted = EnDeConnection.NO_ENCRYPTED;
      try
      {
        string str = "";
        using (FileStream fileStream = new FileStream(fullPath, FileMode.Open))
        {
          for (int index = fileStream.ReadByte(); index != -1; index = fileStream.ReadByte())
          {
            int num = index - (int) EnDeConnection.DELTA_KEY;
            if (num < 0)
              num += 256;
            str += ((char) num).ToString();
          }
        }
        if (str == string.Empty)
          return false;
        string[] strArray = str.Split(EnDeConnection.SEPARATE_CHAR.ToCharArray(), 9);
        strServer = strArray[0];
        strDatabase = strArray[1];
        strUserName = strArray[2];
        strPassword = strArray[3];
        _AuthSecret = strArray[4];
        _BasePath = strArray[5];
        userId = strArray[6];
        strEncrypted = strArray[7];
        return true;
      }
      catch
      {
        return false;
      }
      finally
      {
        EnDeConnection.m_strServer = strServer;
        EnDeConnection.m_strDatabase = strDatabase;
        EnDeConnection.m_strUser = strUserName;
        EnDeConnection.m_strPassword = strPassword;
        EnDeConnection.m_strEncrypted = strEncrypted;
      }
    }
  }
}
