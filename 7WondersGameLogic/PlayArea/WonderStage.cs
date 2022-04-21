using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// Represents a single state of a wonder.
    /// </summary>
    public class WonderStage : TableauElement
    {
        public WonderStage(string name, XmlElement stageElement)
        : base(name, stageElement)
        {}
    }
}
