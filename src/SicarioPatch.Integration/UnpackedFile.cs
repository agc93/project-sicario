// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace SicarioPatch.Integration
{
    public record UnpackedFile
    {
        public string? AssetPath { get; set; }
        public string? OutputPath { get; set; }
        public string? SourceIndexHash { get; set; }
    }
}