namespace CleanArchitectureTemplate.WebApi.Middlewares;

public static class TrackReqInformationMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogger(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestsInOutLoggerMiddleware>();
    }
}
