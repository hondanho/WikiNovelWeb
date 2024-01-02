
using System.Collections.Generic;


namespace WowCommon.Model
{
  public class KeyValue
  {
    public string DisplayMember { set; get; }

    public long ValueMember { set; get; }

    public long ParentValue { set; get; } = -1;

    public string TableName { set; get; }

    public bool IsCommon { set; get; }

    public Dictionary<string, KeyValue> ExtendColumn { set; get; }

    public KeyValue(long value, string displaymember)
    {
      this.ValueMember = value;
      this.DisplayMember = displaymember;
      this.ExtendColumn = new Dictionary<string, KeyValue>();
    }

    public KeyValue(long valueParent, long value, string displaymember)
    {
      this.ValueMember = value;
      this.DisplayMember = displaymember;
      this.ParentValue = valueParent;
      this.ExtendColumn = new Dictionary<string, KeyValue>();
    }

    public KeyValue(long value, string displaymember, Dictionary<string, KeyValue> ext)
    {
      this.ValueMember = value;
      this.DisplayMember = displaymember;
      this.ExtendColumn = ext;
    }
  }
}
