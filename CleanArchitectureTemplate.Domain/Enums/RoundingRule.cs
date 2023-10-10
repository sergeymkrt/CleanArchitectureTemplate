using CleanArchitectureTemplate.Domain.Attributes;

namespace CleanArchitectureTemplate.Domain.Enums;

public enum RoundingRule
{
    [EnumDisplay(Name = "Mathematical Rounding")]
    MathematicalRounding = 1,
    [EnumDisplay(Name = "Rounding Down")]
    RoundingDown = 2,
    [EnumDisplay(Name = "Rounding Up")]
    RoundingUp = 3
}
