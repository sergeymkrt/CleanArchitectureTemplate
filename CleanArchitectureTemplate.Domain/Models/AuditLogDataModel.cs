namespace CleanArchitectureTemplate.Domain.Models;

public class AuditLogDataModel
{
    public string ColumnName { get; set; }
    public object OldValue { get; set; }
    public object NewValue { get; set; }
}
