using AFAADMIN.Common.Models;
using AFAADMIN.System.Application.Dtos;
using AFAADMIN.System.Application.Services;
using AFAADMIN.Web.Core.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AFAADMIN.WebApi.Controllers;

/// <summary>
/// 菜单/权限管理
/// </summary>
[Route("api/system/menu")]
[Authorize]
public class MenuController : ApiControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// 获取菜单树
    /// </summary>
    [HttpGet("tree")]
    [Permission("sys:menu:list")]
    public async Task<IActionResult> GetTree()
    {
        var result = await _menuService.GetTreeAsync();
        return Ok(ApiResult<List<MenuDto>>.Success(result));
    }

    [HttpGet("{id}")]
    [Permission("sys:menu:list")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _menuService.GetByIdAsync(id);
        return Ok(ApiResult<MenuDto?>.Success(result));
    }

    [HttpPost]
    [Permission("sys:menu:add")]
    public async Task<IActionResult> Create([FromBody] CreateMenuDto dto)
    {
        var id = await _menuService.CreateAsync(dto);
        return Ok(ApiResult<long>.Success(id, "创建成功"));
    }

    [HttpPut]
    [Permission("sys:menu:edit")]
    public async Task<IActionResult> Update([FromBody] UpdateMenuDto dto)
    {
        await _menuService.UpdateAsync(dto);
        return Ok(ApiResult.Success("修改成功"));
    }

    [HttpDelete("{id}")]
    [Permission("sys:menu:delete")]
    public async Task<IActionResult> Delete(long id)
    {
        await _menuService.DeleteAsync(id);
        return Ok(ApiResult.Success("删除成功"));
    }

    /// <summary>
    /// 获取角色已分配的菜单 ID（供角色编辑页回显）
    /// </summary>
    [HttpGet("role/{roleId}")]
    [Permission("sys:role:list")]
    public async Task<IActionResult> GetMenuIdsByRole(long roleId)
    {
        var result = await _menuService.GetMenuIdsByRoleIdAsync(roleId);
        return Ok(ApiResult<List<long>>.Success(result));
    }
}
