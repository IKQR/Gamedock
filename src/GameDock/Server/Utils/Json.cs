using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace GameDock.Server.Utils;

public static class Json
{
    public static IServiceCollection ConfigureJson(this IServiceCollection services) =>
        services.ConfigureHttpJsonOptions(opt =>
        {
            opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
}