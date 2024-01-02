
using System.Data;


namespace WowModel
{
  public class SettingWeb : BaseSetting
  {
    public SettingWeb(string tableName)
      : base(tableName)
    {
    }

    public SettingWeb(int _formID)
      : base(_formID)
    {
    }

    public override BaseLayoutModel GetLayoutItemModel(DataRow dr, string colName)
    {
      return (BaseLayoutModel) new LayoutWebItem(dr, colName);
    }

    public override void InitAfterContructor(DataTable dt)
    {
    }
  }
}
