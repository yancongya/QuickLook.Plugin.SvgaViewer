# QuickLook.Plugin.SvgaViewer

[English](#english) | 中文

---

## 中文

### 简介

一个用于预览 SVGA（SVGA Animation）动画文件的 [QuickLook](https://github.com/QL-Win/QuickLook) 插件。

**修复官方 ImageViewer 的 SVGA 1.x 空白预览问题**，完整支持 1.x（ZIP 格式）和 2.x（zlib 格式）。

该修复已于 2026-09-01 通过 [PR #1994](https://github.com/QL-Win/QuickLook/pull/1994) 合并进官方 QuickLook 仓库，但**尚未随正式版发布**（最新正式版 4.5.0 发布于 2026-04-14）。在此之上，本插件还提供播放控制、背景切换、文件信息面板、导出 PNG 等预览体验。

🌐 在线预览（GitHub Pages）：https://yancongya.github.io/QuickLook.Plugin.SvgaViewer/

### 功能特性

- 实时 SVGA 动画播放（自动循环）
- **完整支持 SVGA 1.x 和 2.x 格式**
- 播放/暂停、进度条拖拽
- 音量控制（按钮、滑块、滚轮调节）
- 背景切换（默认/棋盘格/白色/黑色/自定义颜色）
- 文件信息面板（悬停显示尺寸、帧率、精灵数、版本等）
- 保存为 PNG（保存到 SVGA 文件旁边）
- 复制文件到剪贴板
- 窗口拖拽（点击背景区域）
- 自动隐藏控制栏
- 适配 QuickLook 暗色/亮色主题
- 中英双语支持（自动检测系统语言）
- 使用 Segoe Fluent Icons 图标

### 安装

1. 从 [Releases](https://github.com/yancongya/QuickLook.Plugin.SvgaViewer/releases) 下载 `.qlplugin` 文件
2. 确保 QuickLook 正在运行
3. 选中 `.qlplugin` 文件，按空格键预览
4. 点击"安装"按钮
5. 重启 QuickLook

### 系统要求

- Windows 10/11
- [QuickLook](https://github.com/QL-Win/QuickLook) 4.x
- [WebView2 Runtime](https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/)（Windows 10/11 通常已预装）

### 从源码构建

```bash
git clone https://github.com/yancongya/QuickLook.Plugin.SvgaViewer.git
cd QuickLook.Plugin.SvgaViewer
dotnet build -c Release
```

打包为 `.qlplugin`：

```powershell
powershell -ExecutionPolicy Bypass -File Scripts/build.ps1
```

### 项目结构

```
├── Plugin.cs                      # IViewer 入口（Priority=10）
├── SvgaViewerPanel.xaml(.cs)      # WebView2 播放面板
├── Resources/Web/                 # Web 资源
│   ├── jszip.min.js               # JSZip（1.x ZIP 解压支持）
│   ├── svga.min.js                # SVGAPlayer-Web 2.3.1
│   └── player.html                # 播放器 UI
├── Samples/                       # 测试用 SVGA 示例文件
├── Translations.config            # 中英文翻译
├── Scripts/
└── *.csproj / *.sln
```

### 为什么需要这个插件？

> **2026-09-01 更新**：下面描述的 1.x 空白预览问题已经修好并合并进官方仓库（[PR #1994](https://github.com/QL-Win/QuickLook/pull/1994) —— `SvgaPlayer.Data.cs` 增加 ZIP 检测与 1.x JSON 解析，并引入 JSZip）。官方最新正式版 **4.5.0 还不包含**这个修复，所以今天要预览 1.x 文件，仍然需要本插件或自行构建的 QuickLook。

（合并前的情况）官方 QuickLook 的 ImageViewer 内置了 SVGA 支持，但使用 SVGAPlayer-Web-Lite（不支持 1.x 格式）。当预览 1.x 格式的 SVGA 文件时会出现空白预览或 `incorrect header check` 错误。

本插件：
- 使用 SVGAPlayer-Web 原版（支持 1.x + 2.x）
- 加载 JSZip 库处理 1.x ZIP 格式
- 设置 Priority=10 覆盖官方 ImageViewer

---

## English

### Introduction

A [QuickLook](https://github.com/QL-Win/QuickLook) plugin for previewing SVGA (SVGA Animation) files.

**Fixes the official ImageViewer's SVGA 1.x blank preview issue**, with full support for both 1.x (ZIP format) and 2.x (zlib format).

The fix was merged into the official QuickLook repository on 2026-09-01 via [PR #1994](https://github.com/QL-Win/QuickLook/pull/1994), but it is **not in a released build yet** (the latest release, 4.5.0, shipped on 2026-04-14). On top of it, this plugin also adds playback controls, background switching, a file info panel, PNG export and more.

🌐 Online preview (GitHub Pages): https://yancongya.github.io/QuickLook.Plugin.SvgaViewer/

### Features

- Real-time SVGA animation playback (auto loop)
- **Full support for SVGA 1.x and 2.x formats**
- Play/Pause, Progress bar with seek
- Volume control (button, slider, mouse wheel)
- Background switcher (Default / Checkerboard / White / Black / Custom color)
- File info panel (hover to show dimensions, FPS, sprites, version, etc.)
- Save as PNG (saves next to SVGA file)
- Copy file to clipboard
- Window drag (click background area)
- Auto-hide control bar
- QuickLook Dark/Light theme support
- Bilingual support (Chinese/English, auto-detect system language)
- Segoe Fluent Icons

### Installation

1. Download `.qlplugin` from [Releases](https://github.com/yancongya/QuickLook.Plugin.SvgaViewer/releases)
2. Ensure QuickLook is running
3. Select the `.qlplugin` file and press Space
4. Click "Install"
5. Restart QuickLook

### Requirements

- Windows 10/11
- [QuickLook](https://github.com/QL-Win/QuickLook) 4.x
- [WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) (usually pre-installed on Windows 10/11)

### Build from source

```bash
git clone https://github.com/yancongya/QuickLook.Plugin.SvgaViewer.git
cd QuickLook.Plugin.SvgaViewer
dotnet build -c Release
```

Package as `.qlplugin`:

```powershell
powershell -ExecutionPolicy Bypass -File Scripts/build.ps1
```

### Project Structure

```
├── Plugin.cs                      # IViewer entry point (Priority=10)
├── SvgaViewerPanel.xaml(.cs)      # WebView2 player panel
├── Resources/Web/                 # Web assets
│   ├── jszip.min.js               # JSZip (1.x ZIP decompression)
│   ├── svga.min.js                # SVGAPlayer-Web 2.3.1
│   └── player.html                # Player UI
├── Samples/                       # Sample SVGA files for testing
├── Translations.config            # i18n translations
├── Scripts/
└── *.csproj / *.sln
```

### Why this plugin?

> **Update 2026-09-01**: the 1.x blank-preview issue described below has been fixed and merged upstream ([PR #1994](https://github.com/QL-Win/QuickLook/pull/1994) — ZIP detection plus 1.x JSON parsing in `SvgaPlayer.Data.cs`, with JSZip added). The latest official release, **4.5.0, does not include it yet**, so today you still need this plugin or a self-built QuickLook to preview 1.x files.

(Before the merge) The official QuickLook's ImageViewer has built-in SVGA support, but uses SVGAPlayer-Web-Lite which doesn't support 1.x format. When previewing 1.x SVGA files, you get blank preview or `incorrect header check` error.

This plugin:
- Uses SVGAPlayer-Web original (supports 1.x + 2.x)
- Loads JSZip library for 1.x ZIP format
- Sets Priority=10 to override official ImageViewer

---

## Changelog

See [CHANGELOG.md](CHANGELOG.md)

## Credits

- [QuickLook](https://github.com/QL-Win/QuickLook) by QL-Win
- [SVGAPlayer-Web](https://github.com/yyued/SVGAPlayer-Web) by YYED (yyued)
- [JSZip](https://github.com/Stuk/jszip) by Stuart Knightley
- UI design references official QuickLook plugins (VideoViewer, ImageViewer)

## License

[GPL-3.0](LICENSE)
