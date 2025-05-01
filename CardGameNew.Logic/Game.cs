using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace CardGameNew.Logic
{
    public class Player
    {
        [JsonInclude]
        public string Name { get; private set; }
        [JsonInclude]
        public Deck Hand { get; private set; } = new Deck();
        [JsonInclude]
        public Deck Foot { get; private set; } = new Deck();
        [JsonInclude]
        public List<IMeld> Melds { get; private set; } = new List<IMeld>();

        [JsonConstructor]
        public Player(string name) => Name = name;

        public void Discard(Card card)
        {
            if (!Hand.Remove(card))
                throw new InvalidOperationException("Cannot discard a card not in hand.");
        }

        public void CreateMeld(Rank rank, bool isClean, IEnumerable<Card> cards)
        {
            IMeld meld = isClean ? new CleanMeld(rank) : new DirtyMeld(rank);
            foreach (var card in cards)
            {
                if (!Hand.Remove(card))
                    throw new InvalidOperationException("Player does not have that card.");
                meld.Add(card);
            }
            Melds.Add(meld);
        }
    }

    public class Game
    {
        [JsonInclude]
        public List<Player> Players { get; private set; }
        [JsonInclude]
        public int CurrentPlayerIndex { get; private set; }
        [JsonInclude]
        public Deck DrawPile { get; set; }

        public Game() { }

        public Game(params string[] playerNames)
        {
            if (playerNames.Length < 1)
                throw new ArgumentException("Must have at least one player.");
            Players = playerNames.Select(n => new Player(n)).ToList();
            CurrentPlayerIndex = 0;
        }

        public Player CurrentPlayer => Players[CurrentPlayerIndex];

        public void NextTurn() => CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;

        public int CalculateScore(Player player)
        {
            var cardScore = ScoreCalculator.CalculateCardScore(player.Hand)
                          + ScoreCalculator.CalculateCardScore(player.Foot);
            var meldScore = player.Melds.Sum(m => ScoreCalculator.CalculateMeldScore(m));
            return cardScore + meldScore;
        }
    }
}