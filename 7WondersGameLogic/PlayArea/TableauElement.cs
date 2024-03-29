﻿using Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// The base class for the elements making up a 7 Wonders tableau, i.e. cards and wonder stages.
    /// </summary>
    public class TableauElement
    {
        public string Name { get; private set; }
        public int CoinGain { get; }
        public int VictoryPoints { get; }
        public int Military { get; }
        public ScienceSymbol? Science { get; }
        public Cost Cost { get; }
        public Production Production { get; }
        public TradeBonus TradeBonus { get; }
        public int FreeBuildsPerAge { get; }
        public bool BuildFromDiscards { get; }
        public bool ExtraAgePlay { get; }

        public IReadOnlyCollection<Gain> EvaluatedGains { get; }
        public IReadOnlyCollection<Copy> EvaluatedCopies { get; }

        public TableauElement(string name, XmlElement xmlElement)
        {
            Name = name;
            CoinGain = xmlElement.GetAttribute_Int("coins");
            VictoryPoints = xmlElement.GetAttribute_Int("victoryPoints");
            Military = xmlElement.GetAttribute_Int("military");
            Science = xmlElement.GetAttribute_Enum<ScienceSymbol>("science");
            Cost = new Cost(xmlElement.GetChildElement("Cost"));
            Production = new Production(xmlElement.GetChildElements("Production"));
            var tradeElement = xmlElement.GetChildElement("Trade");
            if (tradeElement != null)
            {
                TradeBonus = new TradeBonus(tradeElement);
            }
            FreeBuildsPerAge = xmlElement.GetAttribute_Int("freeBuildsPerAge");         // Olympia (A) ability
            BuildFromDiscards = xmlElement.GetAttribute_Int("buildFromDiscards") > 0;   // Halikarnassos ability
            ExtraAgePlay = xmlElement.GetAttribute_Int("extraAgePlay") > 0;             // Babylon (B) ability

            EvaluatedGains = xmlElement.GetChildElements("Gain").Select(gainElement => new Gain(gainElement)).ToList();
            EvaluatedCopies = xmlElement.GetChildElements("Copy").Select(copyElement => new Copy(copyElement)).ToList();    // Olympia (B) ability
        }

        public int EvaluateVictoryPoints(PlayerState actingPlayer, PlayerState leftNeightbour, PlayerState rightNeighbour, bool includeCopyEffects = true)
        {
            int total = VictoryPoints + EvaluatedGains.Sum(gain => gain.GetVictoryPointsGained(actingPlayer, leftNeightbour, rightNeighbour));
            if (includeCopyEffects)
            {
                total += EvaluatedCopies.Sum(copy => copy.GetVictoryPointsGained(actingPlayer, leftNeightbour, rightNeighbour));
            }
            return total;
        }

        /// <summary>
        /// Call when the acting player gains this element to their tableau. This will provide the player with any immediate
        /// benefits of the element but is not responsible for paying any costs or actually moving the element into their tableau.
        /// </summary>
        /// <param name="actingPlayer">The player gaining the element.</param>
        /// <param name="leftNeighbour">The acting player's left-hand neighbour.</param>
        /// <param name="rightNeighbour">The acting player's right-hand neighbour.</param>
        public void OnPlayerGain(PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            actingPlayer.AddCoins(CoinGain);
            foreach (var gain in EvaluatedGains)
            {
                actingPlayer.AddCoins(gain.GetCoinsGained(actingPlayer, leftNeighbour, rightNeighbour));
            }

            if (BuildFromDiscards)
            {
                actingPlayer.PendingBuildFromDiscard = true;
            }
        }
    }
}
