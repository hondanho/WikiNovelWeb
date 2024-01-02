
using System;


namespace WowCore
{
  public class ApproximatString
  {
    private string s;
    private int i;
    private int j;
    private int k;
    private int loi;
    private int saiSo;

    public ApproximatString(string nhap)
    {
      this.s = nhap;
      this.saiSo = (int) Math.Round((double) this.s.Length * 0.3);
    }

    public bool SoSanh(string s1)
    {
      if (s1.Length < this.s.Length - this.saiSo || s1.Length > this.s.Length + this.saiSo)
        return false;
      for (this.i = this.j = this.loi = 0; this.i < this.s.Length && this.j < s1.Length; ++this.j)
      {
        if ((int) this.s[this.i] != (int) s1[this.j])
        {
          ++this.loi;
          for (this.k = 1; this.k <= this.saiSo; ++this.k)
          {
            if (this.i + this.k < this.s.Length && (int) this.s[this.i + this.k] == (int) s1[this.j])
            {
              this.i += this.k;
              this.loi += this.k - 1;
              break;
            }
            if (this.j + this.k < s1.Length && (int) this.s[this.i] == (int) s1[this.j + this.k])
            {
              this.j += this.k;
              this.loi += this.k - 1;
              break;
            }
          }
        }
        ++this.i;
      }
      this.loi += this.s.Length - this.i + s1.Length - this.j;
      return this.loi <= this.saiSo;
    }
  }
}
