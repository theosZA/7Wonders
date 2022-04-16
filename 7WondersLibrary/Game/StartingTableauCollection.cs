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
            return citiesElement.GetChildElements("City")
                                .Select(cityElement => new Tableau(cityElement))
                                .ToList();

        }

        private IReadOnlyCollection<Tableau> tableaus;
    }
}
