using System;
using System.Net.Http;
using Blazored.LocalStorage;
using GameDock.Client;
using GameDock.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TusDotNetClient;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("public", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddHttpClient("private", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<HttpAuthorizationHandler>();

builder.Services
    .AddScoped<TusClient>()
    .AddScoped<AuthService>()
    .AddScoped<HttpAuthorizationHandler>()
    .AddBlazoredLocalStorage()
    .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("private"));

builder.Services
    .AddAuthorizationCore()
    .AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

await builder.Build().RunAsync();