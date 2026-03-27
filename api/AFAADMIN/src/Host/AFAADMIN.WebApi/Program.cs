using AFAADMIN.Database;
using AFAADMIN.System.Application;
using AFAADMIN.System.Infrastructure;
using AFAADMIN.Web.Core.Authentication;
using AFAADMIN.Web.Core.Extensions;
using AFAADMIN.Web.Core.Filters;
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
    options.Filters.Add<PermissionFilter>();       // M3: 权限校验
    options.Filters.Add<ValidationFilter>();        // M3: 参数校验
})
.AddJsonOptions(options =>
{
    // 中文不转义
    options.JsonSerializerOptions.Encoder =
        System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    // 驼峰命名
    options.JsonSerializerOptions.PropertyNamingPolicy =
        System.Text.Json.JsonNamingPolicy.CamelCase;
    // 时间格式
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

// 2.6 数据库（M2 新增）
builder.Services.AddAfaDatabase(builder.Configuration);

// 2.7 安全配置（M2 新增）
builder.Services.AddAfaSecurity(builder.Configuration);

// 2.8 防抖限流（M2 新增）
builder.Services.AddAfaRateLimiting(builder.Configuration);

// 2.9 JWT 认证 + 授权（M3）
builder.Services.AddAfaJwtAuth(builder.Configuration);

// 2.10 System 模块 Application 层（M3）
builder.Services.AddSystemApplication();

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

// 3.4 API 报文加解密中间件（M2 新增）
app.UseAfaEncryption();

// 3.5 防抖限流（M2 新增）
app.UseAfaRateLimiting(builder.Configuration);

// 3.6 认证 & 授权（M3 启用）
app.UseAuthentication();
app.UseAuthorization();

// 3.7 路由映射
app.MapControllers();
app.MapHealthChecks("/health");

// ========== 4. 启动 ==========
Log.Information("AFAADMIN 服务启动中...");
app.Run();
