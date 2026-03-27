using AFAADMIN.Common.Models;
using AFAADMIN.System.Application.Dtos;
using AFAADMIN.System.Application.Services;
using AFAADMIN.Web.Core.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AFAADMIN.WebApi.Controllers;

/// <summary>
/// 角色管理
/// </summary>
[Route("api/system/role")]
[Authorize]
public class RoleController : ApiControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet("list")]
    [Permission("sys:role:list")]
    public async Task<IActionResult> GetList()
    {
        var result = await _roleService.GetListAsync();
        return Ok(ApiResult<List<RoleDto>>.Success(result));
    }

    [HttpGet("{id}")]
    [Permission("sys:role:list")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _roleService.GetByIdAsync(id);
        return Ok(ApiResult<RoleDto?>.Success(result));
    }

    [HttpPost]
    [Permission("sys:role:add")]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
        var id = await _roleService.CreateAsync(dto);
        return Ok(ApiResult<long>.Success(id, "创建成功"));
    }

    [HttpPut]
    [Permission("sys:role:edit")]
    public async Task<IActionResult> Update([FromBody] UpdateRoleDto dto)
    {
        await _roleService.UpdateAsync(dto);
        return Ok(ApiResult.Success("修改成功"));
    }

    [HttpDelete("{id}")]
    [Permission("sys:role:delete")]
    public async Task<IActionResult> Delete(long id)
    {
        await _roleService.DeleteAsync(id);
        return Ok(ApiResult.Success("删除成功"));
    }

    /// <summary>
    /// 分配菜单权限
    /// </summary>
    [HttpPut("{roleId}/menus")]
    [Permission("sys:role:edit")]
    public async Task<IActionResult> SetMenus(long roleId, [FromBody] List<long> menuIds)
    {
        await _roleService.SetMenusAsync(roleId, menuIds);
        return Ok(ApiResult.Success("菜单分配成功"));
    }
}
