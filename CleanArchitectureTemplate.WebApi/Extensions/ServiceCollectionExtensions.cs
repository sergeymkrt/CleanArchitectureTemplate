using System.Reflection;
using System.Security.Cryptography;
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
using Microsoft.IdentityModel.Tokens;
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
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(x =>
            {
                x.Cookie.Name = "authorization";
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
        
                var securityKey = new ECDsaSecurityKey(ECDsa.Create(ECCurve.NamedCurves.nistP521));
                securityKey.ECDsa.ImportFromPem(configuration["JWT:PrivateKey"]);
                
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = securityKey
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.ContainsKey("authorization"))
                        {
                            context.Token = context.Request.Cookies["authorization"];
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
            options.AddPolicy("CanPurge", policy => policy.RequireRole(Domain.Enums.Role.Admin.ToString("F"))));

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
            c.SwaggerDoc("v1",new OpenApiInfo{Title="TemplateAPI",Version = "v1"});
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
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
                        }
                    },
                    new string[] {}
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
        // services.AddScoped<IUserRepository, UserRepository>();
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
