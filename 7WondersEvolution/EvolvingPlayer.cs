﻿using _7Wonders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _7WondersEvolution
{
    internal class EvolvingPlayer
    {
        public string Name => string.Join(".", Weights.Select(i => i.ToString("x2")));

        public int Generation { get; }

        public int Games => victoryPointsPerGame.Count;

        public double AverageVictoryPoints => Games == 0 ? 0 : victoryPointsPerGame.Average();

        public double AveragePosition => Games == 0 ? 7 : positionPerGame.Average();

        public int[] Weights { get; }

        public EvolvingPlayer()
        {
            Weights = CreateRandomWeights(RobotPlayer.WeightsRequired).ToArray();
            Generation = 1;
        }

        public EvolvingPlayer(EvolvingPlayer player)
        {
            Weights = (int[])player.Weights.Clone();
            Generation = player.Generation;
        }

        public EvolvingPlayer(EvolvingPlayer parentA, EvolvingPlayer parentB)
        {
            Weights = parentA.Weights.Zip(parentB.Weights, CreateWeightFromParents)
                                     .ToArray();
            Generation = Math.Max(parentA.Generation, parentB.Generation) + 1;
        }

        public void AddGame(int position, int victoryPoints)
        {
            victoryPointsPerGame.Add(victoryPoints);
            positionPerGame.Add(position);
        }

        private static IEnumerable<int> CreateRandomWeights(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                yield return ThreadSafeRandom.ThisThreadsRandom.Next(0, 255);
            }
        }

        private static int CreateWeightFromParents(int weightA, int weightB)
        {
            // Each weight comes randomly from parent A (40%), parent B (40%) or an average of the two (20%).
            int r = ThreadSafeRandom.ThisThreadsRandom.Next(5);
            int weight = (r < 2) ? weightA
                       : (r < 4) ? weightB
                                 : (weightA + weightB) / 2;
            weight += ThreadSafeRandom.ThisThreadsRandom.Next(-5, 5);    // mutation
            return Math.Min(255, Math.Max(0, weight));
        }

        private List<int> victoryPointsPerGame = new List<int>();
        private List<int> positionPerGame = new List<int>();  // 1-7
    }
}