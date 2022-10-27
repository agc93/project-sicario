using System;
using System.Collections.Generic;

namespace SicarioPatch.Core;

public record PatchRequestSummary(string Id)
{
    public DateTime BuildTime { get;} = DateTime.UtcNow;
    public List<string> IncludedPatches { get; init; } = new List<string>();
    public Dictionary<string, string> Inputs { get; init; } = new Dictionary<string, string>();
    public string FileName { get; set; }
    public string UserName { get; set; }
}