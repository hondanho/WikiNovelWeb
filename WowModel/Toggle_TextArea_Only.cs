

namespace WowModel
{
  public class Toggle_TextArea_Only
  {
    public string Placeholder { get; set; }

    public string Title { get; set; }

    public string Id { get; set; }

    public Toggle_TextArea_Only()
    {
    }

    public Toggle_TextArea_Only(string id, string title, string placeholder)
    {
      this.Placeholder = id;
      this.Title = title;
      this.Id = placeholder;
    }
  }
}
