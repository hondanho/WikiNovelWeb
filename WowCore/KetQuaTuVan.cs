
using System;
using System.Collections.Generic;
using System.Linq;
using WowCommon;


namespace WowCore
{
  public class KetQuaTuVan
  {
    public TuVanHuong itemTuVan;
    public List<string> que_Nam = new List<string>()
    {
      "",
      "Khảm",
      "Ly",
      "Cấn",
      "Đoài",
      "Càn",
      "Khôn",
      "Tốn",
      "Chấn",
      "Khôn"
    };
    public List<string> que_Nu = new List<string>()
    {
      "",
      "Cấn",
      "Càn",
      "Đoài",
      "Cấn",
      "Ly",
      "Khảm",
      "Khôn",
      "Chấn",
      "Tốn"
    };
    public Dictionary<string, List<string>> arr_HuongTot = new Dictionary<string, List<string>>()
    {
      {
        "Càn",
        new List<string>()
        {
          "Tây Bắc (Sinh khí: Phúc lộc vẹn toàn), Tây Nam (Thiên y: Gặp thiên thời, được che chở), Đông Bắc (Diên niên: Mọi sự ổn định), Tây (Phục vị: Được sự giúp đỡ)",
          "Tây Nam (Họa hại: Nhà có hung khí), Đông Bắc (Lục sát: Nhà có sát khí), Tây Bắc (Ngũ quỉ: Gặp tai họa), Tây (Tuyệt mệnh: Chết chóc)",
          "7, 8, 5, 2"
        }
      },
      {
        "Khôn",
        new List<string>()
        {
          "Tây Nam (Sinh khí: Phúc lộc vẹn toàn), Tây Bắc (Thiên y: Gặp thiên thời, được che chở), Tây (Diên niên: Mọi sự ổn định), Đông Bắc (Phục vị: Được sự giúp đỡ)",
          "Đông Bắc (Họa hại: Nhà có hung khí), Tây Nam (Lục sát: Nhà có sát khí), Tây (Ngũ quỉ: Gặp tai họa), Tây Bắc (Tuyệt mệnh: Chết chóc)",
          "8, 7, 2, 5"
        }
      },
      {
        "Cấn",
        new List<string>()
        {
          "Đông Bắc (Sinh khí: Phúc lộc vẹn toàn), Tây (Thiên y: Gặp thiên thời, được che chở), Tây Bắc (Diên niên: Mọi sự ổn định), Tây Nam (Phục vị: Được sự giúp đỡ)",
          "Nam (Họa hại: Nhà có hung khí), Đông (Lục sát: Nhà có sát khí), Bắc (Ngũ quỉ: Gặp tai họa), Đông Nam (Tuyệt mệnh: Chết chóc)",
          "5, 2, 7, 8"
        }
      },
      {
        "Tốn",
        new List<string>()
        {
          "Đông Nam (Sinh khí: Phúc lộc vẹn toàn), Đông (Thiên y: Gặp thiên thời, được che chở), Nam (Diên niên: Mọi sự ổn định), Bắc (Phục vị: Được sự giúp đỡ)",
          "Tây Bắc (Họa hại: Nhà có hung khí), Tây (Lục sát: Nhà có sát khí), Tây Nam (Ngũ quỉ: Gặp tai họa), Đông Bắc (Tuyệt mệnh: Chết chóc)",
          "6, 1, 3, 4"
        }
      },
      {
        "Chấn",
        new List<string>()
        {
          "Đông (Sinh khí: Phúc lộc vẹn toàn), Đông Nam (Thiên y: Gặp thiên thời, được che chở), Bắc (Diên niên: Mọi sự ổn định), Nam (Phục vị: Được sự giúp đỡ)",
          "Bắc (Họa hại: Nhà có hung khí), Đông Nam (Lục sát: Nhà có sát khí), Nam (Ngũ quỉ: Gặp tai họa), Đông (Tuyệt mệnh: Chết chóc)",
          "1, 6, 4, 3"
        }
      },
      {
        "Ly",
        new List<string>()
        {
          "Nam (Sinh khí: Phúc lộc vẹn toàn), Bắc (Thiên y: Gặp thiên thời, được che chở), Đông Nam (Diên niên: Mọi sự ổn định), Đông (Phục vị: Được sự giúp đỡ)",
          "Tây (Họa hại: Nhà có hung khí), Tây Bắc (Lục sát: Nhà có sát khí), Đông Bắc (Ngũ quỉ: Gặp tai họa), Tây Nam (Tuyệt mệnh: Chết chóc)",
          "3, 4, 6, 1"
        }
      },
      {
        "Khảm",
        new List<string>()
        {
          "Bắc (Sinh khí: Phúc lộc vẹn toàn), Nam (Thiên y: Gặp thiên thời, được che chở), Đông (Diên niên: Mọi sự ổn định), Đông Nam (Phục vị: Được sự giúp đỡ)",
          "Đông Nam (Họa hại: Nhà có hung khí), Bắc (Lục sát: Nhà có sát khí), Đông (Ngũ quỉ: Gặp tai họa), Nam (Tuyệt mệnh: Chết chóc)",
          "4, 3, 1, 6"
        }
      },
      {
        "Đoài",
        new List<string>()
        {
          "Tây (Sinh khí: Phúc lộc vẹn toàn), Đông Bắc (Thiên y: Gặp thiên thời, được che chở), Tây Nam (Diên niên: Mọi sự ổn định), Tây Bắc (Phục vị: Được sự giúp đỡ)",
          "Đông (Họa hại: Nhà có hung khí), Nam (Lục sát: Nhà có sát khí), Đông Nam (Ngũ quỉ: Gặp tai họa), Bắc (Tuyệt mệnh: Chết chóc)",
          "2, 5, 8, 7"
        }
      }
    };
    public List<string> que_image_Nam = new List<string>()
    {
      "",
      Common.HOST_URL + "/images/TenQue/kham.png",
      Common.HOST_URL + "/images/TenQue/ly.png",
      Common.HOST_URL + "/images/TenQue/caan.png",
      Common.HOST_URL + "/images/TenQue/doai.png",
      Common.HOST_URL + "/images/TenQue/can.png",
      Common.HOST_URL + "/images/TenQue/khon.png",
      Common.HOST_URL + "/images/TenQue/ton.png",
      Common.HOST_URL + "/images/TenQue/chan.png",
      Common.HOST_URL + "/images/TenQue/khon.png"
    };
    public List<string> que_image_Nu = new List<string>()
    {
      "",
      Common.HOST_URL + "/images/TenQue/caan.png",
      Common.HOST_URL + "/images/TenQue/can.png",
      Common.HOST_URL + "/images/TenQue/doai.png",
      Common.HOST_URL + "/images/TenQue/caan.png",
      Common.HOST_URL + "/images/TenQue/ly.png",
      Common.HOST_URL + "/images/TenQue/kham.png",
      Common.HOST_URL + "/images/TenQue/khon.png",
      Common.HOST_URL + "/images/TenQue/chan.png",
      Common.HOST_URL + "/images/TenQue/ton.png"
    };
    public List<string> que_image_web = new List<string>()
    {
      "",
      Common.HOST_URL + "/images/TenQue/Web/kham.png",
      Common.HOST_URL + "/images/TenQue/Web/khon.png",
      Common.HOST_URL + "/images/TenQue/Web/chan.png",
      Common.HOST_URL + "/images/TenQue/Web/ton.png",
      Common.HOST_URL + "/images/TenQue/Web/T.Cung.png",
      Common.HOST_URL + "/images/TenQue/Web/can.png",
      Common.HOST_URL + "/images/TenQue/Web/doai.png",
      Common.HOST_URL + "/images/TenQue/Web/caan.png",
      Common.HOST_URL + "/images/TenQue/Web/ly.png"
    };
    public List<string> nh_nam = new List<string>()
    {
      "",
      "Thủy",
      "Hỏa",
      "Thổ",
      "Kim",
      "Kim",
      "Thổ",
      "Mộc",
      "Mộc",
      "Thổ"
    };
    public List<string> nh_Nu = new List<string>()
    {
      "",
      "Thổ",
      "Kim",
      "Kim",
      "Thổ",
      "Hỏa",
      "Thủy",
      "Thổ",
      "Mộc",
      "Mộc"
    };
    public List<string> can = new List<string>()
    {
      "Canh",
      "Tân",
      "Nhâm",
      "Quý",
      "Giáp",
      "Ất",
      "Bính",
      "Đinh",
      "Mậu",
      "Kỷ"
    };
    public List<string> chi = new List<string>()
    {
      "Thân",
      "Dậu",
      "Tuất",
      "Hợi",
      "Tý",
      "Sửu",
      "Dần",
      "Mão",
      "Thìn",
      "Tỵ",
      "Ngọ",
      "Mùi"
    };
    public Dictionary<int, List<string>> p_can = new Dictionary<int, List<string>>()
    {
      {
        1,
        new List<string>() { "Giáp", "Ất" }
      },
      {
        2,
        new List<string>() { "Bính", "Đinh" }
      },
      {
        3,
        new List<string>() { "Mậu", "Kỷ" }
      },
      {
        4,
        new List<string>() { "Canh", "Tân" }
      },
      {
        5,
        new List<string>() { "Nhâm", "Quý" }
      }
    };
    public Dictionary<int, List<string>> p_chi = new Dictionary<int, List<string>>()
    {
      {
        0,
        new List<string>() { "Tý", "Sửu", "Ngọ", "Mùi" }
      },
      {
        1,
        new List<string>() { "Dần", "Mão", "Thân", "Dậu" }
      },
      {
        2,
        new List<string>() { "Thìn", "Tỵ", "Tuất", "Hợi" }
      }
    };
    public int diem_Can = -1;
    public int diem_Chi = -1;
    public List<CungMenh> cungMenh = new List<CungMenh>()
    {
      new CungMenh(new List<string>()
      {
        "Giáp tý",
        "Kỷ sửu"
      }, "Hải trung kim"),
      new CungMenh(new List<string>()
      {
        "Bính dần",
        "Đinh mão"
      }, "Lư Trung Hỏa"),
      new CungMenh(new List<string>()
      {
        "Mậu thìn",
        "Kỷ tỵ"
      }, "Đại lâm mộc"),
      new CungMenh(new List<string>()
      {
        "Canh ngọ",
        "Tân mùi"
      }, "Lộ bàng thổ"),
      new CungMenh(new List<string>()
      {
        "Nhâm thân",
        "Quý dậu"
      }, "Kiếm phong kim"),
      new CungMenh(new List<string>()
      {
        "Giáp tuất",
        "Ất hợi"
      }, "Sơn đầu hỏa"),
      new CungMenh(new List<string>()
      {
        "Bính tý",
        "Đinh sửu"
      }, "Giản hạ thủy"),
      new CungMenh(new List<string>()
      {
        "Mậu dần",
        "Kỷ mão"
      }, "Thành đầu thổ"),
      new CungMenh(new List<string>()
      {
        "Canh thìn",
        "Tân tỵ"
      }, "Bạch lạp kim"),
      new CungMenh(new List<string>()
      {
        "Nhân ngọ",
        "Quý mùi"
      }, "Dương liễu mộc"),
      new CungMenh(new List<string>()
      {
        "Giáp thân",
        "Ất dậu"
      }, "Tuyền trung thủy"),
      new CungMenh(new List<string>() { "Mậu tý", "Kỷ sửu" }, "Thích Lịch Hỏa"),
      new CungMenh(new List<string>()
      {
        "Bính tuất",
        "Đinh hợi"
      }, "Ốc thượng thổ"),
      new CungMenh(new List<string>()
      {
        "Canh dần",
        "Tân mão"
      }, "Tùng bách mộc"),
      new CungMenh(new List<string>()
      {
        "Nhâm thìn",
        "Quý tỵ"
      }, "Trường lưu thủy"),
      new CungMenh(new List<string>()
      {
        "Giáp ngọ",
        "Ất mùi"
      }, "Sa trung kim"),
      new CungMenh(new List<string>()
      {
        "Bính thân",
        "Đinh dậu"
      }, "Sơn hạ hỏa"),
      new CungMenh(new List<string>()
      {
        "Mậu tuất",
        "Kỷ hợi"
      }, "Bình địa mộc"),
      new CungMenh(new List<string>()
      {
        "Canh tý",
        "Tân sửu"
      }, "Bích thượng thổ"),
      new CungMenh(new List<string>()
      {
        "Giáp thìn",
        "Ất tỵ"
      }, "Phú đăng hỏa"),
      new CungMenh(new List<string>()
      {
        "Nhâm dần",
        "Quý mão"
      }, "Kim bạch kim"),
      new CungMenh(new List<string>()
      {
        "Bính ngọ",
        "Đinh mùi"
      }, "Thiên hà thủy"),
      new CungMenh(new List<string>()
      {
        "Mậu thân",
        "Kỷ dậu"
      }, "Đại Trạch Thổ"),
      new CungMenh(new List<string>()
      {
        "Canh tuất",
        "Tân hợi"
      }, "Thoa xuyến kim"),
      new CungMenh(new List<string>()
      {
        "Nhâm tý",
        "Quý sửu"
      }, "Tang thạch mộc"),
      new CungMenh(new List<string>()
      {
        "Giáp dần",
        "Ất mão"
      }, "Đại khê thủy"),
      new CungMenh(new List<string>()
      {
        "Bính thìn",
        "Đinh tị"
      }, "Sa trung thổ"),
      new CungMenh(new List<string>()
      {
        "Mậu ngọ",
        "Kỷ mùi"
      }, "Thiên thượng hỏa"),
      new CungMenh(new List<string>()
      {
        "Canh thân",
        "Tân dậu"
      }, "Thạch Lựu mộc"),
      new CungMenh(new List<string>()
      {
        "Nhâm tuất",
        "Quý hợi"
      }, "Thạch Lựu mộc")
    };

    public string hinhAnh { set; get; }

    public string hinhAnh_web { set; get; }

    public string canChi { set; get; }

    public string queMenh { set; get; }

    public string menh { set; get; }

    public string nguHanh { set; get; }

    public string nguHanh_TiengViet { set; get; }

    public string huongTot { set; get; }

    public string listIdHuongTot { set; get; }

    public string huongXau { set; get; }

    public string tuTrach { set; get; }

    public string soTangDeXuat { set; get; }

    public string soTangDeXuat2 { set; get; }

    public string soTangDeXuat3 { set; get; }

    public string mauSac { set; get; }

    public string mauSac2 { set; get; }

    public string mauSac3 { set; get; }

    public Dictionary<string, object> TuVan(string hoVaTen, string ngaySinh, string gioiTinh)
    {
      try
      {
        gioiTinh = !(gioiTinh == "1") ? "Nữ" : "Nam";
        bool flag = !(gioiTinh == "nữ") && !(gioiTinh == "NỮ") && !(gioiTinh == "Nữ") && !(gioiTinh == "0");
        this.itemTuVan = new TuVanHuong(hoVaTen, ngaySinh, gioiTinh);
        if (this.itemTuVan.ngaySinh == "")
          return (Dictionary<string, object>) null;
        int year = this.itemTuVan.ngaySinh.ToDateTime().Year;
        if (ngaySinh.Length == 4)
          year = int.Parse(ngaySinh);
        this.canChi = this.tinhCanChi(year);
        List<string> stringList = new List<string>();
        foreach (char ch in year.ToString())
          stringList.Add(ch.ToString());
        int num1 = (int.Parse(stringList[0]) + int.Parse(stringList[1]) + int.Parse(stringList[2]) + int.Parse(stringList[3])) % 9;
        if (!flag)
          num1 += 4;
        int num2 = num1 % 9;
        if (num2 == 0)
          num2 = 8;
        int index = num2;
        if (index == 5)
          index = flag ? 2 : 8;
        this.queMenh = flag ? this.que_Nam[index] : this.que_Nu[index];
        this.hinhAnh = flag ? this.que_image_Nu[index] : this.que_image_Nu[index];
        this.hinhAnh_web = flag ? this.que_image_Nu[index] : this.que_image_Nu[index];
        this.menh = this.tinhMang(this.itemTuVan);
        this.tuTrach = !new List<string>()
        {
          "Đoài",
          "Cấn",
          "Khôn",
          "Càn"
        }.Contains(this.queMenh) ? "Đông tứ mệnh" : "Tây tứ mệnh";
        this.huongTot = this.arr_HuongTot[this.queMenh][0];
        this.huongXau = this.arr_HuongTot[this.queMenh][1];
        this.listIdHuongTot = this.arr_HuongTot[this.queMenh][2];
        foreach (CungMenh cungMenh in this.cungMenh)
        {
          if (cungMenh.canChi.Where<string>((Func<string, bool>) (i => i.ToLower() == this.canChi.ToLower())).FirstOrDefault<string>() != null)
          {
            this.nguHanh = cungMenh.cungMenh;
            this.nguHanh_TiengViet = this.getCungMentTV(cungMenh.cungMenh);
            break;
          }
        }
        List<int> intList1 = this.tinhSoTangDeXuat(this.menh);
        List<int> intList2 = this.tinhSoTangDeXuat2(this.menh);
        List<int> intList3 = this.tinhSoTangDeXuat3(this.menh);
        this.soTangDeXuat = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7},...", (object) (intList1[0] + 10), (object) (intList1[1] + 10), (object) (intList1[0] + 20), (object) (intList1[1] + 20), (object) (intList1[0] + 30), (object) (intList1[1] + 30), (object) (intList1[0] + 40), (object) (intList1[1] + 40));
        this.soTangDeXuat2 = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7},...", (object) (intList2[0] + 10), (object) (intList2[1] + 10), (object) (intList2[0] + 20), (object) (intList2[1] + 20), (object) (intList2[0] + 30), (object) (intList2[1] + 30), (object) (intList2[0] + 40), (object) (intList2[1] + 40));
        this.soTangDeXuat3 = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7},...", (object) (intList3[0] + 10), (object) (intList3[1] + 10), (object) (intList3[0] + 20), (object) (intList3[1] + 20), (object) (intList3[0] + 30), (object) (intList3[1] + 30), (object) (intList3[0] + 40), (object) (intList3[1] + 40));
        this.mauSac = this.deXuatMauSac(this.menh);
        this.mauSac2 = this.deXuatMauSac2(this.menh);
        this.mauSac3 = this.deXuatMauSac3(this.menh);
        this.nguHanh = this.nguHanh + " - " + this.tuTrach + " - " + this.queMenh;
        this.huongTot = this.huongTot + " - " + this.menh + " - " + this.canChi;
        return this.GetResult();
      }
      catch (Exception ex)
      {
        return new Dictionary<string, object>()
        {
          {
            "status",
            (object) "Thông tin tư vấn không hợp lệ"
          }
        };
      }
    }

    private Dictionary<string, object> GetResult()
    {
      return new Dictionary<string, object>()
      {
        {
          "hinhAnh",
          (object) this.hinhAnh
        },
        {
          "hinhAnh_web",
          (object) this.hinhAnh_web
        },
        {
          "canChi",
          (object) this.canChi
        },
        {
          "queMenh",
          (object) this.queMenh
        },
        {
          "menh",
          (object) this.menh
        },
        {
          "nguHanh",
          (object) this.nguHanh
        },
        {
          "nguHanh_TiengViet",
          (object) this.nguHanh_TiengViet
        },
        {
          "huongTot",
          (object) this.huongTot
        },
        {
          "huongXau",
          (object) this.huongXau
        },
        {
          "tuTrach",
          (object) this.tuTrach
        },
        {
          "soTangDeXuat",
          (object) this.soTangDeXuat
        },
        {
          "soTangDeXuat2",
          (object) this.soTangDeXuat2
        },
        {
          "soTangDeXuat3",
          (object) this.soTangDeXuat3
        },
        {
          "mauSac",
          (object) this.mauSac
        },
        {
          "mauSac2",
          (object) this.mauSac2
        },
        {
          "mauSac3",
          (object) this.mauSac3
        },
        {
          "listIdHuongTot",
          (object) this.listIdHuongTot
        }
      };
    }

    public string tinhCanChi(int namSinh)
    {
      this.diem_Can = namSinh % 10;
      string str = this.can[this.diem_Can];
      this.diem_Chi = namSinh % 12;
      string lower = this.chi[this.diem_Chi].ToLower();
      return string.Format("{0} {1}", (object) str, (object) lower);
    }

    public string tinhMang(TuVanHuong item)
    {
      return new List<string>()
      {
        "",
        "Kim",
        "Thủy",
        "Hỏa",
        "Thổ",
        "Mộc",
        "Kim",
        "Thủy",
        "Hỏa",
        "Thổ",
        "Mộc"
      }[this.p_can.Where<KeyValuePair<int, List<string>>>((Func<KeyValuePair<int, List<string>>, bool>) (p => p.Value.Contains(this.can[this.diem_Can]))).FirstOrDefault<KeyValuePair<int, List<string>>>().Key + this.p_chi.Where<KeyValuePair<int, List<string>>>((Func<KeyValuePair<int, List<string>>, bool>) (p => p.Value.Contains(this.chi[this.diem_Chi]))).FirstOrDefault<KeyValuePair<int, List<string>>>().Key];
    }

    public List<int> tinhSoTangDeXuat(string menh)
    {
      switch (menh)
      {
        case "Hỏa":
          return new List<int>() { 2, 7 };
        case "Mộc":
          return new List<int>() { 3, 8 };
        case "Thổ":
          return new List<int>() { 5, 10 };
        case "Kim":
          return new List<int>() { 4, 9 };
        case "Thủy":
          return new List<int>() { 1, 6 };
        default:
          return new List<int>() { 0, 0 };
      }
    }

    public List<int> tinhSoTangDeXuat2(string menh)
    {
      switch (menh)
      {
        case "Hỏa":
          return new List<int>() { 3, 8 };
        case "Mộc":
          return new List<int>() { 1, 6 };
        case "Thổ":
          return new List<int>() { 2, 7 };
        case "Kim":
          return new List<int>() { 5, 10 };
        case "Thủy":
          return new List<int>() { 4, 9 };
        default:
          return new List<int>() { 0, 0 };
      }
    }

    public List<int> tinhSoTangDeXuat3(string menh)
    {
      switch (menh)
      {
        case "Hỏa":
          return new List<int>() { 4, 9 };
        case "Mộc":
          return new List<int>() { 5, 10 };
        case "Thổ":
          return new List<int>() { 1, 6 };
        case "Kim":
          return new List<int>() { 3, 8 };
        case "Thủy":
          return new List<int>() { 2, 7 };
        default:
          return new List<int>() { 0, 0 };
      }
    }

    public string deXuatMauSac(string menh)
    {
      switch (menh)
      {
        case "Hỏa":
          return "Đỏ, cam đậm, hồng, tím";
        case "Mộc":
          return "Xanh lá cây";
        case "Thổ":
          return "Nâu, vàng đậm, cam đất";
        case "Kim":
          return "Trắng, bạc, vàng kim";
        case "Thủy":
          return "Đen, xám đen, xanh dương";
        default:
          return "";
      }
    }

    public string deXuatMauSac2(string menh)
    {
      switch (menh)
      {
        case "Hỏa":
          return "Xanh lá cây";
        case "Mộc":
          return "Đen, xám đen, xanh dương";
        case "Thổ":
          return "Đỏ, cam đậm, hồng, tím";
        case "Kim":
          return "Nâu, vàng đậm, cam đất";
        case "Thủy":
          return "Trắng, bạc, vàng kim";
        default:
          return "";
      }
    }

    public string deXuatMauSac3(string menh)
    {
      switch (menh)
      {
        case "Hỏa":
          return "Nâu, vàng đậm, cam đất";
        case "Mộc":
          return "Đỏ, cam đậm, hồng, tím";
        case "Thổ":
          return "Trắng, bạc, vàng kim";
        case "Kim":
          return "Đen, xám đen, xanh dương";
        case "Thủy":
          return "Xanh lá cây";
        default:
          return "";
      }
    }

    public string getCungMentTV(string cung)
    {
      switch (cung)
      {
        case "Bình địa mộc":
          return "Gỗ đồng bằng";
        case "Bích thượng thổ":
          return "Đất tò vò";
        case "Bạch lạp kim":
          return "Vàng sáp ong";
        case "Dương liễu mộc":
          return "Gỗ cây dương";
        case "Giản hạ thủy":
          return "Nước cuối nguồn";
        case "Hải trung kim":
          return "Vàng trong biển";
        case "Kim bạch kim":
          return "Vàng pha bạc";
        case "Kiếm phong kim":
          return "Vàng mũi kiếm";
        case "Lư Trung Hỏa":
          return "Lửa trong lò";
        case "Lộ bàng thổ":
          return "Đất đường đi";
        case "Phú đăng hỏa":
          return "Lửa đèn to";
        case "Sa trung kim":
          return "Vàng trong cát";
        case "Sa trung thổ":
          return "Đất pha cát";
        case "Sơn hạ hỏa":
          return "Lửa trên núi";
        case "Sơn đầu hỏa":
          return "Lửa trên núi";
        case "Tang thạch mộc":
          return "Gỗ cây dâu";
        case "Thiên hà thủy":
          return "Nước trên trời";
        case "Thiên thượng hỏa":
          return "Lửa trên trời";
        case "Thoa xuyến kim":
          return "Vàng trang sức";
        case "Thành đầu thổ":
          return "Đất trên thành";
        case "Thích Lịch Hỏa":
          return "Lửa sấm sét";
        case "Thạch Lựu mộc":
          return "Gỗ cây lựu đá";
        case "Trường lưu thủy":
          return "Nước chảy mạnh";
        case "Tuyền trung thủy":
          return "Nước trong suối";
        case "Tùng bách mộc":
          return "Gỗ tùng bách";
        case "Đại Trạch Thổ":
          return "Đất nền nhà";
        case "Đại khê thủy":
          return "Nước khe lớn";
        case "Đại lâm mộc":
          return "Gỗ rừng già";
        case "Ốc thượng thổ":
          return "Đất nóc nhà";
        default:
          return "";
      }
    }
  }
}
