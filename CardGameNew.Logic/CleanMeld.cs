namespace CardGameNew.Logic
{
    public class CleanMeld : Meld
    {
        public CleanMeld(Rank rank) : base(rank, true) { }
        public override bool CanAdd(Card card)
        {
            if (card.IsWild) return false;
            if (card.Rank != Rank) return false;
            return _cards.Count < 7;
        }
    }
}