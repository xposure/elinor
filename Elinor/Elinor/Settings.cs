using System;
using System.IO;
using System.Runtime.Serialization;

namespace Elinor
{
    [Serializable]
    public class Settings : ISerializable
    {
        public Settings()
        {
            ProfileName = "Default";
            MarginThreshold = .1;
            MinimumThreshold = .02;
            Accounting = 5;
            BrokerRelations = 5;
            FactionStanding = .0;
            CorpStanding = .0;
            AdvancedStepSettings = false;
            BuyPercentage = .0;
            BuyThreshold = .0;
            SellPercentage = .0;
            SellThreshold = .0;
        }

        public Settings(SerializationInfo info, StreamingContext ctxt)
        {
            ProfileName = (string) info.GetValue("profilename", typeof (string));
            MarginThreshold = (double) info.GetValue("marginthreshold", typeof (double));
            MinimumThreshold = (double) info.GetValue("minimumthreshold", typeof (double));
            Accounting = (int) info.GetValue("accounting", typeof (int));
            BrokerRelations = (int) info.GetValue("brokerrelations", typeof (int));
            FactionStanding = (double) info.GetValue("factionstanding", typeof (double));
            CorpStanding = (double) info.GetValue("corpstanding", typeof (double));
            AdvancedStepSettings = info.GetBoolean("advancedstepsettings");
            BuyPercentage = info.GetDouble("buypercentage");
            BuyThreshold = info.GetDouble("buythreshold");
            SellPercentage = info.GetDouble("sellpercentage");
            SellThreshold = info.GetDouble("sellthreshold");
        }

        internal string ProfileName { get; set; }
        internal double MarginThreshold { get; set; }
        internal double MinimumThreshold { get; set; }
        internal int Accounting { get; set; }
        internal int BrokerRelations { get; set; }
        internal double FactionStanding { get; set; }
        internal double CorpStanding { get; set; }
        internal bool AdvancedStepSettings { get; set; }
        internal double BuyPercentage { get; set; }
        internal double BuyThreshold { get; set; }
        internal double SellPercentage { get; set; }
        internal double SellThreshold { get; set; }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("profilename", ProfileName);
            info.AddValue("marginthreshold", MarginThreshold);
            info.AddValue("minimumthreshold", MinimumThreshold);
            info.AddValue("accounting", Accounting);
            info.AddValue("brokerrelations", BrokerRelations);
            info.AddValue("factionstanding", FactionStanding);
            info.AddValue("corpstanding", CorpStanding);
            info.AddValue("advancedstepsettings", AdvancedStepSettings);
            info.AddValue("buypercentage", BuyPercentage);
            info.AddValue("buythreshold", BuyThreshold);
            info.AddValue("sellpercentage", SellPercentage);
            info.AddValue("sellthreshold", SellThreshold);
        }

        #endregion

        public override string ToString()
        {
            return ProfileName;
        }

        public static Settings ReadSettings(string profileName)
        {
            if (File.Exists(string.Format("profiles\\{0}.dat", profileName)))
            {
                try
                {
                    return Serializer.DeSerializeObject(string.Format("profiles\\{0}.dat", profileName));
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public static void SaveSettings(Settings settings)
        {
            if (settings.ProfileName == "Default") return;
            Directory.CreateDirectory("profiles");
            Serializer.SerializeObject(string.Format("profiles\\{0}.dat", settings.ProfileName), settings);
        }
    }
}