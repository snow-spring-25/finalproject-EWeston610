namespace CardGameNew.Logic
{
    // REQ#2.1.1
    public abstract class Meld : IMeld
    {
        protected readonly List<Card> _cards = new List<Card>();
        public Rank Rank { get; }
        public bool IsClean { get; }
        public IReadOnlyList<Card> Cards => _cards;

        protected Meld(Rank rank, bool isClean)
        {
            Rank = rank;
            IsClean = isClean;
        }

        public abstract bool CanAdd(Card card);

        public void Add(Card card)
        {
            if (!CanAdd(card))
                throw new InvalidOperationException("Cannot add that card to this meld.");
            _cards.Add(card);
        }

        // REQ#2.1.1
        public bool IsComplete => _cards.Count >= 3 && _cards.Count <= 7;
    }
}