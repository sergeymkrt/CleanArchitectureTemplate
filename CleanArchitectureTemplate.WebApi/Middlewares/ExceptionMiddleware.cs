using System.Net;
using System.Text.Json;
using CleanArchitectureTemplate.Application.Models;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.Exceptions;

namespace CleanArchitectureTemplate.WebApi.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _request;
    private readonly IWebHostEnvironment _env;
    
    public ExceptionMiddleware(RequestDelegate request, IWebHostEnvironment env)
    {
        _request = request;
        _env = env;
    }

    public async Task Invoke(HttpContext context) => await InvokeAsync(context);

    async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _request(context);
        }
        catch (Exception error)
        {
            switch (error)
            {
                case DomainException e:
                    await InvokeException(
                        context,
                        HttpStatusCode.BadRequest,
                        ResponseModel<object>.Create(ResponseType.Warning, null, e.Message));
                    break;
                default:
                    await InvokeException(context, HttpStatusCode.InternalServerError, GenerateResponse(error));
                    break;
            }
        }
    }

    private ResponseModel<object> GenerateResponse(Exception ex)
    {
        if (_env.EnvironmentName == "Development" || _env.EnvironmentName == "Develop")
            return ResponseModel<object>.Create(ResponseType.Error, ex.StackTrace, ex.Message);
        return ResponseModel<object>.Create(ResponseType.Error,"An error occurred while processing your request.");
    }

    private static async Task InvokeException(HttpContext context, HttpStatusCode statusCode,
        ResponseModel<object> response)
    {
        context.Response.StatusCode = (int)statusCode;
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase ,WriteIndented = true};
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
