using Prism.Events;

namespace ESD.JC_Infrastructure.Events
{
    public class GR_ItemMessageEvent : PubSubEvent<GRItemMessage>
    {
    }

    public class GRItemMessage
    {
        public string State { get; set; }

        public double PercentageValue { get; set; }

        public bool HasValue { get { return !(PercentageValue < 0.0); } }

        public string TimerValue { get; set; }

        public GRItemMessage()
        {
            State = "None";
            PercentageValue = -1.0;
            TimerValue = "00:00:00";
        }
    }
}
