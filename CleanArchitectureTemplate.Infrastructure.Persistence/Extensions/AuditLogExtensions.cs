using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Extensions;

public static class AuditLogExtensions
{
    public static AuditType ToAuditType(this EntityState entityState)
    {
        switch (entityState)
        {
            case EntityState.Added:
            case EntityState.Unchanged:
                return AuditType.Added;
            case EntityState.Modified:
                return AuditType.Modified;
            case EntityState.Deleted:
                return AuditType.Deleted;
            default:
                return default;
        }
    }

    public static List<AuditLogDataModel> GetChangeData(this EntityEntry entityEntry)
    {
        return entityEntry.GetPropertyNames()
            .Where(propName => entityEntry.IsPropertyValueChanged(propName))
            .Select(propName => new AuditLogDataModel
            {
                ColumnName = propName,
                OldValue = entityEntry.GetOriginalValue(propName),
                NewValue = entityEntry.GetCurrentValue(propName)
            })
            .ToList();
    }

    #region Private Methods
    private static IEnumerable<string> GetPropertyNames(this EntityEntry entityEntry)
    {
        var propertyValues = (entityEntry.State == EntityState.Unchanged) || (entityEntry.State == EntityState.Added)
            ? entityEntry.CurrentValues
            : entityEntry.OriginalValues;
        return propertyValues.Properties.Select(prop => prop.Name);
    }

    private static string GetOriginalValue(this EntityEntry entityEntry, string propertyName)
    {
        // Added state is here interpreted as Unchanged, since we already called SaveChanges()
        if (entityEntry.State == EntityState.Unchanged || entityEntry.State == EntityState.Added)
        {
            return null;
        }

        var value = entityEntry.Property(propertyName).OriginalValue;
        return value?.ToString();
    }

    private static string GetCurrentValue(this EntityEntry entityEntry, string propertyName)
    {
        if (entityEntry.State == EntityState.Deleted)
        {
            // It will be invalid operation when its in deleted state. in that case, new value should be null
            return null;
        }

        var value = entityEntry.Property(propertyName).CurrentValue;
        return value?.ToString();
    }

    private static bool IsPropertyValueChanged(this EntityEntry entityEntry, string propertyName)
    {
        var prop = entityEntry.Property(propertyName);
        // Added state is here interpreted as Unchanged, since we already called SaveChanges()
        // the conditions bellow are commented to include all the columns on additions and deletions with NULL being current or original value respectively
        var changed = (entityEntry.State == EntityState.Unchanged) ||
                      (entityEntry.State == EntityState.Added) ||
                      (entityEntry.State == EntityState.Deleted) ||
                      (entityEntry.State == EntityState.Modified && prop.IsModified
                                                                 && !Equals(entityEntry.GetCurrentValue(propertyName), entityEntry.GetOriginalValue(propertyName)));

        return changed;
    }
    #endregion
}
