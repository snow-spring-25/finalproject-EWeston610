using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HandAndFoot.Logic
{
    public class Deck
    {
        private readonly List<Card> _cards = new();
        private readonly Random _rng = new();

        public Deck() { }

        public Deck(int decks)
        {
            var suits = new[] { Suit.Clubs, Suit.Diamonds, Suit.Hearts, Suit.Spades };
            for (int d = 0; d < decks; d++)
            {
                foreach (var suit in suits)
                    foreach (var rank in Enum.GetValues<Rank>().Where(r => r != Rank.Joker))
                        _cards.Add(new Card(suit, rank));
                _cards.Add(new Card(Suit.Joker, Rank.Joker));
                _cards.Add(new Card(Suit.Joker, Rank.Joker));
            }
        }

        [JsonInclude]
        public List<Card> Cards
        {
            get => _cards;
            private set { _cards.Clear(); _cards.AddRange(value); }
        }

        public int Count => _cards.Count;

        public void Shuffle()
        {
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                var tmp = _cards[i];
                _cards[i] = _cards[j];
                _cards[j] = tmp;
            }
        }

        public void AddToBottom(Card card) => _cards.Insert(0, card);

        public Card Draw()
        {
            if (_cards.Count == 0)
                throw new InvalidOperationException("Stock is empty");
            var c = _cards[^1];
            _cards.RemoveAt(_cards.Count - 1);
            return c;
        }
    }
}
