using Extensions;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// Represents a single state of a wonder.
    /// </summary>
    public class WonderStage
    {
        public string Name { get; }
        public int VictoryPoints { get; }
        public Cost Cost { get; }

        public WonderStage(string name, XmlElement stageElement)
        {
            Name = name;
            VictoryPoints = stageElement.GetAttribute_Int("victoryPoints");
            Cost = new Cost(stageElement.GetChildElement("Cost"));
        }
    }
}
