using CleanArchitectureTemplate.Domain.SeedWork;
using MediatR;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Extensions;

public static class MediatorExtensions
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, List<DomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}
