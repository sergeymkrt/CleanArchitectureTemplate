using CleanArchitectureTemplate.Domain.Aggregates.Users;

namespace CleanArchitectureTemplate.Domain.SeedWork;

public interface IModifier
{
    public User ModifiedBy { get; set; }
    public long? ModifiedById { get; set; }
    public DateTimeOffset? ModifiedDate { get; set; }

    public void SetModifier(long userId, DateTimeOffset? date = null)
    {
        ModifiedById = userId;
        ModifiedDate = date ?? DateTimeOffset.UtcNow;
    }
}
