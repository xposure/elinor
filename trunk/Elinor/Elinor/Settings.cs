using System;
using System.Runtime.Serialization;

namespace Elinor
{
    [Serializable]
    public class Settings : ISerializable
    {
        internal string ProfileName;
        internal double MarginThreshold;
        internal double MinimumThreshold;
        internal int Accounting;
        internal int BrokerRelations;
        internal double FactionStanding;
        internal double CorpStanding;
      


        public Settings()
        {
            ProfileName = "Default";
            MarginThreshold = .1;
            MinimumThreshold = .02;
            Accounting = 5;
            BrokerRelations = 5;
            FactionStanding = .0;
            CorpStanding = .0;
        }



        public override string ToString()
        {
            return ProfileName;
        }

        public Settings(SerializationInfo info, StreamingContext ctxt)
        {
            ProfileName = (string)info.GetValue("profilename", typeof(string));
            MarginThreshold = (double)info.GetValue("marginthreshold", typeof(double));
            MinimumThreshold = (double)info.GetValue("minimumthreshold", typeof(double));
            Accounting = (int)info.GetValue("accounting", typeof(int));
            BrokerRelations = (int)info.GetValue("brokerrelations", typeof(int));
            FactionStanding = (double)info.GetValue("factionstanding", typeof(double));
            CorpStanding = (double)info.GetValue("corpstanding", typeof(double));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.AddValue("senderName", SenderName);
            info.AddValue("profilename", ProfileName);
            info.AddValue("marginthreshold", MarginThreshold);
            info.AddValue("minimumthreshold", MinimumThreshold);
            info.AddValue("accounting", Accounting);
            info.AddValue("brokerrelations", BrokerRelations);
            info.AddValue("factionstanding", FactionStanding);
            info.AddValue("corpstanding", CorpStanding);
        }
    }
}
