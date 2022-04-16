using System;
using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    public class PlayerState
    {
        public string CityName => tableau.CityName;

        public int Coins { get; private set; } = 3;

        public int Military => tableau.Military;

        public int MilitaryDefeats { get; private set; } = 0;

        public int WonderStagesBuilt => tableau.WonderStagesBuilt;

        public PlayerState(Tableau tableau)
        {
            this.tableau = tableau;
        }

        public PlayerState(PlayerState source)
        {
            Coins = source.Coins;
            MilitaryDefeats = source.MilitaryDefeats;
            militaryVictoryPoints = source.militaryVictoryPoints;
            tableau = new Tableau(source.tableau);
        }

        public int CalculateVictoryPoints(PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            int treasuryVictoryPoints = Coins / 3;
            int wonderVictoryPoints = tableau.CalculateWonderVictoryPoints();
            int civilianVictoryPoints = tableau.CalculateCivilianVictoryPoints(this, leftNeighbour, rightNeighbour);
            int scienceVictoryPoints = tableau.CalculateScienceVictoryPoints();
            int commercialVictoryPoints = tableau.CalculateCommercialVictoryPoints(this, leftNeighbour, rightNeighbour);
            int guildVictoryPoints = tableau.CalculateGuildVictoryPoints(this, leftNeighbour, rightNeighbour);

            return militaryVictoryPoints + treasuryVictoryPoints + wonderVictoryPoints + civilianVictoryPoints + scienceVictoryPoints + commercialVictoryPoints + guildVictoryPoints;
        }

        public void AddCardToTableau(Card card)
        {
            tableau.Add(card);
        }

        public void BuildNextWonderStage()
        {
            tableau.BuildNextWonderStage();
        }

        public int CountColour(Colour colour)
        {
            return tableau.CountColour(colour);
        }

        public void AddCoins(int coins)
        {
            Coins += coins;
        }

        public void PayCoins(int coins)
        {
            if (Coins < coins)
            {
                throw new InvalidOperationException($"Can't pay {coins} {TextHelper.Pluralize("coin", coins)} because coins available is only {Coins}");
            }
            Coins -= coins;
        }

        public void AwardMilitaryVictoryPoints(int militaryVictoryPoints)
        {
            this.militaryVictoryPoints += militaryVictoryPoints;
            if (militaryVictoryPoints < 0)
            {
                ++MilitaryDefeats;
            }
        }

        public void WriteStateToConsole(IEnumerable<Card> hand, PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            // Score
            int treasuryVictoryPoints = Coins / 3;
            int wonderVictoryPoints = tableau.CalculateWonderVictoryPoints();
            int civilianVictoryPoints = tableau.CalculateCivilianVictoryPoints(this, leftNeighbour, rightNeighbour);
            int scienceVictoryPoints = tableau.CalculateScienceVictoryPoints();
            int commercialVictoryPoints = tableau.CalculateCommercialVictoryPoints(this, leftNeighbour, rightNeighbour);
            int guildVictoryPoints = tableau.CalculateGuildVictoryPoints(this, leftNeighbour, rightNeighbour);

            ConsoleHelper.ClearConsoleColours();
            Console.Write($"Score: {CalculateVictoryPoints(leftNeighbour, rightNeighbour)} (");
            ConsoleHelper.SetConsoleColours(Colour.Red);
            Console.Write(militaryVictoryPoints);
            ConsoleHelper.ClearConsoleColours();
            Console.Write($" + {treasuryVictoryPoints} + {wonderVictoryPoints} + ");
            ConsoleHelper.SetConsoleColours(Colour.Blue);
            Console.Write(civilianVictoryPoints);
            ConsoleHelper.ClearConsoleColours();
            Console.Write(" + ");
            ConsoleHelper.SetConsoleColours(Colour.Green);
            Console.Write(scienceVictoryPoints);
            ConsoleHelper.ClearConsoleColours();
            Console.Write(" + ");
            ConsoleHelper.SetConsoleColours(Colour.Yellow);
            Console.Write(commercialVictoryPoints);
            ConsoleHelper.ClearConsoleColours();
            Console.Write(" + ");
            ConsoleHelper.SetConsoleColours(Colour.Purple);
            Console.Write(guildVictoryPoints);
            ConsoleHelper.ClearConsoleColours();
            Console.WriteLine(")");

            // Military
            Console.WriteLine($"Military: {Military}");
            Console.WriteLine();

            // Coins
            Console.WriteLine($"{Coins} {TextHelper.Pluralize("coin", Coins)}");

            // Resources available
            Console.WriteLine(tableau.ResourceProductionToString());

            // Cards in tableau
            tableau.WriteToConsole();

            // Hand
            Console.WriteLine();
            Console.Write("Hand: ");
            ConsoleHelper.WriteCardsToConsole(hand);

            Console.WriteLine();
        }

        public int GetResourceCount(Resource resource)
        {
            int count = 1;
            while (tableau.HasResources(new ResourceCollection(resource, count)))
            {
                ++count;
            }
            return count - 1;
        }

        public IReadOnlyCollection<int> GetScienceCount()
        {
            return tableau.CalculateScience();
        }

        public IEnumerable<IAction> GetAllActions(IEnumerable<Card> hand, PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            var actions = new List<IAction>();

            // Builds
            foreach (var card in hand.Distinct())
            {
                if (!tableau.Has(card.Name))
                {
                    if (CanAfford(card.Cost))
                    {
                        actions.Add(new Build(card));
                    }
                    else
                    {   // May be able to build the card using trades. Create all possible trade options.
                        var requiredResources = new ResourceCollection(card.Cost.Resources);
                        var possibleTradeCosts = CalculatePossibleTradeCosts(requiredResources, leftNeighbour, rightNeighbour);
                        var tradeActions = possibleTradeCosts.Select(trade => new Build(card, trade.costToLeftNeighbour, trade.costToRightNeighbour));
                        actions.AddRange(tradeActions);
                    }
                }
            }

            // Wonder stage builds
            var wonderStage = tableau.NextWonderStage();
            if (wonderStage != null)
            {
                foreach (var card in hand.Distinct())
                {
                    if (CanAfford(wonderStage.Cost))
                    {
                        actions.Add(new BuildWonderStage(wonderStage, card));
                    }
                    else
                    {   // May be able to build the wonder stage using trades. Create all possible trade options.
                        var requiredResources = new ResourceCollection(wonderStage.Cost.Resources);
                        var possibleTradeCosts = CalculatePossibleTradeCosts(requiredResources, leftNeighbour, rightNeighbour);
                        var tradeActions = possibleTradeCosts.Select(trade => new BuildWonderStage(wonderStage, card, trade.costToLeftNeighbour, trade.costToRightNeighbour));
                        actions.AddRange(tradeActions);
                    }
                }
            }

            // Sells
            foreach (var card in hand.Distinct())
            {
                actions.Add(new Sell(card));
            }

            return actions;
        }

        private IEnumerable<(int costToLeftNeighbour, int costToRightNeighbour)> CalculatePossibleTradeCosts(ResourceCollection requiredResources, PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            var distributions = new DistributionHelper(requiredResources, tableau, leftNeighbour.tableau, rightNeighbour.tableau).ResourceDistributions;
            return distributions.Where(distribution => distribution.ResourcesFromLeftNeighbour.Any() || distribution.ResourcesFromRightNeighbour.Any()) // must include at least 1 trade
                                .Where(distribution => distribution.SatisfiedBy(tableau, leftNeighbour.tableau, rightNeighbour.tableau))                // resources must be available from the respective tableaus  
                                .Select(distribution => (CostForTrade(Direction.Left, distribution.ResourcesFromLeftNeighbour),
                                                         CostForTrade(Direction.Right, distribution.ResourcesFromRightNeighbour)))
                                .Where(costPair => costPair.Item1 + costPair.Item2 <= Coins)                                                            // must be able to afford the trades
                                .MinimalElements();                                                                                                     // only include the cheapest trades
        }

        private int CostForTrade(Direction direction, ResourceCollection resourcesToTrade)
        {
            return resourcesToTrade.Sum((resource, count) => count * tableau.TradeCost(resource, direction));
        }

        private bool CanAfford(Cost cost)
        {
            if (cost.FreeWithOtherCard != null && tableau.Has(cost.FreeWithOtherCard))
            {
                // Can build for free because of having the prerequisite card.
                return true;
            }
            if (Coins < cost.Coins)
            {
                // Not enough coins to pay the coin cost.
                return false;
            }
            // Must have the resources for the building. (Not counting for trades yet.)
            return tableau.HasResources(cost.Resources);
        }

        private int militaryVictoryPoints = 0;

        private Tableau tableau;
    }
}
