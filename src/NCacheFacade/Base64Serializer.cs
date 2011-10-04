namespace NCacheFacade
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using NBasicExtensionMethod;

    /// <summary>
    ///   Allows serialisation from any CLI object marked with the Serialisable attribute to 
    ///   a base 64 encoded string. Also allows deserialisation from a base 64 string to an 
    ///   object of type T.
    /// </summary>
    /// <typeparam name = "T">The type of the objects to serialise/deserialise.</typeparam>
    public class Base64Serializer<T>
    {
        /// <summary>
        ///   Serialise o to a base 64 encoded string.
        /// </summary>
        /// <param name = "o">The object of type T to serialise</param>
        /// <returns>A base 64 string that contains a binary representation of the object.</returns>
        public string Serialize(T o)
        {
            var memoryStream = new MemoryStream();
            IFormatter bformatter = new BinaryFormatter();
            bformatter.Serialize(memoryStream, o);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var binaryReader = new BinaryReader(memoryStream);
            var bytes = binaryReader.ReadBytes((int) memoryStream.Length);
            var b64 = Convert.ToBase64String(bytes);
            binaryReader.Close();
            memoryStream.Close();

            return b64;
        }

        /// <summary>
        ///   Deserialise to an object of type T from a base 64 string.
        /// </summary>
        /// <param name = "s">A string containing binary data in base 64.</param>
        /// <returns>An object of type T, deserialized from the string.</returns>
        /// <exception cref = "SerializationException">Thrown if the base 64 string does not represent an object of the expected type.</exception>
        public T DeSerialize(string s)
        {
            if (s.IsNullOrWhiteSpace()) return default(T);

            var bytes = Convert.FromBase64String(s);
            IFormatter bformatter = new BinaryFormatter();
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            bw.Write(bytes);
            ms.Seek(0, SeekOrigin.Begin);
            var temp = bformatter.Deserialize(ms);
            bw.Close();

            if (!(temp is T))
                throw new SerializationException(
                    "The Base64 string did not deserialize to the expected type. The actual type was " +
                    temp.GetType().FullName);
            else
                return (T) temp;
        }
    }
}