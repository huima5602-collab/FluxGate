using Avalonia.Controls;
using Avalonia.Interactivity;
using v2rayN.Desktop.Common;
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
            btnReloadFreeNodes.Click += ReloadFreeNodes_Click;
            btnCopyFreeNodesUrl.Click += CopyFreeNodesUrl_Click;
        }

        private void OpenFreeNodes_Click(object? sender, RoutedEventArgs e)
        {
            ProcUtils.ProcessStart(FreeNodesUrl);
        }

        private void ReloadFreeNodes_Click(object? sender, RoutedEventArgs e)
        {
            webFreeNodes.Source = new Uri(FreeNodesUrl);
        }

        private async void CopyFreeNodesUrl_Click(object? sender, RoutedEventArgs e)
        {
            await AvaUtils.SetClipboardData(this, FreeNodesUrl);
        }
    }
}
