using Prism.Commands;

namespace ESD.JC_Infrastructure.Controls
{
    public interface ICompositeCommands
    {
        CompositeCommand ImportFGCommand { get; }
        CompositeCommand ExportFGCommand { get; }
        CompositeCommand PrintLblCommand { get; }
        CompositeCommand OKCommand { get; }
        CompositeCommand XOKCommand { get; }
    }

    public class CompositeCommands : ICompositeCommands
    {
        private CompositeCommand _importFGCommand = new CompositeCommand(true);
        public CompositeCommand ImportFGCommand
        {
            get { return _importFGCommand; }
        }

        private CompositeCommand _exportFGCommand = new CompositeCommand(true);
        public CompositeCommand ExportFGCommand
        {
            get { return _exportFGCommand; }
        }

        private CompositeCommand _printLblCommand = new CompositeCommand(true);
        public CompositeCommand PrintLblCommand
        {
            get { return _printLblCommand; }
        }

        private CompositeCommand _okCommand = new CompositeCommand(true);
        public CompositeCommand OKCommand
        {
            get { return _okCommand; }
        }

        private CompositeCommand _xokCommand = new CompositeCommand(true);
        public CompositeCommand XOKCommand
        {
            get { return _xokCommand; }
        }
    }
}
