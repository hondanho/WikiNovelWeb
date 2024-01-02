
using System;
using System.Data;


namespace WowModel
{
  public class UserRight
  {
    public bool IsView { get; set; }

    public bool IsAdd { get; set; }

    public bool IsUpdate { get; set; }

    public bool IsDelete { get; set; }

    public UserRight(DataRow dr)
    {
      this.IsView = dr["isView"] != DBNull.Value && "true1".Contains(dr["isView"].ToString().ToLower());
      this.IsAdd = dr["isAdd"] != DBNull.Value && "true1".Contains(dr["isAdd"].ToString().ToLower());
      this.IsUpdate = dr["isUpdate"] != DBNull.Value && "true1".Contains(dr["isUpdate"].ToString().ToLower());
      this.IsDelete = dr["isDelete"] != DBNull.Value && "true1".Contains(dr["isDelete"].ToString().ToLower());
    }

    public UserRight() => this.IsView = this.IsAdd = this.IsUpdate = this.IsDelete = false;
  }
}
