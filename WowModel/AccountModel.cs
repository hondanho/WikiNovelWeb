
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WowCommon;
using WowSQL;


namespace WowModel
{
  public class AccountModel : BaseModel, IDisposable
  {
    public DataTable dtInfo;
    private int _UserId = -1;

    public DataRow RowInfo
    {
      get
      {
        return this.dtInfo == null || this.dtInfo.Rows.Count != 1 || this.dtInfo.Rows[0]["ID"] == DBNull.Value ? (DataRow) null : this.dtInfo.Rows[0];
      }
    }

    public int UserId
    {
      get
      {
        if (this._UserId > 0)
          return this._UserId;
        if (this.dtInfo != null && this.dtInfo.Rows.Count == 1)
        {
          if (this.dtInfo.Rows[0]["ID"] != DBNull.Value)
          {
            try
            {
              this._UserId = int.Parse(this.dtInfo.Rows[0]["ID"].ToString());
            }
            catch
            {
              this._UserId = -1;
            }
            return this._UserId;
          }
        }
        return -1;
      }
    }

    public bool IsSupperAdmin
    {
      get
      {
        if (this.dtInfo != null && this.dtInfo.Rows.Count == 1)
        {
          if (this.dtInfo.Rows[0]["isSupperAdmin"] != DBNull.Value)
          {
            try
            {
              return Convert.ToBoolean(this.dtInfo.Rows[0]["ID"]);
            }
            catch
            {
              return false;
            }
          }
        }
        return false;
      }
    }

    public AccountModel()
      : base((Dictionary<string, object>) null)
    {
      this.dtInfo = (DataTable) null;
    }

    public void CreateWithUid(int uid)
    {
      try
      {
        if (uid <= 0)
          return;
        this.dtInfo = SqlHelper.ExecuteDataTable("sp_getLogin", (object) "@using_uid", (object) uid, (object) "@mode", (object) Common.Mode.ToString());
        if (this.dtInfo.Rows.Count != 0)
          return;
        this.dtInfo = (DataTable) null;
      }
      catch (Exception ex)
      {
        ex.Log();
        this.dtInfo = (DataTable) null;
      }
    }

    public void CreateWithPhonePassword(string phone, string passWord)
    {
      try
      {
        if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(passWord))
          return;
        DataSet dataSet = SqlHelper.ExecuteDataset("sp_GetLogin", (object) "@phone", (object) phone, (object) "@passWord", (object) EnDe.EncryptText(passWord), (object) "@mode", (object) Common.Mode.ToString());
        if (dataSet.Tables.Count > 0)
          this.dtInfo = dataSet.Tables[0];
        if (this.dtInfo.Rows.Count != 0)
          return;
        this.dtInfo = (DataTable) null;
      }
      catch (Exception ex)
      {
        ex.Log();
        this.dtInfo = (DataTable) null;
      }
    }

    public void CreateWithUidPhonePhone(string uidPhone, string phone)
    {
      try
      {
        if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(uidPhone))
          return;
        DataSet dataSet = SqlHelper.ExecuteDataset("sp_GetLogin", (object) "@phone", (object) phone, (object) "@uidPhone", (object) uidPhone, (object) "@mode", (object) Common.Mode.ToString());
        if (dataSet.Tables.Count > 0)
          this.dtInfo = dataSet.Tables[0];
        if (this.dtInfo.Rows.Count != 0)
          return;
        this.dtInfo = (DataTable) null;
      }
      catch (Exception ex)
      {
        ex.Log();
        this.dtInfo = (DataTable) null;
      }
    }

    public void CreateWithUidEmail(string uidEmail, string email)
    {
      try
      {
        if (string.IsNullOrEmpty(uidEmail) || string.IsNullOrEmpty(email))
          return;
        DataSet dataSet = SqlHelper.ExecuteDataset("sp_GetLogin_Email", (object) "@uidEmail", (object) uidEmail, (object) "@mode", (object) Common.Mode.ToString());
        if (dataSet.Tables.Count > 0)
          this.dtInfo = dataSet.Tables[0];
        if (this.dtInfo.Rows.Count != 0)
          return;
        this.dtInfo = (DataTable) null;
      }
      catch (Exception ex)
      {
        ex.Log();
        this.dtInfo = (DataTable) null;
      }
    }

    public void CreateWithUidFB(string uidFB)
    {
      try
      {
        if (string.IsNullOrEmpty(uidFB))
          return;
        DataSet dataSet = SqlHelper.ExecuteDataset("sp_GetLogin_FB", (object) "@uidFB", (object) uidFB, (object) "@mode", (object) Common.Mode.ToString());
        if (dataSet.Tables.Count > 0)
          this.dtInfo = dataSet.Tables[0];
        if (this.dtInfo.Rows.Count != 0)
          return;
        this.dtInfo = (DataTable) null;
      }
      catch (Exception ex)
      {
        ex.Log();
        this.dtInfo = (DataTable) null;
      }
    }

    public void KhoiTaoDataDemo(string idAccountDemo)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "REF_ID",
          (object) new List<string>()
          {
            "tblAccount_ThietLapTCCN",
            "tblAccount_dsTaiSanVaNo",
            "tblAccount_dsLoTrinhNgheNghiep",
            "tblAccount_dsMucTieuTaiChinh",
            "tblAccount_dsDanhMucThuChi_Data_ThucTe",
            "tblAccount_dsDanhMucThuChi",
            "tblAccont_dsSanPhamDauTu",
            "tblAccount_dsTaiKhoanThanhToan",
            "tblAccount_LoTrinhNgheNghiepCuaToi"
          }
        },
        {
          "CHILD_REF_ID",
          (object) new List<string>()
          {
            "tblAccount_dsMucTieuTaiChinh_dsSanPhamDauTu_ThucTe"
          }
        },
        {
          "CHILD_REF_ID_PARENT",
          (object) new List<string>()
          {
            "tblAccount_dsMucTieuTaiChinh"
          }
        }
      };
      foreach (KeyValuePair<string, object> keyValuePair in dictionary)
      {
        string upper = keyValuePair.Key.ToUpper();
        if (!(upper == "CHILD_REF_ID_PARENT") && !string.IsNullOrEmpty(upper))
        {
          List<string> stringList = (List<string>) keyValuePair.Value;
          foreach (string str1 in stringList)
          {
            SqlHelper.ExecuteNonQuery(string.Format("DELETE [{0}] where uidCreate = @uidDemo", (object) str1), (object) "@uidDemo", (object) idAccountDemo);
            SqlHelper.ExecuteNonQuery("if not exists(select 1 from INFORMATION_SCHEMA.COLUMNS where table_name = '" + str1 + "' and column_name = 'map_id') alter table " + str1 + " add map_id int");
            SqlHelper.ExecuteNonQuery("if not exists(select 1 from INFORMATION_SCHEMA.COLUMNS where table_name = '" + str1 + "' and column_name = 'is_tmp_data') alter table " + str1 + " add is_tmp_data bit");
            List<string> list = SqlHelper.ExecuteDataTable("select column_name, data_type  from INFORMATION_SCHEMA.COLUMNS  where table_name = '" + str1 + "' ").AsEnumerable().Select<DataRow, string>((System.Func<DataRow, string>) (dr => dr["column_name"].ToString())).ToList<string>().Where<string>((System.Func<string, bool>) (c => c.ToUpper() != "ID")).ToList<string>().Where<string>((System.Func<string, bool>) (c => c.ToUpper() != "REFID")).ToList<string>().Where<string>((System.Func<string, bool>) (c => c.ToLower() != "map_id".ToLower())).ToList<string>().Where<string>((System.Func<string, bool>) (c => c.ToLower() != "is_tmp_data".ToLower())).ToList<string>().Where<string>((System.Func<string, bool>) (c => c.ToLower() != "uidCreate".ToLower())).ToList<string>();
            switch (upper)
            {
              case "REF_ID":
                string.Format("if not exists(select 1 from [{0}] where uidCreate = @uidDemo)\n insert into [{0}](is_tmp_data,map_id, refID ,uidCreate  , {1})\n select 1 as is_tmp_data,[ID],@uidDemo as refID, @uidDemo as uidCreate , {2}\n from [{0}]\n where refID = @uidGUSTE", (object) str1, (object) string.Join(",", list.Select<string, string>((System.Func<string, string>) (c => "[" + c + "]"))), (object) string.Join(",", list.Select<string, string>((System.Func<string, string>) (c => "[" + c + "]"))));
                continue;
              case "CHILD_REF_ID":
                string str2 = ((List<string>) dictionary["CHILD_REF_ID_PARENT"])[stringList.IndexOf(str1)];
                string.Format("if not exists(select 1 from [{0}] where uidCreate = @uidDemo) \n insert into [{0}](is_tmp_data,map_id, refID  ,uidCreate , {1})\n select 1 as is_tmp_data, a.[ID],p.ID as refID , @uidDemo as uidCreate, {2}\n from [{0}] as a  \n inner join [{3}] as p on a.ID = p.map_id\n where a.refID = @uidGUSTE", (object) str1, (object) string.Join(",", list.Select<string, string>((System.Func<string, string>) (c => "[" + c + "]"))), (object) string.Join(",", list.Select<string, string>((System.Func<string, string>) (c => "a.[" + c + "]"))), (object) str2);
                continue;
              default:
                continue;
            }
          }
        }
      }
    }

    public void Dispose()
    {
      this._UserId = -1;
      this.dtInfo = (DataTable) null;
    }
  }
}
