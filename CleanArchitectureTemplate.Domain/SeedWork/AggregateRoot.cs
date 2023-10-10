using System.ComponentModel.DataAnnotations.Schema;
using CleanArchitectureTemplate.Domain.Aggregates.Users;

namespace CleanArchitectureTemplate.Domain.SeedWork;

public abstract class AggregateRoot : RegularEntity, IAggregateRoot, ICreator, IModifier
{
    public User CreatedBy { get; set; }
    public long CreatedById { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public User ModifiedBy { get; set; }
    public long? ModifiedById { get; set; }
    public DateTimeOffset? ModifiedDate { get; set; }

    [NotMapped]
    public long? InitiatorId => ModifiedById ?? CreatedById;

    protected AggregateRoot() { }

    protected AggregateRoot(long id)
    {
        Id = id;
    }

    public bool CheckIsInitiator(long userId)
    {
        return userId == InitiatorId;
    }
}
