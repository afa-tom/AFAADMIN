using AFAADMIN.Common.Models;
using AFAADMIN.System.Application.Dtos;
using AFAADMIN.System.Application.Services;
using AFAADMIN.Web.Core.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AFAADMIN.WebApi.Controllers;

/// <summary>
/// 字典管理
/// </summary>
[Route("api/system/dict")]
[Authorize]
public class DictController : ApiControllerBase
{
    private readonly IDictService _dictService;

    public DictController(IDictService dictService)
    {
        _dictService = dictService;
    }

    // ===== 字典类型 =====

    [HttpGet("type/list")]
    [Permission("sys:dict:list")]
    public async Task<IActionResult> GetTypeList()
    {
        var result = await _dictService.GetTypeListAsync();
        return Ok(ApiResult<List<DictTypeDto>>.Success(result));
    }

    [HttpPost("type")]
    [Permission("sys:dict:add")]
    public async Task<IActionResult> CreateType([FromBody] CreateDictTypeDto dto)
    {
        var id = await _dictService.CreateTypeAsync(dto);
        return Ok(ApiResult<long>.Success(id, "创建成功"));
    }

    [HttpPut("type")]
    [Permission("sys:dict:edit")]
    public async Task<IActionResult> UpdateType([FromBody] UpdateDictTypeDto dto)
    {
        await _dictService.UpdateTypeAsync(dto);
        return Ok(ApiResult.Success("修改成功"));
    }

    [HttpDelete("type/{id}")]
    [Permission("sys:dict:delete")]
    public async Task<IActionResult> DeleteType(long id)
    {
        await _dictService.DeleteTypeAsync(id);
        return Ok(ApiResult.Success("删除成功"));
    }

    // ===== 字典数据 =====

    /// <summary>
    /// 根据字典类型 ID 获取数据列表
    /// </summary>
    [HttpGet("data/list/{dictTypeId}")]
    [Permission("sys:dict:list")]
    public async Task<IActionResult> GetDataByTypeId(long dictTypeId)
    {
        var result = await _dictService.GetDataListByTypeIdAsync(dictTypeId);
        return Ok(ApiResult<List<DictDataDto>>.Success(result));
    }

    /// <summary>
    /// 根据字典编码获取数据列表（前端公用接口，无需登录）
    /// </summary>
    [HttpGet("data/code/{dictCode}")]
    [AllowAnonymous]
    [Unencrypted]
    public async Task<IActionResult> GetDataByCode(string dictCode)
    {
        var result = await _dictService.GetDataListByCodeAsync(dictCode);
        return Ok(ApiResult<List<DictDataDto>>.Success(result));
    }

    [HttpPost("data")]
    [Permission("sys:dict:add")]
    public async Task<IActionResult> CreateData([FromBody] CreateDictDataDto dto)
    {
        var id = await _dictService.CreateDataAsync(dto);
        return Ok(ApiResult<long>.Success(id, "创建成功"));
    }

    [HttpPut("data")]
    [Permission("sys:dict:edit")]
    public async Task<IActionResult> UpdateData([FromBody] UpdateDictDataDto dto)
    {
        await _dictService.UpdateDataAsync(dto);
        return Ok(ApiResult.Success("修改成功"));
    }

    [HttpDelete("data/{id}")]
    [Permission("sys:dict:delete")]
    public async Task<IActionResult> DeleteData(long id)
    {
        await _dictService.DeleteDataAsync(id);
        return Ok(ApiResult.Success("删除成功"));
    }
}
