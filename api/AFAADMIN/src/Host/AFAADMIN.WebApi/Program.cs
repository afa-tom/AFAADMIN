using AFAADMIN.AI;
using AFAADMIN.Common.IdGen;
using AFAADMIN.Database;
using AFAADMIN.EventBus;
using AFAADMIN.Storage;
using AFAADMIN.System.Application;
using AFAADMIN.System.Infrastructure;
using AFAADMIN.Tools;
using AFAADMIN.Web.Core.Authentication;
using AFAADMIN.Web.Core.Extensions;
using AFAADMIN.Web.Core.Filters;
using AFAADMIN.Web.Core.Middlewares;
using Serilog;

// ========== 1. 构建 Host ==========
var builder = WebApplication.CreateBuilder(args);

// 1.1 多配置文件加载
builder.Configuration.AddAfaConfigurations(builder.Environment.ContentRootPath);

// 1.2 Serilog 日志
builder.Host.UseAfaSerilog();

// ========== 2. 注册服务 ==========

// 2.1 控制器 + 全局过滤器
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<ApiResultWrapperFilter>();
    options.Filters.Add<PermissionFilter>();
    options.Filters.Add<ValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Encoder =
        System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    options.JsonSerializerOptions.PropertyNamingPolicy =
        System.Text.Json.JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(
        new AFAADMIN.WebApi.Converters.DateTimeJsonConverter());
});

// 2.2 Swagger
builder.Services.AddAfaSwagger();

// 2.3 自动扫描 DI
builder.Services.AddAfaDependencies();

// 2.4 CORS 跨域
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 2.5 健康检查
builder.Services.AddHealthChecks();

// 2.6 数据库（M2）
builder.Services.AddAfaDatabase(builder.Configuration);

// 2.7 安全配置（M2）
builder.Services.AddAfaSecurity(builder.Configuration);

// 2.8 防抖限流（M2）
builder.Services.AddAfaRateLimiting(builder.Configuration);

// 2.9 JWT 认证 + 授权（M3）
builder.Services.AddAfaJwtAuth(builder.Configuration);

// 2.10 System 模块 Application 层（M3）
builder.Services.AddSystemApplication();

// 2.11 Redis 缓存（M4）
builder.Services.AddAfaRedis(builder.Configuration);

// 2.12 文件存储引擎（M4）
builder.Services.AddAfaStorage(builder.Configuration);

// 2.13 事件总线 MediatR（M4）
builder.Services.AddAfaEventBus();

// 2.14 工具链（M4）
builder.Services.AddAfaTools(builder.Configuration);

// 2.15 AI Copilot（M8）
builder.Services.AddAfaAI(builder.Configuration);

// 2.16 雪花 ID 初始化（M4）
IdHelper.Init(datacenterId: 1, workerId: 1);

// ========== 3. 构建并配置中间件管道 ==========
var app = builder.Build();

// 3.0 数据库初始化 + 种子数据（开发环境）
if (app.Environment.IsDevelopment())
{
    app.Services.InitDatabase(createTable: true);
    app.Services.InitSystemModule();
}

// 3.1 开发环境启用 Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AFAADMIN API V1");
        c.RoutePrefix = "swagger";
    });
}

// 3.2 全局中间件
app.UseCors("AllowAll");
app.UseRouting();

// 3.3 请求日志
app.UseSerilogRequestLogging();

// 3.4 API 报文加解密中间件（M2）
app.UseAfaEncryption();

// 3.5 防抖限流（M2）
app.UseAfaRateLimiting(builder.Configuration);

// 3.6 认证 & 授权（M3）
app.UseAuthentication();
app.UseAuthorization();

// 3.7 Token 黑名单中间件（M4，放在认证之后）
app.UseMiddleware<TokenBlacklistMiddleware>();

// 3.8 本地文件静态访问（M4）
var storagePath = Path.Combine(AppContext.BaseDirectory, "uploads");
if (!Directory.Exists(storagePath)) Directory.CreateDirectory(storagePath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(storagePath),
    RequestPath = "/files"
});

// 3.9 路由映射
app.MapControllers();
app.MapHealthChecks("/health");

// ========== 4. 启动 ==========
Log.Information("AFAADMIN 服务启动中...");
app.Run();
