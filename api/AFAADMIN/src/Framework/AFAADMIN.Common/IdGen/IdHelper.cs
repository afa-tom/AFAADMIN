namespace AFAADMIN.Common.IdGen;

/// <summary>
/// 全局 ID 生成器静态入口
/// </summary>
public static class IdHelper
{
    private static SnowflakeIdGenerator? _generator;
    private static readonly object _lock = new();

    /// <summary>
    /// 初始化（在 Program.cs 中调用一次）
    /// </summary>
    public static void Init(long datacenterId = 1, long workerId = 1)
    {
        if (_generator != null) return;
        lock (_lock)
        {
            _generator ??= new SnowflakeIdGenerator(datacenterId, workerId);
        }
    }

    /// <summary>
    /// 生成雪花 ID
    /// </summary>
    public static long NextId()
    {
        if (_generator == null)
            Init();
        return _generator!.NextId();
    }
}
