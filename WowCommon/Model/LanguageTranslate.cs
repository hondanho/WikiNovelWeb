
namespace WowCommon.Model
{
  public class LanguageTranslate
  {
    public LanguageTranslate(string t, string k, string v, string langId)
    {
      this.type = t;
      this.key = k;
      this.value = v;
      this.langId = langId;
    }

    public string type { set; get; }

    public string key { set; get; }

    public string value { set; get; }

    public string langId { set; get; }
  }
}
