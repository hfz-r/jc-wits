using Prism.Events;

namespace ESD.JC_Infrastructure.Events
{
    public class FCU_ItemMessageEvent : PubSubEvent<FCUItemMessage>
    {
    }

    public class FCUItemMessage
    {
        public string State { get; set; }

        public double PercentageValue { get; set; }

        public bool HasValue { get { return !(PercentageValue < 0.0); } }

        public string TimerValue { get; set; }

        public FCUItemMessage()
        {
            State = "None";
            PercentageValue = -1.0;
            TimerValue = "00:00:00";
        }
    }
}
