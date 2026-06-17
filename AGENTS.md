# AGENTS.md

## 项目目标

FluxGate（流门）是基于 `v2rayN 7.10.3` 的 Windows 代理客户端二次开发项目。第一阶段目标是保留 v2rayN 核心能力，只做品牌替换、资源整理和后续 UI 改造准备。

## 技术栈

- .NET 8
- Avalonia
- ReactiveUI
- Windows x64 桌面应用
- 上游基线：`2dust/v2rayN` tag `7.10.3`
- 热键依赖：`2dust/GlobalHotKeys` commit `b3b635e`

## 目录结构

- `v2rayN/v2rayN.sln`：主解决方案
- `v2rayN/v2rayN.Desktop/`：Avalonia 桌面端
- `v2rayN/ServiceLib/`：核心服务、配置、订阅、路由、代理控制逻辑
- `v2rayN/AmazTool/`：辅助工具库
- `v2rayN/GlobalHotKeys/`：全局热键依赖源码

## 启动命令

```powershell
dotnet run --project .\v2rayN\v2rayN.Desktop\v2rayN.Desktop.csproj
```

## 构建命令

```powershell
dotnet restore .\v2rayN\v2rayN.sln
dotnet build .\v2rayN\v2rayN.sln -c Debug
```

## 测试命令

当前仓库尚未整理独立测试入口。第一阶段先使用以下验证：

```powershell
dotnet build .\v2rayN\v2rayN.sln -c Debug
```

## Git 规则

- 不要在用户未明确要求时推送远程仓库。
- 修改前先确认工作区状态。
- 优先最小必要修改，避免无关重构。
- 提交后向用户报告短 commit id、完整 commit hash 和提交信息。

## 路径规则

- 实体项目目录：`E:\codex Documents\Projects\V2rayN二次开发`
- C 盘兼容入口：`C:\Users\23886\Documents\V2rayN二次开发`
- C 盘路径必须是指向 E 盘实体目录的 NTFS Junction。

## 注意事项

- 第一阶段不重写代理核心、不新增协议、不大规模改命名空间。
- 普通用户界面不展示开源协议说明。
- 如果未来公开分发，需要在发布材料中单独处理许可证和源代码发布要求。
- 不要记录或输出订阅链接、Token、Cookie、密钥等敏感信息。

## 已知问题

- 当前仓库通过 `global.json` 固定使用 .NET SDK `8.0.422` 构建。
- sing-box 启动时会注入 `ENABLE_DEPRECATED_LEGACY_DNS_SERVERS=true`，用于兼容 v2rayN 7.10.3 的 DNS 配置格式。
- Release 包以 FluxGate 发布输出为根目录，只从官方 v2rayN 7.10.3 包复制 `bin/` Core 目录，根目录禁止包含 `v2rayN.exe`。
- 普通更新界面不提供上游 v2rayN 应用本体更新，只保留 Core 更新能力。
- 性能验收以真实浏览器打开 Google/YouTube 等网页的体感和加载耗时为准，不以应用内测速数字为准。
- 当前只完成首轮品牌替换，完整 UI 重构尚未开始。
