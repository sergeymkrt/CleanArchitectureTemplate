using CleanArchitectureTemplate.Domain.Attributes;

namespace CleanArchitectureTemplate.Domain.Enums;

public enum Role : int
{
    [EnumDisplay(Name = "Admin", ShortName = "Admin")]
    Admin = 2,
    [EnumDisplay(Name = "User", ShortName = "User")]
    User = 3
}
