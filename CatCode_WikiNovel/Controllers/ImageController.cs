using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using WowSQL;

namespace CatCode_WikiNovel.Controllers
{
    [Route("api")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ImageController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string title_url, [FromForm] string key)
        {
            try
            {
                if (key != "Trinh@123")
                {
                    return BadRequest("Lỗi");
                }
                var dt = SqlHelper.ExecuteDataTable("SELECT title_url,urlHinhAnh_image_path FROM tblTruyen where title_url = @title_url", "@title_url", title_url);
                if (dt == null || dt.Rows.Count == 0)
                {
                    return BadRequest("Lỗi không tìm thấy: " + title_url);
                }
                string fileName = string.Format("{0}.png", title_url);
                string? oldPath = dt.Rows[0]["urlHinhAnh_image_path"]?.ToString();
                if (!string.IsNullOrEmpty(oldPath) && oldPath.Contains("-"))
                {
                    var inx_etx = oldPath.Split('-').Last();
                    int inx = 0;
                    int.TryParse(inx_etx.Split('.').First(), out inx);
                    inx++;
                    fileName = string.Format("{0}-{1}.png", title_url, inx);
                }
                var urlHinhAnh_image_path = string.Format("https://truyenfree.net/img2/{0}", fileName);

                if (file == null || file.Length == 0)
                    return BadRequest("Không có tập tin nào được chọn.");

                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img2");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                SqlHelper.ExecuteNonQuery("update tblTruyen set urlHinhAnh_image_path = @urlHinhAnh_image_path where title_url = @title_url", "@title_url", title_url, "@urlHinhAnh_image_path", urlHinhAnh_image_path);
                try
                {
                    if(!string.IsNullOrEmpty(oldPath))
                    {
                        var old_filePath = Path.Combine(uploadsFolder, oldPath.Replace("https://truyenfree.net/img2/", ""));
                        if (System.IO.File.Exists(old_filePath))
                            System.IO.File.Delete(old_filePath);
                    }
                }
                catch (Exception)
                {
                }
                return Ok(new { fileName = fileName });
            }
            catch (Exception ex)
            {
                return BadRequest("Exception: " + ex.Message);
                return BadRequest("Lỗi: " + ex.Message);
            }

        }
    }

}
