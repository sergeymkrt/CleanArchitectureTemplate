using CleanArchitectureTemplate.Application.Models;
using CleanArchitectureTemplate.Domain.Aggregates.ToDos;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.Services.External;
using MapsterMapper;

namespace CleanArchitectureTemplate.Application.UseCases.Todos.Commands;

public class DeleteTodoCommand : BaseCommand
{
    public long Id { get; set; }
    
    public DeleteTodoCommand(long id)
    {
        Id = id;
    }
    
    
    public class DeleteTodoCommandHandler : BaseCommandHandler<DeleteTodoCommand>
    {
        private readonly ITodoRepository _todoRepository;

        public DeleteTodoCommandHandler(ITodoRepository todoRepository, IIdentityService identityService, IMapper mapper) 
            : base(identityService,mapper)
        {
            _todoRepository = todoRepository;
        }

        public override async Task<ResponseModel> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
        {
            var todo = await _todoRepository.GetByIdAsync(request.Id, enableTracking: true);
            _todoRepository.Remove(todo);
            return ResponseModel.Create(ResponseCode.Success);
        }
    }
}
