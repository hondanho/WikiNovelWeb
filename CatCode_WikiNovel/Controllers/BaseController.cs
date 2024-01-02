using CatCode_WikiNovel.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using WowCommon;
using WowCore;
using WowSQL;

namespace CatCode_WikiNovel.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
        }
        #region COOKIE

        [NonAction]
        internal void SetCookie(string key, string? val, DateTimeOffset? Expires = null)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (string.IsNullOrEmpty(val))
            {
                DeleteCookie(key);
                return;
            }
            Response.Cookies.Append(key, EnDe.EncryptText(val), new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                Expires = Expires ?? DateTime.Now.AddDays(30),
            });

        }
        [NonAction]
        internal string GetCookie(string key)
        {
            try
            {
                var val = Request.Cookies[key];
                if (string.IsNullOrEmpty(val)) return string.Empty;
                return EnDe.DecryptText(val);
            }
            catch (Exception)
            {

                return string.Empty;
            }

        }
        [NonAction]
        internal void DeleteCookie(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            int maxLoop = 10;
            while (maxLoop > 0 && !string.IsNullOrEmpty(GetCookie(key)))
            {
                maxLoop--;
                Response.Cookies.Delete(key);
            }
        }
        #endregion

    }
}