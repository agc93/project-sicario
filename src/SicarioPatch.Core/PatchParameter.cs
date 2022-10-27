using System.Text.Json.Serialization;

namespace SicarioPatch.Core;

public class PatchParameter
{
    public string Id { get; set; }
    public ParameterType Type { get; set; }
    public string Message { get; set; }
    public string Default { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Range { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Pattern { get; set; }
}

public enum ParameterType
{
    String,
    Number,
    Boolean
}