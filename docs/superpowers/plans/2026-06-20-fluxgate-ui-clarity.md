# FluxGate UI Clarity Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Turn the approved FluxGate UI clarity design into a staged implementation: a clearer home dashboard, visible proxy port/TUN/system proxy state, a beginner guide page, and a separately evaluated free-node WebView center.

**Architecture:** Reuse existing v2rayN/FluxGate service and ViewModel behavior instead of creating a new state system. The first implementation pass should stay in the Avalonia desktop shell and `StatusBarViewModel`, then add isolated guide/free-node views so `MainWindow.axaml` does not continue growing unchecked.

**Tech Stack:** .NET 8, Avalonia, ReactiveUI, existing `ServiceLib` models/handlers, existing FluxGate styles.

---

## File Structure

- Modify: `v2rayN/ServiceLib/ViewModels/StatusBarViewModel.cs`
  - Owns home status text for system proxy, TUN mode, local proxy port, and copy-local-proxy command.
- Modify: `v2rayN/v2rayN.Desktop/Views/StatusBarView.axaml`
  - Renders the home dashboard status cards and proxy port copy action.
- Modify: `v2rayN/v2rayN.Desktop/Views/StatusBarView.axaml.cs`
  - Binds any new command or control that cannot be handled by compiled bindings in XAML.
- Create: `v2rayN/v2rayN.Desktop/Views/GuideView.axaml`
  - Static beginner guide page with basic operations, terms, and scenario recommendations.
- Create: `v2rayN/v2rayN.Desktop/Views/GuideView.axaml.cs`
  - Minimal code-behind for the guide user control.
- Create: `v2rayN/v2rayN.Desktop/Views/FreeNodesView.axaml`
  - Free-node center shell with explanatory text and external-browser fallback.
- Create: `v2rayN/v2rayN.Desktop/Views/FreeNodesView.axaml.cs`
  - Opens the approved lovable.dev URL in the system browser. WebView integration stays behind a later evaluation task.
- Modify: `v2rayN/v2rayN.Desktop/Views/MainWindow.axaml`
  - Adds navigation entries and content hosts for Free Nodes and Guide.
- Modify: `v2rayN/v2rayN.Desktop/Views/MainWindow.axaml.cs`
  - Wires navigation and initializes the new views.
- Modify: `v2rayN/v2rayN.Desktop/Assets/GlobalStyles.axaml`
  - Adds only reusable, small styles needed by the new guide/free-node pages.

## Task 1: Home Status Data

**Files:**
- Modify: `v2rayN/ServiceLib/ViewModels/StatusBarViewModel.cs`

- [ ] **Step 1: Add reactive home status properties and copy command**

Add properties near the existing `InboundDisplay`, `InboundLanDisplay`, and `EnableTun` UI properties:

```csharp
[Reactive]
public string SystemProxyStatusDisplay { get; set; }

[Reactive]
public string TunStatusDisplay { get; set; }

[Reactive]
public string LocalProxyAddressDisplay { get; set; }

[Reactive]
public string LocalProxyAddressCopyText { get; set; }

public ReactiveCommand<Unit, Unit> CopyLocalProxyAddressCmd { get; }
```

Initialize the strings in the constructor after `RunningServerToolTipText = "-";`:

```csharp
SystemProxyStatusDisplay = "-";
TunStatusDisplay = "-";
LocalProxyAddressDisplay = "-";
LocalProxyAddressCopyText = string.Empty;
```

Create the command after `CopyProxyCmdToClipboardCmd`:

```csharp
CopyLocalProxyAddressCmd = ReactiveCommand.CreateFromTask(async () =>
{
    if (LocalProxyAddressCopyText.IsNotEmpty())
    {
        await _updateView?.Invoke(EViewAction.SetClipboardData, LocalProxyAddressCopyText);
        NoticeHandler.Instance.SendMessageAndEnqueue(LocalProxyAddressCopyText);
    }
});
```

- [ ] **Step 2: Add a small status refresh helper**

Add this method inside the `#region UI` block before `InboundDisplayStatus()`:

```csharp
private void RefreshHomeStatusDisplay()
{
    SystemProxyStatusDisplay = _config.SystemProxyItem.SysProxyType switch
    {
        ESysProxyType.ForcedClear => ResUI.menuSystemProxyClear,
        ESysProxyType.ForcedChange => ResUI.menuSystemProxySet,
        ESysProxyType.Unchanged => ResUI.menuSystemProxyNothing,
        ESysProxyType.Pac => ResUI.menuSystemProxyPac,
        _ => _config.SystemProxyItem.SysProxyType.ToString()
    };

    TunStatusDisplay = EnableTun ? ResUI.TbSettingsTunMode : Global.None;

    var localPort = AppHandler.Instance.GetLocalPort(EInboundProtocol.socks);
    LocalProxyAddressCopyText = $"{Global.Loopback}:{localPort}";
    LocalProxyAddressDisplay = $"{EInboundProtocol.mixed}:{LocalProxyAddressCopyText}";
}
```

- [ ] **Step 3: Call the helper from state-changing methods**

At the end of `ChangeSystemProxyAsync`, before the closing brace, add:

```csharp
RefreshHomeStatusDisplay();
```

At the end of `InboundDisplayStatus`, before `await Task.CompletedTask;`, add:

```csharp
RefreshHomeStatusDisplay();
```

After `await ConfigHandler.SaveConfig(_config);` in `DoEnableTun`, add:

```csharp
RefreshHomeStatusDisplay();
```

- [ ] **Step 4: Build to catch C# errors**

Run:

```powershell
dotnet build .\v2rayN\v2rayN.sln -c Debug
```

Expected: build succeeds.

- [ ] **Step 5: Commit**

```powershell
git add v2rayN\ServiceLib\ViewModels\StatusBarViewModel.cs
git commit -m "feat: expose FluxGate home status data"
```

## Task 2: Home Dashboard Presentation

**Files:**
- Modify: `v2rayN/v2rayN.Desktop/Views/StatusBarView.axaml`
- Modify: `v2rayN/v2rayN.Desktop/Views/StatusBarView.axaml.cs`

- [ ] **Step 1: Update the four top metric cards**

In `StatusBarView.axaml`, replace the static metric card texts for system proxy, TUN, local port, and Core with bindings:

```xml
<TextBlock Classes="FluxTinyLabel" Text="系统代理" />
<TextBlock Classes="FluxMetricText" Text="{Binding SystemProxyStatusDisplay}" />
```

```xml
<TextBlock Classes="FluxTinyLabel" Text="TUN 模式" />
<TextBlock Classes="FluxMetricText" Text="{Binding TunStatusDisplay}" />
```

```xml
<TextBlock Classes="FluxTinyLabel" Text="本机代理端口" />
<TextBlock
    Classes="FluxMetricText"
    Text="{Binding LocalProxyAddressDisplay}"
    TextWrapping="Wrap" />
```

```xml
<TextBlock Classes="FluxTinyLabel" Text="Core" />
<TextBlock Classes="FluxMetricText" Text="核心托管" />
```

- [ ] **Step 2: Add a copy button near the local port**

Inside the local port card `StackPanel`, below the bound `TextBlock`, add:

```xml
<Button
    x:Name="btnCopyLocalProxy"
    Classes="FluxMiniActionButton"
    Content="复制地址" />
```

- [ ] **Step 3: Bind the copy command**

In `StatusBarView.axaml.cs`, inside the existing `WhenActivated` block, add:

```csharp
this.BindCommand(ViewModel, vm => vm.CopyLocalProxyAddressCmd, v => v.btnCopyLocalProxy).DisposeWith(disposables);
```

- [ ] **Step 4: Build to verify XAML names and bindings**

Run:

```powershell
dotnet build .\v2rayN\v2rayN.sln -c Debug
```

Expected: build succeeds.

- [ ] **Step 5: Commit**

```powershell
git add v2rayN\v2rayN.Desktop\Views\StatusBarView.axaml v2rayN\v2rayN.Desktop\Views\StatusBarView.axaml.cs
git commit -m "feat: show proxy status on FluxGate home"
```

## Task 3: Beginner Guide View

**Files:**
- Create: `v2rayN/v2rayN.Desktop/Views/GuideView.axaml`
- Create: `v2rayN/v2rayN.Desktop/Views/GuideView.axaml.cs`

- [ ] **Step 1: Create the guide XAML**

Create `GuideView.axaml`:

```xml
<UserControl
    x:Class="v2rayN.Desktop.Views.GuideView"
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <ScrollViewer>
        <StackPanel Spacing="18">
            <StackPanel Spacing="8">
                <TextBlock Classes="FluxPageTitle" Text="新手指南" />
                <TextBlock Classes="FluxHeroText" Text="用最少步骤理解 FluxGate 的基本操作、常见名词和排障顺序。" />
            </StackPanel>

            <Border Classes="FluxPanel FluxNodePanel">
                <StackPanel Spacing="10">
                    <TextBlock Classes="FluxSectionTitle" Text="第一次使用" />
                    <TextBlock Classes="FluxHeroText" Text="1. 添加订阅或导入节点&#x0a;2. 刷新订阅&#x0a;3. 选择一个节点&#x0a;4. 开启系统代理&#x0a;5. 打开浏览器测试网页" />
                </StackPanel>
            </Border>

            <Border Classes="FluxPanel FluxNodePanel">
                <StackPanel Spacing="10">
                    <TextBlock Classes="FluxSectionTitle" Text="什么时候用哪种模式" />
                    <TextBlock Classes="FluxHeroText" Text="只是上网页：系统代理 + 自动/规则模式。&#x0a;某个软件不能用：先尝试手动填写首页代理端口。&#x0a;软件不支持系统代理和手动代理：再考虑 TUN 模式。&#x0a;所有网页打不开：先换节点、刷新订阅、看日志，不要先乱改高级设置。" />
                </StackPanel>
            </Border>

            <Border Classes="FluxPanel FluxNodePanel">
                <StackPanel Spacing="10">
                    <TextBlock Classes="FluxSectionTitle" Text="常见名词" />
                    <TextBlock Classes="FluxHeroText" Text="节点：一条代理线路。&#x0a;订阅：一组在线节点列表。&#x0a;系统代理：让支持系统代理的软件走 FluxGate。&#x0a;代理端口：给其他软件手动填写的本机入口，例如 127.0.0.1:10808。&#x0a;TUN 模式：更底层的接管方式，适合不支持系统代理的软件。&#x0a;Core：真正负责代理连接的底层程序。&#x0a;日志：排查连接、订阅和 Core 错误时使用。" />
                </StackPanel>
            </Border>
        </StackPanel>
    </ScrollViewer>
</UserControl>
```

- [ ] **Step 2: Create code-behind**

Create `GuideView.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace v2rayN.Desktop.Views
{
    public partial class GuideView : UserControl
    {
        public GuideView()
        {
            InitializeComponent();
        }
    }
}
```

- [ ] **Step 3: Build to verify the new user control**

Run:

```powershell
dotnet build .\v2rayN\v2rayN.sln -c Debug
```

Expected: build succeeds.

- [ ] **Step 4: Commit**

```powershell
git add v2rayN\v2rayN.Desktop\Views\GuideView.axaml v2rayN\v2rayN.Desktop\Views\GuideView.axaml.cs
git commit -m "feat: add FluxGate beginner guide view"
```

## Task 4: Free Nodes View Shell

**Files:**
- Create: `v2rayN/v2rayN.Desktop/Views/FreeNodesView.axaml`
- Create: `v2rayN/v2rayN.Desktop/Views/FreeNodesView.axaml.cs`

- [ ] **Step 1: Create the free nodes shell XAML**

Create `FreeNodesView.axaml`:

```xml
<UserControl
    x:Class="v2rayN.Desktop.Views.FreeNodesView"
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Grid RowDefinitions="Auto,*">
        <Grid ColumnDefinitions="*,Auto">
            <StackPanel Spacing="8">
                <TextBlock Classes="FluxPageTitle" Text="免费节点" />
                <TextBlock Classes="FluxHeroText" Text="这里是免费节点中心。内嵌 WebView 需要单独验证依赖和打包影响；当前先保留可靠的浏览器打开入口。" />
            </StackPanel>
            <Button
                x:Name="btnOpenFreeNodes"
                Grid.Column="1"
                Classes="FluxToolButton Green"
                Content="在浏览器打开" />
        </Grid>

        <Border Grid.Row="1" Margin="0,20,0,0" Classes="FluxPanel FluxNodePanel">
            <StackPanel Spacing="12">
                <TextBlock Classes="FluxSectionTitle" Text="WebView 集成待评估" />
                <TextBlock Classes="FluxHeroText" Text="目标页面：https://lovable.dev/preview/lZwTAW5Wyepb3fplbbhlJLUZpa7z6kCO" />
                <TextBlock Classes="FluxHeroText" Text="要求：不保存网页 Cookie，不注入脚本，不读取网页数据；加载失败时必须保留外部浏览器兜底。" />
            </StackPanel>
        </Border>
    </Grid>
</UserControl>
```

- [ ] **Step 2: Create code-behind with external browser fallback**

Create `FreeNodesView.axaml.cs`:

```csharp
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
```

- [ ] **Step 3: Build**

Run:

```powershell
dotnet build .\v2rayN\v2rayN.sln -c Debug
```

Expected: build succeeds.

- [ ] **Step 4: Commit**

```powershell
git add v2rayN\v2rayN.Desktop\Views\FreeNodesView.axaml v2rayN\v2rayN.Desktop\Views\FreeNodesView.axaml.cs
git commit -m "feat: add FluxGate free nodes view shell"
```

## Task 5: Main Navigation Integration

**Files:**
- Modify: `v2rayN/v2rayN.Desktop/Views/MainWindow.axaml`
- Modify: `v2rayN/v2rayN.Desktop/Views/MainWindow.axaml.cs`

- [ ] **Step 1: Add nav buttons**

In `MainWindow.axaml`, add two buttons after `btnNavSubs`:

```xml
<Button
    x:Name="btnNavFreeNodes"
    Classes="FluxNavButton"
    Content="免费" />
<Button
    x:Name="btnNavGuide"
    Classes="FluxNavButton"
    Content="指南" />
```

- [ ] **Step 2: Add content hosts**

In the main content grid, add these borders near the other page borders:

```xml
<Border
    x:Name="pageFreeNodes"
    Classes="FluxPanel FluxWorkspace"
    IsVisible="False">
    <ContentControl x:Name="conFreeNodes" />
</Border>

<Border
    x:Name="pageGuide"
    Classes="FluxPanel FluxWorkspace"
    IsVisible="False">
    <ContentControl x:Name="conGuide" />
</Border>
```

- [ ] **Step 3: Initialize and wire navigation**

In `MainWindow.axaml.cs`, after existing nav click handlers, add:

```csharp
btnNavFreeNodes.Click += (_, _) => SelectFluxPage(3);
btnNavGuide.Click += (_, _) => SelectFluxPage(7);
```

Update the existing page indices so routing/settings/logs shift to 4/5/6:

```csharp
btnNavRoutes.Click += (_, _) => SelectFluxPage(4);
btnNavSettings.Click += (_, _) => SelectFluxPage(5);
btnNavLogs.Click += (_, _) => SelectFluxPage(6);
```

After existing content initialization, add:

```csharp
conFreeNodes.Content ??= new FreeNodesView();
conGuide.Content ??= new GuideView();
```

- [ ] **Step 4: Update SelectFluxPage**

Update the valid range and visibility logic:

```csharp
if (index < 0 || index > 7)
{
    index = 0;
}

pageHome.IsVisible = index == 0;
pageNodes.IsVisible = index == 1;
pageSubscription.IsVisible = index == 2;
pageFreeNodes.IsVisible = index == 3;
pageRouting.IsVisible = index == 4;
pageSettings.IsVisible = index == 5;
pageLogs.IsVisible = index == 6;
pageGuide.IsVisible = index == 7;

SetNavActive(btnNavHome, index == 0);
SetNavActive(btnNavNodes, index == 1);
SetNavActive(btnNavSubs, index == 2);
SetNavActive(btnNavFreeNodes, index == 3);
SetNavActive(btnNavRoutes, index == 4);
SetNavActive(btnNavSettings, index == 5);
SetNavActive(btnNavLogs, index == 6);
SetNavActive(btnNavGuide, index == 7);
```

- [ ] **Step 5: Build**

Run:

```powershell
dotnet build .\v2rayN\v2rayN.sln -c Debug
```

Expected: build succeeds.

- [ ] **Step 6: Commit**

```powershell
git add v2rayN\v2rayN.Desktop\Views\MainWindow.axaml v2rayN\v2rayN.Desktop\Views\MainWindow.axaml.cs
git commit -m "feat: add FluxGate guide and free node navigation"
```

## Task 6: WebView Evaluation Spike

**Files:**
- Create: `docs/superpowers/specs/2026-06-20-fluxgate-webview-evaluation.md`

- [ ] **Step 1: Verify the official Avalonia WebView candidate**

Use the official Avalonia WebView documentation and NuGet package as the first candidate:

```text
Candidate package: Avalonia.Controls.WebView
Primary control: NativeWebView
Windows engine: WebView2
Official docs: https://docs.avaloniaui.net/docs/app-development/embedding-web-content
NuGet: https://www.nuget.org/packages/Avalonia.Controls.WebView/
```

Expected: confirm whether the package version is compatible with the repository's Avalonia version before adding it to the app.

- [ ] **Step 2: Create the evaluation document**

Create `docs/superpowers/specs/2026-06-20-fluxgate-webview-evaluation.md` with this content if no package has been installed yet:

```markdown
# FluxGate WebView Evaluation

## Requirement

Embed the approved lovable.dev free-node page inside FluxGate while keeping an external browser fallback.

## Constraints

- Windows desktop first.
- Do not save page Cookie, Token, subscription links, or other sensitive data.
- Do not inject scripts.
- Keep "Open in browser" always available.
- Verify release package impact before enabling by default.

## Candidate

Package: `Avalonia.Controls.WebView`
Control: `NativeWebView`
Windows runtime dependency: WebView2
Reason to evaluate first: it is documented by Avalonia, uses platform-native engines, and avoids bundling a full Chromium runtime.

## Checks Before Adoption

- Confirm package compatibility with the repository's Avalonia package version.
- Confirm Windows release package still builds.
- Confirm WebView2 runtime availability on target Windows machines.
- Confirm no cookies, tokens, subscription URLs, or other page data are read or persisted by FluxGate code.
- Confirm the external browser fallback remains available even if embedded loading fails.

## Preliminary Decision

Defer implementation until compatibility and package impact are checked in a separate branch or commit. Keep the external browser fallback in the first UI clarity implementation.
```

- [ ] **Step 3: Commit**

```powershell
git add docs/superpowers/specs/2026-06-20-fluxgate-webview-evaluation.md
git commit -m "docs: evaluate FluxGate WebView integration"
```

## Task 7: Final Verification

**Files:**
- No new files unless fixes are required.

- [ ] **Step 1: Build**

Run:

```powershell
dotnet build .\v2rayN\v2rayN.sln -c Debug
```

Expected: build succeeds.

- [ ] **Step 2: Manual UI check with user approval**

Do not start FluxGate while the user's existing v2rayN is running. Ask the user to confirm they can pause v2rayN first.

Run only after confirmation:

```powershell
dotnet run --project .\v2rayN\v2rayN.Desktop\v2rayN.Desktop.csproj
```

Expected:

- Home shows current node, system proxy status, TUN status, and proxy port.
- Copy proxy address button places `127.0.0.1:<port>` on the clipboard.
- Nodes, subscriptions, free nodes, routing, settings, logs, and guide pages open.
- Existing dialogs still open from their original commands.
- Free nodes external browser button opens the approved lovable.dev URL.

- [ ] **Step 3: Commit any verification fixes**

If fixes were needed:

```powershell
git add <fixed-files>
git commit -m "fix: stabilize FluxGate UI clarity preview"
```

If no fixes were needed, do not create an empty commit.
