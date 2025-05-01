namespace CardGameNew.Logic
{
    // REQ#2.1.2: CleanMeld inherits from base class Meld
    // REQ#2.2.2: Implements IMeld through base class
    public class CleanMeld : Meld
    {
        public CleanMeld(Rank rank) : base(rank, true) { }
        public override bool CanAdd(Card card)
        {
            if (card.IsWild) return false;
            if (card.Rank != Rank) return false;
            // REQ#1.1.3
            return _cards.Count < 7;
        }
    }
}