using Microsoft.Extensions.FileProviders;
using System.Data.SqlClient;
using WowCommon;
using WowCore;
using WowSQL;

Methods.CreateDirectory("NLog");
Common.Mode = ModeLog.Web;
Common.IS_WEB = true;
WowCommon.Common.HIEN_THI_ID_WEB = false;
var connection = EConnection.TruyenFree;
WowCommon.Common.EConnection = connection;

SqlHelper.SetDBConnectionInfo("202.92.7.204\\MSSQLSERVER2019,1437", "wikinovel", "sa_wikinovel", "@xuq#xlkAA_host");

Common.UsingCompareUI = false;
SqlHelper.IsCacheParam = false;
Common.HIEN_THI_ID_DATA = false;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseStatusCodePagesWithReExecute("/");
app.UseHsts();
app.UseExceptionHandler("/");
app.Use(async (ctx, next) =>
{
    try
    {
        await next();
        Common.Log(ctx.Response.StatusCode.ToString() + ": " + ctx.Response.ToString());
        if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
        {
            //Re-execute the request so the user gets the error page
            ctx.Request.Path = "/Truyen/TimKiem";
            await next();
        }
        else if (ctx.Response.StatusCode == 403)
        {
            ctx.Request.Path = "/Truyen/TimKiem";
            await next();
        }
    }
    catch (Exception ex)
    {
        Common.Log(ctx.Response.StatusCode.ToString() + ": " + ctx.Response.ToString());
    }

});
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=index}/{tenTruyen?}/{chuong?}");

app.Run();
