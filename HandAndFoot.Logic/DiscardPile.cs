using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HandAndFoot.Logic
{
    public class DiscardPile
    {
        private readonly List<Card> _cards = new();

        public DiscardPile() { }

        public DiscardPile(Deck stock)
        {
            Card first;
            do
            {
                first = stock.Draw();
                if (first.IsRedThree || first.IsWild)
                    stock.AddToBottom(first);
            } while (first.IsRedThree || first.IsWild);
            _cards.Add(first);
        }

        [JsonInclude]
        public List<Card> Cards
        {
            get => _cards;
            private set { _cards.Clear(); _cards.AddRange(value); }
        }

        public Card Peek() => _cards[^1];

        public void Add(Card card) => _cards.Add(card);

        public List<Card> TakeTop(int count)
        {
            if (_cards.Count < count)
                throw new InvalidOperationException("Not enough cards in discard pile.");
            var top = _cards.GetRange(_cards.Count - count, count);
            _cards.RemoveRange(_cards.Count - count, count);
            return top;
        }
    }
}