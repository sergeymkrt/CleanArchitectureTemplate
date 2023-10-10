using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    private IMediator _mediator;
    private IMapper _mapper;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>();
}
