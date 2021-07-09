namespace SicarioPatch.Integration
{
    public interface IGameSource
    {
        string? GetGamePath();
        string? GetGamePakPath();
    }
}