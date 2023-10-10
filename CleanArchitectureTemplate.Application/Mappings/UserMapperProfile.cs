using CleanArchitectureTemplate.Application.DTOs.Users;
using CleanArchitectureTemplate.Application.Extensions;
using CleanArchitectureTemplate.Domain.Aggregates.Users;
using CleanArchitectureTemplate.Domain.SeedWork;
using Mapster;

namespace CleanArchitectureTemplate.Application.Mappings;

public class UserMapperProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User,UserDto>()
            .IgnoreAllNonExisting();
        
        config.NewConfig<PaginatedResult<User>,PaginatedResult<UserDto>>()
            .IgnoreAllNonExisting();

    }
}
