using System.Text.Json.Serialization;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.Resources;

namespace CleanArchitectureTemplate.Application.Models;

public class ResponseModel
{
    public ResponseType ResponseType { get; set; }
    public string Message { get; set; }

    public static ResponseModel Create(ResponseCode responseCode, params object[] stringParams)
    {
        var resource = responseCode.GetResource(stringParams);
        return new ResponseModel {ResponseType = resource.ResponseType, Message = resource.Message};
    }

    public static ResponseModel Create(ResponseType responseType, string message) => new() {ResponseType = responseType, Message = message};
    
    [JsonIgnore]
    public bool IsSuccess => ResponseType == ResponseType.Success;
}

public class ResponseModel<TData> : ResponseModel
{
    public TData Data { get; private set; }

    public static ResponseModel<TData> Create(ResponseCode responseCode, TData data = default, params object[] args)
    {
        var resource = responseCode.GetResource(args);
        return new ResponseModel<TData> {ResponseType = resource.ResponseType, Message = resource.Message, Data = data};
    }
    
    public static ResponseModel<TData> Create(ResponseType responseType, string message, TData data = default) =>
        new() {ResponseType = responseType, Message = message, Data = data};
    
    public static ResponseModel<TData> Create(TData data) => new() {ResponseType = ResponseType.Success, Data = data};
}
