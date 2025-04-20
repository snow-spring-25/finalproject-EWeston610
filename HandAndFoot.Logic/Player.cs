using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HandAndFoot.Logic
{
    public class Player
    {
        [JsonInclude]
        public string Name { get; private set; }

        [JsonInclude]
        public List<Card> Hand { get; private set; } = new();

        [JsonInclude]
        public List<Card> Foot { get; private set; } = new();

        public bool HasUsedHand => Hand.Count == 0;

        [JsonConstructor]
        public Player(string name, List<Card> hand, List<Card> foot)
        {
            Name = name;
            Hand = hand ?? new List<Card>();
            Foot = foot ?? new List<Card>();
        }

        public Player(string name)
        {
            Name = name;
        }

        public IEnumerable<Card> DrawFromStock(Deck stock, int count = 2)
        {
            var drawn = new List<Card>();
            for (int i = 0; i < count; i++)
                drawn.Add(stock.Draw());
            return drawn;
        }

        public void Discard(Card c, DiscardPile discardPile)
        {
            var pile = HasUsedHand ? Foot : Hand;
            if (!pile.Remove(c))
                throw new InvalidOperationException("Player does not have that card.");
            discardPile.Add(c);
        }

        public void PlayToMeld(Card c, Meld meld)
        {
            var pile = HasUsedHand ? Foot : Hand;
            if (!pile.Remove(c))
                throw new InvalidOperationException("Player does not have that card.");
            meld.Add(c);
        }

        public void PickupDiscard(DiscardPile discardPile)
        {
            var cards = discardPile.TakeTop(7);
            foreach (var c in cards)
                Hand.Add(c);
        }

        public void ActivateFoot()
        {
            if (!HasUsedHand)
                throw new InvalidOperationException("Cannot activate foot before emptying hand.");
        }
    }
}
