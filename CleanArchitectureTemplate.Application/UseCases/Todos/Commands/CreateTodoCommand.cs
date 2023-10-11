using CleanArchitectureTemplate.Application.DTOs.Todos;
using CleanArchitectureTemplate.Application.Models;
using CleanArchitectureTemplate.Domain.Aggregates.ToDos;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.Services.External;
using MapsterMapper;
using MediatR;

namespace CleanArchitectureTemplate.Application.UseCases.Todos.Commands;

public class CreateTodoCommand : BaseCommand
{
    public CreateTodoDTO Dto { get; set; }
    
    public CreateTodoCommand(CreateTodoDTO dto)
    {
        Dto = dto;
    }
    
    public class CreateTodoCommandHandler : BaseCommandHandler<CreateTodoCommand>
    {
        private readonly ITodoRepository _todoRepository;

        public CreateTodoCommandHandler(ITodoRepository todoRepository, IIdentityService identityService, IMapper mapper) 
            : base(identityService,mapper)
        {
            _todoRepository = todoRepository;
        }

        public override async Task<ResponseModel> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
        {
            var todo = Mapper.Map<Todo>(request.Dto);
            await _todoRepository.AddAsync(todo);
            return ResponseModel.Create(ResponseCode.Success);
        }
    }
}
