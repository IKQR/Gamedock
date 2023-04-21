using GameDock.Application;
using GameDock.Infrastructure;
using GameDock.Server.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.UseSerilog();

builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();

var mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services
    .RegisterApplication()
    .RegisterInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
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

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();