using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CardGameNew.Logic;

namespace CardGameNew.Persistence
{
    public class JsonGameSerializer : IGameSerializer
    {
        private readonly JsonSerializerOptions _options;

        public JsonGameSerializer()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            };
            _options.Converters.Add(new JsonStringEnumConverter());
        }

        public void Save(Game game, string path)
        {
            var json = JsonSerializer.Serialize(game, _options);
            File.WriteAllText(path, json);
        }

        public Game Load(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Game>(json, _options);
        }
    }
}