using System;

namespace Timer
{
    public interface ITimerModel
    {
        TimeSpan Duration { get; set; }
        TimeSpan Interval { get; }
        TimeSpan Remaining { get; }

        event EventHandler<TimerModelEventArgs> Completed;
        event EventHandler<TimerModelEventArgs> Started;
        event EventHandler<TimerModelEventArgs> Stopped;
        event EventHandler<TimerModelEventArgs> Tick;
        event EventHandler<TimerModelEventArgs> TimerReset;

        bool Complete { get; }
        void Start();
        void Stop();
        void Reset();
    }
}