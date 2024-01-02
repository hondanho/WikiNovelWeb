
using System.Collections.Generic;


namespace WowCommon.Model
{
  public class CoutryPhoneNumber
  {
    public string CountryCode { set; get; }

    public string PhoneHeaderCode { set; get; }

    public List<string> ListStartPhoneHeader { set; get; }

    public CoutryPhoneNumber(
      string _countryCode,
      string _phoneHeaderCode,
      List<string> _tartPhoneHeader)
    {
      this.CountryCode = _countryCode;
      this.PhoneHeaderCode = _phoneHeaderCode;
      this.ListStartPhoneHeader = _tartPhoneHeader;
    }

    public CoutryPhoneNumber()
    {
    }
  }
}
