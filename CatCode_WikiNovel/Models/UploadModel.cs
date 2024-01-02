using Microsoft.AspNetCore.Mvc;

namespace CatCode_WikiNovel.Models
{
    public class UploadModel
    {
        public IFormFile File { get; set; }
        public string TitleUrl { get; set; }
        public string Key { get; set; }
    }
}