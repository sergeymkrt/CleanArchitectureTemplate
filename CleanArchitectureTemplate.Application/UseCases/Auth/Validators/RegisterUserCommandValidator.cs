using CleanArchitectureTemplate.Application.UseCases.Auth.Commands;
using FluentValidation;

namespace CleanArchitectureTemplate.Application.UseCases.Auth.Validators;

public class RegisterUserCommandValidator : CompositeValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Dto.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x => x.Dto.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(50)
            .Matches(@"^(?=.*\d)(?=.*[a-zA-Z])(?=.*[A-Z])(?=.*[-\#\$\.\%\&\*])(?=.*[a-zA-Z]).{8,16}$");
        RuleFor(x => x.Dto.ConfirmPassword).NotEmpty().Equal(x => x.Dto.Password);
    }
}
