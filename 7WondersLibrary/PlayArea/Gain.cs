using System.Collections.Generic;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// Represents a gain in coins (awarded immediately) or victory points (scored at the end of the game).
    /// The gain is based on a criteria in your own tableau and/or the tableaus of your immediate neighbours.
    /// </summary>
    public class Gain
    {
        public Gain(XmlElement gainElement)
        {
            if (gainElement.HasAttribute("coins"))
            {
                coinsPerMatch = gainElement.GetAttribute_Int("coins");
            }
            if (gainElement.HasAttribute("victoryPoints"))
            {
                victoryPointsPerMatch = gainElement.GetAttribute_Int("victoryPoints");
            }
            if (gainElement.HasAttribute("colour"))
            {
                gainSource = GainSource.Colour;
                matchingColour = gainElement.GetAttribute_Enum<Colour>("colour");
            }
            else
            {
                gainSource = gainElement.GetAttribute_Enum<GainSource>("other").Value;
            }
            lookAt = gainElement.GetAttribute_EnumSet<RelativePlayer>("lookAt");
        }

        public int GetCoinsGained(PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            if (coinsPerMatch == 0)
            {
                return 0;
            }
            return coinsPerMatch * GetMatches(actingPlayer, leftNeighbour, rightNeighbour);
        }

        public int GetVictoryPointsGained(PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            if (victoryPointsPerMatch == 0)
            {
                return 0;
            }
            return victoryPointsPerMatch * GetMatches(actingPlayer, leftNeighbour, rightNeighbour);
        }

        private int GetMatches(PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour)
        {
            int count = 0;
            if (lookAt.Contains(RelativePlayer.Self))
            {
                count += GetMatches(actingPlayer);
            }
            if (lookAt.Contains(RelativePlayer.Left))
            {
                count += GetMatches(leftNeighbour);
            }
            if (lookAt.Contains(RelativePlayer.Right))
            {
                count += GetMatches(rightNeighbour);
            }
            return count;
        }

        private int GetMatches(PlayerState player)
        {
            switch (gainSource)
            {
                case GainSource.Colour:
                    return player.CountColour(matchingColour.Value);

                case GainSource.Defeats:
                    return player.MilitaryDefeats;

                case GainSource.Wonder:
                    return player.WonderStagesBuilt;

                default:
                    return 0;
            }
        }

        private enum RelativePlayer
        {
            Left = -1,
            Self = 0,
            Right = +1
        }

        private enum GainSource
        {
            Colour,
            Defeats,
            Wonder
        }

        private ISet<RelativePlayer> lookAt;    // Which tableaus to look at when evaluating the gain.
        private int coinsPerMatch;
        private int victoryPointsPerMatch;
        private GainSource gainSource;
        private Colour? matchingColour;
    }
}
