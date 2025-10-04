using System.Text.Json.Serialization;

namespace Mediavax.Core;

[JsonSerializable(typeof(YtDlpInfo))]
[JsonSerializable(typeof(YtDlpFormat))]
internal partial class YtDlpJsonContext : JsonSerializerContext
{
}
