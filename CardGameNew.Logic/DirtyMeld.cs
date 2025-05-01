namespace CardGameNew.Logic
{
    public class DirtyMeld : Meld
    {
        public DirtyMeld(Rank rank) : base(rank, false) { }
        public override bool CanAdd(Card card)
        {
            if (card.Rank != Rank && !card.IsWild) return false;
            return _cards.Count < 7;
        }
    }
}