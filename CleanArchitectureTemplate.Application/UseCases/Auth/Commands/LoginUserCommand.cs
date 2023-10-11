using CleanArchitectureTemplate.Application.DTOs.Users;
using CleanArchitectureTemplate.Application.Models;
using CleanArchitectureTemplate.Domain.Aggregates.Users;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.Services.External;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Application.UseCases.Auth.Commands;

public class LoginUserCommand : BaseCommand<string>
{
    public UserAuthDto Dto { get; set; }

    public LoginUserCommand(UserAuthDto dto)
    {
        Dto = dto;
    }   
    
    
    public class LoginUserCommandHandler : BaseCommandHandler<LoginUserCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;
        public LoginUserCommandHandler(IIdentityService identityService, IMapper mapper, UserManager<User> userManager, IAuthService authService) 
            : base(identityService,mapper)
        {
            _userManager = userManager;
            _authService = authService;
        }

        public override async Task<ResponseModel<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Dto.Email);
            if (user == null)
            {
                throw new ApplicationException("Invalid login attempt.");
            }
            
            var result = await _userManager.CheckPasswordAsync(user, request.Dto.Password);
            
            if (!result)
            {
                throw new ApplicationException("Invalid login attempt.");
            }
            var roles = (await _userManager.GetRolesAsync(user)).ToArray();
            var token = _authService.GenerateJwtToken(user, roles);
            
            return ResponseModel<string>.Create(ResponseCode.Success, token);
        }
    }
}
