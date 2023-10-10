using CleanArchitectureTemplate.Domain.Aggregates.Users;

namespace CleanArchitectureTemplate.Domain.SeedWork;

public interface ICreator
{
    User CreatedBy { get; set; }
    long CreatedById { get; set; }
    DateTimeOffset CreatedDate { get; set; }

    public void SetCreator(long userId, DateTimeOffset? date = null)
    {
        CreatedById = userId;
        CreatedDate = date ?? DateTimeOffset.UtcNow;
    }
}
