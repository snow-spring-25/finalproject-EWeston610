using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HandAndFoot.Logic
{
    public class HandAndFootGame
    {
        public int CurrentRound { get; private set; } = 1;

        [JsonInclude]
        public List<Player> Players { get; private set; } = new();

        [JsonInclude]
        public Deck StockPile { get; private set; }

        [JsonInclude]
        public DiscardPile DiscardPile { get; private set; }

        [JsonInclude]
        public List<Meld> TableMelds { get; private set; } = new();

        public HandAndFootGame() {}

        public HandAndFootGame(int numPlayers)
        {
            if (numPlayers < 2) throw new ArgumentException("At least two players required.");
            StockPile = new Deck(decks: 6);
            StockPile.Shuffle();
            DiscardPile = new DiscardPile(StockPile);

            for (int i = 1; i <= numPlayers; i++)
                Players.Add(new Player($"Player {i}"));

            DealHandsAndFeet();
        }

        private void DealHandsAndFeet()
        {
            foreach (var player in Players)
                for (int j = 0; j < 11; j++)
                    player.Hand.Add(StockPile.Draw());

            foreach (var player in Players)
                for (int j = 0; j < 11; j++)
                    player.Foot.Add(StockPile.Draw());
        }

        public bool CanOpenInitialMeld(int playerIndex, Meld meld)
        {
            if (playerIndex < 0 || playerIndex >= Players.Count)
                throw new ArgumentOutOfRangeException(nameof(playerIndex));
            return GameRound.MeetsMinimum(CurrentRound, meld.MeldPoints());
        }

        public void TakeTurn(int playerIndex, IList<Meld> newMelds, Card discardCard, bool pickUpDiscard = false)
        {
            var player = Players[playerIndex];

            if (player.HasUsedHand && player.Foot.Any())
                player.ActivateFoot();


            foreach (var c in player.DrawFromStock(StockPile, 2))
                (player.HasUsedHand ? player.Foot : player.Hand).Add(c);

            if (pickUpDiscard)
            {
                if (DiscardPile.Peek().IsBlackThree)
                    throw new InvalidOperationException("Cannot pick up pile with black three on top.");
                player.PickupDiscard(DiscardPile);
            }

            foreach (var meld in newMelds)
            {
                if (!CanOpenInitialMeld(playerIndex, meld))
                    throw new InvalidOperationException("Initial meld doesn't meet minimum.");
                TableMelds.Add(meld);
            }

            player.Discard(discardCard, DiscardPile);
        }

        public bool IsRoundOver =>
            Players.Any(p => p.HasUsedHand && !p.Foot.Any());

        public int CalculateRoundNetPoints()
        {
            int meldPoints = TableMelds.Sum(m => m.MeldPoints());
            int leftoverPoints = Players.Sum(p => p.Hand.Concat(p.Foot).Sum(c => c.PointValue));
            return meldPoints - leftoverPoints;
        }

        public int EndRound()
        {
            if (!IsRoundOver)
                throw new InvalidOperationException("Cannot end round before someone goes out.");
            int net = CalculateRoundNetPoints();
            NextRound();
            return net;
        }

        public void NextRound()
        {
            CurrentRound++;
            TableMelds.Clear();
            StockPile = new Deck(decks: 6);
            StockPile.Shuffle();
            DiscardPile = new DiscardPile(StockPile);
            foreach (var player in Players)
            {
                player.Hand.Clear();
                player.Foot.Clear();
            }
            DealHandsAndFeet();
        }
    }
}
