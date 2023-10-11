using CleanArchitectureTemplate.Application.DTOs;
using CleanArchitectureTemplate.Application.DTOs.Todos;
using CleanArchitectureTemplate.Application.Models;
using CleanArchitectureTemplate.Domain.Aggregates.ToDos;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.SeedWork;
using CleanArchitectureTemplate.Domain.Services.External;
using MapsterMapper;
using MediatR;

namespace CleanArchitectureTemplate.Application.UseCases.Todos.Queries;

public class GetTodosQuery : BaseQuery<PaginatedResult<TodoDTO>>
{
    public PaginationDto Dto { get; set; }
    
    public GetTodosQuery(PaginationDto dto)
    {
        Dto = dto;
    }
    
    public class GetTodosQueryHandler : BaseQueryHandler<GetTodosQuery>
    {
        private readonly ITodoRepository _todoRepository;

        public GetTodosQueryHandler(ITodoRepository todoRepository,IIdentityService identityService, IMapper mapper) 
            : base(identityService,mapper)
        {
            _todoRepository = todoRepository;
        }

        public override async Task<ResponseModel<PaginatedResult<TodoDTO>>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
        {
            var todos = await _todoRepository.GetPaginatedAsync(
                search: request.Dto.Search,
                pageNumber: request.Dto.PageNumber,
                pageSize: request.Dto.PageSize,
                orderBy: request.Dto.OrderBy);
            
            var result = Mapper.Map<PaginatedResult<TodoDTO>>(todos);
            return ResponseModel<PaginatedResult<TodoDTO>>.Create(ResponseCode.Success, result);
        }
    }
}
