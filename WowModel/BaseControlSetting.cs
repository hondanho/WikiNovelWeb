
using System;
using System.Collections.Generic;
using System.Data;
using WowCore;


namespace WowModel
{
  public class BaseControlSetting
  {
    public static string MaxLengthField = "maxlength=\"200\"";
    private int? _iDData;
    public bool AutoFormat = true;

    public BaseControlSetting()
    {
      if (!string.IsNullOrEmpty(this.TableName) || this.Data == null || this.Data.Table == null || string.IsNullOrEmpty(this.Data.Table.TableName))
        return;
      this.TableName = this.Data.Table.TableName;
    }

    public BaseControlSetting(
      string id,
      DataRow dr,
      string placeholder,
      DataTable dtDropDown,
      string valueMember = "ID",
      string displayMember = "title")
      : this(id, dr, placeholder, false)
    {
      this.DropDownData = dtDropDown;
      this.ValueMember = valueMember;
      this.DisplayMember = displayMember;
    }

    public BaseControlSetting(string id, DataRow dr, string placeholder, bool _autoFormat = false)
      : this(id, dr, placeholder)
    {
      this.AutoFormat = _autoFormat;
    }

    public BaseControlSetting(string id, DataRow dr, string placeholder)
      : this()
    {
      this.Placeholder = placeholder;
      this.Data = dr;
      this.Id = id;
    }

    public string Id { get; set; }

    public string TableName { get; set; }

    public string OnAddFunction { get; set; }

    public string ValueMember { get; set; }

    public string DisplayMember { get; set; }

    public DataTable DropDownData { get; set; }

    public bool SelectedFirstValue { get; set; }

    public bool AddAllItem { get; set; }

    public DataRow Data { get; set; }

    public Dictionary<string, object> Dict { get; set; }

    public string Value
    {
      get
      {
        return this.Dict == null && this.Data == null || string.IsNullOrEmpty(this.Id) ? this.DefaultValue : this.GetCellValue(this.Id).ToString();
      }
    }

    public string DefaultValue { get; set; }

    public bool IsParameter { get; set; }

    public double MinValue { get; set; } = double.MinValue;

    public double MaxValue { get; set; } = double.MaxValue;

    public string FormatDatepicker { get; set; } = "DD/MM/YYYY";

    public string MinDate { get; set; } = "today";

    public string MaxDate { get; set; } = "";

    private object GetCellValue(string fName, string defaultValue = "")
    {
      if (this.Data != null)
        return (object) this.Data.GetValue(fName, defaultValue);
      if (this.Dict == null)
        return (object) null;
      object obj = this.Dict.getValue(fName);
      return obj == null ? (object) null : (object) obj.ToString();
    }

    public int IDData
    {
      get
      {
        if (!this._iDData.HasValue || !this._iDData.HasValue)
        {
          try
          {
            object cellValue = this.GetCellValue("ID", "-1");
            this._iDData = new int?(string.IsNullOrEmpty(cellValue?.ToString()) ? -1 : Convert.ToInt32(cellValue));
          }
          catch (Exception ex)
          {
            this._iDData = new int?(-1);
          }
        }
        return this._iDData.Value;
      }
      set => this._iDData = new int?(value);
    }

    public string Placeholder { get; set; }

    public string ClassCss { get; set; }

    public string IdCss { get; set; }

    public bool Require { get; set; }

    public string Rows { get; set; } = "auto";

    public string Cols { get; set; } = "auto";

    public string ExtendDataSet { get; set; }

    public string IdListSuggest { get; set; }

    public string Unit { get; set; }

    public double RightUnit { get; set; }

    public int Index { get; set; } = -1;

    public bool Enable { get; set; } = true;

    public string Enable_Attr => !this.Enable ? "disabled" : "";

    public string CallbackMethodSuccess { get; set; }

    public string CallbackMethodError { get; set; }

    public string CallbackMethodOnChange { get; set; }

    public InputGroupType GType { get; set; }

    public bool HideDefaultIconCalendar { get; set; }

    public string InputGroup_IconStart { get; set; }

    public string InputGroup_IconEnd { get; set; }

    public string InputGroup_TextStart { get; set; }

    public string InputGroup_TextEnd { get; set; }
  }
}
