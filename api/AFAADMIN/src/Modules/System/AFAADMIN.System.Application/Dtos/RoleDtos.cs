namespace AFAADMIN.System.Application.Dtos;

public class RoleDto
{
    public long Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public int Sort { get; set; }
    public int Status { get; set; }
    public string? Remark { get; set; }
    public DateTime CreateTime { get; set; }
    public List<long> MenuIds { get; set; } = [];
}

public class CreateRoleDto
{
    public string RoleName { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public int Sort { get; set; } = 0;
    public int Status { get; set; } = 1;
    public string? Remark { get; set; }
    public List<long> MenuIds { get; set; } = [];
}

public class UpdateRoleDto
{
    public long Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public int Sort { get; set; }
    public int Status { get; set; }
    public string? Remark { get; set; }
    public List<long> MenuIds { get; set; } = [];
}
