using CleanArchitectureTemplate.Domain.Attributes;

namespace CleanArchitectureTemplate.Domain.Enums;

public enum ResponseCode : int
{
    [ResponseCode(ResponseType.Error)]
    Failed,
    [ResponseCode(ResponseType.Success)]
    Success,
    [ResponseCode(ResponseType.Warning, true)]
    NotExists,
    [ResponseCode(ResponseType.Warning, true)]
    Duplication,
    [ResponseCode(ResponseType.Success, true)]
    SuccessfullyCreated,
    [ResponseCode(ResponseType.Success, true)]
    SuccessfullyUpdated,
    [ResponseCode(ResponseType.Success, true)]
    SuccessfullyDeleted,
    [ResponseCode(ResponseType.Success)]
    SuccessfullyUploaded,
}

public enum ResponseType
{
    Success = 1,
    Info = 2,
    Warning = 3,
    Error = 4
}
