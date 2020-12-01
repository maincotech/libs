using System.Collections.Generic;
using System.Linq;

namespace System.Xml.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Gets the value of an attribute on an XElement
        /// without having to worry about null reference
        /// problems.
        /// </summary>
        /// <param name="parent">Node to start searching from</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <returns>The attribute value on the element or
        /// String.Empty if the attribute does not exist.</returns>
        public static string AttributeValue(
            this XElement parent, string attributeName)
        {
            if (parent == null)
            {
                return string.Empty;
            }
            if (parent.HasAttributes == false)
            {
                return string.Empty;
            }
            if (parent.Attribute(attributeName) == null)
            {
                return string.Empty;
            }
            return parent.Attribute(attributeName).Value;
        }

        /// <summary>
        /// Returns the first of element that matches
        /// a desired name while ignoring any
        /// namespace management issues.
        /// </summary>
        /// <param name="parent">Node to start searching from.</param>
        /// <param name="name">Node name that you want to retrieve.</param>
        /// <returns>The first matching XElement object.</returns>
        public static XElement ElementByLocalName(
            this XElement parent, string name)
        {
            if (parent == null)
            {
                return null;
            }
            var result = (from temp in parent.Elements()
                          where temp.Name.LocalName == name
                          select temp).FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Find the first matching child element by element name
        /// that has a particular attribute and attribute value.
        /// </summary>
        /// <param name="parent">Node to start searching from.</param>
        /// <param name="elementName">Node name that you want to retrieve.</param>
        /// <param name="attributeName">Attribute to search for.</param>
        /// <param name="attributeValue">Desired attribute value for the search.</param>
        /// <returns>The first matching XElement object.</returns>
        public static XElement ElementByLocalNameAndAttributeValue(
            this XElement parent,
            string elementName,
            string attributeName,
            string attributeValue)
        {
            var matchingElementsByName = parent.ElementsByLocalName(elementName);

            var match = (from temp in matchingElementsByName
                         where
                         temp.HasAttributes == true &&
                         temp.AttributeValue(attributeName) == attributeValue
                         select temp).FirstOrDefault();

            return match;
        }

        /// <summary>
        /// Returns a list of elements that match a desired
        /// name while ignoring any namespace management
        /// issues.
        /// </summary>
        /// <param name="parent">Node to start searching from.</param>
        /// <param name="name">Node name that you want to retrieve.</param>
        /// <returns>List of matching XElement objects.</returns>
        public static IEnumerable<XElement> ElementsByLocalName(
            this XElement parent, string name)
        {
            if (parent == null)
            {
                return null;
            }
            var result = (from temp in parent.Elements()
                          where temp.Name.LocalName == name
                          select temp);

            return result;
        }

        /// <summary>
        /// Finds a child element starting from parent and returns the inner text value.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childElement"></param>
        /// <returns></returns>
        public static string ElementValue(
            this XElement parent, string childElement)
        {
            var child = parent.ElementByLocalName(childElement);

            return child?.Value;
        }
    }
}