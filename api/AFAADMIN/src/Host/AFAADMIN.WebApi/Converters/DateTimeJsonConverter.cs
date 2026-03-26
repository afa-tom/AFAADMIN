using System.Text.Json;
using System.Text.Json.Serialization;

namespace AFAADMIN.WebApi.Converters;

/// <summary>
/// 统一 DateTime 序列化格式
/// </summary>
public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.TryParse(reader.GetString(), out var dt) ? dt : default;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}
