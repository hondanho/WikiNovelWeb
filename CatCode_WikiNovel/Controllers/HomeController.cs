using CatCode_WikiNovel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WowCommon;
using WowSQL;
using WowCore;
using System.Data;

namespace CatCode_WikiNovel.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) : base()
        {
            _logger = logger;
        }
 
        public IActionResult Index(string? tenTruyen, string? chuong)
        {
            try
            {
                if (string.IsNullOrEmpty(tenTruyen))
                {
                    var val = this.GetCookie("truyen-dang-doc");
                    return View(new Dictionary<string, object>() { { "truyen-dang-doc", val ?? "" } });
                }
                Common.Log("tenTruyen: " + tenTruyen);

                string? p2 = chuong;
                if (string.IsNullOrEmpty(tenTruyen)) return RedirectToAction("Search");

                var dt = SqlHelper.ExecuteDataTable("SELECT TOP(1) * FROM tblTruyen where title_url = @title_url ", "@title_url", tenTruyen);
                if (dt == null || !dt.AnyRow())
                {
                    dt = SqlHelper.ExecuteDataTable("SELECT TOP(1) * FROM tblTruyen where title_url LIKE N'%'+@title_url+'%' ", "@title_url", tenTruyen);
                    if (dt == null || !dt.AnyRow())
                    {
                        return Redirect("/");
                    }
                }
                int idTruyen = (int)dt.Rows[0]["ID"];

                if (string.IsNullOrEmpty(chuong)) chuong = base.GetCookie("c-" + idTruyen);
                string? titleChuong = null;
                string tableChuong = "tblTruyen_dsChuong_" + idTruyen / 1000;
                var dtChuongDangDoc = SqlHelper.ExecuteDataTable("SELECT TOP(1) N'Chương ' + isnull(nullif(title,''), CAST(chuong as nvarchar(20))) as title FROM " + tableChuong + " where refID = " + idTruyen + " AND  uriChuong = N'" + chuong + "'");
                if (dtChuongDangDoc.AnyRow())
                {
                    titleChuong = dtChuongDangDoc.Rows[0]["title"]?.ToString();
                }

                return View("Views\\Truyen\\ThongTinTruyen.cshtml", new Dictionary<string, object>()
                {
                    {"data", dt},
                    {"uriChuong", chuong},
                    {"p2", p2 ?? ""},
                    {"titleChuong", titleChuong ?? ""},
                });
            }
            catch
            {
                return Redirect("/error?code=400");
            }
        }

        [Route("privacy-policy.html")]
        public IActionResult Privacy()
        {
            return View();
        }
        [Route("terms-of-service.html")]
        public IActionResult TermsOfService()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public string TestConnection()
        {
            string mgs;
            SqlHelper.CheckConnectionInfo(out mgs);
            var dt = SqlHelper.ExecuteDataTable("SELECT TOP(10) * FROM tblTruyen ");
            mgs += "\n" + string.Join("\n", dt.AsEnumerable().Select(dr=> dr["title"]));
            return mgs;
        }
        [Route("reset-cached")]
        public string ResetCached()
        {
            Methods.ResetCached();
            return "reset-cached done";
        }

    }
}