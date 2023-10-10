using CleanArchitectureTemplate.Domain.Aggregates.Users;
using CleanArchitectureTemplate.Infrastructure.Persistence.Contexts;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Repositories;

public class UserRepository: RepositoryBase<User, long>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override IQueryable<User> BuildQuery()
        => Context.Users.AsQueryable();
}
