using System;
using System.Windows.Threading;

namespace Timer
{
    internal class TimerModel : ITimerModel
    {
        readonly DispatcherTimer timer = new DispatcherTimer();

        public TimerModel() : this(new TimeSpan(0, 0, 0, 10))
        {
        }

        public TimerModel(TimeSpan duration)
        {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (sender, e) => OnDispatcherTimerTick();

            Duration = duration;
            Reset();
        }

        #region Properties

        public TimeSpan Interval
        {
            get
            {
                return timer.Interval;
            }
        }

        public TimeSpan Remaining { get; private set; }

        public bool Complete
        {
            get
            {
                return Remaining <= TimeSpan.Zero;
            }
        }

        private TimeSpan duration;
        public TimeSpan Duration
        {
            get
            {
                return duration;
            }
            set
            {
                if (duration != value)
                {
                    duration = value;
                }
            }
        }

        #endregion

        public void Start()
        {
            timer.Start();
            OnStarted();
        }

        public void Stop()
        {
            timer.Stop();
            OnStopped();
        }

        public void Reset()
        {
            Stop();
            Remaining = Duration;
            OnReset();
        }

        #region Event Handlers

        private void OnDispatcherTimerTick()
        {
            Remaining = Remaining - Interval;
            OnTick();
            if (Complete)
            {
                Stop();
                Remaining = TimeSpan.Zero;
                OnCompleted();
            }
        }

        #endregion

        #region Events

        public event EventHandler<TimerModelEventArgs> Tick;
        public event EventHandler<TimerModelEventArgs> Started;
        public event EventHandler<TimerModelEventArgs> Stopped;
        public event EventHandler<TimerModelEventArgs> Completed;
        public event EventHandler<TimerModelEventArgs> TimerReset;

        private void OnTick()
        {
            Tick?.Invoke(this, new TimerModelEventArgs(Duration, Remaining, TimerModelEventArgs.Status.Running));
        }

        private void OnStarted()
        {
            Started?.Invoke(this, new TimerModelEventArgs(Duration, Remaining, TimerModelEventArgs.Status.Started));
        }

        private void OnStopped()
        {
            Stopped?.Invoke(this, new TimerModelEventArgs(Duration, Remaining, TimerModelEventArgs.Status.Stopped));
        }

        private void OnCompleted()
        {
            Completed?.Invoke(this, new TimerModelEventArgs(Duration, Remaining, TimerModelEventArgs.Status.Completed));
        }

        private void OnReset()
        {
            TimerReset?.Invoke(this, new TimerModelEventArgs(Duration, Remaining, TimerModelEventArgs.Status.Reset));
        }

        #endregion
    }
}
