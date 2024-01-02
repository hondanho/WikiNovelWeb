
using System.Data;


namespace WowCommon
{
  public interface ILayoutItem
  {
    CtrlTyp Type { set; get; }

    string FieldName { get; }

    string Title { set; get; }

    object DefaultValue { set; get; }

    bool ReadOnly { set; get; }

    int GridColumn_MinWidth { set; get; }

    bool GridColumn_Visible { set; get; }

    int GridColumn_Orderby { set; get; }

    int MaxLength { set; get; }

    string ParentFieldName { set; get; }

    string Dropdown { set; get; }

    bool HideEditor { set; get; }

    bool HideColumn { get; set; }

    DataTable GetDropdownData(
      ILayoutItem layoutItem,
      int parentValue,
      int id,
      int refID,
      bool getAll,
      int UID,
      DataRow dataRow = null,
      bool showIDData = false);
  }
}
