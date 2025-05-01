using System;
using System.Collections.Generic;

namespace CardGameNew.Logic
{
    public static class DeckBuilder
    {
        public static Deck CreateDrawPile(int numberOfDecks)
        {
            var cards = new List<Card>();
            for (int i = 0; i < numberOfDecks; i++)
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    if (rank == Rank.Joker) continue;
                    for (int j = 0; j < 4; j++) cards.Add(new Card(rank));
                }
                cards.Add(new Card(Rank.Joker));
                cards.Add(new Card(Rank.Joker));
            }
            var deck = new Deck(cards);
            deck.Shuffle();
            return deck;
        }

        public static void DealInitialHands(Game game, int cardsPerHand = 11)
        {
            for (int i = 0; i < cardsPerHand; i++)
                foreach (var player in game.Players)
                    player.Hand.Add(game.DrawPile.Draw());
        }

        public static void DealInitialFoots(Game game, int cardsPerFoot = 11)
        {
            for (int i = 0; i < cardsPerFoot; i++)
                foreach (var player in game.Players)
                    player.Foot.Add(game.DrawPile.Draw());
        }

        public static Game SetupGame(string[] playerNames, int numberOfDecks = 5)
        {
            var game = new Game(playerNames);
            game.DrawPile = CreateDrawPile(numberOfDecks);
            DealInitialHands(game);
            DealInitialFoots(game);
            return game;
        }
    }
}