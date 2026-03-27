using AFAADMIN.Common.Models;
using AFAADMIN.System.Application.Dtos;
using AFAADMIN.System.Application.Services;
using AFAADMIN.Web.Core.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AFAADMIN.WebApi.Controllers;

/// <summary>
/// 用户管理
/// </summary>
[Route("api/system/user")]
[Authorize]
public class UserController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 分页查询用户
    /// </summary>
    [HttpGet("page")]
    [Permission("sys:user:list")]
    public async Task<IActionResult> GetPage([FromQuery] UserQueryDto query)
    {
        var result = await _userService.GetPageAsync(query);
        return Ok(ApiResult<PageResult<UserDto>>.Success(result));
    }

    /// <summary>
    /// 根据 ID 获取用户详情
    /// </summary>
    [HttpGet("{id}")]
    [Permission("sys:user:list")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _userService.GetByIdAsync(id);
        return Ok(ApiResult<UserDto?>.Success(result));
    }

    /// <summary>
    /// 新增用户
    /// </summary>
    [HttpPost]
    [Permission("sys:user:add")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var id = await _userService.CreateAsync(dto);
        return Ok(ApiResult<long>.Success(id, "创建成功"));
    }

    /// <summary>
    /// 修改用户
    /// </summary>
    [HttpPut]
    [Permission("sys:user:edit")]
    public async Task<IActionResult> Update([FromBody] UpdateUserDto dto)
    {
        await _userService.UpdateAsync(dto);
        return Ok(ApiResult.Success("修改成功"));
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    [HttpDelete("{id}")]
    [Permission("sys:user:delete")]
    public async Task<IActionResult> Delete(long id)
    {
        await _userService.DeleteAsync(id);
        return Ok(ApiResult.Success("删除成功"));
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    [HttpPut("reset-password")]
    [Permission("sys:user:resetpwd")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        await _userService.ResetPasswordAsync(dto);
        return Ok(ApiResult.Success("密码已重置"));
    }

    /// <summary>
    /// 分配角色
    /// </summary>
    [HttpPut("{userId}/roles")]
    [Permission("sys:user:edit")]
    public async Task<IActionResult> SetRoles(long userId, [FromBody] List<long> roleIds)
    {
        await _userService.SetRolesAsync(userId, roleIds);
        return Ok(ApiResult.Success("角色分配成功"));
    }
}
