namespace Elinor
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class Serializer
    {
        internal static void SerializeObject(string filename, Settings objectToSerialize)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            var bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, objectToSerialize);
            stream.Close();
        }

        internal static Settings DeSerializeObject(string filename)
        {
            Stream stream = File.Open(filename, FileMode.Open);
            var bFormatter = new BinaryFormatter();
            var objectToSerialize = (Settings)bFormatter.Deserialize(stream);
            stream.Close();
            return objectToSerialize;
        }
    }
}
