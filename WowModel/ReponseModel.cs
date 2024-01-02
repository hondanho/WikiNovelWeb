
using System.Collections.Generic;


namespace WowModel
{
  public class ReponseModel
  {
    public int statusCode { set; get; }

    public string message { set; get; }

    public string redirect_to { set; get; }

    public bool? reload { set; get; }

    public Dictionary<string, object> data { set; get; }

    public ReponseModel(int _statusCode, string _message, string _redirect_to = null)
    {
      this.statusCode = _statusCode;
      this.message = _message;
      this.redirect_to = _redirect_to;
    }
  }
}
