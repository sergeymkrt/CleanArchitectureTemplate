using FluentValidation;
using FluentValidation.Results;

namespace CleanArchitectureTemplate.Application.UseCases;

public abstract class CompositeValidator<TType> : AbstractValidator<TType>
{
    private readonly List<IValidator> otherValidators = new List<IValidator>();

    protected void RegisterBaseValidator<TBase>(IValidator<TBase> validator)
    {
        otherValidators.Add(validator);

    }

    public override async Task<ValidationResult> ValidateAsync(ValidationContext<TType> context, CancellationToken cancellation = default)
    {
        var errorsFromOtherValidators = otherValidators.SelectMany(x => x.Validate(context).Errors);
        var mainErrors = (await base.ValidateAsync(context, cancellation)).Errors;
        var combinedErrors = mainErrors.Concat(errorsFromOtherValidators);

        return new ValidationResult(combinedErrors);
    }
}
