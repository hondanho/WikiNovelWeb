
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using WowCommon;
using WowSQL;


namespace WowCore
{
  public class EmailSender
  {
    private DataTable dtSettingEmail;

    public EmailSender()
    {
      this.dtSettingEmail = SqlHelper.ExecuteDataTable("SELECT * FROM tblEmailSetting");
    }

    public void Send(Dictionary<string, object> datamap, string toEmail, string emailKey)
    {
      if (!this.dtSettingEmail.AsEnumerable().Any<DataRow>((System.Func<DataRow, bool>) (dr => dr[nameof (emailKey)] != DBNull.Value && dr[nameof (emailKey)].ToString() == emailKey)))
        return;
      DataRow dataRow = this.dtSettingEmail.AsEnumerable().FirstOrDefault<DataRow>((System.Func<DataRow, bool>) (dr => dr[nameof (emailKey)] != DBNull.Value && dr[nameof (emailKey)].ToString() == emailKey));
      MailAddress from = new MailAddress(dataRow["sendByEmail"].ToString());
      MailAddress to = new MailAddress(toEmail);
      string str = dataRow["body"].ToString();
      foreach (KeyValuePair<string, object> keyValuePair in datamap)
      {
        string newValue = keyValuePair.Value == null || keyValuePair.Value == DBNull.Value ? "" : keyValuePair.Value.ToString();
        str = str.Replace("%" + keyValuePair.Key + "%", newValue);
      }
      string password = EnDe.DecryptText(dataRow["password"].ToString());
      using (MailMessage message = new MailMessage(from, to)
      {
        Subject = dataRow["subject"].ToString(),
        Body = str,
        IsBodyHtml = true
      })
        new SmtpClient()
        {
          Host = dataRow["host"].ToString(),
          Port = int.Parse(dataRow["port"].ToString()),
          EnableSsl = true,
          DeliveryMethod = SmtpDeliveryMethod.Network,
          UseDefaultCredentials = false,
          Credentials = ((ICredentialsByHost) new NetworkCredential(from.Address, password))
        }.Send(message);
    }
  }
}
