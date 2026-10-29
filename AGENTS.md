# AGENTS.md

## Build Commands

```bash
# Build
dotnet build -c Release

# Package（快速打包；CI 用 Scripts/build.ps1 -SkipBuild -SkipVersion，命名只在 build.ps1 维护）
powershell -ExecutionPolicy Bypass -File Scripts/pack-zip.ps1
```

## Project Overview

QuickLook plugin for previewing SVGA (Space SVG Animation) files. Uses WebView2 + svga.min.js Web SDK for rendering.

## Key Files

- `Plugin.cs` - IViewer entry point, handles .svga file detection
- `SvgaViewerPanel.xaml.cs` - WebView2 panel, manages lifecycle and theme
- `Resources/Web/player.html` - Player UI (HTML/CSS/JS)
- `Resources/Web/svga.min.js` + `jszip.min.js` / `jszip-utils.min.js` - SVGA Web SDK

## Architecture

```Plugin.cs (IViewer)
  └── SvgaViewerPanel (WPF UserControl)
        └── WebView2
              └── player.html
                    └── svga.min.js (SVGA Web SDK)
```

## CI / Release

- push `master` / PR / 手动：构建 + 打包，上传 Artifact（14 天），**不**发 Release
- tag `v*`：构建打包后自动创建 GitHub Release，资产为 `QuickLook.Plugin.SvgaViewer.qlplugin`
- 打包命名唯一维护在 `Scripts/build.ps1`；workflow 调用它，**禁止**在 workflow 内联第二份打包逻辑
- 发版只需 `git tag vX.Y.Z && git push --tags`，无需手动上传

## Conventions

- Target framework: .NET Framework 4.6.2
- Namespace: `QuickLook.Plugin.SvgaViewer`
- Follow official QuickLook plugin structure and quicklook-plugin-ship SKILL.md (see VideoViewer, ImageViewer)
- XAML files at project root (not in subdirectories)
- Web assets in `Resources/Web/`
- Native P/Invoke stubs in `Native/`
- `.ps1` files must stay UTF-8 with BOM (PowerShell 5.1 Chinese comments)
