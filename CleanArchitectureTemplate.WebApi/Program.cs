using CleanArchitectureTemplate.Infrastructure.Persistence.Extensions;
using CleanArchitectureTemplate.WebApi.Extensions;
using CleanArchitectureTemplate.WebApi.Middlewares;

// ========================================= Add services to the container. ============================================ //
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ConfigureCore(builder.Configuration)
    .ConfigureControllers(builder.Configuration)
    .ConfigureAuth(builder.Configuration)
    .ConfigureBehaviours()
    .ConfigureExternalServices(builder.Configuration)
    .ConfigureSwagger(builder.Configuration)
    .ConfigureCors(builder.Configuration)
    .ConfigureCaching(builder.Configuration, builder.Environment)
    .ConfigureDbContext(builder.Configuration, builder.Environment);


// ========================================= Configure the HTTP request pipeline ======================================= //
var app = builder.Build();


if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseDatabaseMigration();
app.UseHttpsRedirection();

app.UseSession();

app.UseRequestLogger();
app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionMiddleware();

app.UseSwagger(builder.Configuration);

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    endpoints.MapGet("/", async context =>
    {
        context.Response.Redirect("./swagger", permanent: false);
    });
});

// await SeedInitialDataAsync(app);

app.Run();

// static async Task SeedInitialDataAsync(IApplicationBuilder app)
// {
//     var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
//     using var scope = scopeFactory.CreateScope();
//     var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer>();
//
//     await dbInitializer.SeedAsync();
// }
