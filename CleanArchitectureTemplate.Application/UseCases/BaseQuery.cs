using CleanArchitectureTemplate.Application.Models;
using CleanArchitectureTemplate.Domain.SeedWork;
using CleanArchitectureTemplate.Domain.Services.External;
using MapsterMapper;
using MediatR;

namespace CleanArchitectureTemplate.Application.UseCases;

public abstract class BaseQuery<TResponse> : IRequest<ResponseModel<TResponse>>
{
    public abstract class BaseQueryHandler<TRequest> : IRequestHandler<TRequest, ResponseModel<TResponse>>
        where TRequest : BaseQuery<TResponse>
    {
        protected readonly IMapper Mapper;
        protected readonly IIdentityService IdentityService;
        protected long? UserId => IdentityService.Id;

        protected BaseQueryHandler(IIdentityService identityService, IMapper mapper)
        {
            IdentityService = identityService;
            Mapper = mapper;
        }

        public abstract Task<ResponseModel<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);
    }
}

public abstract class BasePaginatedQuery<TResponse> : IRequest<ResponseModel<PaginatedResult<TResponse>>>
{
    public int PageNumber { get; }
    public int PageSize { get; }
    public string Search { get; }
    public string OrderByColumn { get; }
    public bool IsAsc { get; }

    protected BasePaginatedQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string search = null,
        string orderByColumn = null,
        bool isAsc = false)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Search = search;
        OrderByColumn = orderByColumn;
        IsAsc = isAsc;
    }

    public abstract class BaseQueryHandler<TRequest> : IRequestHandler<TRequest, ResponseModel<PaginatedResult<TResponse>>>
        where TRequest : BasePaginatedQuery<TResponse>
    {
        protected readonly IMapper Mapper;
        protected readonly IIdentityService IdentityService;
        protected BaseQueryHandler(IIdentityService identityService, IMapper mapper)
        {
            IdentityService = identityService;
            Mapper = mapper;
        }

        public abstract Task<ResponseModel<PaginatedResult<TResponse>>> Handle(TRequest request, CancellationToken cancellationToken);
    }
}

