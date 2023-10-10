using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureTemplate.Application.Extensions;

public static class MapperExtensions
{
    public static TypeAdapterSetter<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(
        this TypeAdapterSetter<TSource, TDestination> config)
    {
        var sourceProperties = typeof(TSource).GetProperties();
        var destProperties = typeof(TDestination).GetProperties();
        
        foreach (var property in destProperties.Where(dp => !sourceProperties.Any(sp => sp.Name == dp.Name)))
        {
            config.Ignore(property.Name);
        }
    
        return config;
    }

    public static void AddMapster(this IServiceCollection services)
    {
        TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(MapperExtensions).Assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}
