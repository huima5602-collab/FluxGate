# FluxGate

FluxGate (流门) is a Windows proxy client UI fork based on `v2rayN 7.10.3`.

The first development stage keeps the v2rayN core, service layer, protocol handling, subscription handling, routing logic, and Avalonia desktop architecture intact. The current work focuses on importing the baseline, replacing visible product branding, and preparing the project for later UI redesign.

## Current Baseline

- Upstream baseline: `2dust/v2rayN` tag `7.10.3`
- Desktop stack: .NET 8, Avalonia, ReactiveUI
- Main solution: `v2rayN/v2rayN.sln`
- Main desktop project: `v2rayN/v2rayN.Desktop/v2rayN.Desktop.csproj`
- Core/service project: `v2rayN/ServiceLib/ServiceLib.csproj`
- Global hotkey dependency: `2dust/GlobalHotKeys` commit `b3b635e`

## Download and Run

For normal Windows users, use the GitHub Releases package instead of building from source.

1. Open the Releases page: https://github.com/huima5602-collab/FluxGate/releases
2. Download `FluxGate-windows-64-desktop.zip`.
3. Extract the zip file to a normal folder, for example `D:\Apps\FluxGate`.
4. Run `FluxGate.exe`.

Use the `windows-64` desktop package for most Windows computers. The `windows-arm64` package is only for ARM64 Windows devices.

## Development Commands

```powershell
dotnet restore .\v2rayN\v2rayN.sln
dotnet build .\v2rayN\v2rayN.sln -c Debug
dotnet run --project .\v2rayN\v2rayN.Desktop\v2rayN.Desktop.csproj
```

The repository includes `global.json` to pin local and CI builds to .NET SDK `8.0.422`.

## Current Compatibility Notes

TUIC imports default `allowInsecure` to enabled when the share link does not specify it. Runtime sing-box config generation also treats existing TUIC records with empty `allowInsecure` as enabled. This keeps compatibility with common TUIC nodes that use self-signed or otherwise untrusted certificates.

FluxGate starts sing-box with `ENABLE_DEPRECATED_LEGACY_DNS_SERVERS=true` for compatibility with the v2rayN 7.10.3 DNS configuration format when a newer sing-box core is present.

Release packaging uses the official `2dust/v2rayN` `7.10.3` release assets only as the Core source. The FluxGate package root is built from the FluxGate desktop publish output, then the official `bin/` Core directory is copied in. The root package must not include `v2rayN.exe`.

The normal update dialog does not offer upstream v2rayN application updates. Keep application updates on FluxGate releases; Core updates remain available.

Browser performance validation should use a real browser with the same node and proxy mode. For the v0.1.0-alpha.8 package cleanup, the generated Xray config matched the original v2rayN 7.10.3 config except for the temporary local test port.

## Branding

- Product name: `FluxGate`
- Chinese display name: `流门`
- Icon direction: dark shield, blue routing path, green active node
- User-visible application title and output assembly name are set to `FluxGate`

## Development Notes

- Do not rewrite proxy core logic in the first stage.
- Keep risky namespace and internal type renames for a later controlled refactor.
- Ordinary user UI should not show license or source-code notices.
- Before public distribution, handle license and source-release obligations separately in release materials.

## UI Preview Branch

The `codex/ui-redesign` branch contains the first FluxGate UI preview. It keeps the validated proxy behavior from `v0.1.0-alpha.8` and focuses on the Avalonia shell: colorful dark theme, brand header, simplified home dashboard, refreshed node toolbar, and retained advanced menus.

The `v0.2.0-ui-preview.2` preview extends that direction into a 1920-style Minimal Command layout. The main Avalonia shell now presents six left-side pages: home, nodes, subscriptions, routing, settings, and logs. Subscription, routing, settings, and maintenance actions continue to call the existing v2rayN/FluxGate commands and dialogs instead of replacing the underlying business logic.

The top bar includes a `免费节点` entry that opens `https://lovable.dev/preview/lZwTAW5Wyepb3fplbbhlJLUZpa7z6kCO` in the system browser. The page is external and is not embedded in FluxGate.

When validating the desktop UI, do not run FluxGate at the same time as an existing v2rayN client on the same Windows profile. They can compete for system proxy state, Core ports, and tray behavior; pause v2rayN before a full manual FluxGate run.
