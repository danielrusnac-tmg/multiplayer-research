using System.Threading.Tasks;
using Fusion;

namespace TMG.Survival.Networking
{
    public interface INetworkManager
    {
        Task<bool> StartGame(GameMode mode);
    }
}