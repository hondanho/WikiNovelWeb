
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WowCommon;
using WowCore;
using WowSQL;


namespace WowModel
{
  public abstract class BaseLayoutModel : ILayoutItem
  {
    private DataRow rowData;

    public BaseLayoutModel(DataRow dr, string fileName)
    {
      this.FieldName = fileName;
      if (dr == null)
        return;
      this.TableName = dr.Get<string>("TABLE_NAME", "");
      this.MaxLength = dr.Get<int>("CHARACTER_MAXIMUM_LENGTH", 50);
      this.Type = (CtrlTyp) dr.Get<int>("controlType");
      this.ParentFieldName = dr.Get<string>("parentFieldName", "");
      this.IsParameter = dr.Get<bool>("isParameter");
      this.Dropdown = dr.Get<string>("dropdown", "");
      if (string.IsNullOrEmpty(dr.Get<string>("customDropDown", "")) && string.IsNullOrEmpty(this.Dropdown) && this.Type == CtrlTyp.DropDown)
        this.Type = CtrlTyp.Int;
      this.DefaultValue = dr.Get<object>("defaultValue");
      this.ReadOnly = dr.Get<bool>("readOnly");
      this.Title = dr.Get<string>("title", "");
      this.HtmlClass = dr.Get<string>("html_class", "");
      this.SubTitle = dr.Get<string>("subTitle", "");
      this.HideEditor = dr.Get<bool>("hideEditor");
      this.HideColumn = dr.Get<bool>("hideColumn");
      this.RowSpan = dr.Get<int>("rowSpan", 3);
    }

    public string TableName { get; }

    public string FieldName { get; set; }

    public string Title { get; set; }

    public string HtmlClass { get; set; }

    public string SubTitle { get; set; }

    public bool HideEditor { get; set; }

    public bool HideColumn { get; set; }

    public int RowSpan { get; set; }

    public string FieldName_image_name => this.FieldName + "_image_name";

    public string FieldName_image_tag => this.FieldName + "_image_tag";

    public string FieldName_image_path => this.FieldName + "_image_path";

    public string FieldName_smnall_path => this.FieldName + "_small_path";

    public string FieldName_image_title => this.FieldName + "_image_title";

    public string FieldName_image_Size => this.FieldName + "_image_size";

    public string FieldName_image_Width => this.FieldName + "_image_width";

    public string FieldName_image_Height => this.FieldName + "_image_height";

    public string FieldName_file_name => this.FieldName + "_file_name";

    public string FieldName_image_tmp_local_path => this.FieldName + "_image_tmp_local_path";

    public string FieldName_file_tmp_local_path => this.FieldName + "_file_tmp_local_path";

    public string FieldName_file_tmp_modifyDate => this.FieldName + "_file_tmp_modifyDate";

    public bool ReadOnly { set; get; }

    public int GridColumn_MinWidth { set; get; } = 100;

    public bool GridColumn_Visible { set; get; } = true;

    public int GridColumn_Orderby { set; get; } = 1;

    public List<ProcedureSQL> ListColEventChangeProc { get; set; }

    public string ControlName => "_" + this.FieldName;

    public CtrlTyp Type { set; get; } = CtrlTyp.Text;

    public object DefaultValue { set; get; }

    public int MaxLength { set; get; }

    public string ParentFieldName { set; get; }

    public bool IsParameter { set; get; }

    public string Dropdown { set; get; }

    public DataRow ParentRowData
    {
      get => this.rowData;
      set => this.rowData = value;
    }

    public override string ToString() => this.FieldName;

    public DataTable GetDropdownData(
      ILayoutItem layoutItem,
      int parentValue,
      int id,
      int refID,
      bool getAll,
      int UID,
      DataRow rowData = null,
      bool showIDData = false)
    {
      if (string.IsNullOrEmpty(layoutItem.Dropdown))
        throw new Exception("Chưa cấu hình dropdown cho [" + this.TableName + "][" + this.FieldName + "]");
      DataTable source = (DataTable) null;
      string str1 = "";
      if (parentValue > 0)
        str1 = "  WHERE [refId] = " + parentValue.ToString();
      string str2;
      if (layoutItem.Dropdown.ToUpper().StartsWith("SELECT"))
      {
        str2 = string.Format("{0} {1} ORDER BY isnull([priority],9999),ID ", (object) layoutItem.Dropdown, (object) str1);
      }
      else
      {
        if (layoutItem.Dropdown.ToUpper().StartsWith("SP"))
        {
          string dropdown = layoutItem.Dropdown;
          List<object> objectList = new List<object>()
          {
            (object) "@parentID",
            (object) parentValue,
            (object) "@ID",
            (object) id,
            (object) "@refID",
            (object) refID,
            (object) "@getAll",
            (object) getAll,
            (object) "@uid",
            (object) UID
          };
          if (rowData != null)
          {
            foreach (DataColumn dataColumn in rowData.Table.Columns.Cast<DataColumn>())
            {
              if (!(dataColumn.ColumnName == "ID") && !(dataColumn.ColumnName == nameof (refID)))
              {
                objectList.Add((object) ("@" + dataColumn.ColumnName));
                objectList.Add(rowData[dataColumn.ColumnName]);
              }
            }
          }
          string key = dropdown + " " + JsonConvert.SerializeObject((object) objectList);
          if (!Common.DICT_TABLE_SETTING.ContainsKey(key))
          {
            DataTable dataTable = SqlHelper.ExecuteDataTable(dropdown, objectList.ToArray());
            Common.DICT_TABLE_SETTING.Add(key, dataTable);
          }
          return Common.DICT_TABLE_SETTING[key];
        }
        str2 = string.Format("SELECT ID,title FROM {0} {1} ORDER BY isnull([priority],9999),ID ", (object) layoutItem.Dropdown, (object) ((!str1.ToUpper().Contains(" WHERE ") ? str1 + " WHERE " : str1 + " AND ") + " [statusData] = 3"));
      }
      if (source == null)
      {
        if (!Common.DICT_TABLE_SETTING.ContainsKey(str2))
        {
          DataTable dataTable = SqlHelper.ExecuteDataTable(str2);
          Common.DICT_TABLE_SETTING.Add(str2, dataTable);
        }
        return Common.DICT_TABLE_SETTING[str2];
      }
      if (source == null)
        throw new Exception("[GetDropdownData] Không thể lấy dữ liệu cho: " + layoutItem.FieldName);
      if (Common.HIEN_THI_ID_DATA | showIDData && source.Columns.Contains("title") && source.Columns.Contains("ID"))
        source.AsEnumerable().ToList<DataRow>().ForEach((Action<DataRow>) (dr => dr["title"] = (object) ("[" + dr["ID"].ToString() + "] " + (dr["title"] != DBNull.Value ? dr["title"].ToString() : "NULL"))));
      source.AcceptChanges();
      if (parentValue == -1 && UID == -1)
        Common.DICT_TABLE_SETTING[str2] = source;
      return source;
    }

    public virtual void ReloadDataSource(
      int parentValue = -1,
      int id = -1,
      int refID = -1,
      bool getAll = false,
      int UID = -1)
    {
    }

    public List<string> GetListAttributeHtml()
    {
      List<string> listAttributeHtml = new List<string>();
      if (this.ReadOnly)
        listAttributeHtml.Add("readonly");
      if (this.Title.EndsWith("*") || this.Title.EndsWith("(*)") || this.Title.StartsWith("*") || this.Title.StartsWith("(*)"))
        listAttributeHtml.Add("required");
      if (this.Type == CtrlTyp.CheckBox && this.ReadOnly)
        listAttributeHtml.Add("onclick=\"return false; \"");
      if (this.Type == CtrlTyp.Numberic)
        listAttributeHtml.Add("step=\"any\"");
      return listAttributeHtml;
    }
  }
}
