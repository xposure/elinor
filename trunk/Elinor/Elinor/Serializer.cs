namespace Elinor
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class Serializer
    {
        internal static void SerializeObject(string filename, Settings objectToSerialize)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, objectToSerialize);
            stream.Close();
        }

        internal static Settings DeSerializeObject(string filename)
        {
            Settings objectToSerialize;
            Stream stream = File.Open(filename, FileMode.Open);
            BinaryFormatter bFormatter = new BinaryFormatter();
            objectToSerialize = (Settings)bFormatter.Deserialize(stream);
            stream.Close();
            return objectToSerialize;
        }
    }
}
