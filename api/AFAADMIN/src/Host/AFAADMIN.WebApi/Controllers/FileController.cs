using AFAADMIN.Common.Models;
using AFAADMIN.Storage;
using AFAADMIN.Web.Core.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AFAADMIN.WebApi.Controllers;

/// <summary>
/// 文件管理接口
/// </summary>
[Route("api/system/file")]
[Authorize]
public class FileController : ApiControllerBase
{
    private readonly IStorageProvider _storage;
    private readonly ILogger<FileController> _logger;

    public FileController(IStorageProvider storage, ILogger<FileController> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    [HttpPost("upload")]
    [Permission("sys:file:upload")]
    [RequestSizeLimit(50 * 1024 * 1024)] // 50MB
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string? directory = null)
    {
        if (file.Length == 0)
            return Ok(ApiResult.Fail("文件不能为空"));

        await using var stream = file.OpenReadStream();
        var filePath = await _storage.UploadAsync(stream, file.FileName, directory);
        var url = _storage.GetUrl(filePath);

        var result = new FileUploadResult
        {
            FilePath = filePath,
            Url = url,
            OriginalName = file.FileName,
            Size = file.Length,
            Extension = Path.GetExtension(file.FileName)
        };

        return Ok(ApiResult<FileUploadResult>.Success(result, "上传成功"));
    }

    /// <summary>
    /// 批量上传
    /// </summary>
    [HttpPost("upload/batch")]
    [Permission("sys:file:upload")]
    [RequestSizeLimit(100 * 1024 * 1024)] // 100MB
    public async Task<IActionResult> BatchUpload(List<IFormFile> files, [FromQuery] string? directory = null)
    {
        if (files.Count == 0)
            return Ok(ApiResult.Fail("请选择文件"));

        var results = new List<FileUploadResult>();
        foreach (var file in files)
        {
            if (file.Length == 0) continue;

            await using var stream = file.OpenReadStream();
            var filePath = await _storage.UploadAsync(stream, file.FileName, directory);

            results.Add(new FileUploadResult
            {
                FilePath = filePath,
                Url = _storage.GetUrl(filePath),
                OriginalName = file.FileName,
                Size = file.Length,
                Extension = Path.GetExtension(file.FileName)
            });
        }

        return Ok(ApiResult<List<FileUploadResult>>.Success(results, $"成功上传 {results.Count} 个文件"));
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    [HttpGet("download")]
    [Unencrypted]
    public async Task<IActionResult> Download([FromQuery] string filePath)
    {
        var stream = await _storage.DownloadAsync(filePath);
        if (stream == null)
            return NotFound(ApiResult.Fail("文件不存在", 404));

        var fileName = Path.GetFileName(filePath);
        return File(stream, "application/octet-stream", fileName);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    [HttpDelete]
    [Permission("sys:file:delete")]
    public async Task<IActionResult> Delete([FromQuery] string filePath)
    {
        var result = await _storage.DeleteAsync(filePath);
        return Ok(result ? ApiResult.Success("删除成功") : ApiResult.Fail("文件不存在"));
    }
}
