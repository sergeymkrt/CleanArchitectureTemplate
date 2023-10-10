using CleanArchitectureTemplate.Application.Models;
using CleanArchitectureTemplate.Domain.Services.External;
using MapsterMapper;
using MediatR;

namespace CleanArchitectureTemplate.Application.UseCases;

public abstract class BaseCommand : IBaseCommand, IRequest<ResponseModel>
{
    public abstract class BaseCommandHandler<TRequest> : IRequestHandler<TRequest, ResponseModel>
        where TRequest : BaseCommand
    {
        protected readonly IMapper Mapper;
        protected readonly IIdentityService IdentityService;
        protected long? UserId => IdentityService.Id;

        protected BaseCommandHandler(IIdentityService identityService, IMapper mapper)
        {
            Mapper = mapper;
            IdentityService = identityService;
        }
        public abstract Task<ResponseModel> Handle(TRequest request, CancellationToken cancellationToken);
    }
}

public abstract class BaseCommand<TResponse> : IBaseCommand, IRequest<ResponseModel<TResponse>>
{
    public abstract class BaseCommandHandler<TRequest> : IRequestHandler<TRequest, ResponseModel<TResponse>>
        where TRequest : BaseCommand<TResponse>
    {
        protected readonly IMapper Mapper;
        protected readonly IIdentityService IdentityService;
        protected long? UserId => IdentityService.Id;

        protected BaseCommandHandler(IIdentityService identityService, IMapper mapper)
        {
            Mapper = mapper;
            IdentityService = identityService;
        }
        public abstract Task<ResponseModel<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);

    }
}

public interface IBaseCommand
{
}
