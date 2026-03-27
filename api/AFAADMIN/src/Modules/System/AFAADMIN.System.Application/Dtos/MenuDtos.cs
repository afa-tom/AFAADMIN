namespace AFAADMIN.System.Application.Dtos;

public class MenuDto
{
    public long Id { get; set; }
    public long ParentId { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public int MenuType { get; set; }
    public string? Permission { get; set; }
    public string? Path { get; set; }
    public string? Component { get; set; }
    public string? Icon { get; set; }
    public int Sort { get; set; }
    public bool Visible { get; set; }
    public int Status { get; set; }
    public DateTime CreateTime { get; set; }
    public List<MenuDto> Children { get; set; } = [];
}

public class CreateMenuDto
{
    public long ParentId { get; set; } = 0;
    public string MenuName { get; set; } = string.Empty;
    public int MenuType { get; set; } = 1;
    public string? Permission { get; set; }
    public string? Path { get; set; }
    public string? Component { get; set; }
    public string? Icon { get; set; }
    public int Sort { get; set; } = 0;
    public bool Visible { get; set; } = true;
    public int Status { get; set; } = 1;
}

public class UpdateMenuDto
{
    public long Id { get; set; }
    public long ParentId { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public int MenuType { get; set; }
    public string? Permission { get; set; }
    public string? Path { get; set; }
    public string? Component { get; set; }
    public string? Icon { get; set; }
    public int Sort { get; set; }
    public bool Visible { get; set; }
    public int Status { get; set; }
}
