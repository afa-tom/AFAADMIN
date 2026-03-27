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

namespace AFAADMIN.System.Application.Services.Impl;

public class AuthService : IAuthService, IScopedDependency
{
    private readonly IBaseRepository<SysUser> _userRepo;
    private readonly ISqlSugarClient _db;
    private readonly IJwtService _jwtService;
    private readonly SecurityConfig _securityConfig;
    private readonly SensitiveFieldEncryptor _encryptor;

    // 内存存储 RefreshToken（M4 阶段迁移至 Redis）
    private static readonly Dictionary<string, (long UserId, DateTime Expiry)> _refreshTokens = new();

    public AuthService(IBaseRepository<SysUser> userRepo, ISqlSugarClient db,
        IJwtService jwtService, SecurityConfig securityConfig, SensitiveFieldEncryptor encryptor)
    {
        _userRepo = userRepo;
        _db = db;
        _jwtService = jwtService;
        _securityConfig = securityConfig;
        _encryptor = encryptor;
    }

    public async Task<LoginResultDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepo.GetFirstAsync(u => u.UserName == dto.UserName);
        if (user == null)
            throw new BusinessException("用户名或密码错误");

        if (user.Status != 1)
            throw new BusinessException("账号已被停用");

        // SM3 密码校验
        if (!SM3Helper.Verify(dto.Password, user.Salt, user.Password))
            throw new BusinessException("用户名或密码错误");

        // 获取角色编码
        var roles = await _db.Queryable<SysUserRole>()
            .LeftJoin<SysRole>((ur, r) => ur.RoleId == r.Id)
            .Where((ur, r) => ur.UserId == user.Id && r.Status == 1)
            .Select((ur, r) => r.RoleCode)
            .ToListAsync();

        // 签发 Token
        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.UserName, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // 存储 RefreshToken
        _refreshTokens[refreshToken] = (user.Id,
            DateTime.UtcNow.AddDays(_securityConfig.Jwt.RefreshTokenExpireDays));

        return new LoginResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _securityConfig.Jwt.AccessTokenExpireMinutes * 60
        };
    }

    public async Task<LoginResultDto> RefreshTokenAsync(string refreshToken)
    {
        if (!_refreshTokens.TryGetValue(refreshToken, out var tokenInfo))
            throw new BusinessException("RefreshToken 无效", 401);

        if (tokenInfo.Expiry < DateTime.UtcNow)
        {
            _refreshTokens.Remove(refreshToken);
            throw new BusinessException("RefreshToken 已过期", 401);
        }

        // 移除旧 Token
        _refreshTokens.Remove(refreshToken);

        var user = await _userRepo.GetByIdAsync(tokenInfo.UserId);
        if (user == null || user.Status != 1)
            throw new BusinessException("账号异常", 401);

        var roles = await _db.Queryable<SysUserRole>()
            .LeftJoin<SysRole>((ur, r) => ur.RoleId == r.Id)
            .Where((ur, r) => ur.UserId == user.Id && r.Status == 1)
            .Select((ur, r) => r.RoleCode)
            .ToListAsync();

        var newAccessToken = _jwtService.GenerateAccessToken(user.Id, user.UserName, roles);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        _refreshTokens[newRefreshToken] = (user.Id,
            DateTime.UtcNow.AddDays(_securityConfig.Jwt.RefreshTokenExpireDays));

        return new LoginResultDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = _securityConfig.Jwt.AccessTokenExpireMinutes * 60
        };
    }

    public async Task<CurrentUserDto> GetCurrentUserAsync(long userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new BusinessException("用户不存在");

        // 角色编码
        var roles = await _db.Queryable<SysUserRole>()
            .LeftJoin<SysRole>((ur, r) => ur.RoleId == r.Id)
            .Where((ur, r) => ur.UserId == userId && r.Status == 1)
            .Select((ur, r) => r.RoleCode)
            .ToListAsync();

        // 权限标识
        List<string> permissions;
        if (roles.Contains("admin"))
        {
            permissions = ["*:*:*"]; // 超管拥有全部权限
        }
        else
        {
            permissions = await _db.Queryable<SysUserRole>()
                .LeftJoin<SysRoleMenu>((ur, rm) => ur.RoleId == rm.RoleId)
                .LeftJoin<SysMenu>((ur, rm, m) => rm.MenuId == m.Id)
                .Where((ur, rm, m) => ur.UserId == userId
                    && m.Status == 1 && m.Permission != null && m.Permission != "")
                .Select((ur, rm, m) => m.Permission!)
                .Distinct()
                .ToListAsync();
        }

        return new CurrentUserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            NickName = user.NickName,
            Avatar = user.Avatar,
            Roles = roles,
            Permissions = permissions
        };
    }
}
