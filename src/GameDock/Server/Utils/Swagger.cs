using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GameDock.Server.Utils;

public static class Swagger
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(opt =>
        {
            opt.SchemaFilter<EnumSchemaFilter>();
            opt.DescribeAllParametersInCamelCase();
        });

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

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum)
        {
            return;
        }

        model.Enum.Clear();
        Enum.GetNames(context.Type)
            .ToList()
            .ForEach(n => model.Enum.Add(new OpenApiString(n)));
    }
}
