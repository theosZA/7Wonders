using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// A player's wonder board (i.e. city + wonder) and all the cards that he's built.
    /// </summary>
    public class Tableau
    {
        public string CityName { get; private set; }

        public int Military => builtElements.Sum(element => element.Military);

        public int FreeBuildsPerAge => builtElements.Sum(element => element.FreeBuildsPerAge);

        public int WonderStagesBuilt => builtElements.Count(element => element is WonderStage);

        public Tableau(XmlElement cityElement)
        {
            CityName = cityElement.GetAttribute("name");
            cityProduction = new Production(cityElement.GetChildElements("Production"));
            AddProduction(cityProduction, availableForTrade: true);
            var wonderElement = cityElement.GetChildElement("Wonder");
            var wonderName = wonderElement.GetAttribute_String("name");
            availableWonderStages = wonderElement.GetChildElements("Stage").Select((stageElement, i) => new WonderStage($"{wonderName} {i + 1}", stageElement)).ToList();
            builtElements = new List<TableauElement>();
        }

        public Tableau(Tableau tableau)
        {
            CityName = tableau.CityName;
            cityProduction = new Production(tableau.cityProduction);
            availableWonderStages = new List<WonderStage>(tableau.availableWonderStages);
            builtElements = new List<TableauElement>(tableau.builtElements);
            resourceProductionOptions = new ResourceOptions(tableau.resourceProductionOptions);
            resourceTradeOptions = new ResourceOptions(tableau.resourceTradeOptions);
        }

        public void Add(TableauElement element)
        {
            bool availableForTrade = element is Card card && (card.Colour == Colour.Brown || card.Colour == Colour.Gray);
            builtElements.Add(element);
            AddProduction(element.Production, availableForTrade);
        }

        public bool Has(string elementName)
        {
            return builtElements.Any(element => element.Name == elementName);
        }

        public WonderStage NextWonderStage()
        {
            return availableWonderStages.FirstOrDefault();
        }

        public void BuildNextWonderStage()
        {
            Add(availableWonderStages.First());
            availableWonderStages.RemoveAt(0);
        }

        public int CountColour(Colour colour)
        {
            return builtElements.Count(element => element is Card card && card.Colour == colour);
        }

        public int CountScienceSymbol(ScienceSymbol scienceSymbol)
        {
            return builtElements.Count(card => card.Science == scienceSymbol);
        }

        public IReadOnlyCollection<int> GetScienceCountsWithWildsAllocated()
        {
            var basicSymbolCounts = ScienceSymbolHelper.GetAllBasicScienceSymbols().Select(scienceSymbol => CountScienceSymbol(scienceSymbol))
                                                                                   .ToList();
            return AllocateScienceWilds(basicSymbolCounts, CountScienceSymbol(ScienceSymbol.Wild));
        }

        public bool HasResources(ResourceCollection resources)
        {
            return resourceProductionOptions.HasResources(resources);
        }

        public bool HasResourcesForTrade(ResourceCollection resources)
        {
            return resourceTradeOptions.HasResources(resources);
        }

        public int TradeCost(Resource resource, Direction direction)
        {
            return builtElements.Select(element => element.TradeBonus == null ? 2 : element.TradeBonus.TradeCost(resource, direction))
                                .DefaultIfEmpty(2)
                                .Min();
        }

        public int CalculateWonderVictoryPoints(PlayerState self, PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            return builtElements.Sum(element => element is WonderStage ? element.EvaluateVictoryPoints(self, leftNeighbour, rightNeighbour) : 0);
        }

        public int CalculateCivilianVictoryPoints(PlayerState self, PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            return CalculateVictoryPointsForColour(Colour.Blue, self, leftNeighbour, rightNeighbour);
        }

        public int CalculateCommercialVictoryPoints(PlayerState self, PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            return CalculateVictoryPointsForColour(Colour.Yellow, self, leftNeighbour, rightNeighbour);
        }

        public int CalculateGuildVictoryPoints(PlayerState self, PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            return CalculateVictoryPointsForColour(Colour.Purple, self, leftNeighbour, rightNeighbour);
        }

        public int CalculateScienceVictoryPoints()
        {
            return CalculateScienceVictoryPoints(GetScienceCountsWithWildsAllocated());
        }

        private int CalculateVictoryPointsForColour(Colour colour, PlayerState self, PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            return builtElements.Sum(element => (element is Card card && card.Colour == colour) ? card.EvaluateVictoryPoints(self, leftNeighbour, rightNeighbour) : 0);
        }

        private static int CalculateScienceVictoryPoints(IReadOnlyCollection<int> scienceSymbolCounts)
        {
            // 7 points for each complete set.
            int completeSets = scienceSymbolCounts.Min();
            int completeSetsScore = 7 * completeSets;

            // n^2 points for each symbol where n is the number of that symbol in the tableau.
            int squareSymbolsScore = scienceSymbolCounts.Sum(n => n * n);

            return completeSetsScore + squareSymbolsScore;
        }

        private void AddProduction(Production production, bool availableForTrade)
        {
            resourceProductionOptions.AddProduction(production);
            if (availableForTrade)
            {
                resourceTradeOptions.AddProduction(production);
            }
        }

        private static IReadOnlyCollection<int> AllocateScienceWilds(IReadOnlyCollection<int> symbolCounts, int wildCount)
        {
            if (wildCount == 0)
            {
                return symbolCounts;
            }

            var least = symbolCounts.Min();
            var most = symbolCounts.Max();
            var middle = symbolCounts.Sum() - (least + most);
            return AllocateScienceWildsOnOrderedCounts((most, middle, least), wildCount);
        }

        private static IReadOnlyCollection<int> AllocateScienceWildsOnOrderedCounts((int most, int middle, int least) symbolCounts, int wildCount)
        {
            switch (wildCount)
            {
                case 1:
                    switch (symbolCounts)
                    {

                        case var counts when counts == (0, 0, 0):   return new[] { 1, 0, 0 };
                        case var counts when counts == (1, 0, 0):   return new[] { 2, 0, 0 };
                        case var counts when counts == (1, 1, 0):   return new[] { 1, 1, 1 };
                        case var counts when counts == (1, 1, 1):   return new[] { 2, 1, 1 };
                        case var counts when counts == (2, 0, 0):   return new[] { 3, 0, 0 };
                        case var counts when counts == (2, 1, 0):   return new[] { 2, 1, 1 };
                        case var counts when counts == (2, 1, 1):   return new[] { 3, 1, 1 };
                        case var counts when counts == (2, 2, 0):   return new[] { 2, 2, 1 };
                        case var counts when counts == (2, 2, 1):   return new[] { 2, 2, 2 };
                        case var counts when counts == (2, 2, 2):   return new[] { 3, 2, 2 };
                        case var counts when counts == (3, 0, 0):   return new[] { 4, 0, 0 };
                        case var counts when counts == (3, 1, 0):   return new[] { 3, 1, 1 };
                        case var counts when counts == (3, 1, 1):   return new[] { 4, 1, 1 };
                        case var counts when counts == (3, 2, 0):   return new[] { 3, 2, 1 };
                        case var counts when counts == (3, 2, 1):   return new[] { 3, 2, 2 };
                        case var counts when counts == (3, 2, 2):   return new[] { 4, 2, 2 };
                        case var counts when counts == (3, 3, 0):   return new[] { 3, 3, 1 };
                        case var counts when counts == (3, 3, 1):   return new[] { 3, 3, 2 };
                        case var counts when counts == (3, 3, 2):   return new[] { 3, 3, 3 };
                        case var counts when counts == (3, 3, 3):   return new[] { 4, 3, 3 };
                        case var counts when counts == (4, 0, 0):   return new[] { 5, 0, 0 };
                        case var counts when counts == (4, 1, 0):   return new[] { 5, 1, 0 };   // 5,1,0 = 26 VP; 4,1,1 = 25 VP (all 4,1+,0 are better upping the 4 count)
                        case var counts when counts == (4, 1, 1):   return new[] { 5, 1, 1 };
                        case var counts when counts == (4, 2, 0):   return new[] { 5, 2, 0 };
                        case var counts when counts == (4, 2, 1):   return new[] { 4, 2, 2 };   // 5,2,1 = 37 VP; 4,2,2 = 38 VP (all 4,2+,1 are better upping the 1 count)
                        case var counts when counts == (4, 2, 2):   return new[] { 5, 2, 2 };
                        case var counts when counts == (4, 3, 0):   return new[] { 5, 3, 0 };
                        case var counts when counts == (4, 3, 1):   return new[] { 4, 3, 2 };
                        case var counts when counts == (4, 3, 2):   return new[] { 4, 3, 3 };
                        case var counts when counts == (4, 3, 3):   return new[] { 5, 3, 3 };
                        case var counts when counts == (4, 4, 0):   return new[] { 5, 4, 0 };
                        case var counts when counts == (4, 4, 1):   return new[] { 4, 4, 2 };
                        case var counts when counts == (4, 4, 2):   return new[] { 4, 4, 3 };
                        case var counts when counts == (4, 4, 3):   return new[] { 4, 4, 4 };
                        case var counts when counts == (4, 4, 4):   return new[] { 5, 4, 4 };

                        default:
                            throw new Exception($"Unexpected number of science symbols: ({symbolCounts.most}, {symbolCounts.middle}, {symbolCounts.least})");
                    }

                case 2:
                    switch (symbolCounts)
                    {

                        case var counts when counts == (0, 0, 0):   return new[] { 2, 0, 0 };
                        case var counts when counts == (1, 0, 0):   return new[] { 1, 1, 1 };
                        case var counts when counts == (1, 1, 0):   return new[] { 2, 1, 1 };
                        case var counts when counts == (1, 1, 1):   return new[] { 3, 1, 1 };
                        case var counts when counts == (2, 0, 0):   return new[] { 4, 0, 0 };   // 4,0,0 = 16 VP; 2,1,1 = 13 VP (all 2+,0,0 are better upping the high count)
                        case var counts when counts == (2, 1, 0):   return new[] { 3, 1, 1 };   // 4,1,0 = 17 VP; 3,1,1 = 18 VP
                        case var counts when counts == (2, 1, 1):   return new[] { 2, 2, 2 };   // 4,1,1 = 25 VP; 2,2,2 = 26 VP
                        case var counts when counts == (2, 2, 0):   return new[] { 2, 2, 2 };
                        case var counts when counts == (2, 2, 1):   return new[] { 3, 2, 2 };
                        case var counts when counts == (2, 2, 2):   return new[] { 4, 2, 2 };
                        case var counts when counts == (3, 0, 0):   return new[] { 5, 0, 0 };
                        case var counts when counts == (3, 1, 0):   return new[] { 5, 1, 0 };   // 5,1,0 = 26 VP; 4,1,1 = 25 VP (all 3+,1,0 are better upping the high count)
                        case var counts when counts == (3, 1, 1):   return new[] { 5, 1, 1 };   // 5,1,1 = 34 VP; 3,2,2 = 31 VP (all 3+,1,1 are better upping the high count)
                        case var counts when counts == (3, 2, 0):   return new[] { 3, 2, 2 };   // 5,2,0 = 29 VP; 3,2,2 = 31 VP
                        case var counts when counts == (3, 2, 1):   return new[] { 4, 2, 2 };   // 5,2,1 = 37 VP; 4,2,2 = 38 VP
                        case var counts when counts == (3, 2, 2):   return new[] { 3, 3, 3 };   // 5,2,2 = 47 VP; 3,3,3 = 48 VP
                        case var counts when counts == (3, 3, 0):   return new[] { 3, 3, 2 };   // 5,3,0 = 34 VP; 3,3,2 = 36 VP
                        case var counts when counts == (3, 3, 1):   return new[] { 3, 3, 3 };   // 5,3,1 = 42 VP; 3,3,3 = 48 VP
                        case var counts when counts == (3, 3, 2):   return new[] { 4, 3, 3 };   // 5,3,2 = 52 VP; 4,3,3 = 55 VP
                        case var counts when counts == (3, 3, 3):   return new[] { 5, 3, 3 };
                        case var counts when counts == (4, 0, 0):   return new[] { 6, 0, 0 };
                        case var counts when counts == (4, 1, 0):   return new[] { 6, 1, 0 };
                        case var counts when counts == (4, 1, 1):   return new[] { 6, 1, 1 };
                        case var counts when counts == (4, 2, 0):   return new[] { 6, 2, 0 };   // 6,2,0 = 40 VP; 4,2,2 = 38 VP (all 4,2+,0 are better upping the 4 count)
                        case var counts when counts == (4, 2, 1):   return new[] { 6, 2, 1 };   // 6,2,1 = 48 VP; 5,2,2 = 47 VP
                        case var counts when counts == (4, 2, 2):   return new[] { 6, 2, 2 };   // 6,2,2 = 58 VP; 4,3,3 = 55 VP
                        case var counts when counts == (4, 3, 0):   return new[] { 6, 3, 0 };
                        case var counts when counts == (4, 3, 1):   return new[] { 4, 3, 3 };   // 6,3,1 = 53 VP; 4,3,3 = 55 VP
                        case var counts when counts == (4, 3, 2):   return new[] { 5, 3, 3 };   // 6,3,2 = 63 VP; 5,3,3 = 64 VP
                        case var counts when counts == (4, 3, 3):   return new[] { 4, 4, 4 };   // 6,3,3 = 75 VP; 4,4,4 = 76 VP
                        case var counts when counts == (4, 4, 0):   return new[] { 6, 4, 0 };
                        case var counts when counts == (4, 4, 1):   return new[] { 4, 4, 3 };   // 6,4,1 = 60 VP; 4,4,3 = 62 VP
                        case var counts when counts == (4, 4, 2):   return new[] { 4, 4, 4 };   // 6,4,2 = 70 VP; 4,4,4 = 76 VP
                        case var counts when counts == (4, 4, 3):   return new[] { 5, 4, 4 };   // 6,4,3 = 82 VP; 5,4,4 = 85 VP
                        case var counts when counts == (4, 4, 4):   return new[] { 6, 4, 4 };

                        default:
                            throw new Exception($"Unexpected number of science symbols: ({symbolCounts.most}, {symbolCounts.middle}, {symbolCounts.least})");
                    }

                default:
                    throw new Exception($"Unexpected number of science wild symbols: {wildCount}");
            }
        }

        private Production cityProduction;
        private IList<WonderStage> availableWonderStages;
        private IList<TableauElement> builtElements;
        private ResourceOptions resourceProductionOptions = new ResourceOptions();
        private ResourceOptions resourceTradeOptions = new ResourceOptions();
    }
}
