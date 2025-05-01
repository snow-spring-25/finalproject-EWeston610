namespace CardGameNew.Logic
{
    // REQ#2.1.2: DirtyMeld inherits from base class Meld
    // REQ#2.2.2: Implements IMeld through base class
    public class DirtyMeld : Meld
    {
        public DirtyMeld(Rank rank) : base(rank, false) { }
        public override bool CanAdd(Card card)
        {
            if (card.Rank != Rank && !card.IsWild) return false;
            // REQ#1.3.3
            return _cards.Count < 7;
        }
    }
}