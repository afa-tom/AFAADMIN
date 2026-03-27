using System.Diagnostics;

namespace AFAADMIN.Common.IdGen;

/// <summary>
/// 雪花算法 ID 生成器
/// 结构: 1bit符号位 + 41bit时间戳 + 5bit数据中心 + 5bit机器ID + 12bit序列号
/// </summary>
public class SnowflakeIdGenerator
{
    private const long Twepoch = 1704067200000L; // 2024-01-01 00:00:00 UTC
    private const int DatacenterIdBits = 5;
    private const int WorkerIdBits = 5;
    private const int SequenceBits = 12;

    private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits); // 31
    private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);         // 31
    private const long SequenceMask = -1L ^ (-1L << SequenceBits);        // 4095

    private const int WorkerIdShift = SequenceBits;
    private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
    private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

    private readonly long _datacenterId;
    private readonly long _workerId;
    private long _sequence;
    private long _lastTimestamp = -1L;
    private readonly object _lock = new();

    public SnowflakeIdGenerator(long datacenterId = 1, long workerId = 1)
    {
        if (datacenterId > MaxDatacenterId || datacenterId < 0)
            throw new ArgumentException($"datacenterId 不能大于 {MaxDatacenterId} 或小于 0");
        if (workerId > MaxWorkerId || workerId < 0)
            throw new ArgumentException($"workerId 不能大于 {MaxWorkerId} 或小于 0");

        _datacenterId = datacenterId;
        _workerId = workerId;
    }

    /// <summary>
    /// 生成下一个 ID
    /// </summary>
    public long NextId()
    {
        lock (_lock)
        {
            var timestamp = GetTimestamp();

            if (timestamp < _lastTimestamp)
                throw new InvalidOperationException($"时钟回退，拒绝生成 ID，回退 {_lastTimestamp - timestamp}ms");

            if (timestamp == _lastTimestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0)
                    timestamp = WaitNextMillis(_lastTimestamp);
            }
            else
            {
                _sequence = 0L;
            }

            _lastTimestamp = timestamp;

            return ((timestamp - Twepoch) << TimestampLeftShift)
                   | (_datacenterId << DatacenterIdShift)
                   | (_workerId << WorkerIdShift)
                   | _sequence;
        }
    }

    private static long GetTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static long WaitNextMillis(long lastTimestamp)
    {
        var timestamp = GetTimestamp();
        while (timestamp <= lastTimestamp)
        {
            timestamp = GetTimestamp();
        }
        return timestamp;
    }
}
