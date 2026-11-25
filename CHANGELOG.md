# Changelog

## [Unreleased]

## [1.1.1] - 2026-11-25

### Changed
- Unified monochrome spinning Loading indicator and localized initial loading text in `SvgaViewerPanel.xaml(.cs)`

## [1.1.0] - 2026-11-25

### Added
- Frame-by-frame stepping controls (`stepToFrame`, previous/next buttons, Left/Right arrow keys, Space to play/pause)
- Frame counter in playback bar (`currentFrame/totalFrames`)
- Unified bottom-right transparent Info & Performance panel via Info button or `L` key (duration, dimensions, file size, resource memory, sprites/images/audios count, real-time FPS & frame-time waveform with target budget line, bar hover readout, status dot, edge resize, double-click reset, and summary copy)
- Independent single-panel File Structure table via Structure button or `F` key (sortable headers, live key filter, semantic type badges, left-edge resize, tag footer, double-click inline `setImage` / `setText`, and `clearDynamicObjects` reset)
- Playback & Debug options menu (`#btnOpts`): frame range playback (`startAnimationWithRange`), reverse playback, `fillMode` (Forward/Backward), and `setContentMode` (AspectFit/AspectFill/Fill)
- Zoom percentage indicator (`#zoomInfo`) and `onFinished` non-loop playback state sync
- Native monochrome gray/white styling and full bilingual i18n across all menus and panels

### Fixed
- Merged duplicate `wheel` event listeners for Ctrl+Scroll zoom and volume control
- Fixed `setContentMode` canvas container sizing and control bar `z-index` layering

## [1.0.0] - 2026-08-25

### Added
- Initial release
- SVGA 1.x and 2.x format support
- SVGAPlayer-Web 2.3.1 + JSZip 3.1.4
- Play/Pause, Progress bar, Loop toggle
- Volume control (button, slider, mouse wheel)
- Background switcher (default/checkerboard/white/black/custom color)
- File info panel (dimensions, frames, FPS, sprites, version)
- Save as PNG, Copy file to clipboard
- Window drag, Auto-hide control bar
- QuickLook Dark/Light theme support
- Bilingual support (Chinese/English)
- Priority=10 to override official ImageViewer
- GitHub Actions CI/CD workflow
