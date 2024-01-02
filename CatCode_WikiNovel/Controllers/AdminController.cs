using CatCode_WikiNovel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WowCommon;
using WowSQL;
using WowCore;
using System.Data;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace CatCode_WikiNovel.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        public string WebRootPath { get { return _webHostEnvironment.WebRootPath; } }

        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment) : base()
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;

        }


        [HttpGet]
        public IActionResult SwitchAdminMode(bool mode, string key)
        {
            //https://localhost:44308/Admin/SwitchAdminMode?mode=True&key=57967b961eafb6cd5c134c40cf8f788e4bced2f9
            if (key == "57967b961eafb6cd5c134c40cf8f788e4bced2f9")
            {

                if (mode)
                {
                    this.SetCookie("is_admin", "1", DateTime.Now.AddDays(1));
                }
                else
                {
                    this.DeleteCookie("is_admin");
                }
            }
            return Redirect("/Truyen/TimKiem");
        }

        [HttpPost]
        public async Task<string> UploadImage(IFormFile file, string tenTruyen)
        {

            if (file == null || file.Length == 0) return "File is null or empty";
            try
            {
                var dtTruyen = SqlHelper.ExecuteDataTable("SELECT * FROM tblTruyen where title_url = @tenTruyen", "@tenTruyen", tenTruyen);
                if (dtTruyen == null || dtTruyen.Rows.Count == 0) return "Find not found: " + tenTruyen;
                string path = dtTruyen.Rows[0]["urlHinhAnh_image_path"]?.ToString();
                string basePath = path;
                if (path.Contains("truyenfree.net") && !string.IsNullOrEmpty(path))
                {
                    if (!path.Contains("?v="))
                    {
                        path = path + "?v=1";
                    }
                    else
                    {
                        try
                        {
                            var arr = path.Split(new string[] { "?v=" }, StringSplitOptions.RemoveEmptyEntries);
                            var v = arr[1].ToString();
                            if (string.IsNullOrEmpty(v))
                            {
                                v = "1";
                            }
                            basePath = arr[0];
                            path = basePath + "?v=" + (int.Parse(v) + 1);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    string pathUpdate = path;
                    if (!pathUpdate.Contains("truyenfree.net"))
                    {
                        pathUpdate = "https://truyenfree.net" + path;
                    }
                    SqlHelper.ExecuteNonQuery("update tblTruyen" +
                        " set urlHinhAnh_image_path = @urlHinhAnh_image_path" +
                        " where title_url = @tenTruyen", "@urlHinhAnh_image_path", pathUpdate, "@tenTruyen", tenTruyen);

                    basePath = basePath.Replace("https://truyenfree.net", "");
                    basePath = WebRootPath + basePath;
                    try
                    {
                        System.IO.File.Delete(basePath);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    basePath = file.FileName;
                    basePath = "/img2/" + basePath;
                    SqlHelper.ExecuteDataTable("update tblTruyen" +
                        " set urlHinhAnh_image_path = @urlHinhAnh_image_path" +
                        " where title_url = @tenTruyen", "@urlHinhAnh_image_path", "https://truyenfree.net" + path, "@tenTruyen", tenTruyen);
                    basePath = WebRootPath + basePath;
                }

                using (var fileStream = new FileStream(basePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return "Update success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}