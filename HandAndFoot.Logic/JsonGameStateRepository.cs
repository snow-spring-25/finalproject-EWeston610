using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace HandAndFoot.Logic
{
    public class JsonGameStateRepository : IGameStateRepository
    {
        public async Task SaveAsync(HandAndFootGame game, string path)
        {
            var opts = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(game, opts);
            await File.WriteAllTextAsync(path, json);
        }

        public async Task<HandAndFootGame> LoadAsync(string path)
        {
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<HandAndFootGame>(json)
                   ?? throw new FileNotFoundException($"Could not find or deserialize '{path}'");
        }
    }
}