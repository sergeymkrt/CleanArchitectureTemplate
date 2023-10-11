using CleanArchitectureTemplate.Application.DTOs.Users;
using CleanArchitectureTemplate.Application.Models;
using CleanArchitectureTemplate.Domain.Aggregates.Users;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.Services.External;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Application.UseCases.Auth.Commands;

public class RegisterUserCommand : BaseCommand
{
    public UserRegisterDto Dto { get; set; }
    
    public RegisterUserCommand(UserRegisterDto dto)
    {
        Dto = dto;
    }
    
    public class RegisterUserCommandHandler : BaseCommandHandler<RegisterUserCommand>
    {
        private readonly UserManager<User> _userManager;
        public RegisterUserCommandHandler(IIdentityService identityService, IMapper mapper, UserManager<User> userManager) : base(identityService,mapper)
        {
            _userManager = userManager;
        }

        public override async Task<ResponseModel> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User(request.Dto.Email);
            
            var result = await _userManager.CreateAsync(user, request.Dto.Password);
            if (!result.Succeeded)
            {
                throw new ApplicationException(result.Errors.FirstOrDefault()?.Description);
            }

            return ResponseModel.Create(ResponseCode.SuccessfullyCreated);
        }
    }
}
