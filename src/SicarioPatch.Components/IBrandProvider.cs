namespace SicarioPatch.Components
{
    public interface IBrandProvider
    {
        public string ProjectName { get; }
        public string AppName { get; }
        public string OwnerName { get; }
        public string ShortName { get; }
        public string ToolName { get; }
        public string GameName { get; }
    }
}