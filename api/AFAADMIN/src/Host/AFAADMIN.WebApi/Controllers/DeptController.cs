using AFAADMIN.Common.Models;
using AFAADMIN.System.Application.Dtos;
using AFAADMIN.System.Application.Services;
using AFAADMIN.Web.Core.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AFAADMIN.WebApi.Controllers;

/// <summary>
/// 部门管理
/// </summary>
[Route("api/system/dept")]
[Authorize]
public class DeptController : ApiControllerBase
{
    private readonly IDeptService _deptService;

    public DeptController(IDeptService deptService)
    {
        _deptService = deptService;
    }

    [HttpGet("tree")]
    [Permission("sys:dept:list")]
    public async Task<IActionResult> GetTree()
    {
        var result = await _deptService.GetTreeAsync();
        return Ok(ApiResult<List<DeptDto>>.Success(result));
    }

    [HttpGet("{id}")]
    [Permission("sys:dept:list")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _deptService.GetByIdAsync(id);
        return Ok(ApiResult<DeptDto?>.Success(result));
    }

    [HttpPost]
    [Permission("sys:dept:add")]
    public async Task<IActionResult> Create([FromBody] CreateDeptDto dto)
    {
        var id = await _deptService.CreateAsync(dto);
        return Ok(ApiResult<long>.Success(id, "创建成功"));
    }

    [HttpPut]
    [Permission("sys:dept:edit")]
    public async Task<IActionResult> Update([FromBody] UpdateDeptDto dto)
    {
        await _deptService.UpdateAsync(dto);
        return Ok(ApiResult.Success("修改成功"));
    }

    [HttpDelete("{id}")]
    [Permission("sys:dept:delete")]
    public async Task<IActionResult> Delete(long id)
    {
        await _deptService.DeleteAsync(id);
        return Ok(ApiResult.Success("删除成功"));
    }
}
