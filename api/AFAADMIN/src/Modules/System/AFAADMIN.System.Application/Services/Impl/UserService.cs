using AFAADMIN.Common.Crypto;
using AFAADMIN.Common.DependencyInjection;
using AFAADMIN.Common.Exceptions;
using AFAADMIN.Common.Models;
using AFAADMIN.Database.Encryption;
using AFAADMIN.Database.Repositories;
using AFAADMIN.System.Application.Dtos;
using AFAADMIN.System.Domain.Entities;
using Mapster;
using SqlSugar;

namespace AFAADMIN.System.Application.Services.Impl;

public class UserService : IUserService, IScopedDependency
{
    private readonly IBaseRepository<SysUser> _userRepo;
    private readonly ISqlSugarClient _db;
    private readonly SensitiveFieldEncryptor _encryptor;

    public UserService(IBaseRepository<SysUser> userRepo, ISqlSugarClient db,
        SensitiveFieldEncryptor encryptor)
    {
        _userRepo = userRepo;
        _db = db;
        _encryptor = encryptor;
    }

    public async Task<PageResult<UserDto>> GetPageAsync(UserQueryDto query)
    {
        var page = new PageRequest { PageIndex = query.PageIndex, PageSize = query.PageSize };

        RefAsync<int> totalCount = 0;
        var items = await _db.Queryable<SysUser>()
            .LeftJoin<SysDept>((u, d) => u.DeptId == d.Id)
            .WhereIF(!string.IsNullOrEmpty(query.UserName), (u, d) => u.UserName.Contains(query.UserName!))
            .WhereIF(query.Status.HasValue, (u, d) => u.Status == query.Status!.Value)
            .WhereIF(query.DeptId.HasValue, (u, d) => u.DeptId == query.DeptId!.Value)
            .Where((u, d) => u.IsDeleted == false)
            .OrderBy((u, d) => u.CreateTime, OrderByType.Desc)
            .Select((u, d) => new UserDto
            {
                Id = u.Id,
                DeptId = u.DeptId,
                UserName = u.UserName,
                NickName = u.NickName,
                Phone = u.Phone,
                Email = u.Email,
                Avatar = u.Avatar,
                Status = u.Status,
                Remark = u.Remark,
                CreateTime = u.CreateTime,
                DeptName = d.DeptName
            })
            .ToPageListAsync(page.PageIndex, page.PageSize, totalCount);

        // 解密敏感字段
        foreach (var item in items)
        {
            item.Phone = TryDecrypt(item.Phone);
        }

        // 查询角色
        var userIds = items.Select(x => x.Id).ToList();
        var userRoles = await _db.Queryable<SysUserRole>()
            .Where(ur => userIds.Contains(ur.UserId))
            .ToListAsync();
        foreach (var item in items)
        {
            item.RoleIds = userRoles.Where(ur => ur.UserId == item.Id).Select(ur => ur.RoleId).ToList();
        }

        return new PageResult<UserDto>
        {
            PageIndex = page.PageIndex,
            PageSize = page.PageSize,
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task<UserDto?> GetByIdAsync(long id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null) return null;

        _encryptor.Decrypt(user);
        var dto = user.Adapt<UserDto>();

        // 查询角色
        dto.RoleIds = await _db.Queryable<SysUserRole>()
            .Where(ur => ur.UserId == id)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        // 查询部门名
        if (user.DeptId.HasValue)
        {
            var dept = await _db.Queryable<SysDept>().InSingleAsync(user.DeptId.Value);
            dto.DeptName = dept?.DeptName;
        }

        return dto;
    }

    public async Task<long> CreateAsync(CreateUserDto dto)
    {
        if (await _userRepo.AnyAsync(u => u.UserName == dto.UserName))
            throw new BusinessException("用户名已存在");

        var user = dto.Adapt<SysUser>();
        user.Salt = SM3Helper.GenerateSalt();
        user.Password = SM3Helper.HashWithSalt(dto.Password, user.Salt);

        _encryptor.Encrypt(user);

        var id = await _userRepo.InsertReturnIdAsync(user);

        // 分配角色
        if (dto.RoleIds.Count > 0)
        {
            var userRoles = dto.RoleIds.Select(rid => new SysUserRole { UserId = id, RoleId = rid }).ToList();
            await _db.Insertable(userRoles).ExecuteCommandAsync();
        }

        return id;
    }

    public async Task<bool> UpdateAsync(UpdateUserDto dto)
    {
        var user = await _userRepo.GetByIdAsync(dto.Id);
        if (user == null) throw new BusinessException("用户不存在");

        user.DeptId = dto.DeptId;
        user.NickName = dto.NickName;
        user.Phone = dto.Phone;
        user.Email = dto.Email;
        user.Status = dto.Status;
        user.Remark = dto.Remark;

        _encryptor.Encrypt(user);
        var result = await _userRepo.UpdateAsync(user);

        // 更新角色
        await _db.Deleteable<SysUserRole>().Where(ur => ur.UserId == dto.Id).ExecuteCommandAsync();
        if (dto.RoleIds.Count > 0)
        {
            var userRoles = dto.RoleIds.Select(rid => new SysUserRole { UserId = dto.Id, RoleId = rid }).ToList();
            await _db.Insertable(userRoles).ExecuteCommandAsync();
        }

        return result;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        // 同时删除角色关联
        await _db.Deleteable<SysUserRole>().Where(ur => ur.UserId == id).ExecuteCommandAsync();
        return await _userRepo.SoftDeleteAsync(id);
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userRepo.GetByIdAsync(dto.UserId);
        if (user == null) throw new BusinessException("用户不存在");

        user.Salt = SM3Helper.GenerateSalt();
        user.Password = SM3Helper.HashWithSalt(dto.NewPassword, user.Salt);

        return await _userRepo.UpdateColumnsAsync(user,
            u => new { u.Password, u.Salt, u.UpdateTime });
    }

    public async Task<bool> SetRolesAsync(long userId, List<long> roleIds)
    {
        await _db.Deleteable<SysUserRole>().Where(ur => ur.UserId == userId).ExecuteCommandAsync();
        if (roleIds.Count == 0) return true;

        var userRoles = roleIds.Select(rid => new SysUserRole { UserId = userId, RoleId = rid }).ToList();
        return await _db.Insertable(userRoles).ExecuteCommandAsync() > 0;
    }

    private string? TryDecrypt(string? value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        try
        {
            // 尝试解密，若失败返回原值（可能未加密的旧数据）
            var dummy = new SysUser { Phone = value };
            _encryptor.Decrypt(dummy);
            return dummy.Phone;
        }
        catch { return value; }
    }
}
