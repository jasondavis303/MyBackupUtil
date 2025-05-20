using System.Text.Json.Serialization;

namespace MyBackupUtil.ConfigOptions;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Config))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
