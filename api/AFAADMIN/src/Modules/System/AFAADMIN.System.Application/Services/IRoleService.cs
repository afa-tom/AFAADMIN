using AFAADMIN.System.Application.Dtos;

namespace AFAADMIN.System.Application.Services;

public interface IRoleService
{
    Task<List<RoleDto>> GetListAsync();
    Task<RoleDto?> GetByIdAsync(long id);
    Task<long> CreateAsync(CreateRoleDto dto);
    Task<bool> UpdateAsync(UpdateRoleDto dto);
    Task<bool> DeleteAsync(long id);
    Task<bool> SetMenusAsync(long roleId, List<long> menuIds);
}
