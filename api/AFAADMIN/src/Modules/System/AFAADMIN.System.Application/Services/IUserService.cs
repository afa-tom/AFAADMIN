using AFAADMIN.Common.Models;
using AFAADMIN.System.Application.Dtos;

namespace AFAADMIN.System.Application.Services;

public interface IUserService
{
    Task<PageResult<UserDto>> GetPageAsync(UserQueryDto query);
    Task<UserDto?> GetByIdAsync(long id);
    Task<long> CreateAsync(CreateUserDto dto);
    Task<bool> UpdateAsync(UpdateUserDto dto);
    Task<bool> DeleteAsync(long id);
    Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    Task<bool> SetRolesAsync(long userId, List<long> roleIds);
}
