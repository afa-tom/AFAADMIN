using System.IdentityModel.Tokens.Jwt;
using AFAADMIN.Common.Cache;
using AFAADMIN.Common.Config;
using AFAADMIN.Common.Crypto;
using AFAADMIN.Common.DependencyInjection;
using AFAADMIN.Common.Exceptions;
using AFAADMIN.Database.Encryption;
using AFAADMIN.Database.Repositories;
using AFAADMIN.System.Application.Dtos;
using AFAADMIN.System.Domain.Entities;
using AFAADMIN.Web.Core.Authentication;
using SqlSugar;
using ICacheService = AFAADMIN.Common.Cache.ICacheService;

namespace AFAADMIN.System.Application.Services.Impl;

public class AuthService : IAuthService, IScopedDependency
{
    private readonly IBaseRepository<SysUser> _userRepo;
    private readonly ISqlSugarClient _db;
    private readonly IJwtService _jwtService;
    private readonly SecurityConfig _securityConfig;
    private readonly SensitiveFieldEncryptor _encryptor;
    private readonly ICacheService _cache;

    public AuthService(IBaseRepository<SysUser> userRepo, ISqlSugarClient db,
        IJwtService jwtService, SecurityConfig securityConfig,
        SensitiveFieldEncryptor encryptor, ICacheService cache)
    {
        _userRepo = userRepo;
        _db = db;
        _jwtService = jwtService;
        _securityConfig = securityConfig;
        _encryptor = encryptor;
        _cache = cache;
    }

    public async Task<LoginResultDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepo.GetFirstAsync(u => u.UserName == dto.UserName);
        if (user == null)
            throw new BusinessException("用户名或密码错误");

        if (user.Status != 1)
            throw new BusinessException("账号已被停用");

        if (!SM3Helper.Verify(dto.Password, user.Salt, user.Password))
            throw new BusinessException("用户名或密码错误");

        var roles = await _db.Queryable<SysUserRole>()
            .LeftJoin<SysRole>((ur, r) => ur.RoleId == r.Id)
            .Where((ur, r) => ur.UserId == user.Id && r.Status == 1)
            .Select((ur, r) => r.RoleCode)
            .ToListAsync();

        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.UserName, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // RefreshToken 存入 Redis
        var expiry = TimeSpan.FromDays(_securityConfig.Jwt.RefreshTokenExpireDays);
        await _cache.SetAsync(CacheKeys.RefreshToken(refreshToken), user.Id, expiry);

        return new LoginResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _securityConfig.Jwt.AccessTokenExpireMinutes * 60
        };
    }

    public async Task<LoginResultDto> RefreshTokenAsync(string refreshToken)
    {
        var userId = await _cache.GetAsync<long>(CacheKeys.RefreshToken(refreshToken));
        if (userId == 0)
            throw new BusinessException("RefreshToken 无效或已过期", 401);

        // 移除旧 Token
        await _cache.RemoveAsync(CacheKeys.RefreshToken(refreshToken));

        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null || user.Status != 1)
            throw new BusinessException("账号异常", 401);

        var roles = await _db.Queryable<SysUserRole>()
            .LeftJoin<SysRole>((ur, r) => ur.RoleId == r.Id)
            .Where((ur, r) => ur.UserId == user.Id && r.Status == 1)
            .Select((ur, r) => r.RoleCode)
            .ToListAsync();

        var newAccessToken = _jwtService.GenerateAccessToken(user.Id, user.UserName, roles);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        var expiry = TimeSpan.FromDays(_securityConfig.Jwt.RefreshTokenExpireDays);
        await _cache.SetAsync(CacheKeys.RefreshToken(newRefreshToken), user.Id, expiry);

        return new LoginResultDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = _securityConfig.Jwt.AccessTokenExpireMinutes * 60
        };
    }

    /// <summary>
    /// 登出 — 将 AccessToken 加入黑名单
    /// </summary>
    public async Task LogoutAsync(string accessToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(accessToken))
            {
                var jwt = handler.ReadJwtToken(accessToken);
                var jti = jwt.Id;
                var expiry = jwt.ValidTo - DateTime.UtcNow;
                if (!string.IsNullOrEmpty(jti) && expiry > TimeSpan.Zero)
                {
                    await _cache.SetStringAsync(CacheKeys.TokenBlacklist(jti), "1", expiry);
                }
            }
        }
        catch { /* Token 解析失败不影响登出 */ }
    }

    public async Task<CurrentUserDto> GetCurrentUserAsync(long userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new BusinessException("用户不存在");

        // 角色编码（带缓存）
        var roles = await _cache.GetOrSetAsync(
            CacheKeys.UserRoles(userId),
            async () => await _db.Queryable<SysUserRole>()
                .LeftJoin<SysRole>((ur, r) => ur.RoleId == r.Id)
                .Where((ur, r) => ur.UserId == userId && r.Status == 1)
                .Select((ur, r) => r.RoleCode)
                .ToListAsync(),
            TimeSpan.FromMinutes(30));

        // 权限标识（带缓存）
        List<string> permissions;
        if (roles != null && roles.Contains("admin"))
        {
            permissions = ["*:*:*"];
        }
        else
        {
            permissions = await _cache.GetOrSetAsync(
                CacheKeys.UserPermissions(userId),
                async () => await _db.Queryable<SysUserRole>()
                    .LeftJoin<SysRoleMenu>((ur, rm) => ur.RoleId == rm.RoleId)
                    .LeftJoin<SysMenu>((ur, rm, m) => rm.MenuId == m.Id)
                    .Where((ur, rm, m) => ur.UserId == userId
                        && m.Status == 1 && m.Permission != null && m.Permission != "")
                    .Select((ur, rm, m) => m.Permission!)
                    .Distinct()
                    .ToListAsync(),
                TimeSpan.FromMinutes(30)) ?? [];
        }

        return new CurrentUserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            NickName = user.NickName,
            Avatar = user.Avatar,
            Roles = roles ?? [],
            Permissions = permissions
        };
    }
}
