using System;
using ESD.JC_Infrastructure.Events;
using Prism.Events;
using Prism.Mvvm;

namespace ESD.JC_Infrastructure.Controls
{
    public class ProgressDialogViewModel : BindableBase
    {
        private string pdDescText;
        public string PDdescText
        {
            get { return pdDescText; }
            set
            {
                SetProperty(ref pdDescText, value);
                RaisePropertyChanged("PDdescText");
            }
        }

        private IEventAggregator EventAggregator;

        public ProgressDialogViewModel(IEventAggregator EventAggregator)
        {
            this.EventAggregator = EventAggregator;
            this.EventAggregator.GetEvent<ProgressDialogTextEvent>().Subscribe(InitPDDescText);

        }

        private void InitPDDescText(string obj)
        {
            PDdescText = obj;
        }
    }
}
