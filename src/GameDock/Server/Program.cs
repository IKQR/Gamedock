using GameDock.Server.Application;
using GameDock.Server.Hosted;
using GameDock.Server.Infrastructure;
using GameDock.Server.Infrastructure.Database;
using GameDock.Server.TUS;
using GameDock.Server.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using tusdotnet;
using Microsoft.AspNetCore.Identity;

// Host
var builder = WebApplication.CreateBuilder(args);
builder.Logging.UseSerilog();

// Services
builder.Services.AddControllersWithViews();

var mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = int.MaxValue; });

builder.Services
    .ConfigureJson()
    .ConfigureSwagger()
    .RegisterApplication()
    .RegisterInfrastructure(builder.Configuration)
    .ConfigureAuth()
    .MigrateOnStartup();

// Application
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.AddSwaggerEndpoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();
app.UseAuthorization();

app.ConfigureExceptionHandler();

app.MapBuildUpload();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();