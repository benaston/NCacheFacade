namespace NCacheFacade
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using NBasicExtensionMethod;
    using NSure;
    using ArgumentException = NHelpfulException.FrameworkExceptions.ArgumentException;
    using ArgumentNullException = NHelpfulException.FrameworkExceptions.ArgumentNullException;

    /// <summary>
    ///   Helper class for serializing and desserializing objects on the fly.
    /// </summary>
    public static class ObjectSerializer
    {
        /// <summary>
        ///   Serialise and object to an XML string.
        ///   Note: use requires configuration section to be present in the wen.config.
        ///   See http://blog.actcode.com/2009/11/net-application-logging-with-log4net-in.html
        ///   for an example.
        /// </summary>
        /// <typeparam name = "T">The object type</typeparam>
        /// <param name = "objectToSerialize">The object to serialize</param>
        /// <returns>A string representing the object serialized to XML.</returns>
        public static string SerializeObjectToXmlString<T>(T objectToSerialize)
        {
            Ensure.That<ArgumentNullException>(objectToSerialize.IsNotNull(), "objectToSerialize not supplied");

            var doc = new XmlDocument();
            var serializer = new XmlSerializer(typeof (T));
            var stream = new MemoryStream();

            serializer.Serialize(stream, objectToSerialize);
            stream.Position = 0;
            doc.Load(stream);

            return doc.DocumentElement != null ? doc.DocumentElement.OuterXml : String.Empty;
        }

        /// <summary>
        ///   De-serialize an object from and XML string to an object instance.
        /// </summary>
        /// <typeparam name = "T">The object type</typeparam>
        /// <param name = "serializedObjectString">The XML string to de-serialize</param>
        /// <returns>An instantiated object from the XML string</returns>
        public static T DeSerializeObjectFromXmlString<T>(string serializedObjectString)
        {
            Ensure.That<ArgumentException>(serializedObjectString.IsNotNullOrWhiteSpace(), "serializedObjectString");

            var serializer = new XmlSerializer(typeof (T));
            TextReader tw = new StringReader(serializedObjectString);

            return (T) serializer.Deserialize(tw);
        }
    }
}