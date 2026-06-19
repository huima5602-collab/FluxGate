using Avalonia.Controls;
using Avalonia.Interactivity;
using ServiceLib.Common;

namespace v2rayN.Desktop.Views
{
    public partial class FreeNodesView : UserControl
    {
        private const string FreeNodesUrl = "https://lovable.dev/preview/lZwTAW5Wyepb3fplbbhlJLUZpa7z6kCO";

        public FreeNodesView()
        {
            InitializeComponent();
            btnOpenFreeNodes.Click += OpenFreeNodes_Click;
        }

        private void OpenFreeNodes_Click(object? sender, RoutedEventArgs e)
        {
            ProcUtils.ProcessStart(FreeNodesUrl);
        }
    }
}
