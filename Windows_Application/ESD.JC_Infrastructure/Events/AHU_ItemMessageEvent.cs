using Prism.Events;

namespace ESD.JC_Infrastructure.Events
{
    public class AHU_ItemMessageEvent : PubSubEvent<AHUItemMessage>
    {
    }

    public class AHUItemMessage
    {
        public string State { get; set; }

        public double PercentageValue { get; set; }

        public bool HasValue { get { return !(PercentageValue < 0.0); } }

        public string TimerValue { get; set; }

        public AHUItemMessage()
        {
            State = "None";
            PercentageValue = -1.0;
            TimerValue = "00:00:00";
        }
    }
}
