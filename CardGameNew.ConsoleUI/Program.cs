using System;
using System.Linq;
using System.Collections.Generic;
using CardGameNew.Logic;

namespace CardGameNew.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Hand and Foot!");
            var playerNames = PromptForPlayerNames();
            var game = DeckBuilder.SetupGame(playerNames);
            var ui = new GameConsoleUI(game);
            ui.Run();
        }

        private static string[] PromptForPlayerNames()
        {
            Console.Write("Enter number of players: ");
            if (!int.TryParse(Console.ReadLine(), out var count) || count < 1)
            {
                Console.WriteLine("Please enter a valid number of players.");
                return PromptForPlayerNames();
            }
            var names = new string[count];
            for (int i = 0; i < count; i++)
            {
                Console.Write($"Enter name for player {i + 1}: ");
                names[i] = Console.ReadLine()?.Trim() ?? $"Player{i + 1}";
            }
            return names;
        }
    }

    public class GameConsoleUI
    {
        private readonly Game _game;
        private readonly bool[] _hasTakenInitialDraw;

        public GameConsoleUI(Game game)
        {
            _game = game;
            _hasTakenInitialDraw = new bool[_game.Players.Count];
        }

        public void Run()
        {
            while (true)
            {
                var player = _game.CurrentPlayer;
                Console.Clear();
                Console.WriteLine($"It's {player.Name}'s turn.");

                DrawPhase(player);
                ActionPhase(player);

                // End game when current player has emptied hand and foot
                if (player.Hand.Cards.Count == 0 && player.Foot.Cards.Count == 0)
                    break;

                _game.NextTurn();
            }
            ShowFinalScores();
        }

        private void DrawPhase(Player player)
        {
            // Move to foot if hand empty
            if (player.Hand.Cards.Count == 0 && player.Foot.Cards.Count > 0)
            {
                Console.WriteLine("Your hand is empty—picking up your foot.");
                while (player.Foot.Cards.Count > 0)
                    player.Hand.Add(player.Foot.Draw());
                Pause();
            }

            int idx = _game.CurrentPlayerIndex;
            if (!_hasTakenInitialDraw[idx])
            {
                _hasTakenInitialDraw[idx] = true;
                Pause();
                return;
            }

            Console.WriteLine("Drawing 2 cards...");
            for (int i = 0; i < 2; i++)
            {
                var card = _game.DrawPile.Draw();
                player.Hand.Add(card);
                Console.WriteLine($"Drew: {FormatCard(card)}");
            }
            Console.WriteLine();
            DisplayHand(player);
        }

        private void ActionPhase(Player player)
        {
            // Move to foot if hand empty
            if (player.Hand.Cards.Count == 0 && player.Foot.Cards.Count > 0)
            {
                Console.WriteLine("Your hand is empty—picking up your foot.");
                while (player.Foot.Cards.Count > 0)
                    player.Hand.Add(player.Foot.Draw());
                Pause();
            }

            bool turnOver = false;
            while (!turnOver)
            {
                Console.Clear();
                Console.WriteLine($"It's {player.Name}'s turn.");
                DisplayHand(player);
                DisplayPlayerStatus(player);
                DisplayMelds(player);

                Console.WriteLine("Choose an action:");
                Console.WriteLine("1) Create meld");
                Console.WriteLine("2) Add to meld");
                Console.WriteLine("3) Discard");
                Console.WriteLine("4) View melds");
                Console.Write("Selection: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": CreateMeld(player); break;
                    case "2": AddToMeld(player); break;
                    case "3":
                        turnOver = Discard(player);
                        break;
                    case "4":
                        DisplayMelds(player);
                        Pause();
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        Pause();
                        break;
                }
            }
        }

        private void DisplayPlayerStatus(Player player)
        {
            Console.WriteLine($"Status — Hand: {player.Hand.Cards.Count} | Foot: {player.Foot.Cards.Count} | Melds: {player.Melds.Count}");
            Console.WriteLine();
        }

        private void DisplayHand(Player player)
        {
            Console.WriteLine($"Player: {player.Name}");
            Console.WriteLine("Your hand:");
            var sorted = player.Hand.Cards.OrderBy(c => c.Rank).ThenBy(c => !c.IsWild).ToList();
            for (int i = 0; i < sorted.Count; i++)
                Console.WriteLine($"{i + 1}) {FormatCard(sorted[i])}");
            Console.WriteLine();
        }

        private void DisplayMelds(Player player)
        {
            Console.WriteLine("Your melds:");
            for (int i = 0; i < player.Melds.Count; i++)
            {
                var meld = player.Melds[i];
                var status = meld.IsClean ? "Clean" : "Dirty";
                Console.WriteLine($"{i + 1}) {status} meld of {meld.Rank} — {meld.Cards.Count}/7 cards");
            }
            Console.WriteLine();
        }

        private void ShowFinalScores()
        {
            Console.Clear();
            Console.WriteLine("Game over! Final scores:");
            foreach (var player in _game.Players)
            {
                var score = _game.CalculateScore(player);
                Console.WriteLine($"{player.Name}: {score}");
            }
            Console.WriteLine();
            Pause();
        }

        private void Pause()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private string FormatCard(Card card)
        {
            var rank = card.Rank.ToString();
            return card.IsWild ? $"{rank} (Wild)" : rank;
        }

        private void CreateMeld(Player player)
        {
            Console.Clear();
            Console.WriteLine("Select rank for new meld:");
            var ranks = Enum.GetValues(typeof(Rank)).Cast<Rank>().Where(r => r != Rank.Two && r != Rank.Joker).ToList();
            for (int i = 0; i < ranks.Count; i++) Console.WriteLine($"{i + 1}) {ranks[i]}");
            Console.Write("Selection: ");
            if (!int.TryParse(Console.ReadLine(), out var choice) || choice < 1 || choice > ranks.Count)
            {
                Console.WriteLine("Invalid rank choice."); Pause(); return;
            }

            var selectedRank = ranks[choice - 1];
            var valid = player.Hand.Cards.Where(c => c.Rank == selectedRank || c.IsWild).ToList();
            if (valid.Count < 3)
            {
                Console.WriteLine($"Not enough cards ({valid.Count}).");
                Pause();
                return;
            }

            Console.WriteLine("Valid cards:");
            for (int i = 0; i < valid.Count; i++)
                Console.WriteLine($"{i + 1}) {FormatCard(valid[i])}");

            Console.WriteLine("Enter indices (3-7), separated by spaces:");
            var inds = Console.ReadLine()
                           .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                           .Select(s => int.TryParse(s, out var n) ? n - 1 : -1)
                           .Where(n => n >= 0 && n < valid.Count)
                           .Distinct()
                           .ToList();
            if (inds.Count < 3 || inds.Count > 7)
            {
                Console.WriteLine("Must select 3-7.");
                Pause();
                return;
            }

            var sel = inds.Select(n => valid[n]).ToList();
            var clean = sel.All(c => !c.IsWild);
            try
            {
                player.CreateMeld(selectedRank, clean, sel);
                Console.WriteLine("Meld created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Pause();
        }

                private void AddToMeld(Player player)
        {
            Console.Clear();
            if (!player.Melds.Any())
            {
                Console.WriteLine("You have no melds to add to.");
                Pause();
                return;
            }

            // Select meld
            Console.WriteLine("Select a meld to add cards to:");
            for (int i = 0; i < player.Melds.Count; i++)
            {
                var meld = player.Melds[i];
                var status = meld.IsClean ? "Clean" : "Dirty";
                Console.WriteLine($"{i + 1}) {status} meld of {meld.Rank} — {meld.Cards.Count}/7 cards");
            }
            Console.Write("Selection: ");
            if (!int.TryParse(Console.ReadLine(), out var meldIndex) || meldIndex < 1 || meldIndex > player.Melds.Count)
            {
                Console.WriteLine("Invalid meld choice.");
                Pause();
                return;
            }
            var selectedMeld = player.Melds[meldIndex - 1];

            // Determine valid cards to add
            var maxAdds = 7 - selectedMeld.Cards.Count;
            if (maxAdds <= 0)
            {
                Console.WriteLine("This meld is already closed (7 cards).");
                Pause();
                return;
            }

            var validCards = player.Hand.Cards
                .Where(c => selectedMeld.CanAdd(c) || (selectedMeld.IsClean && c.IsWild))
                .OrderBy(c => c.Rank)
                .ThenBy(c => !c.IsWild)
                .ToList();
            if (!validCards.Any())
            {
                Console.WriteLine("No valid cards in hand to add to this meld.");
                Pause();
                return;
            }

            // List valid cards
            Console.WriteLine($"Valid cards (max {maxAdds}); enter indices separated by spaces:");
            for (int i = 0; i < validCards.Count; i++)
                Console.WriteLine($"{i + 1}) {FormatCard(validCards[i])}");
            Console.Write("Selection: ");
            var input = Console.ReadLine() ?? string.Empty;
            var selections = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out var n) ? n - 1 : -1)
                .Where(n => n >= 0 && n < validCards.Count)
                .Distinct()
                .Take(maxAdds)
                .ToList();

            if (selections.Count == 0)
            {
                Console.WriteLine("No cards selected.");
                Pause();
                return;
            }

            // Add cards to meld
            try
            {
                // If adding any wild to a clean meld, convert to dirty first
                if (selectedMeld.IsClean && selections.Any(i => validCards[i].IsWild))
                {
                    var newDirty = new DirtyMeld(selectedMeld.Rank);
                    foreach (var c in selectedMeld.Cards)
                        newDirty.Add(c);
                    player.Melds.Remove(selectedMeld);
                    selectedMeld = newDirty;
                    player.Melds.Add(newDirty);
                }

                foreach (var idx in selections)
                {
                    var card = validCards[idx];
                    if (!selectedMeld.CanAdd(card) && !card.IsWild)
                        throw new InvalidOperationException($"Cannot add {FormatCard(card)} to the meld.");
                    player.Hand.Remove(card);
                    selectedMeld.Add(card);
                }

                Console.WriteLine($"Added {selections.Count} card(s) to the meld.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add cards: {ex.Message}");
            }

            Pause();
        }

        private bool Discard(Player player)
        {
            Console.Clear();
            var sorted = player.Hand.Cards.OrderBy(c => c.Rank).ThenBy(c => !c.IsWild).ToList();
            if (!sorted.Any())
            {
                Console.WriteLine("No cards.");
                Pause();
                return false;
            }
            Console.WriteLine("Select card to discard:");
            for (int i = 0; i < sorted.Count; i++)
                Console.WriteLine($"{i + 1}) {FormatCard(sorted[i])}");
            Console.Write("Selection: ");
            if (!int.TryParse(Console.ReadLine(), out var ci) || ci < 1 || ci > sorted.Count)
            {
                Console.WriteLine("Invalid.");
                Pause();
                return false;
            }
            var card = sorted[ci - 1];
            player.Discard(card);
            Console.WriteLine($"Discarded {FormatCard(card)}");
            Console.WriteLine();
            Pause();
            return true;
        }
    }
}
