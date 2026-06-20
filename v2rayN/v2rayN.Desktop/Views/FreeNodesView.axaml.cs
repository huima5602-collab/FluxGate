using Avalonia.Controls;
using Avalonia.Interactivity;
using v2rayN.Desktop.Common;
using ServiceLib.Common;

namespace v2rayN.Desktop.Views
{
    public partial class FreeNodesView : UserControl
    {
        private const string FreeNodesUrl = "https://node-nations-feed.lovable.app/";

        public FreeNodesView()
        {
            InitializeComponent();
            btnLoadFreeNodes.Click += LoadFreeNodes_Click;
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
            LoadFreeNodes();
        }

        private async void CopyFreeNodesUrl_Click(object? sender, RoutedEventArgs e)
        {
            await AvaUtils.SetClipboardData(this, FreeNodesUrl);
        }

        private void LoadFreeNodes_Click(object? sender, RoutedEventArgs e)
        {
            LoadFreeNodes();
        }

        private void LoadFreeNodes()
        {
            webFreeNodesPlaceholder.IsVisible = false;
            webFreeNodes.Source = new Uri(FreeNodesUrl);
        }
    }
}
