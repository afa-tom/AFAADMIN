using MediatR;

namespace AFAADMIN.System.Domain.Events;

/// <summary>
/// 用户创建事件
/// </summary>
public class UserCreatedEvent : INotification
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 用户角色变更事件（用于清除权限缓存）
/// </summary>
public class UserRoleChangedEvent : INotification
{
    public long UserId { get; set; }
}

/// <summary>
/// 角色权限变更事件（用于清除相关用户权限缓存）
/// </summary>
public class RolePermissionChangedEvent : INotification
{
    public long RoleId { get; set; }
}

/// <summary>
/// 字典数据变更事件（用于清除字典缓存）
/// </summary>
public class DictDataChangedEvent : INotification
{
    public string? DictCode { get; set; }
}
