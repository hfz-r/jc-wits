using System;
using ESD.JC_Infrastructure.Events;
using Prism.Events;
using Timer;

namespace ESD.JC_GoodsReceive.Services
{
    public class GRTimerServices : IGRTimerSevices
    {
        #region Properties

        private string timerValue;
        public string TimerValue
        {
            get
            {
                return timerValue;
            }
            set
            {
                if (timerValue != value)
                {
                    timerValue = value;
                }
            }
        }

        private double percentElapsed;
        public double PercentElapsed
        {
            get
            {
                return percentElapsed;
            }
            set
            {
                if (value != percentElapsed)
                {
                    percentElapsed = value;
                }
            }
        }

        #endregion Properties

        readonly ITimerModel _timer = TimerModelBuilder.GetNewTimer();
        private IEventAggregator eventAggregator;

        public GRTimerServices(IEventAggregator eventAggregator)
        {
            AddEventHandlers();

            this.eventAggregator = eventAggregator;
        }

        public void StopTimerExecute()
        {
            _timer.Reset();
        }

        public void StartTimerExecute()
        {
            _timer.Start();
        }

        private void AddEventHandlers()
        {
            _timer.Started += (sender, e) => OnStarted(sender, e);
            _timer.Tick += (sender, e) => OnTick(sender, e);
            _timer.Completed += (sender, e) => OnCompleted(sender, e);
            _timer.Stopped += (sender, e) => OnStopped(sender, e);
            _timer.TimerReset += (sender, e) => OnReset(sender, e);
        }

        private void OnStarted(object sender, TimerModelEventArgs e)
        {
            UpdateTimer(e);
        }

        private void OnTick(object sender, TimerModelEventArgs e)
        {
            UpdateTimer(e);

            eventAggregator.GetEvent<GR_ItemMessageEvent>().Publish(new GRItemMessage
            {
                State = e.State.ToString(),
                PercentageValue = PercentElapsed,
                TimerValue = TimerValue
            });
        }

        private void OnCompleted(object sender, TimerModelEventArgs e)
        {
            UpdateTimer(e);

            eventAggregator.GetEvent<GR_ItemMessageEvent>().Publish(new GRItemMessage
            {
                State = e.State.ToString(),
                PercentageValue = 100
            });
        }

        private void OnStopped(object sender, TimerModelEventArgs e)
        {
            UpdateTimer(e);

            eventAggregator.GetEvent<GR_ItemMessageEvent>().Publish(new GRItemMessage { State = e.State.ToString() });
        }

        private void OnReset(object sender, TimerModelEventArgs e)
        {
            UpdateTimer(e);

            eventAggregator.GetEvent<GR_ItemMessageEvent>().Publish(new GRItemMessage
            {
                State = e.State.ToString(),
                PercentageValue = 0
            });
        }

        private void UpdateTimer(TimerModelEventArgs e)
        {
            TimeSpan t = _timer.Remaining;
            TimerValue = string.Format("{0}:{1}:{2}", t.Hours.ToString("D2"), t.Minutes.ToString("D2"), t.Seconds.ToString("D2"));

            PercentElapsed = 100.0 - (100.0 * _timer.Remaining.TotalSeconds / _timer.Duration.TotalSeconds);
        }
    }

    public interface IGRTimerSevices
    {
        void StartTimerExecute();
        void StopTimerExecute();
    }
}
