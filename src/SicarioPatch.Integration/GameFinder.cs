using System.IO;
using System.Linq;
using GameFinder;
using GameFinder.StoreHandlers.GOG;
using GameFinder.StoreHandlers.Steam;

namespace SicarioPatch.Integration
{
    public class GameFinder : IGameSource
    {
        public string? LocateGamePath() {
            AStoreGame game;
            var steam = new SteamHandler();
            if (steam.TryGetByID(895870, out var steamGame) && !string.IsNullOrWhiteSpace(steamGame?.Path)) {
                return steamGame.Path;
            }

            var gog = new GOGHandler();
            if (gog.Games.Any() && gog.Games.FirstOrDefault(g => g.GameID == 1430183808) is { } gogEntry) {
                return gogEntry.Path;
            }

            return null;
        }

        public string? GetGamePath() {
            return LocateGamePath();
        }

        public string? GetGamePakPath() {
            var gamePath = LocateGamePath();
            if (gamePath != null) {
                var pakPath = Path.Join(gamePath, "ProjectWingman", "Content", "Paks");
                if (Directory.Exists(pakPath) &&
                    Directory.GetFiles(pakPath, "ProjectWingman-WindowsNoEditor.pak").FirstOrDefault() is { } gamePak) {
                    return gamePak;
                }
            }

            return null;
        }
    }
}