using GameDock.Server.Application;
using GameDock.Server.Hosted;
using GameDock.Server.Infrastructure;
using GameDock.Server.TUS;
using GameDock.Server.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Host
var builder = WebApplication.CreateBuilder(args);
builder.Logging.UseSerilog();

// Services
builder.Services.ConfigureAuth(builder.Configuration);

builder.Services.AddControllersWithViews();

var mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});

builder.Services
    .ConfigureJson()
    .ConfigureSwagger()
    .RegisterApplication()
    .RegisterInfrastructure(builder.Configuration)
    .MigrateOnStartup();

// Application
var app =  builder.Build();

if (app.Environment.IsDevelopment())
{
    app.AddSwaggerEndpoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseHsts();
}
app.UseDeveloperExceptionPage();
app
    .UseHttpsRedirection()
    .ConfigureExceptionHandler()
    .UseBlazorFrameworkFiles()
    .UseStaticFiles()
    .UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBuildUpload();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();