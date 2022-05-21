using Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// Represents an imaginary copy of a single card (of yours or one of your immediate neighbour). Only the victory
    /// points of the card are gained. The copied card is the one awarding the most victory points.
    /// </summary>
    public class Copy
    {
        public Copy(XmlElement copyElement)
        {
            copyColour = copyElement.GetAttribute_Enum<Colour>("colour").Value;
            lookAt = copyElement.GetAttribute_EnumSet<RelativePlayer>("lookAt");
        }

        public int GetVictoryPointsGained(PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            // Get all cards of the matching colour.
            var matchingCards = new List<Card>();
            if (lookAt.Contains(RelativePlayer.Self))
            {
                matchingCards.AddRange(actingPlayer.GetAllBuiltCards(copyColour));
            }
            if (lookAt.Contains(RelativePlayer.Left))
            {
                matchingCards.AddRange(leftNeighbour.GetAllBuiltCards(copyColour));
            }
            if (lookAt.Contains(RelativePlayer.Right))
            {
                matchingCards.AddRange(rightNeighbour.GetAllBuiltCards(copyColour));
            }
            if (!matchingCards.Any())
            {
                return 0;
            }

            // Evaluate how many victory points each of the cards would give us and choose the highest value.
            // We currently exclude ALL copy effects from this evaluation to avoid infinite recursion. This might lead us to
            // miss a combined selection of copies that results in a higher number of victory points when there are 2 or more
            // copy effects. Since there's only one copy effect in the game at the moment (Olympia B), this isn't a problem.
            int baselineVPs = actingPlayer.CalculateVictoryPoints(leftNeighbour, rightNeighbour, includeCopyEffects: false);
            int bestVPs = matchingCards.Distinct()
                                       .Select(card => EvaluateCard(card, actingPlayer, leftNeighbour, rightNeighbour))
                                       .Max();
            return bestVPs - baselineVPs;
        }

        private int EvaluateCard(Card card, PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            var actingPlayerClone = new PlayerState(actingPlayer);
            actingPlayerClone.AddCardToTableau(card);
            return actingPlayerClone.CalculateVictoryPoints(leftNeighbour, rightNeighbour, includeCopyEffects: false);
        }

        private ISet<RelativePlayer> lookAt;    // Which tableaus to look at when evaluating the gain.
        private Colour copyColour;
    }
}
