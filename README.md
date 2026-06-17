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

## Development Commands

```powershell
dotnet restore .\v2rayN\v2rayN.sln
dotnet build .\v2rayN\v2rayN.sln -c Debug
dotnet run --project .\v2rayN\v2rayN.Desktop\v2rayN.Desktop.csproj
```

## Current Known Issue

This machine currently has .NET runtimes but no .NET SDK installed, so `dotnet build` cannot run here yet. Install the .NET 8 SDK before building.

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
