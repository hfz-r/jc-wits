using ESD.JC_Infrastructure.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ESD.JC_Infrastructure.Controls
{
    public class ProgressDialogHelper
    {
        public ProgressDialog pgdialog;
        public Dispatcher _Dispatcher;

        public IEventAggregator EventAggregator;

        public ProgressDialogHelper(IEventAggregator EventAggregator)
        {
            this.EventAggregator = EventAggregator;

            pgdialog = new ProgressDialog();
            _Dispatcher = pgdialog.Dispatcher;
        }

        public void Initialize()
        {
            for (int x = 1; x < 100; x++)
            {
                Thread.Sleep(10);

                UpdateProgressDelegate update = new UpdateProgressDelegate(UpdateProgressText);
                _Dispatcher.BeginInvoke(update, x / 10);
            }
        }

        public void SetDialogDescription(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                EventAggregator.GetEvent<ProgressDialogTextEvent>().Publish(input);
            }
        }

        public delegate void UpdateProgressDelegate(int percentage);

        public void UpdateProgressText(int percentage)
        {
            pgdialog.ProgressValue = percentage;
        }
    }
}
