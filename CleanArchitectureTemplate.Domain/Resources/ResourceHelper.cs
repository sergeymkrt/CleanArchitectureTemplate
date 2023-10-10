using CleanArchitectureTemplate.Domain.Attributes;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.Extensions;

namespace CleanArchitectureTemplate.Domain.Resources;

public static class ResourceHelper
{
    public static (ResponseType ResponseType, string Message) GetResource(this ResponseCode responseCode,
        params object[] args)
    {
        var codeAttribute = responseCode.GetAttribute<ResponseCodeAttribute>();
        var message = GetResourceMessageValue(responseCode.ToString());

        if (codeAttribute.IsFormattable && args.Length > 0)
        {
            message = string.Format(message, args);
        }

        return (codeAttribute.ResponseType, message);
    }

    public static string GetResourceMessage(this ResponseCode responseCode, params object[] stringParams) =>
        GetResource(responseCode, stringParams).Message;

    private static string GetResourceMessageValue(string key) => ResourceMessages.ResourceManager.GetString(key) ?? key;
}
