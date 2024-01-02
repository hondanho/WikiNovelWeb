
using System;
using System.Collections.Generic;
using System.Linq;
using WowCore;


namespace WowModel
{
  public class BaseModel
  {
    public string Access_Token;
    private Dictionary<string, object> data;

    public Dictionary<string, object> Data => this.data ?? new Dictionary<string, object>();

    public BaseModel(Dictionary<string, object> data) => this.data = data;

    public List<BaseModel> GetList(
      string access_token,
      string tableName,
      string refID,
      string key)
    {
      List<BaseModel> list = new List<BaseModel>();
      if (string.IsNullOrEmpty(access_token))
        return list;
      List<Dictionary<string, object>> source = Methods.RequestAPI(access_token, "api/list-object", new Dictionary<string, object>()
      {
        {
          "entityName",
          (object) tableName
        },
        {
          nameof (refID),
          (object) refID
        },
        {
          nameof (key),
          (object) key
        }
      });
      list.AddRange(source.Select<Dictionary<string, object>, BaseModel>((Func<Dictionary<string, object>, BaseModel>) (e => new BaseModel(e))));
      return list;
    }

    public BaseModel GetDetail(string access_token, string tableName, int id)
    {
      return new BaseModel(Methods.RequestAPI(access_token, "api/detail-object", new Dictionary<string, object>()
      {
        {
          "entityName",
          (object) tableName
        },
        {
          "ID",
          (object) id
        }
      }).First<Dictionary<string, object>>());
    }

    public bool CreateOrUpdate(string Access_Token, Dictionary<string, object> data)
    {
      try
      {
        this.Data.Remove("error_description");
        Dictionary<string, object> dictionary = Methods.RequestAPI(Access_Token, "api/CreateOrUpdate", data).FirstOrDefault<Dictionary<string, object>>();
        this.data = dictionary;
        return !dictionary.ContainsKey("error_description");
      }
      catch (Exception ex)
      {
        this.Data.Put("error_description", (object) ex.Message);
        return false;
      }
    }
  }
}
