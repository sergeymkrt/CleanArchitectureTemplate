using CleanArchitectureTemplate.Application.DTOs.Todos;
using CleanArchitectureTemplate.Application.Models;
using CleanArchitectureTemplate.Domain.Aggregates.ToDos;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.Services.External;
using MapsterMapper;

namespace CleanArchitectureTemplate.Application.UseCases.Todos.Commands;

public class UpdateTodoCommand : BaseCommand
{
    public TodoDTO Dto { get; set; }

    public UpdateTodoCommand(TodoDTO dto)
    {
        Dto = dto;
    }

    public class UpdateTodoCommandHandler : BaseCommandHandler<UpdateTodoCommand>
    {
        private readonly ITodoRepository _todoRepository;

        public UpdateTodoCommandHandler(ITodoRepository todoRepository, IIdentityService identityService, IMapper mapper)
            : base(identityService, mapper)
        {
            _todoRepository = todoRepository;
        }

        public override async Task<ResponseModel> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
        {
            var currentTodo = await _todoRepository.GetByIdAsync(request.Dto.Id, enableTracking: true);
            Mapper.Map(request.Dto, currentTodo);
            return ResponseModel.Create(ResponseCode.Success);
        }
    }
}
