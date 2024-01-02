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

var configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
           .AddEnvironmentVariables()
           .Build();

// Now you can use Configuration anywhere in your Program class
//SqlHelper.connectionString = configuration["SQL_CONNECTION"];
SqlHelper.SetDBConnectionInfo(
    configuration["SQL_CONNECTION_JSON:Server"],
    configuration["SQL_CONNECTION_JSON:Database"],
    configuration["SQL_CONNECTION_JSON:User"],
    configuration["SQL_CONNECTION_JSON:Password"]
);

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
