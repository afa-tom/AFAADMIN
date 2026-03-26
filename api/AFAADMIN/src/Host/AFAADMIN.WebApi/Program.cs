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

// ========== 3. 构建并配置中间件管道 ==========
var app = builder.Build();

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

// 3.4 认证 & 授权（M3 阶段启用）
// app.UseAuthentication();
// app.UseAuthorization();

// 3.5 路由映射
app.MapControllers();
app.MapHealthChecks("/health");

// ========== 4. 启动 ==========
Log.Information("AFAADMIN 服务启动中...");
app.Run();
