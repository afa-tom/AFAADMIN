using Microsoft.AspNetCore.Mvc;

namespace AFAADMIN.WebApi.Controllers;

/// <summary>
/// API 控制器基类 - 所有 Controller 必须继承
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
}
