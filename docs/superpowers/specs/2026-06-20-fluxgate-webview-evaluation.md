# FluxGate WebView Evaluation

## Requirement

Embed the approved lovable.dev free-node page inside FluxGate while keeping an external browser fallback.

Target URL:

```text
https://lovable.dev/preview/lZwTAW5Wyepb3fplbbhlJLUZpa7z6kCO
```

## Constraints

- Windows desktop first.
- Do not save page Cookie, Token, subscription links, or other sensitive data.
- Do not inject scripts.
- Keep "Open in browser" always available.
- Verify release package impact before enabling embedded WebView by default.

## Candidate

Package: `Avalonia.Controls.WebView`

Control: `NativeWebView`

Windows runtime dependency: WebView2

Reason to evaluate first: it is documented by Avalonia, uses platform-native engines, and avoids bundling a full Chromium runtime.

References:

- Avalonia documentation: https://docs.avaloniaui.net/docs/app-development/embedding-web-content
- NuGet package: https://www.nuget.org/packages/Avalonia.Controls.WebView/

## Checks Before Adoption

- Confirm package compatibility with the repository's Avalonia package version.
- Confirm Windows release package still builds.
- Confirm WebView2 runtime availability on target Windows machines.
- Confirm no cookies, tokens, subscription URLs, or other page data are read or persisted by FluxGate code.
- Confirm the external browser fallback remains available even if embedded loading fails.
- Confirm the embedded page does not interfere with system proxy, Core ports, tray state, or subscription handling.

## Preliminary Decision

Defer embedded WebView implementation until compatibility and package impact are checked in a separate branch or commit. Keep the external browser fallback in the first UI clarity implementation.
