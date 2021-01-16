using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// Safe XML access operations.
    /// </summary>
    internal static class XmlExtensions
    {
        public static XmlElement GetChildElement(this XmlElement xmlElement, string childElementName)
        {
            return (XmlElement)xmlElement.SelectSingleNode(childElementName);
        }

        public static IEnumerable<XmlElement> GetChildElements(this XmlElement xmlElement, string childElementName)
        {
            var childNodes = xmlElement.SelectNodes(childElementName);
            foreach (var childNode in childNodes)
            {
                if (childNode is XmlElement childElement)
                {
                    yield return childElement;
                }
            }
        }

        public static string GetAttribute_String(this XmlElement xmlElement, string attributeName)
        {
            if (!xmlElement.HasAttribute(attributeName))
            {
                return null;
            }
            return xmlElement.GetAttribute(attributeName);
        }

        public static int GetAttribute_Int(this XmlElement xmlElement, string attributeName)
        {
            if (!xmlElement.HasAttribute(attributeName))
            {
                return 0;
            }
            return int.Parse(xmlElement.GetAttribute(attributeName));
        }

        public static IReadOnlyCollection<int> GetAttribute_IntSequence(this XmlElement xmlElement, string attributeName)
        {
            return xmlElement.GetAttribute_String(attributeName).Split(',').Select(int.Parse).ToList();
        }

        public static E? GetAttribute_Enum<E>(this XmlElement xmlElement, string attributeName) where E : struct, Enum
        {
            if (!xmlElement.HasAttribute(attributeName))
            {
                return null;
            }
            return (E)Enum.Parse(typeof(E), xmlElement.GetAttribute(attributeName), ignoreCase: true);
        }


        public static ISet<E> GetAttribute_EnumSet<E>(this XmlElement xmlElement, string attributeName) where E : struct, Enum
        {
            if (!xmlElement.HasAttribute(attributeName))
            {
                return new HashSet<E>();
            }
            return new HashSet<E>(xmlElement.GetAttribute(attributeName).Split(',').Select(text => (E)Enum.Parse(typeof(E), text, ignoreCase: true)));
        }
    }
}
