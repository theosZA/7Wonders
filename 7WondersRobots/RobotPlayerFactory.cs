using Extensions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// Factory class to produce <see cref="RobotPlayer"/> instances or a <see cref="CityPlayer"/> instance
    /// which has different logic for each city board.
    /// </summary>
    public class RobotPlayerFactory
    {
        public RobotPlayerFactory(string robotsXmlFileName)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(robotsXmlFileName);
            weights = ReadRobotsFromXml(xmlDocument.DocumentElement);
        }

        public RobotPlayer CreateRobotPlayer(string playerName, string cityName, char citySide)
        {
            return new RobotPlayer(playerName, weights[(cityName, citySide)]);
        }

        public CityPlayer CreatePlayer(string playerName, char citySide)
        {
            // Create one RobotPlayer for each city (of the matching side).
            var robotPlayers = weights.Where(mapping => mapping.Key.side == citySide)
                                      .ToDictionary(mapping => mapping.Key.cityName,
                                                    mapping => (PlayerAgent)new RobotPlayer(playerName, mapping.Value));
            return new CityPlayer(playerName, robotPlayers);
        }

        private static IDictionary<(string cityName, char side), IReadOnlyCollection<int>> ReadRobotsFromXml(XmlElement robotsElement)
        {
            return robotsElement.GetChildElements("Robot").Select(ReadRobotInfoFromXml)
                                                          .ToDictionary(robotInfo => (robotInfo.cityName, robotInfo.side),
                                                                        robotInfo => robotInfo.weights);
        }

        private static (string cityName, char side, IReadOnlyCollection<int> weights) ReadRobotInfoFromXml(XmlElement robotElement)
        {
            return (robotElement.GetAttribute("city"),
                    robotElement.GetAttribute("side")[0],
                    ReadWeights(robotElement.FirstChild.Value).ToList());
        }

        private static IEnumerable<int> ReadWeights(string weightsText)
        {
            return weightsText.Split('.')
                              .Select(weightText => int.Parse(weightText, NumberStyles.HexNumber));
        }

        private readonly IDictionary<(string cityName, char side), IReadOnlyCollection<int>> weights;
    }
}
