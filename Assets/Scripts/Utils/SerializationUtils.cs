using System.IO;
using System.Xml.Serialization;

namespace Utils
{
    public static class SerializationUtils
    {
        
        public static T DeserializeXml<T>(string path)
        {
            var serializer = new XmlSerializer(typeof(T));
            var reader = new StreamReader(path);
            var deserialized = (T)serializer.Deserialize(reader.BaseStream);
            reader.Close();
            return deserialized;
        }
    }
}