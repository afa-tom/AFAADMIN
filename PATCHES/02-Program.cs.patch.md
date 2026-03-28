# Program.cs 修改

## 2.1 顶部新增 using:
```csharp
using AFAADMIN.AI;
```

## 2.2 在 `// 2.14 工具链（M4）` 之后新增:
```csharp
// 2.15 AI Copilot（M8）
builder.Services.AddAfaAI(builder.Configuration);
```

注意：原 `// 2.15 雪花 ID` 改为 `// 2.16 雪花 ID`
