using System;
using System.IO;
using System.Windows;
using QuickLook.Common.Plugin;

namespace QuickLook.Plugin.SvgaViewer
{
    public class Plugin : IViewer
    {
        private static double _lastWidth = 600;
        private static double _lastHeight = 440;

        private SvgaViewerPanel _viewerPanel;

        public int Priority => 10;

        public void Init()
        {
        }

        public bool CanHandle(string path)
        {
            if (Directory.Exists(path)) return false;
            if (!path.EndsWith(".svga", StringComparison.OrdinalIgnoreCase)) return false;

            try
            {
                using (var fs = File.OpenRead(path))
                {
                    if (fs.Length < 4) return false;
                    var header = new byte[4];
                    fs.Read(header, 0, 4);
                    // Only handle 1.x (ZIP format: PK\x03\x04)
                    // 2.x files will be handled by official ImageViewer
                    if (header[0] == 0x50 && header[1] == 0x4B &&
                        header[2] == 0x03 && header[3] == 0x04)
                        return true;
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public void Prepare(string path, ContextObject context)
        {
            context.SetPreferredSizeFit(new Size(_lastWidth, _lastHeight), 0.9);
        }

        public void View(string path, ContextObject context)
        {
            _viewerPanel = new SvgaViewerPanel();
            _viewerPanel.SetTheme(context.Theme);
            _viewerPanel.LoadFile(path);

            context.ViewerContent = _viewerPanel;
            context.Title = $"{Path.GetFileName(path)}";

            _viewerPanel.Dispatcher.Invoke(() => { context.IsBusy = false; },
                System.Windows.Threading.DispatcherPriority.Loaded);
        }

        public void Cleanup()
        {
            if (_viewerPanel != null)
            {
                _lastWidth = _viewerPanel.ActualWidth > 0 ? _viewerPanel.ActualWidth : _lastWidth;
                _lastHeight = _viewerPanel.ActualHeight > 0 ? _viewerPanel.ActualHeight : _lastHeight;
            }

            _viewerPanel?.Dispose();
            _viewerPanel = null;
            GC.SuppressFinalize(this);
        }
    }
}
