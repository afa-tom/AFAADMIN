using AFAADMIN.Common.DependencyInjection;
using AFAADMIN.Common.Exceptions;
using AFAADMIN.Database.Repositories;
using AFAADMIN.EventBus;
using AFAADMIN.System.Application.Dtos;
using AFAADMIN.System.Domain.Entities;
using AFAADMIN.System.Domain.Events;
using Mapster;
using SqlSugar;

namespace AFAADMIN.System.Application.Services.Impl;

public class RoleService : IRoleService, IScopedDependency
{
    private readonly IBaseRepository<SysRole> _roleRepo;
    private readonly ISqlSugarClient _db;
    private readonly IEventPublisher _eventPublisher;

    public RoleService(IBaseRepository<SysRole> roleRepo, ISqlSugarClient db,
        IEventPublisher eventPublisher)
    {
        _roleRepo = roleRepo;
        _db = db;
        _eventPublisher = eventPublisher;
    }

    public async Task<List<RoleDto>> GetListAsync()
    {
        var roles = await _roleRepo.GetListAsync();
        var dtos = roles.Adapt<List<RoleDto>>();

        var roleIds = dtos.Select(r => r.Id).ToList();
        var roleMenus = await _db.Queryable<SysRoleMenu>()
            .Where(rm => roleIds.Contains(rm.RoleId))
            .ToListAsync();

        foreach (var dto in dtos)
            dto.MenuIds = roleMenus.Where(rm => rm.RoleId == dto.Id).Select(rm => rm.MenuId).ToList();

        return dtos.OrderBy(r => r.Sort).ToList();
    }

    public async Task<RoleDto?> GetByIdAsync(long id)
    {
        var role = await _roleRepo.GetByIdAsync(id);
        if (role == null) return null;

        var dto = role.Adapt<RoleDto>();
        dto.MenuIds = await _db.Queryable<SysRoleMenu>()
            .Where(rm => rm.RoleId == id)
            .Select(rm => rm.MenuId)
            .ToListAsync();
        return dto;
    }

    public async Task<long> CreateAsync(CreateRoleDto dto)
    {
        if (await _roleRepo.AnyAsync(r => r.RoleCode == dto.RoleCode))
            throw new BusinessException("角色编码已存在");

        var role = dto.Adapt<SysRole>();
        var id = await _roleRepo.InsertReturnIdAsync(role);

        if (dto.MenuIds.Count > 0)
        {
            var roleMenus = dto.MenuIds.Select(mid => new SysRoleMenu { RoleId = id, MenuId = mid }).ToList();
            await _db.Insertable(roleMenus).ExecuteCommandAsync();
        }

        return id;
    }

    public async Task<bool> UpdateAsync(UpdateRoleDto dto)
    {
        var role = await _roleRepo.GetByIdAsync(dto.Id);
        if (role == null) throw new BusinessException("角色不存在");

        if (await _roleRepo.AnyAsync(r => r.RoleCode == dto.RoleCode && r.Id != dto.Id))
            throw new BusinessException("角色编码已存在");

        role.RoleName = dto.RoleName;
        role.RoleCode = dto.RoleCode;
        role.Sort = dto.Sort;
        role.Status = dto.Status;
        role.Remark = dto.Remark;

        var result = await _roleRepo.UpdateAsync(role);

        await _db.Deleteable<SysRoleMenu>().Where(rm => rm.RoleId == dto.Id).ExecuteCommandAsync();
        if (dto.MenuIds.Count > 0)
        {
            var roleMenus = dto.MenuIds.Select(mid => new SysRoleMenu { RoleId = dto.Id, MenuId = mid }).ToList();
            await _db.Insertable(roleMenus).ExecuteCommandAsync();
        }

        // 发布角色权限变更事件
        await _eventPublisher.PublishAsync(new RolePermissionChangedEvent { RoleId = dto.Id });

        return result;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var hasUser = await _db.Queryable<SysUserRole>().AnyAsync(ur => ur.RoleId == id);
        if (hasUser) throw new BusinessException("该角色下还有用户，无法删除");

        await _db.Deleteable<SysRoleMenu>().Where(rm => rm.RoleId == id).ExecuteCommandAsync();
        return await _roleRepo.SoftDeleteAsync(id);
    }

    public async Task<bool> SetMenusAsync(long roleId, List<long> menuIds)
    {
        await _db.Deleteable<SysRoleMenu>().Where(rm => rm.RoleId == roleId).ExecuteCommandAsync();
        if (menuIds.Count == 0) return true;

        var roleMenus = menuIds.Select(mid => new SysRoleMenu { RoleId = roleId, MenuId = mid }).ToList();
        var result = await _db.Insertable(roleMenus).ExecuteCommandAsync() > 0;

        // 发布角色权限变更事件
        await _eventPublisher.PublishAsync(new RolePermissionChangedEvent { RoleId = roleId });

        return result;
    }
}
