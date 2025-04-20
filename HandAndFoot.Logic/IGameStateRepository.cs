using System.Threading.Tasks;

namespace HandAndFoot.Logic
{
    public interface IGameStateRepository
    {
        Task SaveAsync(HandAndFootGame game, string path);
        Task<HandAndFootGame> LoadAsync(string path);
    }
}