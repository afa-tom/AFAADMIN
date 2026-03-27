namespace AFAADMIN.System.Application.Dtos;

// ===== 部门 =====
public class DeptDto
{
    public long Id { get; set; }
    public long ParentId { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public int Sort { get; set; }
    public string? Leader { get; set; }
    public string? Phone { get; set; }
    public int Status { get; set; }
    public DateTime CreateTime { get; set; }
    public List<DeptDto> Children { get; set; } = [];
}

public class CreateDeptDto
{
    public long ParentId { get; set; } = 0;
    public string DeptName { get; set; } = string.Empty;
    public int Sort { get; set; } = 0;
    public string? Leader { get; set; }
    public string? Phone { get; set; }
    public int Status { get; set; } = 1;
}

public class UpdateDeptDto
{
    public long Id { get; set; }
    public long ParentId { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public int Sort { get; set; }
    public string? Leader { get; set; }
    public string? Phone { get; set; }
    public int Status { get; set; }
}

// ===== 字典类型 =====
public class DictTypeDto
{
    public long Id { get; set; }
    public string DictName { get; set; } = string.Empty;
    public string DictCode { get; set; } = string.Empty;
    public int Status { get; set; }
    public string? Remark { get; set; }
    public DateTime CreateTime { get; set; }
}

public class CreateDictTypeDto
{
    public string DictName { get; set; } = string.Empty;
    public string DictCode { get; set; } = string.Empty;
    public int Status { get; set; } = 1;
    public string? Remark { get; set; }
}

public class UpdateDictTypeDto
{
    public long Id { get; set; }
    public string DictName { get; set; } = string.Empty;
    public string DictCode { get; set; } = string.Empty;
    public int Status { get; set; }
    public string? Remark { get; set; }
}

// ===== 字典数据 =====
public class DictDataDto
{
    public long Id { get; set; }
    public long DictTypeId { get; set; }
    public string DictLabel { get; set; } = string.Empty;
    public string DictValue { get; set; } = string.Empty;
    public int Sort { get; set; }
    public string? CssClass { get; set; }
    public int Status { get; set; }
    public string? Remark { get; set; }
}

public class CreateDictDataDto
{
    public long DictTypeId { get; set; }
    public string DictLabel { get; set; } = string.Empty;
    public string DictValue { get; set; } = string.Empty;
    public int Sort { get; set; } = 0;
    public string? CssClass { get; set; }
    public int Status { get; set; } = 1;
    public string? Remark { get; set; }
}

public class UpdateDictDataDto
{
    public long Id { get; set; }
    public long DictTypeId { get; set; }
    public string DictLabel { get; set; } = string.Empty;
    public string DictValue { get; set; } = string.Empty;
    public int Sort { get; set; }
    public string? CssClass { get; set; }
    public int Status { get; set; }
    public string? Remark { get; set; }
}
