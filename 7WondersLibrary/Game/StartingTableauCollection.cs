using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// The collection of all possible starting tableaus.
    /// </summary>
    /// <remarks>For now we only allow a single tableau and simply replicate it 7 times.</remarks>
    public class StartingTableauCollection
    {
        public StartingTableauCollection(string citiesXmlFileName)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(citiesXmlFileName);
            tableaus = ReadCitiesFromXml(xmlDocument.DocumentElement).ToList();
        }

        public StartingTableauCollection(XmlElement citiesElement)
        {
            tableaus = ReadCitiesFromXml(citiesElement).ToList();
        }

        public IEnumerable<Tableau> GetCopyOfTableaus()
        {
            return tableaus.Select(tableau => new Tableau(tableau));
        }

        private static IEnumerable<Tableau> ReadCitiesFromXml(XmlElement citiesElement)
        {
            var cityElements = citiesElement.GetChildElements("City");

            // For now we just use one common tableau, replicated across all our players. TBD implement all city boards.
            return Enumerable.Range(0, 7).Select(i => new Tableau(cityElements.First())).ToList();

        }

        private IReadOnlyCollection<Tableau> tableaus;
    }
}
