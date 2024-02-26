using CatCode_WikiNovel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WowCore;
using WowSQL;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Text;
using System.Data;
using Newtonsoft.Json.Linq;

namespace CatCode_WikiNovel.Controllers
{
    public class TruyenController : BaseController
    {
        private readonly ILogger<TruyenController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;
        private DriveService _googleDriveService;
        private readonly IConfiguration _configuration;

        public TruyenController(ILogger<TruyenController> logger, IWebHostEnvironment environment, IConfiguration configuration) : base()
        {
            _logger = logger;
            _hostEnvironment = environment;
            _configuration = configuration;
            if (_googleDriveService == null)
            {
                //phuongthuyyentrinh@gmail.com
                //string jsonCREDENTIALS = "{\"type\": \"service_account\",  \"project_id\": \"truyenfree\",  \"private_key_id\": \"57967b961eafb6cd5c134c40cf8f788e4bced2f9\",  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCXY+nLjf0kuNPP\\nH3316oSQWcKNSRQVIZYSty5i+ROJTFQAID562xKEM4VDUHqfpLsY1TBQnQK1gANO\\n44qWURMn9hr2VZsSQlApiA1pjTyVcDOdFCN5A133FaajdIfc2GqiRfqCZ/DZwlVN\\nLTIEow41wE8eQuepNPTcl7ADdXNrY3YI4nLScpUrE8tis4I24uGJs6gePWhV1Yri\\ndYeuDNJ+nZFzka8xj6blWSl+vzFCHpuKXZiWoZV3LEFJn4uSDK3Ntf9p6qpNOnq2\\nsv2m1xVQDF6sj9dqTBIf1X4FUBGcC29MrBL91gQOP6CAC0sE1f4FnGXRY+nzJWXR\\nlfKLGFNBAgMBAAECggEAIX5p9e6aoJrBxdPmVteFb7hkKkUOS+SL4uUjPqNzto8s\\nEIERwbWhQZ1ycOOn1ZUpDcPw9gze0hAZ60S9Xydd0RwKeBCVh/Mi8CVZLk6gpkFW\\nMf0IIhhkHCvum8BG5M3+vZs04tAjQdJUdAg5RjxbAvpipsPPpCiCuiFXQFSmbg4S\\nrSsN70J+SCWI1qCY+RQixCxojeg1q+V+9tcHzMgqcC4LWGFjLqBrHwgpYzgJNiRY\\nXV6dTC/3x76NrTWvNdvwCcISu0UxlsHC+Ijw0S1zQNFQfGlZ+G7AOEUxJdJl4bZL\\nZhz3ueRF/y07tG10Too5XD9EVGZQS3OiIGxUQOizTwKBgQDFvWBs3Sxov5hzBfRa\\nitQV6kEH3t9Ri2BTrIYldX3gRAvMaAx/LTNtiwfkTQg3MkjFkFSdahDJOZUSVCC+\\n8SgwN+SgQHMS5Pxnnez30cFqNxy60XFzcbBpIO/NzizUti2WXfAeiKlF8Al1yAOU\\nqk2inZHgX+GnC9soS8u1NA5RnwKBgQDD/pqkd1YyBelOPh+2cUVdA0L6Ul6L8+bg\\nkV2bMw8omjzRh2pk0P9wQ15oWVXu2yrHFs44Qfx44ASSO5dZ4f7hAqMmKb38ziL6\\nT8gb2Ncx0Kt3OhnR2w6kBnSYfp3D8e5jjv0/s0dsKSdz7z7q9YqBBcaN0XBhpXYx\\naOWn6jfvHwKBgCL6ifkX8sggxE3siroNftDVnGVRBn39Qq/qf9xWeUrXnqKs8TD6\\nBPPmuSGogsCI05inHGCwJ4IA/p68ZQKB1FMbQAUdAX4hJYkKxaVc5HLuhtWBQSlj\\nvgoKuoDUbNe/1jaYLWapVBA8EuBT3lZI/ey7JNfk/hy3my/4oHNQXwwPAoGBAKUG\\n5hMBYJRyIcX9zRoDOhJdQrIfVPimf4orHBQn4+WeKQOL7+u/hrVyJDXcstyRse08\\nqJr6BKmKho7SmlfWUJQJcnIZx5zrvMvjDW1VIa0SNK4JP+BRgHxf6yDTy+dG+CDg\\nLyDJxaOu60dU9TdFVD/bKxoSdXvipChqWKUQM9SlAoGAL24s6XbW4W0NBg0+Cseu\\nMvUZl0EzER0P7ppSDeK/BAIqXgvJgEcalg2HVOEfjyle6DbE82p9+FjK22B+Npo6\\nUnwbkKx6hBJlA+NvmEF0rycuRoJ/EyF4pqUkJUEWvx4ZRU6JU5ll3aYcbxwIPtwO\\nH4W9+eN2Bni2B3uDy/xG/o8=\\n-----END PRIVATE KEY-----\\n\",  \"client_email\": \"wikinovel-service-account@truyenfree.iam.gserviceaccount.com\",  \"client_id\": \"108740878997144217116\",  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",  \"token_uri\": \"https://oauth2.googleapis.com/token\",  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/wikinovel-service-account%40truyenfree.iam.gserviceaccount.com\"}";

                // wintb
                IConfigurationSection myConfigSection = configuration.GetSection("CREDENTIALS_GG_Drive");
                string jsonCREDENTIALS = ToJsonString(myConfigSection);

                //nguyendongquan@gmail.com
                //string jsonCREDENTIALS_Quan = "{\r\n  \"type\": \"service_account\",\r\n  \"project_id\": \"rare-seeker-328107\",\r\n  \"private_key_id\": \"dd7420bfd64eab114e5f44304c1b25099467200a\",\r\n  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCxfh0QuSeFBl45\\n+sIs/w+Xhj2gC75OUthziYrvLdG/XWaH9OVjP/+ZdThw4Kze+ajI9Iw4tZ2m46GB\\nuw2tHxwPBEYZalTKuOpgflh2Iu5ffFLysX8H2urthaPwyKpabhlRYl8R7OWvyQp3\\nQeKWGmKOne4/eqORhMQzDWNsKu79D6fJ9syntwamypsMGN+bC2IyK7jQ0Vaz+f7V\\n+A7mM/ubyiNeailIxVp4zVnGipfX5xNG4jb2GJtVnu9p7D+SFBmN/rc4uTO2kkwy\\nueLIB5stPa7NxImKskY0EqIBVP+/DpsYjdKdMh9/fcTu/F9ZrD0+nn/k6pQn3pzS\\nv35GCBvnAgMBAAECggEAAI7hb5LjXRHxSF0AVePf0CFQ7AdH4/X3MiDU08kIwPCV\\n7XFxarZl+F+iNbhOI4+/ZAsvQllJD45pdjMXhKDYj065RXSCTKrUmycY2HrVaeA4\\nZpqNzdBGMlDlqWJkIKgHGyiKdxkaaso0VretRSki2/n/pjbI/taYNZyJLCxGNnce\\n3Xej6pJDrUJzLoJ1yceWZ/CeJIbGBHBxhLXyaAalWBkU0xvkge4dcjE2WJqWRGKp\\nRZa3EYMxX7kjsEzTOUwzOxTI5c87zMeSB1MBmB+xdtrpfFmqDzZvr77UHcT4xz9e\\nxeX77FgQeU2qLsKDZSZTEdEVnUVPJxjYG8ED7QZx0QKBgQD1lpfm7/FAbdjDcywp\\nowsY6jqjdvbGIHRSTKgVtYuoN2JKQtum6f6fFXNMSfJq1P4Siz2M0DLMBsltI2tE\\nGnk2PHS6TohhitpJuwT+XosK7/2m9o+aHA2arWQZOwJIoAv4lAjyYrzi5bSB5GMS\\nAUhTJTrc23SmeHgNk8lH2erD/wKBgQC5BHfP6ky4XjnV18cc+JDPX0O1uz/6cLLg\\nXLGouKjHtMUscbEYmFE01peYxiYHJWYOtkTGbb9JInnslZI4/w0iKpK9srijbyrE\\nXLeaW48LX9agnvzIt3mbQsY074EHTePFu2KKsXYeiJ0MIyYxLVi+RiMBc0ESnIFm\\nHm6bLAUIGQKBgC0gEKVEbWXTSUMQhIraBI39a+zhTRK2n+kHYNMsWo78LjlK86VF\\n1Z1Ria/43mw22sB5iUO4dbz7ekNtgKYw3xjSHvikuXaC1v05Of+rS40k78yf/V7p\\nzK4hMgVYzGCf19NDECnJbr+aSYKcfn+ucKWmb5xpTpPQCu8jupZTOkntAoGAT7F5\\n/ie49i04EkFIddUnYvtwyeWnEDPdCMFEkR1BLgxSZetu0d5CFFVepSKsnKViSXH/\\nO81n+JOOGvbuLfjGanIWFPnXiCZJMJ2TRDyt58NXSoZQ8g/9pH/lqkWDuWa2dqRZ\\nNM5tx4cmxmSZfdM/h23khCpl3CNnpVn0YIhcfnkCgYEAmYpeP/Rx5Frlnxn1fQRS\\nL4QBcDXMQnwhrKI3sZSJ9a4IiaYbMKoe9TY4ZYvlcFShNZHv0VL7Qq6BbX4kPdFP\\niondQwppnkDxV3F+HlgsgnPcbtjpQfGiuXisNZqdw6ZrWfANlHDlzcBsHZO7q6A2\\nmiWXYUsKmftFR5l1KAwa6Rw=\\n-----END PRIVATE KEY-----\\n\",\r\n  \"client_email\": \"truyenfree-net@rare-seeker-328107.iam.gserviceaccount.com\",\r\n  \"client_id\": \"112416990905494784073\",\r\n  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\r\n  \"token_uri\": \"https://oauth2.googleapis.com/token\",\r\n  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\r\n  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/truyenfree-net%40rare-seeker-328107.iam.gserviceaccount.com\",\r\n  \"universe_domain\": \"googleapis.com\"\r\n}";

                GoogleCredential credential = GoogleCredential.FromJson(jsonCREDENTIALS).CreateScoped(DriveService.Scope.Drive);

                _googleDriveService = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = _configuration["ApplicationName"]
            });
            }
        }

        private string ToJsonString(IConfigurationSection section)
        {
            JObject jsonObject = new JObject();
            foreach (var child in section.GetChildren())
            {
                jsonObject[child.Key] = child.Value;
            }
            return jsonObject.ToString();
        }

        public ActionResult TimKiem(string truyen, string theLoai, string trangThai, string trang)
        {
            return View("Views\\Truyen\\DanhMucTruyen.cshtml", new Dictionary<string, object>()
            {
                {"theLoai", theLoai  }
                ,
                {"truyen", truyen}
                ,
                {"trangThai", trangThai}
                ,
                {"trang", trang}
            });
        }

        public PartialViewResult Pagers(string? total, string? current, string? uriPage)
        {
            return PartialView(@"..\Common\_Pager", new Dictionary<string, object>()
            {
                {"total", total ?? "0"}
                ,
                {"current", current ?? "1"}
                ,
                {"uriPage", uriPage ?? "/"}
            });
        }

        [HttpPost]
        public JsonResult ListChapter(string id, string page)
        {
            try
            {
                var dtChuong = SqlHelper.ExecuteDataTable("sp_dsChuong", "@idTruyen", id, "@page", page);
                return Json(dtChuong.ToDictionaryDataTable());
            }
            catch
            {
            }
            return Json("[]");
        }

        [HttpPost]
        public JsonResult Finds(string truyen, string theLoai,string trang, string trangThai, bool? is_audio)
        {
            truyen = truyen ?? "";
            truyen = truyen.Replace("-"," ");
            Dictionary<string, object> result = new Dictionary<string, object>();
            try
            {
                if(string.IsNullOrEmpty(trang) || !int.TryParse(trang, out int ipage))
                {
                    trang = "1";
                }
                var key = !string.IsNullOrEmpty(truyen) ? Methods.RemoveToneMark(truyen) : truyen;
                key = key ?? "";
                theLoai = theLoai ?? "";
                trangThai = trangThai ?? "";
                var isAdmin = this.GetCookie("is_admin");
                var ds = SqlHelper.ExecuteDataset("sp_TimKiemTruyen"
                    , "@q", key
                    , "@isAdmin", !string.IsNullOrEmpty(isAdmin)
                    , "@theLoai", theLoai
                    , "@trangThai", trangThai
                    , "@page", Convert.ToInt32(trang)
                    , "@is_audio", is_audio.HasValue ? is_audio.Value : DBNull.Value
                );
                var dt = ds.Tables[0];
                result.Add("key", key);
                result.Add("count", dt.Rows.Count);
                var dictResult = dt.ToDictionaryDataTable();
                result.Add("data", dictResult);
                result.Add("total", ds.Tables[1].Rows[0]["total"]);
            }
            catch (Exception ex)
            {
                result.Add("exception", ex.Message);
            }
            return Json(result);
        }

        const string CONTENT_UPDATE = "<center>Nội dung đang được cập nhật vui lòng quay lại sau!</center>";
        [HttpPost]
        public async Task<JsonResult> Contents(string truyen, string chuong)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            try
            {
                var dtChuong = SqlHelper.ExecuteDataTable("sp_NoiDungTruyen"
                    , "@title_url", truyen
                    , "@uriChuong", chuong
                    );
                if (dtChuong == null || !dtChuong.AnyRow())
                {
                    result["noiDungTruyen"] = CONTENT_UPDATE;
                    return Json(result);
                }
                XuLyCookie(dtChuong);
                result = dtChuong.ToDictionaryDataTable().First();
                string ggd_id = (string)result["gid"];
                string ggd_ids = (string)result["gids"];
                result.Remove("gid");
                result.Remove("gids");
                if(!string.IsNullOrEmpty(ggd_id))
                {
                    var txt = await DriveDownloadTextFile(ggd_id);
                    result["type"] = "text";
                    result["noiDungTruyen"] = !string.IsNullOrEmpty(txt) ? txt : CONTENT_UPDATE;
                }
                else if (!string.IsNullOrEmpty(ggd_ids))
                {
                    result["type"] = "images";
                    List<string>? images = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(ggd_ids);
                    result["noiDungTruyen"] = images != null && images.Any() ? images : CONTENT_UPDATE;
                }
                else
                {
                    result["type"] = "not_found";
                    result["noiDungTruyen"] = CONTENT_UPDATE;
                }
                
            }
            catch (Exception ex)
            {
                result["noiDungTruyen"] = "<br><br><h3><center>không tìm thấy nội dung của chương này, Chúng tôi sẽ kiểm tra và cập nhật trong thời gian sớm nhất.</h3></center>";

#if DEBUG
                result["noiDungTruyen"] = "<h3>Exception: " + ex.Message + "</h3> ";
#endif
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult DanhSachTheLoai()
        {
            try
            {
                var dtChuong = SqlHelper.ExecuteDataTable("sp_dsTheLoaiTruyen");
                return Json(dtChuong.ToDictionaryDataTable());
            }
            catch
            {
            }
            return Json("[]");
        }

        [NonAction]
        private void XuLyCookie(DataTable dtChuong)
        {
            int idTruyen = (int)dtChuong.Rows[0]["refID"];
            string uriChuong = (string)dtChuong.Rows[0]["uriChuong"];
            this.SetCookie("c-" + idTruyen, uriChuong);
            var truyenDangDoc = this.GetCookie("truyen-dang-doc");
            truyenDangDoc = truyenDangDoc ?? "";
            var lstTruyenDangDoc = truyenDangDoc.Split(',').Where(i => int.TryParse(i, out int ii)).Select(i => int.Parse(i)).ToList();
            lstTruyenDangDoc.Remove(idTruyen);
            lstTruyenDangDoc.Insert(0, idTruyen);
            truyenDangDoc = String.Join(",", lstTruyenDangDoc);
            this.SetCookie("truyen-dang-doc", truyenDangDoc);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<JsonResult> GetBase64(string id)
        {
            try
            {
                var base64 = await DriveDownloadTextFile(fileId: id);
                var dict = new Dictionary<string, object>()
                {
                    { "srcBase64" , base64},
                };    
                return Json(dict);
            }
            catch
            {
            }
            return Json("{}");
        }

        #region GoogleDrive
        [NonAction]
        public async Task<string> DriveDownloadTextFile(string fileId)
        {
            if (string.IsNullOrEmpty(fileId)) return string.Empty;
            try
            {

                var request = _googleDriveService.Files.Get(fileId);
                using (var stream = new MemoryStream())
                {
                    await request.DownloadAsync(stream);
                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    { 
                        var rs = reader.ReadToEnd();
                        return rs;
                    }
                }
            }
            catch
            {
            }
            return String.Empty;
        }

        [NonAction]
        public async Task<string> DriveDownloadMediaFile(string fileId)
        {
            if (string.IsNullOrEmpty(fileId)) return string.Empty;
            try
            {
                var request = _googleDriveService.Files.Get(fileId);
                using (var stream = new MemoryStream())
                {
                    await request.DownloadAsync(stream);
                    string rs = ConvertToBase64(stream);
                    Console.WriteLine(rs);
                    return rs;
                }
            }
            catch
            {

            }
            return String.Empty;
        }
        public  string ConvertToBase64( Stream stream)
        {
            if (stream is MemoryStream memoryStream)
            {
                return Convert.ToBase64String(memoryStream.ToArray());
            }

            var bytes = new Byte[(int)stream.Length];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, (int)stream.Length);

            return Convert.ToBase64String(bytes);
        }
        #endregion



        #region MyRegion
        public IActionResult Edit()
        {
            return View("\\Views\\Truyen\\Edit.cshtml");
        }

        #endregion
    }
}