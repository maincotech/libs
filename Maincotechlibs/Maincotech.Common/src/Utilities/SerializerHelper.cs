using Maincotech.IO;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Maincotech.Utilities
{
    public static class SerializerHelper
    {
        /// <summary>
        /// Deserializes an object from a Base64 encoded string
        /// using the <see cref="BinaryFormatter" />.
        /// </summary>
        /// <param name="data">The Base64 encoded data to deserialize.</param>
        /// <typeparam name="TData">The type of data to process.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static TData DeserializeFromBase64String<TData>(string data)
        {
            return (TData)DeserializeFromBase64String(data);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public static TData DeserializeFromXORBase64String<TData>(string data)
        {
            return (TData)DeserializeFromXORBase64String(data);
        }

        /// <summary>
        /// Deserializes an object from a Base64 encoded string
        /// using the <see cref="BinaryFormatter" />.
        /// </summary>
        /// <param name="data">The Base64 encoded data to deserialize.</param>
        /// <typeparam name="TData">The type of data to process.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeFromBase64String(string data)
        {
            using (var ms = new MemoryStream())
            {
                var content = Convert.FromBase64String(data);
                ms.Write(content, 0, content.Length);
                ms.Position = 0;
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }

        public static object DeserializeFromXORBase64String(string data)
        {
            using (var ms = new MemoryStream())
            {
                var content = Convert.FromBase64String(data);
                for (var i = 0; i < content.Length; i++)
                {
                    content[i] = (byte)(content[i] ^ 38);
                }
                ms.Write(content, 0, content.Length);
                ms.Position = 0;
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Serializes an object using the <see cref="BinaryFormatter" />
        /// into a Base64 encoded string.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <typeparam name="TData">The type of data to process.</typeparam>
        /// <returns>A Base64 encoded string containing serialized
        /// data.</returns>
        public static string SerializeToBase64String<TData>(TData data)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, data);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// Serializes an object using the <see cref="BinaryFormatter" />
        /// into a Base64 encoded string.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <typeparam name="TData">The type of data to process.</typeparam>
        /// <returns>A Base64 encoded string containing serialized
        /// data.</returns>
        public static string SerializeToBase64String(object data)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, data);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string SerializeXorToBase64String(object data)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, data);
                var buf = ms.ToArray();
                for (var i = 0; i < buf.Length; i++)
                {
                    buf[i] = (byte)(buf[i] ^ 38);
                }
                return Convert.ToBase64String(buf);
            }
        }

        /// <summary>
        /// Serializes an object using the <see cref="XmlSerializer" />
        /// into an XML string.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <typeparam name="TData">The type of data to process.</typeparam>
        /// <returns>An XML string containing serialized
        /// data.
        /// </returns>
        public static string SerializeToXmlString<TData>(TData data)
        {
            var serializedData = new StringBuilder();
            using (StringWriter writer = new EncodingStringWriter(Encoding.UTF8, serializedData, CultureInfo.InvariantCulture))
            {
                var serializer = new XmlSerializer(typeof(TData));
                serializer.Serialize(writer, data);
            }
            return serializedData.ToString();
        }

        /// <summary>
        /// Serializes an object using the <see cref="XmlSerializer" />
        /// into an XML string.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <typeparam name="TData">The type of data to process.</typeparam>
        /// <returns>An XML string containing serialized
        /// data.
        /// </returns>
        public static string SerializeToXmlString(object data)
        {
            var serializedData = new StringBuilder();
            using (StringWriter writer = new EncodingStringWriter(Encoding.UTF8, serializedData, CultureInfo.InvariantCulture))
            {
                var serializer = new XmlSerializer(data.GetType());
                serializer.Serialize(writer, data);
            }
            return serializedData.ToString();
        }

        /// <summary>
        /// Deserializes an object from an XML string
        /// using the <see cref="XmlSerializer" />.
        /// </summary>
        /// <param name="data">The XML data to Deserializes.</param>
        /// <typeparam name="TData">The type of data to process.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static TData DeserializeFromXmlString<TData>(string data)
        {
            using (var reader = new StringReader(data))
            {
                var serializer = new XmlSerializer(typeof(TData));
                return (TData)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Deserializes an object from an XML string
        /// using the <see cref="XmlSerializer" />.
        /// </summary>
        /// <param name="data">The XML data to Deserializes.</param>
        /// <param name="Type">The type of data to process.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeFromXmlString(string data, Type type)
        {
            using (var reader = new StringReader(data))
            {
                var serializer = new XmlSerializer(type);
                return serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Serializes an object using the <see cref="XmlSerializer" />
        /// into an XML string.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <typeparam name="TData">The type of data to process.</typeparam>
        /// <returns>An XML string containing serialized
        /// data.
        /// </returns>
        public static string SerializeToXmlStringWithoutDecalaring<TData>(TData data)
        {
            var result = SerializeToXmlString(data);
            var doc = new XmlDocument();
            doc.LoadXml(result);
            return doc.DocumentElement.OuterXml;
        }

        /// <summary>
        /// Serializes an object using the <see cref="XmlSerializer" />
        /// into an XML string.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <typeparam name="TData">The type of data to process.</typeparam>
        /// <returns>An XML string containing serialized
        /// data.
        /// </returns>
        public static string SerializeToXmlStringWithoutDecalaring(object data)
        {
            var result = SerializeToXmlString(data);
            var doc = new XmlDocument();
            doc.LoadXml(result);
            return doc.DocumentElement.OuterXml;
        }

        /// <summary>
        /// Serializes an object using the <see cref="XmlSerializer" />
        /// into an XML string.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <typeparam name="TData">The type of data to process.</typeparam>
        /// <returns>An XML string containing serialized
        /// data.
        /// </returns>
        public static object DeserializeToXmlStringWithoutDecalaring(string data, Type type)
        {
            return DeserializeFromXmlString(data, type);
        }

        /// <summary>
        /// Deserializes an object from an XML string
        /// using the <see cref="XmlSerializer" />.
        /// </summary>
        /// <param name="data">The XML data to Deserializes.</param>
        /// <typeparam name="TData">The type of data to process.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static TData DeserializeFromXmlStringWithoutDecalaring<TData>(string data)
        {
            return DeserializeFromXmlString<TData>(data);
        }
    }
}
