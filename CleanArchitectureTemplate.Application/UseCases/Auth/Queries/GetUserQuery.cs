using CleanArchitectureTemplate.Application.DTOs.Users;
using CleanArchitectureTemplate.Application.Models;
using CleanArchitectureTemplate.Domain.Aggregates.Users;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.Services.External;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Application.UseCases.Auth.Queries;

public class GetUserQuery: BaseQuery<UserDto>
{
    public string Id { get; set; }
    
    public GetUserQuery(string id)
    {
        Id = id;
    }
    
    public class GetUserQueryHandler : BaseQueryHandler<GetUserQuery>
    {
        private readonly UserManager<User> _userManager;
        public GetUserQueryHandler(IIdentityService identityService, IMapper mapper, UserManager<User> userManager) 
            : base(identityService,mapper)
        {
            _userManager = userManager;
        }

        public override async Task<ResponseModel<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
            {
                throw new ApplicationException("Invalid user id.");
            }
            var userDto = Mapper.Map<UserDto>(user);
            return ResponseModel<UserDto>.Create(ResponseCode.Success, userDto);
        }
    }
}
