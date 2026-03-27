using AFAADMIN.System.Application.Dtos;

namespace AFAADMIN.System.Application.Services;

public interface IMenuService
{
    Task<List<MenuDto>> GetTreeAsync();
    Task<MenuDto?> GetByIdAsync(long id);
    Task<long> CreateAsync(CreateMenuDto dto);
    Task<bool> UpdateAsync(UpdateMenuDto dto);
    Task<bool> DeleteAsync(long id);
    Task<List<long>> GetMenuIdsByRoleIdAsync(long roleId);
}

public interface IDeptService
{
    Task<List<DeptDto>> GetTreeAsync();
    Task<DeptDto?> GetByIdAsync(long id);
    Task<long> CreateAsync(CreateDeptDto dto);
    Task<bool> UpdateAsync(UpdateDeptDto dto);
    Task<bool> DeleteAsync(long id);
}

public interface IDictService
{
    Task<List<DictTypeDto>> GetTypeListAsync();
    Task<long> CreateTypeAsync(CreateDictTypeDto dto);
    Task<bool> UpdateTypeAsync(UpdateDictTypeDto dto);
    Task<bool> DeleteTypeAsync(long id);

    Task<List<DictDataDto>> GetDataListByTypeIdAsync(long dictTypeId);
    Task<List<DictDataDto>> GetDataListByCodeAsync(string dictCode);
    Task<long> CreateDataAsync(CreateDictDataDto dto);
    Task<bool> UpdateDataAsync(UpdateDictDataDto dto);
    Task<bool> DeleteDataAsync(long id);
}

public interface IAuthService
{
    Task<LoginResultDto> LoginAsync(LoginDto dto);
    Task<LoginResultDto> RefreshTokenAsync(string refreshToken);
    Task<CurrentUserDto> GetCurrentUserAsync(long userId);
    /// <summary>
    /// 登出（将 Token 加入黑名单）
    /// </summary>
    Task LogoutAsync(string accessToken);
}
