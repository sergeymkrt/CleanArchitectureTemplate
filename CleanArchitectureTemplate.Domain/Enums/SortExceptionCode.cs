using System.ComponentModel;

namespace CleanArchitectureTemplate.Domain.Enums;

public enum SortExceptionCode : int
{
    [Description("Invalid sorting property")]
    InvalidSortingProperty = 1,
    [Description("Non-sorting property.")]
    NonSortingProperty = 2 // NotMapped or without setter
}

