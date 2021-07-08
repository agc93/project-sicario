namespace SicarioPatch.Components
{
    public interface IBrandProvider
    {
        public string AppName { get; }
        public string OwnerName { get; }
        public string ShortName { get; }
    }
}