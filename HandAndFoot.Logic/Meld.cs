using System;
using System.Collections.Generic;
using System.Linq;

namespace HandAndFoot.Logic
{
    public class Meld
    {
        private readonly List<Card> cards = new();
        public Rank Rank { get; }
        public bool IsClosed => cards.Count >= 7;
        public bool IsClean => !cards.Any(c => c.IsWild);
        public int Count => cards.Count;

        public Meld(Rank rank) => Rank = rank;
        public IReadOnlyList<Card> Cards => cards;

        public void Add(Card c)
        {
            if (c.Rank == Rank.Three)
                throw new InvalidOperationException("Cannot meld Threes.");
            if (c.Rank != Rank && !c.IsWild)
                throw new InvalidOperationException("Cannot add different rank card.");
            if (cards.Count >= 7)
                throw new InvalidOperationException("Meld is full.");
            cards.Add(c);
        }

        public bool IsValidInitial()
        {
            if (Count < 3 || Count > 7) return false;
            var naturals = cards.Count(c => !c.IsWild);
            var wilds = cards.Count(c => c.IsWild);
            if (wilds == 0)
                return naturals >= 3;
            return naturals >= 4 && wilds >= 1;
        }

        public int MeldPoints() => cards.Sum(c => c.PointValue);
    }
}