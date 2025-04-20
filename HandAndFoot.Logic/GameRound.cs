using System.Collections.Generic;

namespace HandAndFoot.Logic
{
    public static class GameRound
    {
        public static readonly Dictionary<int, int> MinPoints = new()
        {
            [1] = 50,
            [2] = 90,
            [3] = 120,
            [4] = 150
        };

        public static bool MeetsMinimum(int roundNumber, int meldPoints)
        {
            if (!MinPoints.ContainsKey(roundNumber))
                throw new KeyNotFoundException($"Invalid round: {roundNumber}");
            return meldPoints >= MinPoints[roundNumber];
        }
    }
}