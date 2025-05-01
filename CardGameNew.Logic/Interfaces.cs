namespace CardGameNew.Logic
{
    public interface IMeld
    {
        Rank Rank { get; }
        bool IsClean { get; }
        IReadOnlyList<Card> Cards { get; }
        bool CanAdd(Card card);
        void Add(Card card);
        bool IsComplete { get; }
    }


    public interface IGameSerializer
    {
        void Save(Game game, string path);
        Game Load(string path);
    }
}
