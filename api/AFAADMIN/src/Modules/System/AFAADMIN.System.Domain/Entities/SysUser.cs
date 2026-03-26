namespace AFAADMIN.System.Domain.Entities;

/// <summary>
/// 用户实体（M3 阶段完善字段与领域行为）
/// </summary>
public class SysUser
{
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Status { get; set; } = 1;
    public DateTime CreateTime { get; set; } = DateTime.Now;
}
