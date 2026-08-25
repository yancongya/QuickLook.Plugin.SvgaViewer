# AGENTS.md

## Build Commands

```bash
# Build
dotnet build -c Release

# Package
powershell -ExecutionPolicy Bypass -File Scripts/pack-zip.ps1
```

## Project Overview

QuickLook plugin for previewing PAG (Portable Animated Graphics) files. Uses WebView2 + libpag WebAssembly for rendering.

## Key Files

- `Plugin.cs` - IViewer entry point, handles .pag file detection
- `PagViewerPanel.xaml.cs` - WebView2 panel, manages lifecycle and theme
- `Resources/Web/pag-player.html` - Player UI (HTML/CSS/JS)
- `Resources/Web/libpag.min.js` + `libpag.wasm` - PAG Web SDK

## Architecture

```
Plugin.cs (IViewer)
  └── PagViewerPanel (WPF UserControl)
        └── WebView2
              └── pag-player.html
                    └── libpag.min.js + libpag.wasm (WebAssembly)
```

## Conventions

- Target framework: .NET Framework 4.6.2
- Namespace: `QuickLook.Plugin.PagViewer`
- Follow official QuickLook plugin structure (see VideoViewer, ImageViewer)
- XAML files at project root (not in subdirectories)
- Web assets in `Resources/Web/`
- Native P/Invoke stubs in `Native/`
