using System.Text.Json.Serialization;

namespace Mediavax.Core;

[JsonSerializable(type: typeof(YtDlpInfo))]
[JsonSerializable(type: typeof(YtDlpFormat))]
internal partial class YtDlpJsonContext : JsonSerializerContext
{
}