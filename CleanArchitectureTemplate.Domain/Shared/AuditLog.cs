using System.ComponentModel.DataAnnotations;
using CleanArchitectureTemplate.Domain.Aggregates.Users;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.SeedWork;

namespace CleanArchitectureTemplate.Domain.Shared;

public class AuditLog : RegularEntity
{
    public DateTime CreatedDate { get; protected set; }
    public User User { get; protected set; }
    public long UserId { get; protected set; }

    [Range(1, 3)]
    public AuditType AuditType { get; protected set; }

    [Required]
    [MaxLength(50)]
    public string TableName { get; protected set; }

    [Required]
    [MaxLength(50)]
    public string PrimaryKeyName { get; protected set; }
    public long PrimaryKeyValue { get; protected set; }

    [Required]
    public string Data { get; protected set; }

    [Required]
    public Guid ScopeId { get; protected set; }

    protected AuditLog() { }

    public AuditLog(
        long userId,
        AuditType auditType,
        string tableName,
        string primaryKeyName,
        long primaryKeyValue,
        string data,
        Guid scopeId)
    {
        UserId = userId;
        AuditType = auditType;
        TableName = tableName;
        PrimaryKeyName = primaryKeyName;
        PrimaryKeyValue = primaryKeyValue;
        Data = data;
        ScopeId = scopeId;
    }
}
