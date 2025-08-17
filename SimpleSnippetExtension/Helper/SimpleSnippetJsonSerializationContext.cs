using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleSnippetExtension.Helper;

[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(SnippetType))]
[JsonSerializable(typeof(SnippetItem))]
[JsonSerializable(typeof(List<SnippetItem>))]
[JsonSourceGenerationOptions(UseStringEnumConverter = true, WriteIndented = true, IncludeFields = true, PropertyNameCaseInsensitive = true, AllowTrailingCommas = true)]
internal sealed partial class SimpleSnippetJsonSerializationContext : JsonSerializerContext
{
    
}