
using System.Data;
using WowCore;


namespace WowModel
{
  public class AccountCookieModel
  {
    public AccountCookieModel()
    {
    }

    public AccountCookieModel(DataRow dr)
    {
      this.ID = dr.Get<int>(nameof (ID), -1);
      this.FullName = dr.Get<string>("title", "");
      this.UserName = dr.Get<string>("userName", "");
      this.Phone = dr.Get<string>("phone", "");
      this.Email = dr.Get<string>("email", "");
      this.IsSupperAdmin = dr.Get<bool>(nameof (IsSupperAdmin));
      this.Avatar = dr.Get<string>("avatar_image_path", "");
      if (string.IsNullOrEmpty(this.Avatar))
        this.Avatar = "/images/default_image/user.png";
      this.IsDemo = dr.Get<bool>("isDemo");
      this.IsSupperAdmin = dr.Get<bool>("isSupperAdmin");
      this.HienThiLoiMoDau = dr.Get<bool>("hienThiLoiMoDau", true);
      this.HienThiKhoiTaoTaiKhoan = dr.Get<bool>("hienThiKhoiTaoTaiKhoan");
      this.GioiTinh = dr.Get<int>("gioiTinh", 1);
      this.PhongBan = dr.Get<string>("title_Department", "");
      this.RememberMenuKhoiTaoTaiKhoan = dr.Get<string>(nameof (RememberMenuKhoiTaoTaiKhoan), "");
    }

    public int ID { set; get; }

    public string FullName { set; get; }

    public string UserName { set; get; }

    public bool IsDemo { set; get; }

    public string Phone { set; get; }

    public string Avatar { set; get; }

    public string Email { set; get; }

    public bool IsSupperAdmin { set; get; }

    public bool HienThiLoiMoDau { set; get; } = true;

    public bool HienThiKhoiTaoTaiKhoan { set; get; }

    public int GioiTinh { set; get; } = 1;

    public string PhongBan { set; get; }

    public int ID5Plans { set; get; }

    public string Title5Plans { set; get; }

    public bool RememberMe { set; get; }

    public string RememberMenuKhoiTaoTaiKhoan { set; get; }
  }
}
