using CatCode_WikiNovel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WowCommon;
using WowSQL;
using WowCore;
using System.Data;

namespace CatCode_WikiNovel.Controllers
{
    public class FeedbackController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public FeedbackController(ILogger<HomeController> logger) :base()
        {
            _logger = logger;
        }

        #region CreateOrUpdate
        [HttpPost]
        public string Create(IFormCollection data)
        {
           SqlHelper.ExecuteNonQuery("spCreateFeedBack",
               "@soSao", data["soSao"].ToString(),
               "@noiDungPhanHoi", data["noiDungPhanHoi"].ToString(),
               "@url", data["url"].ToString()
               );
            return string.Empty;
        }

        #endregion
    }
}