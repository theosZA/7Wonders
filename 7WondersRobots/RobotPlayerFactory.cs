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

        public RobotPlayer CreateRobotPlayer(string playerName, string cityName, BoardSide boardSide)
        {
            return new RobotPlayer(playerName, weights[(cityName, boardSide)]);
        }

        public CityPlayer CreatePlayer(string playerName, BoardSide boardSide)
        {
            // Create one RobotPlayer for each city (of the matching side).
            var robotPlayers = weights.Where(mapping => mapping.Key.boardSide == boardSide)
                                      .ToDictionary(mapping => mapping.Key.cityName,
                                                    mapping => (PlayerAgent)new RobotPlayer(playerName, mapping.Value));
            return new CityPlayer(playerName, robotPlayers);
        }

        private static IDictionary<(string cityName, BoardSide boardSide), IReadOnlyCollection<int>> ReadRobotsFromXml(XmlElement robotsElement)
        {
            return robotsElement.GetChildElements("Robot").Select(ReadRobotInfoFromXml)
                                                          .ToDictionary(robotInfo => (robotInfo.cityName, robotInfo.boardSide),
                                                                        robotInfo => robotInfo.weights);
        }

        private static (string cityName, BoardSide boardSide, IReadOnlyCollection<int> weights) ReadRobotInfoFromXml(XmlElement robotElement)
        {
            return (robotElement.GetAttribute("city"),
                    robotElement.GetAttribute_Enum<BoardSide>("side").Value,
                    ReadWeights(robotElement.FirstChild.Value).ToList());
        }

        private static IEnumerable<int> ReadWeights(string weightsText)
        {
            return weightsText.Split('.')
                              .Select(weightText => int.Parse(weightText, NumberStyles.HexNumber));
        }

        private readonly IDictionary<(string cityName, BoardSide boardSide), IReadOnlyCollection<int>> weights;
    }
}
