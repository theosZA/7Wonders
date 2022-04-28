using Extensions;
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

        public int MilitaryVictoryPoints { get; private set; } = 0;

        public int TreasuryVictoryPoints => Coins / 3;

        public int ScienceVictoryPoints => tableau.CalculateScienceVictoryPoints();

        public int FreeBuildsPerAge => tableau.FreeBuildsPerAge;

        public int FreeBuildsLeft => FreeBuildsPerAge - freeBuildsMadeThisAge;

        public PlayerState(Tableau tableau)
        {
            this.tableau = tableau;
        }

        public PlayerState(PlayerState source)
        {
            Coins = source.Coins;
            MilitaryDefeats = source.MilitaryDefeats;
            MilitaryVictoryPoints = source.MilitaryVictoryPoints;
            freeBuildsMadeThisAge = source.freeBuildsMadeThisAge;

            tableau = new Tableau(source.tableau);
        }

        public void StartAge(int newAge)
        {
            freeBuildsMadeThisAge = 0;
        }

        public int CalculateVictoryPoints(PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            return MilitaryVictoryPoints
                 + TreasuryVictoryPoints
                 + CalculateWonderVictoryPoints(leftNeighbour, rightNeighbour)
                 + CalculateCivilianVictoryPoints(leftNeighbour, rightNeighbour)
                 + ScienceVictoryPoints
                 + CalculateCommercialVictoryPoints(leftNeighbour, rightNeighbour)
                 + CalculateGuildVictoryPoints(leftNeighbour, rightNeighbour);
        }

        public int CalculateWonderVictoryPoints(PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            return tableau.CalculateWonderVictoryPoints(this, leftNeighbour, rightNeighbour);
        }

        public int CalculateCivilianVictoryPoints(PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            return tableau.CalculateCivilianVictoryPoints(this, leftNeighbour, rightNeighbour);
        }

        public int CalculateCommercialVictoryPoints(PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            return tableau.CalculateCommercialVictoryPoints(this, leftNeighbour, rightNeighbour);
        }

        public int CalculateGuildVictoryPoints(PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            return tableau.CalculateGuildVictoryPoints(this, leftNeighbour, rightNeighbour);
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
                throw new InvalidOperationException($"Can't pay {coins} {(coins == 1 ? "coin" : "coins")} because player only has {Coins} {(Coins == 1 ? "coin" : "coins")}");
            }
            Coins -= coins;
        }

        public void UseFreeBuild()
        {
            ++freeBuildsMadeThisAge;
        }

        public void AwardMilitaryVictoryPoints(int militaryVictoryPoints)
        {
            this.MilitaryVictoryPoints += militaryVictoryPoints;
            if (militaryVictoryPoints < 0)
            {
                ++MilitaryDefeats;
            }
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

        public IReadOnlyCollection<int> GetScienceCountsWithWildsAllocated()
        {
            return tableau.GetScienceCountsWithWildsAllocated();
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
                        actions.Add(new Build
                                    {
                                        Card = card
                                    });
                        
                        // If the card has a coin cost, may be able to ignore this cost with an earned ability.
                        if (card.Cost.Coins > 0 && FreeBuildsLeft > 0)
                        {
                            actions.Add(new Build
                            {
                                Card = card,
                                UsesFreeBuild = true
                            });
                        }
                    }
                    else
                    {   // May be able to build the card using trades. Create all possible trade options.
                        var requiredResources = new ResourceCollection(card.Cost.Resources);
                        var possibleTradeCosts = CalculatePossibleTradeCosts(requiredResources, leftNeighbour, rightNeighbour);
                        var tradeActions = possibleTradeCosts.Select(trade => new Build
                                                                                  {
                                                                                      Card = card,
                                                                                      CoinsToLeftNeighbour = trade.costToLeftNeighbour,
                                                                                      CoinsToRightNeighbour = trade.costToRightNeighbour
                                                                                  });
                        actions.AddRange(tradeActions);

                        // May be able to bypass trades by building the card for free with an earned ability.
                        if (FreeBuildsLeft > 0)
                        {
                            actions.Add(new Build
                                            {
                                                Card = card,
                                                UsesFreeBuild = true
                                            });
                        }
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

        private Tableau tableau;

        private int freeBuildsMadeThisAge = 0;
    }
}
