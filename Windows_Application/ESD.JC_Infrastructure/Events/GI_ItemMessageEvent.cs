using Prism.Events;

namespace ESD.JC_Infrastructure.Events
{
    public class GI_ItemMessageEvent : PubSubEvent<GIItemMessage>
    {
    }

    public class GIItemMessage
    {
        public string State { get; set; }

        public double PercentageValue { get; set; }

        public bool HasValue { get { return !(PercentageValue < 0.0); } }

        public string TimerValue { get; set; }

        public GIItemMessage()
        {
            State = "None";
            PercentageValue = -1.0;
            TimerValue = "00:00:00";
        }
    }
}
