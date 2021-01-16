using System;
using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    public abstract class Player
    {
        protected abstract IAction GetAction(IEnumerable<IAction> possibleActions);

        public string Name { get; }

        public int VictoryPoints => CalculateVictoryPoints();

        public int Coins { get; private set; } = 3;

        public int CardsInHand => hand?.Count ?? 0;

        public int Military => tableau.Military;

        public int MilitaryDefeats { get; private set; } = 0;

        public int WonderStagesBuilt => tableau.WonderStagesBuilt;

        public Player(string name, Tableau tableau)
        {
            Name = name;
            this.tableau = tableau;
        }

        public void SetNeighbours(Player leftNeighbour, Player rightNeighbour)
        {
            this.leftNeighbour = leftNeighbour;
            this.rightNeighbour = rightNeighbour;
        }

        public IAction GetAction()
        {
            return GetAction(GetAllActions());
        }

        public IList<Card> SwapHand(IList<Card> newHand)
        {
            var oldHand = hand;
            hand = newHand;
            return oldHand;
        }

        public void RemoveCardFromHand(Card card)
        {
            if (!hand.Remove(card))
            {
                throw new InvalidOperationException($"Can't play card {card.Name} because it's not in hand");
            }
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

        public void WriteStateToConsole()
        {
            ConsoleHelper.ClearConsoleColours();
            Console.WriteLine(Name);

            // Score
            int treasuryVictoryPoints = Coins / 3;
            int wonderVictoryPoints = tableau.CalculateWonderVictoryPoints();
            int civilianVictoryPoints = tableau.CalculateCivilianVictoryPoints(this, leftNeighbour, rightNeighbour);
            int scienceVictoryPoints = tableau.CalculateScienceVictoryPoints();
            int commercialVictoryPoints = tableau.CalculateCommercialVictoryPoints(this, leftNeighbour, rightNeighbour);
            int guildVictoryPoints = tableau.CalculateGuildVictoryPoints(this, leftNeighbour, rightNeighbour);

            Console.Write($"Score: {VictoryPoints} (");
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

        private IEnumerable<IAction> GetAllActions()
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
                        var resourcesToConsider = new Stack<Resource>(requiredResources.GetResources());
                        var possibleTradeActions = CreateBuildActionsWithTrade(card, requiredResources, new ResourceCollection(), new ResourceCollection(), resourcesToConsider);
                        var tradeActionsToAdd = new List<Build>();
                        foreach (var possibleTradeAction in possibleTradeActions)
                        {
                            // Ensure that there isn't an equal or better trade action already set to be added.
                            if (!tradeActionsToAdd.Any(existingTradeAction => possibleTradeAction.IsWorseOrEqualThan(existingTradeAction)))
                            {
                                tradeActionsToAdd.Add(possibleTradeAction);
                            }
                        }
                        actions.AddRange(tradeActionsToAdd);
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
                        var resourcesToConsider = new Stack<Resource>(requiredResources.GetResources());
                        var possibleTradeActions = CreateBuildWonderStageActionsWithTrade(wonderStage, card, requiredResources, new ResourceCollection(), new ResourceCollection(), resourcesToConsider);
                        var tradeActionsToAdd = new List<BuildWonderStage>();
                        foreach (var possibleTradeAction in possibleTradeActions)
                        {
                            // Ensure that there isn't an equal or better trade action already set to be added.
                            if (!tradeActionsToAdd.Any(existingTradeAction => possibleTradeAction.IsWorseOrEqualThan(existingTradeAction)))
                            {
                                tradeActionsToAdd.Add(possibleTradeAction);
                            }
                        }
                        actions.AddRange(tradeActionsToAdd);
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

        private IEnumerable<Build> CreateBuildActionsWithTrade(Card card, ResourceCollection requiredResources, ResourceCollection resourcesFromLeftNeighbour, ResourceCollection resourcesFromRightNeighbour, Stack<Resource> resourcesToConsider)
        {
            if (resourcesToConsider.Count == 0)
            {
                if (!resourcesFromLeftNeighbour.Any() && !resourcesFromRightNeighbour.Any())
                {
                    return Enumerable.Empty<Build>();
                }
                if (tableau.HasResources(requiredResources) && leftNeighbour.tableau.HasResourcesForTrade(resourcesFromLeftNeighbour) && rightNeighbour.tableau.HasResourcesForTrade(resourcesFromRightNeighbour))
                {
                    int coinsToLeftNeighbour = CostForTrade(Direction.Left, resourcesFromLeftNeighbour);
                    int coinsToRightNeighbour = CostForTrade(Direction.Right, resourcesFromRightNeighbour);
                    if (coinsToLeftNeighbour + coinsToRightNeighbour <= Coins)
                    {
                        return new[] { new Build(card, coinsToLeftNeighbour, coinsToRightNeighbour) };
                    }
                }
                return Enumerable.Empty<Build>();
            }
            else
            {
                var newActions = new List<Build>();
                var currentResource = resourcesToConsider.Pop();
                int resourceCount = requiredResources.GetResourceCount(currentResource);
                for (int leftNeighbourContribution = 0; leftNeighbourContribution <= resourceCount; ++leftNeighbourContribution)
                {
                    for (int rightNeighbourContribution = 0; rightNeighbourContribution <= resourceCount - leftNeighbourContribution; ++rightNeighbourContribution)
                    {
                        requiredResources.Remove(currentResource, leftNeighbourContribution + rightNeighbourContribution);
                        resourcesFromLeftNeighbour.Add(currentResource, leftNeighbourContribution);
                        resourcesFromRightNeighbour.Add(currentResource, rightNeighbourContribution);
                        newActions.AddRange(CreateBuildActionsWithTrade(card, requiredResources, resourcesFromLeftNeighbour, resourcesFromRightNeighbour, resourcesToConsider));
                        requiredResources.Add(currentResource, leftNeighbourContribution + rightNeighbourContribution);
                        resourcesFromLeftNeighbour.Remove(currentResource, leftNeighbourContribution);
                        resourcesFromRightNeighbour.Remove(currentResource, rightNeighbourContribution);
                    }
                }
                resourcesToConsider.Push(currentResource);
                return newActions;
            }
        }

        private IEnumerable<BuildWonderStage> CreateBuildWonderStageActionsWithTrade(WonderStage wonderStage, Card cardToDiscard, ResourceCollection requiredResources, ResourceCollection resourcesFromLeftNeighbour, ResourceCollection resourcesFromRightNeighbour, Stack<Resource> resourcesToConsider)
        {
            if (resourcesToConsider.Count == 0)
            {
                if (!resourcesFromLeftNeighbour.Any() && !resourcesFromRightNeighbour.Any())
                {
                    return Enumerable.Empty<BuildWonderStage>();
                }
                if (tableau.HasResources(requiredResources) && leftNeighbour.tableau.HasResourcesForTrade(resourcesFromLeftNeighbour) && rightNeighbour.tableau.HasResourcesForTrade(resourcesFromRightNeighbour))
                {
                    int coinsToLeftNeighbour = CostForTrade(Direction.Left, resourcesFromLeftNeighbour);
                    int coinsToRightNeighbour = CostForTrade(Direction.Right, resourcesFromRightNeighbour);
                    if (coinsToLeftNeighbour + coinsToRightNeighbour <= Coins)
                    {
                        return new[] { new BuildWonderStage(wonderStage, cardToDiscard, coinsToLeftNeighbour, coinsToRightNeighbour) };
                    }
                }
                return Enumerable.Empty<BuildWonderStage>();
            }
            else
            {
                var newActions = new List<BuildWonderStage>();
                var currentResource = resourcesToConsider.Pop();
                int resourceCount = requiredResources.GetResourceCount(currentResource);
                for (int leftNeighbourContribution = 0; leftNeighbourContribution <= resourceCount; ++leftNeighbourContribution)
                {
                    for (int rightNeighbourContribution = 0; rightNeighbourContribution <= resourceCount - leftNeighbourContribution; ++rightNeighbourContribution)
                    {
                        requiredResources.Remove(currentResource, leftNeighbourContribution + rightNeighbourContribution);
                        resourcesFromLeftNeighbour.Add(currentResource, leftNeighbourContribution);
                        resourcesFromRightNeighbour.Add(currentResource, rightNeighbourContribution);
                        newActions.AddRange(CreateBuildWonderStageActionsWithTrade(wonderStage, cardToDiscard, requiredResources, resourcesFromLeftNeighbour, resourcesFromRightNeighbour, resourcesToConsider));
                        requiredResources.Add(currentResource, leftNeighbourContribution + rightNeighbourContribution);
                        resourcesFromLeftNeighbour.Remove(currentResource, leftNeighbourContribution);
                        resourcesFromRightNeighbour.Remove(currentResource, rightNeighbourContribution);
                    }
                }
                resourcesToConsider.Push(currentResource);
                return newActions;
            }
        }

        private int CostForTrade(Direction direction, ResourceCollection resourcesToTrade)
        {
            return resourcesToTrade.Sum((resource, count) => count * tableau.TradeCost(resource, direction));
        }

        private int CalculateVictoryPoints()
        {
            int treasuryVictoryPoints = Coins / 3;
            int wonderVictoryPoints = tableau.CalculateWonderVictoryPoints();
            int civilianVictoryPoints = tableau.CalculateCivilianVictoryPoints(this, leftNeighbour, rightNeighbour);
            int scienceVictoryPoints = tableau.CalculateScienceVictoryPoints();
            int commercialVictoryPoints = tableau.CalculateCommercialVictoryPoints(this, leftNeighbour, rightNeighbour);
            int guildVictoryPoints = tableau.CalculateGuildVictoryPoints(this, leftNeighbour, rightNeighbour);

            return militaryVictoryPoints + treasuryVictoryPoints + wonderVictoryPoints + civilianVictoryPoints + scienceVictoryPoints + commercialVictoryPoints + guildVictoryPoints;
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
        private IList<Card> hand;

        private Player leftNeighbour;
        private Player rightNeighbour;
    }
}
