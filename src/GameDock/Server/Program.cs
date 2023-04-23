using GameDock.Server.Application;
using GameDock.Server.Infrastructure;
using GameDock.Server.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

builder.Services
    .ConfigureJson()
    .ConfigureSwagger()
    .RegisterApplication()
    .RegisterInfrastructure(builder.Configuration);

// Application
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app
        .AddSwaggerEndpoint()
        .UseWebAssemblyDebugging();
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