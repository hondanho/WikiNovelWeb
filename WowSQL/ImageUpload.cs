
using System.Data;


namespace WowSQL
{
  public class ImageUpload
  {
    public ImageUpload(DataRow _DataRow, string _ColumnName)
    {
      this.DataRow = _DataRow;
      this.ColumnName = _ColumnName;
    }

    public DataRow DataRow { set; get; }

    public string ColumnName { set; get; }
  }
}
