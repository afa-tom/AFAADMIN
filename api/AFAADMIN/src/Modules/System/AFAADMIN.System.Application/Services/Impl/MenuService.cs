using AFAADMIN.Common.DependencyInjection;
using AFAADMIN.Common.Exceptions;
using AFAADMIN.Database.Repositories;
using AFAADMIN.System.Application.Dtos;
using AFAADMIN.System.Domain.Entities;
using Mapster;
using SqlSugar;

namespace AFAADMIN.System.Application.Services.Impl;

public class MenuService : IMenuService, IScopedDependency
{
    private readonly IBaseRepository<SysMenu> _menuRepo;
    private readonly ISqlSugarClient _db;

    public MenuService(IBaseRepository<SysMenu> menuRepo, ISqlSugarClient db)
    {
        _menuRepo = menuRepo;
        _db = db;
    }

    public async Task<List<MenuDto>> GetTreeAsync()
    {
        var all = await _menuRepo.GetListAsync();
        var dtos = all.Adapt<List<MenuDto>>();
        return BuildTree(dtos, 0);
    }

    public async Task<MenuDto?> GetByIdAsync(long id)
    {
        var menu = await _menuRepo.GetByIdAsync(id);
        return menu?.Adapt<MenuDto>();
    }

    public async Task<long> CreateAsync(CreateMenuDto dto)
    {
        var menu = dto.Adapt<SysMenu>();
        return await _menuRepo.InsertReturnIdAsync(menu);
    }

    public async Task<bool> UpdateAsync(UpdateMenuDto dto)
    {
        var menu = await _menuRepo.GetByIdAsync(dto.Id);
        if (menu == null) throw new BusinessException("菜单不存在");

        dto.Adapt(menu);
        return await _menuRepo.UpdateAsync(menu);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        // 检查是否有子菜单
        if (await _menuRepo.AnyAsync(m => m.ParentId == id))
            throw new BusinessException("请先删除子菜单");

        // 删除角色关联
        await _db.Deleteable<SysRoleMenu>().Where(rm => rm.MenuId == id).ExecuteCommandAsync();
        return await _menuRepo.SoftDeleteAsync(id);
    }

    public async Task<List<long>> GetMenuIdsByRoleIdAsync(long roleId)
    {
        return await _db.Queryable<SysRoleMenu>()
            .Where(rm => rm.RoleId == roleId)
            .Select(rm => rm.MenuId)
            .ToListAsync();
    }

    /// <summary>
    /// 构建树形结构
    /// </summary>
    private static List<MenuDto> BuildTree(List<MenuDto> all, long parentId)
    {
        return all
            .Where(m => m.ParentId == parentId)
            .OrderBy(m => m.Sort)
            .Select(m =>
            {
                m.Children = BuildTree(all, m.Id);
                return m;
            })
            .ToList();
    }
}
