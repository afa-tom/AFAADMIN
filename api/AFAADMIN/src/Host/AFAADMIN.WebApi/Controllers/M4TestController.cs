using AFAADMIN.Common.Cache;
using AFAADMIN.Common.IdGen;
using AFAADMIN.Common.Models;
using AFAADMIN.Storage;
using AFAADMIN.Web.Core.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AFAADMIN.WebApi.Controllers;

/// <summary>
/// M4 阶段验证接口
/// </summary>
[Route("api/system/m4")]
public class M4TestController : ApiControllerBase
{
    private readonly ICacheService _cache;
    private readonly IStorageProvider _storage;

    public M4TestController(ICacheService cache, IStorageProvider storage)
    {
        _cache = cache;
        _storage = storage;
    }

    /// <summary>
    /// 测试 Redis 缓存
    /// </summary>
    [HttpGet("redis")]
    [Unencrypted]
    public async Task<IActionResult> TestRedis()
    {
        var key = "m4:test";
        var testValue = $"Hello Redis @ {DateTime.Now:HH:mm:ss}";

        await _cache.SetStringAsync(key, testValue, TimeSpan.FromMinutes(1));
        var cached = await _cache.GetStringAsync(key);
        var exists = await _cache.ExistsAsync(key);

        return Ok(ApiResult<object>.Success(new
        {
            Written = testValue,
            Read = cached,
            Exists = exists,
            Match = testValue == cached
        }));
    }

    /// <summary>
    /// 测试雪花 ID
    /// </summary>
    [HttpGet("snowflake")]
    [Unencrypted]
    public IActionResult TestSnowflake()
    {
        var ids = Enumerable.Range(0, 10).Select(_ => IdHelper.NextId()).ToList();
        return Ok(ApiResult<object>.Success(new
        {
            Ids = ids,
            Count = ids.Distinct().Count(),
            AllUnique = ids.Distinct().Count() == ids.Count
        }));
    }

    /// <summary>
    /// 测试存储引擎信息
    /// </summary>
    [HttpGet("storage")]
    [Unencrypted]
    public IActionResult TestStorage()
    {
        return Ok(ApiResult<object>.Success(new
        {
            Provider = _storage.GetType().Name,
            TestUrl = _storage.GetUrl("test/example.jpg")
        }));
    }
}
