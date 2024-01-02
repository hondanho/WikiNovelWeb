
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
  public abstract class BaseSetting
  {
    private const string sp_GetTableInfor = "sp_GetTableInfor";
    private const string sp_getFormSetting = "sp_getFormSetting";
    private const string sp_SyncFormSetting = "sp_SyncFormSetting";
    private const string p_formID = "@formID";
    private const string COLUMN_NAME = "COLUMN_NAME";
    private const string _small_path = "_small_path";
    private const string _smallWide_path = "_smallWide_path";
    private const string DateTime_Now = "DateTime.Now";
    private const string colEventChangeProc = "colEventChangeProc";
    private const string customDropdown = "customDropdown";
    private const string FieldName = "FieldName";
    private const string Visible = "Visible";
    private const string Orderby = "Orderby";
    private const string Width = "Width";
    private List<string> lstBaseFieldName = new List<string>()
    {
      "refID",
      "uidCreate",
      "createDate",
      "uidModify",
      "modifyDate",
      "uidDelete",
      "deleteDate",
      "visible",
      "visibleHOT",
      "visibleHome",
      "visibleHightlight",
      "priority"
    };
    private List<string> lstBaseSubColumnImage = new List<string>()
    {
      "_image_name",
      "_image_tag",
      "_image_path",
      "_image_title",
      "_image_size",
      "_image_width",
      "_image_height",
      "_image_tmp_local_path"
    };

    public static T GetSetting<T>(string tableName) where T : class
    {
      if (string.IsNullOrEmpty(tableName))
        return default (T);
      if (Common.DICT_BASE_SETTING_LAYOUT.ContainsKey(tableName))
        return (T) Common.DICT_BASE_SETTING_LAYOUT[tableName];
      Common.DICT_BASE_SETTING_LAYOUT[tableName] = (object) (T) Activator.CreateInstance(typeof (T), (object) tableName);
      return (T) Common.DICT_BASE_SETTING_LAYOUT[tableName];
    }

    public DataRow RowMenuSetting { get; }

    public int FormID { get; }

    public string TableName { get; }

    public string Query { get; }

    public string ParentQuery { get; }

    public DataTable TableInfor { get; set; }

    public string ParentTableName { get; set; }

    public string Title { get; set; }

    public List<Dictionary<string, object>> ListDictVisibleIndex { get; set; }

    public List<BaseLayoutModel> Layouts { get; set; }

    public List<BaseLayoutModel> ListParameters { get; set; }

    public List<string> ListStoreProcedure { get; set; }

    public List<string> GroupColumns { get; set; }

    public bool AllowAdd { get; set; } = true;

    public bool AllowDelete { get; set; } = true;

    public bool AllowModify { get; set; } = true;

    public bool LoadAfterShown { get; set; } = true;

    public bool ReloadAfterSave { get; set; }

    public bool ShowEditorLayout { get; set; } = true;

    public string TableName_CoppyFormSetting { get; set; }

    public string ParentTitle { get; set; }

    public string ModalSize { get; set; }

    public byte[] LayoutSteam { set; get; }

    public UserRight Rights { get; set; }

    public Dictionary<string, CtrlTyp> GetControlType()
    {
      Dictionary<string, CtrlTyp> controlType = new Dictionary<string, CtrlTyp>();
      foreach (BaseLayoutModel layout in this.Layouts)
      {
        if (!controlType.ContainsKey(layout.FieldName))
          controlType.Add(layout.FieldName, layout.Type);
      }
      return controlType;
    }

    public BaseSetting(string tableName)
      : this(Convert.ToInt32(SqlHelper.ExecuteDataTable("select ID from tblFormSetting where tableName = @tableName", (object) "@tableName", (object) tableName).Rows[0][0]))
    {
    }

    public BaseSetting(int _formID)
    {
      this.FormID = _formID;
      DataTable dt = SqlHelper.ExecuteDataTable(nameof (sp_getFormSetting), (object) "@formID", (object) this.FormID, (object) "@uid", (object) SqlHelper.UserID);
      if (dt.Rows.Count == 0)
      {
        SqlHelper.ExecuteDataTable(nameof (sp_SyncFormSetting), (object) "@tableName", (object) this.FormID);
        dt = SqlHelper.ExecuteDataTable(nameof (sp_getFormSetting), (object) "@formID", (object) this.FormID, (object) "@uid", (object) SqlHelper.UserID);
      }
      this.RowMenuSetting = dt.Rows[0];
      this.TableName = this.RowMenuSetting["tableName"] == DBNull.Value ? (string) null : this.RowMenuSetting["tableName"].ToString();
      this.ParentTableName = this.RowMenuSetting["parentTableName"] == DBNull.Value ? (string) null : this.RowMenuSetting["parentTableName"].ToString();
      if (SqlHelper.IS_SUPPER_ADMIN)
        this.Rights = new UserRight()
        {
          IsAdd = true,
          IsDelete = true,
          IsUpdate = true,
          IsView = true
        };
      else
        this.Rights = new UserRight(this.RowMenuSetting);
      this.Query = this.RowMenuSetting.Get<string>("query", "");
      this.ParentQuery = this.RowMenuSetting.Get<string>("parentQuery", "");
      this.Title = this.RowMenuSetting.Get<string>("title", "");
      this.ListStoreProcedure = ((IEnumerable<string>) this.RowMenuSetting.Get<string>("storeProcedures", "").Split(',')).Where<string>((System.Func<string, bool>) (i => !string.IsNullOrEmpty(i))).ToList<string>();
      this.LayoutSteam = this.RowMenuSetting["layout"] == DBNull.Value ? (byte[]) null : (byte[]) this.RowMenuSetting["layout"];
      this.GroupColumns = ((IEnumerable<string>) this.RowMenuSetting.Get<string>("groupColumn", "").Split(',')).Where<string>((System.Func<string, bool>) (i => !string.IsNullOrEmpty(i))).ToList<string>();
      this.AllowAdd = this.RowMenuSetting.Get<bool>("allowAdd", true);
      this.AllowDelete = this.RowMenuSetting.Get<bool>("allowDelete", true);
      this.AllowModify = this.RowMenuSetting.Get<bool>("allowModify", true);
      this.LoadAfterShown = this.RowMenuSetting.Get<bool>("loadAfterShown");
      this.ReloadAfterSave = this.RowMenuSetting.Get<bool>("reloadAfterSave");
      this.ShowEditorLayout = this.RowMenuSetting.Get<bool>("showEditorLayout", true);
      this.TableName_CoppyFormSetting = this.RowMenuSetting.Get<string>(nameof (TableName_CoppyFormSetting), "");
      this.ParentTitle = this.RowMenuSetting.Get<string>("parentTitle", this.ParentTableName);
      this.ModalSize = this.RowMenuSetting.Get<string>("modalSize", "lg");
      string str1 = this.RowMenuSetting.Get<string>("visibleIndexColumn", "");
      if (!string.IsNullOrEmpty(str1))
      {
        try
        {
          this.ListDictVisibleIndex = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(str1);
        }
        catch
        {
        }
      }
      this.TableInfor = SqlHelper.ExecuteDataTable(nameof (sp_GetTableInfor), (object) "@formID", (object) this.FormID);
      this.Layouts = this.InitSettingLayout(this.TableInfor, string.IsNullOrEmpty(str1));
      List<string> source = new List<string>();
      if (!string.IsNullOrEmpty(this.Query))
        source.Add(this.Query);
      if (this.ListStoreProcedure != null && this.ListStoreProcedure.Any<string>())
        source.AddRange((IEnumerable<string>) this.ListStoreProcedure);
      if (source.Any<string>())
      {
        this.ListParameters = new List<BaseLayoutModel>();
        foreach (string str2 in source)
          this.ListParameters.AddRange(this.InitSettingLayout(SqlHelper.ExecuteDataTable("sp_getExportParameter_All", (object) "@ProcedureName", (object) str2)).Where<BaseLayoutModel>((System.Func<BaseLayoutModel, bool>) (p => !this.ListParameters.Any<BaseLayoutModel>((System.Func<BaseLayoutModel, bool>) (pa => pa.FieldName == p.FieldName)))));
        if (this.GetType().Name != typeof (SettingWeb).Name)
        {
          foreach (BaseLayoutModel layout in this.Layouts)
          {
            BaseLayoutModel it = layout;
            BaseLayoutModel baseLayoutModel = this.ListParameters.FirstOrDefault<BaseLayoutModel>((System.Func<BaseLayoutModel, bool>) (i => i.FieldName == "@" + it.FieldName));
            if (baseLayoutModel != null)
            {
              baseLayoutModel.Dropdown = it.Dropdown;
              baseLayoutModel.ParentFieldName = it.ParentFieldName;
              baseLayoutModel.MaxLength = it.MaxLength;
              baseLayoutModel.Type = it.Type;
            }
          }
        }
      }
      this.Layouts = this.Layouts.Where<BaseLayoutModel>((System.Func<BaseLayoutModel, bool>) (il => !il.IsParameter)).ToList<BaseLayoutModel>();
      this.InitAfterContructor(dt);
    }

    public abstract void InitAfterContructor(DataTable dt);

    private List<BaseLayoutModel> InitSettingLayout(DataTable dt, bool hideBaseColumn = true)
    {
      List<BaseLayoutModel> source = new List<BaseLayoutModel>();
      foreach (DataRow row in (InternalDataCollectionBase) dt.Rows)
      {
        string colName = row["COLUMN_NAME"].ToString();
        if (!colName.EndsWith("_small_path") && !colName.EndsWith("_smallWide_path") && !(colName.ToLower() == "@uid"))
        {
          BaseLayoutModel item = this.GetLayoutItemModel(row, colName);
          if (!this.AllowModify && !colName.StartsWith("@"))
            item.ReadOnly = true;
          if (item.DefaultValue?.ToString() == "DateTime.Now")
            item.DefaultValue = (object) DateTime.Now;
          string str1 = row.GetValue("colEventChangeProc");
          if (!string.IsNullOrEmpty(str1))
          {
            List<string> list = ((IEnumerable<string>) str1.ToString().Split(',')).Where<string>((System.Func<string, bool>) (i => !string.IsNullOrEmpty(i.Trim()))).ToList<string>();
            if (list.Any<string>())
              item.ListColEventChangeProc = this.CreateListColEventChangeProc(list);
          }
          string str2 = row.GetValue("customDropdown");
          string str3 = !string.IsNullOrEmpty(str2) ? str2.Trim() : (string) null;
          item.Dropdown = string.IsNullOrEmpty(str3) || str3.Trim().ToLower().StartsWith("select ") ? item.Dropdown : str3;
          if (item.Dropdown.Contains(" ") && !item.Dropdown.Trim().ToLower().StartsWith("select "))
            item.Dropdown = item.Dropdown.Split(' ')[0];
          if (item.FieldName.StartsWith("@") && item.FieldName.Length > 5)
          {
            DataTable dt1 = SqlHelper.ExecuteDataTable("SELECT TOP(1) TABLE_NAME   FROM information_schema.tables WHERE   TABLE_NAME in ( '" + ("tbl" + item.FieldName.Substring(3)) + "' ,'" + ("cbb_" + item.FieldName.Substring(3)) + "' )");
            if (dt1.AnyRow())
            {
              item.Dropdown = dt1.Rows[0]["TABLE_NAME"]?.ToString();
              item.Type = CtrlTyp.DropDown;
            }
          }
          if (item.Type == CtrlTyp.NONE)
            item.Type = this.GetDefaultTypeControl(row, item);
          if (string.IsNullOrEmpty(item.Dropdown) && item.Type == CtrlTyp.DropDown)
            item.Type = CtrlTyp.Int;
          this.PrepareGridColumnSetting(item);
          if (hideBaseColumn && item.GridColumn_Visible && (this.isBaseColumn(item.FieldName) || this.isSubColumnImage(item.FieldName)))
            item.GridColumn_Visible = false;
          if (source.Any<BaseLayoutModel>((System.Func<BaseLayoutModel, bool>) (i => i.FieldName == item.FieldName)))
            throw new Exception("Trùng lắp dữ liệu [" + item.FieldName + "], vùi lòng liên hệ admin");
          source.Add(item);
        }
      }
      return source;
    }

    public abstract BaseLayoutModel GetLayoutItemModel(DataRow dr, string colName);

    private List<ProcedureSQL> CreateListColEventChangeProc(List<string> lstProc)
    {
      List<ProcedureSQL> colEventChangeProc = new List<ProcedureSQL>();
      foreach (string spName in lstProc.Where<string>((System.Func<string, bool>) (i => !string.IsNullOrEmpty(i))))
        colEventChangeProc.Add(new ProcedureSQL(spName));
      return colEventChangeProc;
    }

    public virtual void PrepareGridColumnSetting(BaseLayoutModel item)
    {
      if (this.ListDictVisibleIndex == null || !this.ListDictVisibleIndex.Any<Dictionary<string, object>>())
        return;
      Dictionary<string, object> dict = this.ListDictVisibleIndex.Where<Dictionary<string, object>>((System.Func<Dictionary<string, object>, bool>) (i => i.ContainsKey("FieldName") && i["FieldName"].ToString() == item.FieldName)).FirstOrDefault<Dictionary<string, object>>();
      if (dict != null)
      {
        item.GridColumn_Visible = dict.getValueTemplate<bool>("Visible", false);
        item.GridColumn_MinWidth = dict.getValueTemplate<int>("Width", 100);
        item.GridColumn_Orderby = dict.getValueTemplate<int>("Orderby", 999);
      }
      else
      {
        item.GridColumn_Visible = false;
        item.GridColumn_Orderby = 999;
        item.GridColumn_MinWidth = 100;
      }
    }

    private bool isBaseColumn(string fieldName)
    {
      return this.lstBaseFieldName.Any<string>((System.Func<string, bool>) (i => i.ToLower() == fieldName.ToLower()));
    }

    public bool isSubColumnImage(string fieldName)
    {
      return this.lstBaseSubColumnImage.Any<string>((System.Func<string, bool>) (i => fieldName.ToLower().EndsWith(i))) || Common.DicLanguageSupport.Any<KeyValuePair<int, string>>((System.Func<KeyValuePair<int, string>, bool>) (l => fieldName.ToLower().EndsWith("_" + l.Value)));
    }

    internal CtrlTyp GetDefaultTypeControl(DataRow dr, BaseLayoutModel item)
    {
      switch (dr["DATA_TYPE"].ToString())
      {
        case "bit":
          return CtrlTyp.CheckBox;
        case "date":
          return CtrlTyp.Date;
        case "datetime":
          return CtrlTyp.DateTime;
        case "float":
          return CtrlTyp.Numberic;
        case "int":
          return !item.FieldName.StartsWith("id") ? CtrlTyp.Int : CtrlTyp.DropDown;
        case "nvarchar":
          return item.MaxLength != -1 ? CtrlTyp.Text : CtrlTyp.Memo;
        case "time":
          return CtrlTyp.Time;
        case "varbinary":
          return CtrlTyp.Picture;
        default:
          return CtrlTyp.Text;
      }
    }

    public override string ToString()
    {
      return string.Format("FormID:{0}  TableName:{2}", (object) this.FormID, (object) this.TableName);
    }

    public List<T> GetListLayout<T>() => this.Layouts.Cast<T>().ToList<T>();

    public List<T> GetListParameter<T>() => this.ListParameters.Cast<T>().ToList<T>();

    public void InitListLayout(DataTable dt, int idLanguage)
    {
      if (this.Layouts != null && this.Layouts.Count == dt.Columns.Count)
        return;
      this.Layouts = new List<BaseLayoutModel>();
      foreach (DataColumn column in (InternalDataCollectionBase) dt.Columns)
      {
        BaseLayoutModel layoutItemModel = this.GetLayoutItemModel((DataRow) null, column.ColumnName);
        layoutItemModel.Title = Methods.GetMessage(string.Format("{0}.{1}", (object) this.TableName, (object) column.ColumnName), idLanguage);
        layoutItemModel.Type = Methods.ToControlType(column.DataType);
        this.Layouts.Add(layoutItemModel);
      }
    }

    public void SetupDefaultData(DataRow dr, DataRow drAccount)
    {
      foreach (BaseLayoutModel baseLayoutModel in this.GetListLayout<BaseLayoutModel>().Where<BaseLayoutModel>((System.Func<BaseLayoutModel, bool>) (l => !string.IsNullOrEmpty(l.DefaultValue?.ToString()))))
      {
        if (baseLayoutModel.DefaultValue.ToString().StartsWith("Account."))
        {
          string columnName = baseLayoutModel.DefaultValue.ToString().Split('.')[1];
          object obj = drAccount != null ? (object) drAccount.GetValue(columnName) : (object) (string) null;
          switch (baseLayoutModel.Type)
          {
            case CtrlTyp.CheckBox:
              dr[baseLayoutModel.FieldName] = (object) !"false0".Contains(obj.ToString().ToLower());
              continue;
            case CtrlTyp.DateTime:
            case CtrlTyp.Date:
              dr[baseLayoutModel.FieldName] = (object) Convert.ToDateTime(obj);
              continue;
            case CtrlTyp.DropDown:
            case CtrlTyp.Int:
              dr[baseLayoutModel.FieldName] = (object) Convert.ToInt32(obj);
              continue;
            case CtrlTyp.Numberic:
              dr[baseLayoutModel.FieldName] = (object) Convert.ToDouble(obj);
              continue;
            default:
              dr[baseLayoutModel.FieldName] = obj;
              continue;
          }
        }
        else
        {
          try
          {
            switch (baseLayoutModel.Type)
            {
              case CtrlTyp.CheckBox:
                dr[baseLayoutModel.FieldName] = (object) (baseLayoutModel.DefaultValue == null ? 0 : (!"false0".Contains(baseLayoutModel.DefaultValue.ToString().ToLower()) ? 1 : 0));
                continue;
              case CtrlTyp.DateTime:
              case CtrlTyp.Date:
                dr[baseLayoutModel.FieldName] = (object) Convert.ToDateTime(baseLayoutModel.DefaultValue);
                continue;
              case CtrlTyp.DropDown:
              case CtrlTyp.Int:
                dr[baseLayoutModel.FieldName] = (object) Convert.ToInt32(baseLayoutModel.DefaultValue);
                continue;
              case CtrlTyp.Numberic:
                dr[baseLayoutModel.FieldName] = (object) Convert.ToDouble(baseLayoutModel.DefaultValue);
                continue;
              default:
                dr[baseLayoutModel.FieldName] = baseLayoutModel.DefaultValue;
                continue;
            }
          }
          catch
          {
          }
        }
      }
    }
  }
}
