# ========== 阶段1: 编译 ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 先复制 sln 和所有 csproj 以利用 Docker 缓存层
COPY AFAADMIN.sln .
COPY src/Host/AFAADMIN.WebApi/AFAADMIN.WebApi.csproj src/Host/AFAADMIN.WebApi/
COPY src/Framework/AFAADMIN.Common/AFAADMIN.Common.csproj src/Framework/AFAADMIN.Common/
COPY src/Framework/AFAADMIN.Database/AFAADMIN.Database.csproj src/Framework/AFAADMIN.Database/
COPY src/Framework/AFAADMIN.Web.Core/AFAADMIN.Web.Core.csproj src/Framework/AFAADMIN.Web.Core/
COPY src/Framework/AFAADMIN.Storage/AFAADMIN.Storage.csproj src/Framework/AFAADMIN.Storage/
COPY src/Framework/AFAADMIN.EventBus/AFAADMIN.EventBus.csproj src/Framework/AFAADMIN.EventBus/
COPY src/Framework/AFAADMIN.AI/AFAADMIN.AI.csproj src/Framework/AFAADMIN.AI/
COPY src/Framework/AFAADMIN.Tools/AFAADMIN.Tools.csproj src/Framework/AFAADMIN.Tools/
COPY src/Modules/System/AFAADMIN.System.Domain/AFAADMIN.System.Domain.csproj src/Modules/System/AFAADMIN.System.Domain/
COPY src/Modules/System/AFAADMIN.System.Application/AFAADMIN.System.Application.csproj src/Modules/System/AFAADMIN.System.Application/
COPY src/Modules/System/AFAADMIN.System.Infrastructure/AFAADMIN.System.Infrastructure.csproj src/Modules/System/AFAADMIN.System.Infrastructure/

RUN dotnet restore

# 复制全部源码并发布
COPY . .
RUN dotnet publish src/Host/AFAADMIN.WebApi/AFAADMIN.WebApi.csproj \
    -c Release -o /app/publish --no-restore

# ========== 阶段2: 运行 ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 设置时区
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# 创建日志目录
RUN mkdir -p /app/logs

COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "AFAADMIN.WebApi.dll"]
