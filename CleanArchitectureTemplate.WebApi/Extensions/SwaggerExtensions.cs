namespace CleanArchitectureTemplate.WebApi.Extensions;

public static class SwaggerExtension
{
    public static void UseSwagger(this IApplicationBuilder app, IConfiguration configuration)
    {
        if (!bool.Parse(configuration["SwaggerSettings:Enabled"]))
            return;

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1"));
    }
}
