namespace AFAADMIN.System.Application.Dtos;

/// <summary>
/// 用户列表输出
/// </summary>
public class UserDto
{
    public long Id { get; set; }
    public long? DeptId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? NickName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Avatar { get; set; }
    public int Status { get; set; }
    public string? Remark { get; set; }
    public DateTime CreateTime { get; set; }
    public string? DeptName { get; set; }
    public List<long> RoleIds { get; set; } = [];
}

/// <summary>
/// 创建用户
/// </summary>
public class CreateUserDto
{
    public long? DeptId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? NickName { get; set; }
    public string Password { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int Status { get; set; } = 1;
    public string? Remark { get; set; }
    public List<long> RoleIds { get; set; } = [];
}

/// <summary>
/// 更新用户
/// </summary>
public class UpdateUserDto
{
    public long Id { get; set; }
    public long? DeptId { get; set; }
    public string? NickName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int Status { get; set; }
    public string? Remark { get; set; }
    public List<long> RoleIds { get; set; } = [];
}

/// <summary>
/// 用户查询参数
/// </summary>
public class UserQueryDto
{
    public string? UserName { get; set; }
    public string? Phone { get; set; }
    public int? Status { get; set; }
    public long? DeptId { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 重置密码
/// </summary>
public class ResetPasswordDto
{
    public long UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}
