using System;
using Destructurama;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;

namespace GameDock.Shared.Utils;

public static class Serilog
{
    private static readonly LoggerConfiguration Configuration = new LoggerConfiguration()
        .Destructure.JsonNetTypes()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .MinimumLevel.Override("HealthChecks", LogEventLevel.Warning)
        .MinimumLevel.Override("Undeads.Microservices.Auth", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithSpan()
        .WriteToConsole();

    public static ILoggingBuilder UseSerilog(this ILoggingBuilder builder) =>
        builder
            .ClearProviders()
            .AddSerilog(Configuration.CreateLogger(), dispose: true);

    private static LoggerConfiguration WriteToConsole(this LoggerConfiguration config) =>
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? DevelopmentWriteToConsole(config)
            : ProductionWriteToConsole(config);

    private static LoggerConfiguration DevelopmentWriteToConsole(LoggerConfiguration config) =>
        config.WriteTo.Async(configuration =>
            configuration.Console(
                theme: AnsiConsoleTheme.Code,
                outputTemplate:
                "{Timestamp:HH:mm:ss} [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}"));


    private static LoggerConfiguration ProductionWriteToConsole(LoggerConfiguration config) =>
        config.WriteTo.Async(configuration =>
            configuration.Console(new JsonFormatter(renderMessage: true)));
}