namespace Timer
{
    public class TimerModelBuilder
    {
        public static ITimerModel GetNewTimer()
        {
            return new TimerModel();
        }
    }
}
