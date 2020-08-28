namespace System.Xml
{
    /// <summary>
    /// 
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string ReadAttribute(this XmlNode node, string attributeName)
        {

            return ReadAttribute(node.Attributes, attributeName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string ReadAttribute(this XmlAttributeCollection attributes, string attributeName)
        {
            var attr = attributes[attributeName];
            if (attr != null)
            {
                return attr.Value;
            }
            return string.Empty;
        }
    }
}