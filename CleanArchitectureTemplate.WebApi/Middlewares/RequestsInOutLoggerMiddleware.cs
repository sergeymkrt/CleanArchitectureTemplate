namespace CleanArchitectureTemplate.WebApi.Middlewares;

public class RequestsInOutLoggerMiddleware
{
    private readonly ILogger<RequestsInOutLoggerMiddleware> _logger;
    private readonly RequestDelegate _next;

    public RequestsInOutLoggerMiddleware(RequestDelegate next, ILogger<RequestsInOutLoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Value.Contains("/api/"))
        {
            // Log the request
            var url = context.Request.Path.Value;
            var queryString = context.Request.QueryString.Value;

            var requestBodyStream = new MemoryStream();
            var requestBodyCopy = context.Request.Body;

            await context.Request.Body.CopyToAsync(requestBodyStream);
            requestBodyStream.Seek(0, SeekOrigin.Begin);

            var requestBodyString = new StreamReader(requestBodyStream).ReadToEnd();
            _logger.LogTrace("Request URL: {Url} - QueryString: {QueryString} - Body: {RequestBodyString}", url, queryString, requestBodyString);

            requestBodyStream.Seek(0, SeekOrigin.Begin);
            context.Request.Body = requestBodyStream;

            // Start the response log
            var responseBodyCopy = context.Response.Body;

            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await _next(context);

            context.Request.Body = requestBodyCopy;

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

            _logger.LogTrace("Response body: {ResponseBody}", responseBody);

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(responseBodyCopy);
        }
        else
        {
            await _next(context);
        }
    }
}
