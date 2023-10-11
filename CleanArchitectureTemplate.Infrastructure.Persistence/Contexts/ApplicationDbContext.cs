using System.Data;
using System.Reflection;
using System.Text.Json;
using CleanArchitectureTemplate.Application.DTOs;
using CleanArchitectureTemplate.Domain.Aggregates.ToDos;
using CleanArchitectureTemplate.Domain.Aggregates.Users;
using CleanArchitectureTemplate.Domain.Extensions;
using CleanArchitectureTemplate.Domain.SeedWork;
using CleanArchitectureTemplate.Domain.Shared;
using CleanArchitectureTemplate.Domain.Shared.Lookups;
using CleanArchitectureTemplate.Infrastructure.Persistence.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext : IdentityDbContext<User,Role,long>, IUnitOfWork
{
    private readonly IMediator _mediator;
    private IDbContextTransaction _currentTransaction;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Ignore<DomainEvent>();
    }

    #region Models
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Todo> Todos { get; set; }
    #endregion

    #region Lookup models
    public DbSet<RoundingRule> RoundingRules { get; set; }
    #endregion

    #region Save changes

    public async Task<int> SaveChangesWithAuditAsync(long userId, Guid scopeId, CancellationToken cancellationToken = default)
    {
        var auditedAddedEntries = ChangeTracker
            .Entries()
            .Where(p => p.State == EntityState.Added)
            .ToList();

        var auditedModifiedEntries = ChangeTracker
            .Entries()
            .Where(p => p.State is EntityState.Modified or EntityState.Deleted)
            .ToList();
        
        SetCreators(auditedAddedEntries, userId);
        SetModifiers(auditedModifiedEntries.FindAll(p => p.State == EntityState.Modified), userId);

        var auditLogs = CreateAuditLogs(auditedModifiedEntries, userId, scopeId);
        var result = await base.SaveChangesAsync(cancellationToken);

        if (result <= 0)
            return result;

        auditLogs.AddRange(CreateAuditLogs(auditedAddedEntries, userId, scopeId));
        AuditLogs.AddRange(auditLogs);
        await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    public async Task<int> SaveChangesAsync(long userId, CancellationToken cancellationToken = default)
    {
        var addedEntries = ChangeTracker
            .Entries()
            .Where(p => p.State == EntityState.Added)
            .ToList();

        var modifiedEntries = ChangeTracker
            .Entries()
            .Where(p => p.State is EntityState.Modified or EntityState.Deleted)
            .ToList();
        
        SetCreators(addedEntries, userId);
        SetModifiers(modifiedEntries.FindAll(p => p.State == EntityState.Modified), userId);

        return await base.SaveChangesAsync(cancellationToken);
    }
    #endregion

    #region Transaction
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (_currentTransaction != null) return null;

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(
        IDbContextTransaction transaction,
        long userId,
        CancellationToken cancellationToken = default)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        if (transaction != _currentTransaction)
            throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            var domainEvents = GetDomainEvents();
            await SaveChangesWithAuditAsync(userId, transaction.TransactionId, cancellationToken);

            await _mediator.DispatchDomainEventsAsync(domainEvents);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public bool HasActiveTransaction => _currentTransaction != null;

    #endregion

    #region Private Methods

    private static void SetCreators(List<EntityEntry> entityEntries, long userId)
    {
        var newEntries = entityEntries.FindAll(x => x.Entity.GetType().IsTypeImplementsAnInterface(typeof(ICreator)));
        if (newEntries.Any())
        {
            newEntries.ForEach(e => (e.Entity as ICreator).SetCreator(userId));
        }
    }

    private static void SetModifiers(List<EntityEntry> entityEntries, long userId)
    {
        var updatedEntries = entityEntries.FindAll(x => x.Entity.GetType().IsTypeImplementsAnInterface(typeof(IModifier)));
        if (updatedEntries.Any())
        {
            updatedEntries.ForEach(e => (e.Entity as IModifier).SetModifier(userId));
        }
    }

    private List<AuditLog> CreateAuditLogs(IEnumerable<EntityEntry> auditedEntries, long userId, Guid scopeId)
    {
        var auditsToAdd = new List<AuditLog>();
        var entityEntries = auditedEntries.Where(x => x.Entity.GetType().IsLogAuditingEnabled());
        foreach (var entityEntry in entityEntries)
        {
            var auditLogData = entityEntry.GetChangeData();
            if (!auditLogData.Any()) continue;

            var entityType = entityEntry.Entity.GetType(); // TODO
            var primaryKey = GerPrimaryKeyData(entityEntry);

            auditsToAdd.Add(new AuditLog(
                userId: userId,
                auditType: entityEntry.State.ToAuditType(),
                tableName: entityType.Name,
                primaryKeyName: primaryKey.Name,
                primaryKeyValue: primaryKey.Id,
                data: JsonSerializer.Serialize(auditLogData),
                scopeId: scopeId));
        }

        return auditsToAdd;
    }

    private IdNameDto GerPrimaryKeyData(EntityEntry entityEntry)
    {
        var primaryKey = new IdNameDto() { Name = FindPrimaryKey(entityEntry) };
        if (!string.IsNullOrEmpty(primaryKey.Name))
        {
            var values = entityEntry.State == EntityState.Deleted ? entityEntry.OriginalValues : entityEntry.CurrentValues;
            primaryKey.Id = values.GetValue<long>(primaryKey.Name);
        }
        else
        {
            primaryKey.Name = "(NotFound)";
        }

        return primaryKey;
    }

    private string FindPrimaryKey(EntityEntry entityEntry)
    {
        var entityType = entityEntry.Entity.GetType();

        var entity = Model.FindEntityType(entityType);
        //Case when there are owned types which are used in multiple classes
        if (entity == null)
        {
            try
            {
                var definingType = entityEntry.CurrentValues?.EntityType?.ClrType;
                var entityNameFromDefiningType = definingType?.GetProperties()
                    .FirstOrDefault(x => x.PropertyType.Name == entityType.Name)?.Name;
                entity = entityNameFromDefiningType != null
                    ? Model.FindEntityType(entityType, entityNameFromDefiningType, Model.FindEntityType(definingType))
                    : null;
            }
            catch (Exception)
            {
                // ignored
                entity = null;
            }
        }

        return entity?.FindPrimaryKey().Properties.Select(prop => prop.Name).FirstOrDefault();
    }

    private List<DomainEvent> GetDomainEvents(bool onlyPre = true)
    {
        var domainEntities = ChangeTracker
            .Entries<RegularEntity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

        return domainEntities.SelectMany(x => x.Entity.DomainEvents).ToList();
    }
    
    #endregion
}
