using CleanArchitectureTemplate.Application.UseCases.Auth.Commands;
using CleanArchitectureTemplate.Domain.Aggregates.Users;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Application.UseCases.Auth.Validators;

public class LoginUserCommandValidator: CompositeValidator<LoginUserCommand>
{
    private readonly UserManager<User> _userManager;
    public LoginUserCommandValidator(UserManager<User> userManager)
    {
        _userManager = userManager;
        
        RuleFor(x => x.Dto.Email)
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(x => x.Dto.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(50)
            .Matches(@"^(?=.*\d)(?=.*[a-zA-Z])(?=.*[A-Z])(?=.*[-\#\$\.\%\&\*])(?=.*[a-zA-Z]).{8,16}$");
        
        RuleFor(x => x.Dto.Email)
            .MustAsync(UserExists)
            .WithMessage(ResponseCode.NotExists.GetResourceMessage());
    }
    
    private async Task<bool> UserExists(string email, CancellationToken cancellationToken)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }
}
