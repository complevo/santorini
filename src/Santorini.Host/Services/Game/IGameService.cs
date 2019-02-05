using System.Threading.Tasks;

namespace Santorini.Host
{
    public interface IGameService
    {
        Task PlaceWorkers();
        void RegisterPlayers();
        Task StartGame();
        Game GetGameReport();
    }
}