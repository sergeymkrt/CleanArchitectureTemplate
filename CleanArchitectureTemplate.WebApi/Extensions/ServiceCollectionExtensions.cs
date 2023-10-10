using System.Reflection;
using System.Text.Json;
using CleanArchitectureTemplate.Application.Behaviours;
using CleanArchitectureTemplate.Application.Converters;
using CleanArchitectureTemplate.Application.Interfaces.Services;
using CleanArchitectureTemplate.Domain.Aggregates.Users;
using CleanArchitectureTemplate.Domain.Services;
using CleanArchitectureTemplate.Domain.Services.External;
using CleanArchitectureTemplate.Infrastructure.Persistence.Behaviours;
using CleanArchitectureTemplate.Infrastructure.Persistence.Contexts;
using CleanArchitectureTemplate.Infrastructure.Persistence.Extensions;
using CleanArchitectureTemplate.Infrastructure.Persistence.Repositories;
using CleanArchitectureTemplate.Infrastructure.Shared.CacheManager;
using CleanArchitectureTemplate.Infrastructure.Shared.Services;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace CleanArchitectureTemplate.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureCore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddMapster();
        services.AddValidatorsFromAssembly(typeof(ILookupService).GetTypeInfo().Assembly);
        services.AddMediatR(typeof(ILookupService).GetTypeInfo().Assembly);
        
        services.AddRepositories();
        services.AddApplicationServices();
        services.AddDomainServices();
        
        return services;
    }
    
    public static IServiceCollection ConfigureControllers(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new StringToIntConverter());
            });

        return services;
    }
    
    public static IServiceCollection ConfigureExternalServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // configure options
        services.AddOptions();
        //services.Configure<TOptions>(configuration.GetSection("Options"));
        
        services.AddScoped<IIdentityService, IdentityService>();
        return services;
    }
    
    public static IServiceCollection ConfigureAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Setting configuration for protected Web Api (Azure Ad)
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        //todo

        services.AddAuthorization();

        return services;
    }
    
    public static IServiceCollection ConfigureCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var corsDomains = configuration
            .GetSection("ClientSettings:CORSDomains")
            .GetChildren()
            .Select(i => i.Value)
            .ToArray();

        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowAllOrigins",
                p => p
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            options.AddPolicy(
                "AppCORSPolicy",
                p => p
                    .WithOrigins(corsDomains)
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            options.DefaultPolicyName = "AllowAllOrigins";
        });

        return services;
    }
    
    public static IServiceCollection ConfigureSwagger(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!bool.Parse(configuration["SwaggerSettings:Enabled"]))
            return services;

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Fimpact API",
                Description = "Test API",
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        return services;
    }
    
    public static IServiceCollection ConfigureCaching(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // Add Distributed Redis Cache for Session
        if (!environment.IsDevelopment())
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "RedisCache_";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddSession();
        services.AddMemoryCache();

        services.AddSingleton<ICacheProvider, CacheProvider>();
        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }

    public static IServiceCollection ConfigureBehaviours(
        this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        return services;
    }

    public static IServiceCollection ConfigureDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddSqlServerDbContext<ApplicationDbContext>(
            configuration.GetConnectionString("DefaultConnection"),
            environment.IsDevelopment());
        return services;
    }
    
    
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // services.AddScoped<ILookupService, LookupService>();
        return services;
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IBaseDomainService, BaseDomainService>();
        return services;
    }
}
