
using System;


namespace WowCore
{
  public class GetDataAttribute : Attribute
  {
    public double ID { set; get; }

    public string Key { set; get; }

    public string Description { set; get; }

    public GetDataAttribute(int id, string key, string desc)
    {
      this.ID = (double) id;
      this.Key = key;
      this.Description = desc;
    }
  }
}
