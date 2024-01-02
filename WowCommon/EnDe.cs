
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace WowCommon
{
  public class EnDe
  {
    private const string EncryptKey = "56 7F 6B 5E 72 73 6F 78 71 3B 4A 3D";
    private static int Next = 290791;

    public static string DecryptWebContent(string encryptedText)
    {
      string password = EnDe.DecryptText("Gdzjk6KG/PkEeZuOaJn03A==");
      string s1 = EnDe.DecryptText("a0/V4kZqpZgPkyEmp668nA==");
      string s2 = EnDe.DecryptText("w77aMjm582jIPn3gR+Pc7TYP5xxTWrYA");
      string s3 = "";
      for (int index = 0; index < encryptedText.Length; ++index)
        s3 += ((char) ((ulong) encryptedText[index] - (ulong) EnDe.Next)).ToString();
      byte[] buffer = Convert.FromBase64String(s3);
      byte[] bytes = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(s1)).GetBytes(32);
      RijndaelManaged rijndaelManaged = new RijndaelManaged();
      rijndaelManaged.Mode = CipherMode.CBC;
      rijndaelManaged.Padding = PaddingMode.None;
      ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(bytes, Encoding.ASCII.GetBytes(s2));
      MemoryStream memoryStream = new MemoryStream(buffer);
      CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, decryptor, CryptoStreamMode.Read);
      byte[] numArray = new byte[buffer.Length];
      int count = cryptoStream.Read(numArray, 0, numArray.Length);
      memoryStream.Close();
      cryptoStream.Close();
      return Encoding.UTF8.GetString(numArray, 0, count).TrimEnd("\0".ToCharArray());
    }

    public static string EncryptWebContent(string plainText)
    {
      string password = EnDe.DecryptText("Gdzjk6KG/PkEeZuOaJn03A==");
      string s1 = EnDe.DecryptText("a0/V4kZqpZgPkyEmp668nA==");
      string s2 = EnDe.DecryptText("w77aMjm582jIPn3gR+Pc7TYP5xxTWrYA");
      byte[] bytes1 = Encoding.UTF8.GetBytes(plainText);
      byte[] bytes2 = Encoding.ASCII.GetBytes(s1);
      byte[] bytes3 = new Rfc2898DeriveBytes(password, bytes2).GetBytes(32);
      RijndaelManaged rijndaelManaged = new RijndaelManaged();
      rijndaelManaged.Mode = CipherMode.CBC;
      rijndaelManaged.Padding = PaddingMode.Zeros;
      ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(bytes3, Encoding.ASCII.GetBytes(s2));
      byte[] array;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, encryptor, CryptoStreamMode.Write))
        {
          cryptoStream.Write(bytes1, 0, bytes1.Length);
          cryptoStream.FlushFinalBlock();
          array = memoryStream.ToArray();
          cryptoStream.Close();
        }
        memoryStream.Close();
      }
      string base64String = Convert.ToBase64String(array);
      string str = "";
      for (int index = 0; index < base64String.Length; ++index)
        str += ((char) ((ulong) base64String[index] + (ulong) EnDe.Next)).ToString();
      return str;
    }

    public static string EncryptText(string toEncrypt, bool useHashing = true)
    {
      return EnDe.PrivateEncryption(toEncrypt, useHashing);
    }

    public static string DecryptText(string toDecrypt, bool useHashing = true)
    {
      return EnDe.PrivateDecryption(toDecrypt, useHashing);
    }

    private static string PrivateEncryption(string toEncrypt, bool useHashing)
    {
      string encryptionKey = EnDe.getEncryptionKey();
      byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
      byte[] numArray = !useHashing ? Encoding.UTF8.GetBytes(encryptionKey) : new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
      TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
      cryptoServiceProvider.Key = numArray;
      cryptoServiceProvider.Mode = CipherMode.ECB;
      cryptoServiceProvider.Padding = PaddingMode.PKCS7;
      byte[] inArray = cryptoServiceProvider.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
      return Convert.ToBase64String(inArray, 0, inArray.Length);
    }

    private static string PrivateDecryption(string toDecrypt, bool useHashing)
    {
      if (string.IsNullOrEmpty(toDecrypt))
        return (string) null;
      string encryptionKey = EnDe.getEncryptionKey();
      byte[] inputBuffer = Convert.FromBase64String(toDecrypt);
      byte[] numArray = !useHashing ? Encoding.UTF8.GetBytes(encryptionKey) : new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
      TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
      cryptoServiceProvider.Key = numArray;
      cryptoServiceProvider.Mode = CipherMode.ECB;
      cryptoServiceProvider.Padding = PaddingMode.PKCS7;
      return Encoding.UTF8.GetString(cryptoServiceProvider.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length));
    }

    private static string getEncryptionKey()
    {
      string[] strArray = "56 7F 6B 5E 72 73 6F 78 71 3B 4A 3D".Split(' ');
      string encryptionKey = "";
      foreach (string str in strArray)
      {
        int utf32 = Convert.ToInt32(str, 16) - 10;
        encryptionKey += char.ConvertFromUtf32(utf32);
      }
      return encryptionKey;
    }
  }
}
