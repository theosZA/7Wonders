using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// A player's wonder board (i.e. city + wonder) and all the cards that he's built.
    /// </summary>
    public class Tableau
    {
        public int Military => builtCards.Sum(card => card.Military);

        public int WonderStagesBuilt => builtWonderStages.Count();

        public Tableau(XmlElement cityElement)
        {
            cityName = cityElement.GetAttribute("name");
            cityProduction = new Production(cityElement.GetChildElements("Production"));
            AddProduction(cityProduction, availableForTrade: false);
            var wonderElement = cityElement.GetChildElement("Wonder");
            var wonderName = wonderElement.GetAttribute_String("name");
            availableWonderStages = wonderElement.GetChildElements("Stage").Select((stageElement, i) => new WonderStage($"{wonderName} {i + 1}", stageElement)).ToList();
        }

        public void WriteToConsole()
        {
            ConsoleHelper.ClearConsoleColours();
            Console.WriteLine($"{cityName} ({cityProduction.GetFirstResource()})");
            var colours = Enum.GetValues(typeof(Colour)).Cast<Colour>();
            foreach (var colour in colours)
            {
                ConsoleHelper.WriteCardsToConsole(builtCards.Where(card => card.Colour == colour));
            }
            if (builtWonderStages.Count > 0)
            {
                Console.Write(string.Join(", ", builtWonderStages.Select(wonderStage => wonderStage.Name)));
                Console.Write(" ");
            }
            if (availableWonderStages.Count > 0)
            {
                Console.Write("(unbuilt ");
                Console.Write(string.Join(", ", availableWonderStages.Select(wonderStage => wonderStage.Name)));
                Console.Write(")");
            }
            Console.WriteLine();
        }

        public string ResourceProductionToString()
        {
            var text = new StringBuilder();

            // Combine linear resource production of each resource.
            var resources = ResourceHelper.GetAllResources();
            bool firstResource = true;
            foreach (var resource in resources)
            {
                var count = builtCards.Sum(card => card.Production.GetSingleProduction(resource)) + cityProduction.GetSingleProduction(resource);
                if (count > 0)
                {
                    if (!firstResource)
                    {
                        text.Append(", ");
                    }
                    text.Append($"{count} {resource}");
                    firstResource = false;
                }
            }

            // List each resource combination where only 1 of each set of resources can be chosen.
            foreach (var card in builtCards)
            {
                var multipleProduction = card.Production.GetMultipleProduction()?.ToList();
                if (multipleProduction != null && multipleProduction.Any())
                {
                    if (!firstResource)
                    {
                        text.Append(", ");
                    }
                    text.Append($"1 {string.Join("/", multipleProduction)}");
                    firstResource = false;
                }
            }

            return text.ToString();
        }

        public void Add(Card card)
        {
            builtCards.Add(card);
            AddProduction(card.Production, availableForTrade: (card.Colour == Colour.Brown || card.Colour == Colour.Gray));
        }

        public bool Has(string cardName)
        {
            return builtCards.Any(card => card.Name == cardName);
        }

        public WonderStage NextWonderStage()
        {
            return availableWonderStages.FirstOrDefault();
        }

        public void BuildNextWonderStage()
        {
            builtWonderStages.Add(availableWonderStages.First());
            availableWonderStages.RemoveAt(0);
        }

        public int CountColour(Colour colour)
        {
            return builtCards.Count(card => card.Colour == colour);
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
            return builtCards.Select(card => card.TradeBonus == null ? 2 : card.TradeBonus.TradeCost(resource, direction))
                             .DefaultIfEmpty(2)
                             .Min();
        }

        public int CalculateWonderVictoryPoints()
        {
            return builtWonderStages.Sum(wonderStage => wonderStage.VictoryPoints);
        }

        public int CalculateCivilianVictoryPoints(Player self, Player leftNeighbour, Player rightNeighbour)
        {
            return CalculateVictoryPointsForColour(Colour.Blue, self, leftNeighbour, rightNeighbour);
        }

        public int CalculateCommercialVictoryPoints(Player self, Player leftNeighbour, Player rightNeighbour)
        {
            return CalculateVictoryPointsForColour(Colour.Yellow, self, leftNeighbour, rightNeighbour);
        }

        public int CalculateGuildVictoryPoints(Player self, Player leftNeighbour, Player rightNeighbour)
        {
            return CalculateVictoryPointsForColour(Colour.Purple, self, leftNeighbour, rightNeighbour);
        }

        public int CalculateScienceVictoryPoints()
        {
            // Count how many of each symbol we have.
            var symbolCount = Enumerable.Range(0, 3).Select(i => builtCards.Count(card => card.Science.HasValue && card.Science.Value == (ScienceSymbol)i)).ToArray();

            // What should we be counting the wilds as?
            int wildCount = builtCards.Count(card => card.Science.HasValue && card.Science.Value == ScienceSymbol.Wild);
            // TBD - For now, force all to whatever we have the least of.
            var leastIndex = symbolCount.ToList().IndexOf(symbolCount.Min());
            symbolCount[leastIndex] += wildCount;

            // 7 points for each complete set.
            int completeSets = symbolCount.Min();
            int completeSetsScore = 7 * completeSets;

            // n^2 points for each symbol where n is the number of that symbol in the tableau.
            int squareSymbolsScore = symbolCount.Sum(n => n * n);

            return completeSetsScore + squareSymbolsScore;
        }

        private int CalculateVictoryPointsForColour(Colour colour, Player self, Player leftNeighbour, Player rightNeighbour)
        {
            return builtCards.Sum(card => card.Colour == colour ? card.EvaluateVictoryPoints(self, leftNeighbour, rightNeighbour) : 0);
        }

        private void AddProduction(Production production, bool availableForTrade)
        {
            resourceProductionOptions.AddProduction(production);
            if (availableForTrade)
            {
                resourceTradeOptions.AddProduction(production);
            }
        }

        private string cityName;
        private Production cityProduction;
        private IList<WonderStage> availableWonderStages;
        private IList<WonderStage> builtWonderStages = new List<WonderStage>();
        private IList<Card> builtCards = new List<Card>();
        private ResourceOptions resourceProductionOptions = new ResourceOptions();
        private ResourceOptions resourceTradeOptions = new ResourceOptions();
    }
}
