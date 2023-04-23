using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace GameDock.Server.Utils;

public static class Swagger
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen();

        return services;
    }

    public static TBuilder AddSwaggerEndpoint<TBuilder>(this TBuilder app)
        where TBuilder : IApplicationBuilder
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "GameDock V1"); });

        return app;
    }
}