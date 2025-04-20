using System;
using System.Linq;
using System.Collections.Generic;
using HandAndFoot.Logic;

namespace HandAndFoot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Hand and Foot!");
            int numPlayers = PromptInt("Enter number of players (2+): ", 2);
            var game = new HandAndFootGame(numPlayers);

            int currentPlayer = 0;
            while (!game.IsRoundOver)
            {
                var player = game.Players[currentPlayer];
                Console.WriteLine($"\n--- {player.Name}'s Turn ---");

                // Show top of discard pile
                Console.WriteLine($"Discard Top: {game.DiscardPile.Peek()}");

                // Show hand or foot
                var pile = player.HasUsedHand ? player.Foot : player.Hand;
                Console.WriteLine(player.HasUsedHand ? "Your Foot:" : "Your Hand:");
                DisplayPile(pile);

                // Option to pick up discard pile
                bool pickUp = PromptYesNo("Pick up discard pile? (y/n): ");

                // Melding phase
                var newMelds = new List<Meld>();
                if (PromptYesNo("Do you want to start a new meld? (y/n): "))
                {
                    bool moreMelds = true;
                    while (moreMelds)
                    {
                        var meldRank = PromptRank("Select rank for meld (cannot be Three): ", allowThree: false);
                        var meld = new Meld(meldRank);
                        Console.WriteLine($"Building meld of {meldRank}...");

                        while (true)
                        {
                            // Determine valid cards: matching rank or wild, excluding any Threes
                            var validIndices = pile
                                .Select((c, i) => new { c, i })
                                .Where(x => x.c.Rank == meldRank || x.c.IsWild)
                                .Where(x => x.c.Rank != Rank.Three)
                                .Select(x => x.i)
                                .ToList();

                            if (!validIndices.Any())
                            {
                                Console.WriteLine("No more valid cards to add.");
                                break;
                            }

                            Console.WriteLine("Select a card to add to meld or 'd' when done:");
                            foreach (var idx in validIndices)
                                Console.WriteLine($"[{idx}] {pile[idx]}");

                            var input = Console.ReadLine();
                            if (input?.Trim().ToLower() == "d") break;

                            if (int.TryParse(input, out int idxSel) && validIndices.Contains(idxSel))
                            {
                                var card = pile[idxSel];
                                try
                                {
                                    player.PlayToMeld(card, meld);
                                    Console.WriteLine($"Added {card} to meld.");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Cannot add card: {ex.Message}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid selection; please choose a valid index or 'd'.");
                            }
                        }

                        if (!meld.IsValidInitial())
                            Console.WriteLine("Warning: Meld does not meet initial validity requirements.");

                        newMelds.Add(meld);
                        moreMelds = PromptYesNo("Start another meld? (y/n): ");
                    }
                }

                // Prompt for discard
                Console.WriteLine("Your remaining cards:");
                DisplayPile(pile);
                int discardIndex = PromptInt($"Select card to discard [0-{pile.Count - 1}]: ", 0, pile.Count - 1);
                var discardCard = pile[discardIndex];

                // Execute turn
                game.TakeTurn(currentPlayer, newMelds, discardCard, pickUp);

                // Advance to next player
                currentPlayer = (currentPlayer + 1) % game.Players.Count;
            }

            Console.WriteLine("\n*** Round Over! ***");
            int netPoints = game.EndRound();
            Console.WriteLine($"Net points this round: {netPoints}");
        }

        static void DisplayPile(List<Card> pile)
        {
            for (int i = 0; i < pile.Count; i++)
                Console.WriteLine($"[{i}] {pile[i]}");
        }

        static int PromptInt(string prompt, int min, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int val) && val >= min && val <= max)
                    return val;
                Console.WriteLine($"Please enter a number between {min} and {max}.");
            }
        }

        static bool PromptYesNo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var key = Console.ReadKey(intercept: true);
                Console.WriteLine();
                if (key.KeyChar == 'y' || key.KeyChar == 'Y') return true;
                if (key.KeyChar == 'n' || key.KeyChar == 'N') return false;
            }
        }

        static Rank PromptRank(string prompt, bool allowThree)
        {
            var ranks = Enum.GetValues<Rank>()
                .Cast<Rank>()
                .Where(r => allowThree || r != Rank.Three)
                .ToList();
            for (int i = 0; i < ranks.Count; i++)
                Console.WriteLine($"[{i}] {ranks[i]}");
            int choice = PromptInt(prompt, 0, ranks.Count - 1);
            return ranks[choice];
        }
    }
}
