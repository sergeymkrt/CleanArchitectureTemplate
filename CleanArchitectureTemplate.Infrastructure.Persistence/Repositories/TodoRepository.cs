using CleanArchitectureTemplate.Domain.Aggregates.ToDos;
using CleanArchitectureTemplate.Infrastructure.Persistence.Contexts;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Repositories;

public class TodoRepository : RepositoryBase<Todo, long>, ITodoRepository
{
    public TodoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override IQueryable<Todo> BuildQuery() => Context.Todos.AsQueryable();
}
