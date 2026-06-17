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

- 当前机器只有 .NET Runtime，没有 .NET SDK，`dotnet build` 会失败。需要安装 .NET 8 SDK 后再构建。
- 当前只完成首轮品牌替换，完整 UI 重构尚未开始。
