using Microsoft.Extensions.Configuration;
using SicarioPatch.Components;

namespace SicarioPatch.App.Shared
{
    public class BrandProvider : IBrandProvider
    {
        public string ProjectName { get; init; } = "Project Sicario";
        public string OwnerName { get; init; } = "agc93";
        public string ShortName { get; init; } = "Sicario";
        public string ToolName { get; } = "Merger";
        public string AppName { get; } = "Builder";
    }
}