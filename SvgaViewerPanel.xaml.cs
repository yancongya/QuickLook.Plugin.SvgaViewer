using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using QuickLook.Common.Plugin;

namespace QuickLook.Plugin.SvgaViewer
{
    public partial class SvgaViewerPanel : UserControl
    {
        private string _svgaFilePath;
        private readonly string _webAssetsDir;
        private WebView2 _webView;
        private Themes _theme = Themes.Dark;
        private Window _hostWindow;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public SvgaViewerPanel()
        {
            _webAssetsDir = Path.Combine(
                Path.GetDirectoryName(typeof(SvgaViewerPanel).Assembly.Location) ?? "",
                "Resources", "Web");

            if (!IsWebView2Available())
            {
                Content = CreateDownloadButton();
                return;
            }

            InitializeComponent();
            _webView = WebView;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public void LoadFile(string path)
        {
            _svgaFilePath = path;
        }

        public void SetTheme(Themes theme)
        {
            _theme = theme;
        }

        private static bool IsWebView2Available()
        {
            try
            {
                return !string.IsNullOrEmpty(
                    CoreWebView2Environment.GetAvailableBrowserVersionString());
            }
            catch
            {
                return false;
            }
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var htmlPath = Path.Combine(_webAssetsDir, "player.html");
                if (!File.Exists(htmlPath))
                {
                    ShowError("player.html not found.");
                    return;
                }

                var env = await CoreWebView2Environment.CreateAsync(
                    null,
                    Path.Combine(Path.GetTempPath(), "SvgaViewer_WebView2"));

                await _webView.EnsureCoreWebView2Async(env);

                _webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
                _webView.CoreWebView2.NavigationStarting += OnNavigationStarting;
                _webView.CoreWebView2.Settings.IsZoomControlEnabled = false;

                _webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "assets.local",
                    _webAssetsDir,
                    CoreWebView2HostResourceAccessKind.Allow);

                _webView.CoreWebView2.Navigate(htmlPath);

                _hostWindow = Window.GetWindow(this);
                if (_hostWindow != null)
                    _hostWindow.DpiChanged += OnDpiChanged;
            }
            catch (Exception ex)
            {
                ShowError($"WebView2 init failed: {ex.Message}");
            }
        }

        private void OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri.StartsWith("data:")) return;
            if (e.Uri.StartsWith("https://assets.local/")) return;
            if (e.Uri.StartsWith("file:")) return;
            e.Cancel = true;
        }

        private async void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var json = e.WebMessageAsJson;
                var msg = JsonSerializer.Deserialize<JsonElement>(json);

                if (!msg.TryGetProperty("type", out var typeProp)) return;
                var type = typeProp.GetString();

                if (type == "ready")
                {
                    await ApplyTheme();
                    await SendFileToWebView();
                }
                else if (type == "loaded")
                {
                    Dispatcher.Invoke(() => TxtStatus.Visibility = Visibility.Collapsed);
                }
                else if (type == "error")
                {
                    var message = msg.TryGetProperty("message", out var m) ? m.GetString() : "Unknown error";
                    Dispatcher.Invoke(() => ShowError($"SVGA Error: {message}"));
                }
                else if (type == "startDrag")
                {
                    Dispatcher.Invoke(() => StartWindowDrag());
                }
                else if (type == "copyImage")
                {
                    var base64 = msg.GetProperty("data").GetString();
                    Dispatcher.Invoke(() => CopyImageToClipboard(base64));
                }
                else if (type == "saveImage")
                {
                    var base64 = msg.GetProperty("data").GetString();
                    Dispatcher.Invoke(() => SaveImage(base64));
                }
            }
            catch { }
        }

        private void StartWindowDrag()
        {
            if (_hostWindow == null) return;
            var helper = new System.Windows.Interop.WindowInteropHelper(_hostWindow);
            ReleaseCapture();
            SendMessage(helper.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        private void CopyImageToClipboard(string base64)
        {
            try
            {
                var bytes = Convert.FromBase64String(base64);
                var pagDir = Path.GetDirectoryName(_svgaFilePath) ?? "";
                var baseName = Path.GetFileNameWithoutExtension(_svgaFilePath);
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var savePath = Path.Combine(pagDir, $"{baseName}_{timestamp}.png");

                File.WriteAllBytes(savePath, bytes);

                Dispatcher.Invoke(() =>
                {
                    var fileList = new System.Collections.Specialized.StringCollection { savePath };
                    Clipboard.SetFileDropList(fileList);
                });
            }
            catch { }
        }

        private void SaveImage(string base64)
        {
            try
            {
                var bytes = Convert.FromBase64String(base64);
                var pagDir = Path.GetDirectoryName(_svgaFilePath) ?? "";
                var baseName = Path.GetFileNameWithoutExtension(_svgaFilePath);
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var savePath = Path.Combine(pagDir, $"{baseName}_{timestamp}.png");

                File.WriteAllBytes(savePath, bytes);
            }
            catch { }
        }

        private async Task ApplyTheme()
        {
            string bg, fg, fgAlt;
            if (_theme == Themes.Light)
            {
                bg = "#ffffff"; fg = "#1a1a1a"; fgAlt = "#666666";
            }
            else
            {
                bg = "#202020"; fg = "#cccccc"; fgAlt = "#888888";
            }

            var translations = LoadTranslations();
            var isEn = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "zh";
            var lang = isEn ? "en" : "zh";

            var script = $@"
                document.documentElement.style.setProperty('--bg', '{bg}');
                document.documentElement.style.setProperty('--fg', '{fg}');
                document.documentElement.style.setProperty('--fg-alt', '{fgAlt}');
                window.__lang = '{lang}';
                window.__translations = {translations};
                if (typeof applyI18n === 'function') applyI18n();
            ";
            await _webView.CoreWebView2.ExecuteScriptAsync(script);
        }

        private string LoadTranslations()
        {
            try
            {
                var exeDir = Path.GetDirectoryName(typeof(SvgaViewerPanel).Assembly.Location) ?? "";
                var configPath = Path.Combine(exeDir, "Translations.config");
                if (!File.Exists(configPath)) return "{}";

                var doc = new System.Xml.XmlDocument();
                doc.Load(configPath);
                var dict = new System.Collections.Generic.Dictionary<string, object>();
                var nodes = doc.SelectNodes("//Entry");
                if (nodes == null) return "{}";
                foreach (System.Xml.XmlNode entry in nodes)
                {
                    var key = entry.Attributes?["Key"]?.Value;
                    if (key == null) continue;
                    dict[key] = new
                    {
                        zh = entry.Attributes["Value"]?.Value ?? "",
                        en = entry.Attributes["ValueEn"]?.Value ?? ""
                    };
                }
                return JsonSerializer.Serialize(dict);
            }
            catch { return "{}"; }
        }

        private async Task SendFileToWebView()
        {
            if (string.IsNullOrEmpty(_svgaFilePath) || !File.Exists(_svgaFilePath))
            {
                ShowError("SVGA file not found.");
                return;
            }

            try
            {
                var bytes = File.ReadAllBytes(_svgaFilePath);
                var base64 = Convert.ToBase64String(bytes);

                var msg = new
                {
                    type = "loadFile",
                    jszipSrc = "https://assets.local/jszip.min.js",
                    svgaSrc = "https://assets.local/svga.min.js",
                    base64 = base64
                };

                var jsonMsg = JsonSerializer.Serialize(msg);
                await _webView.CoreWebView2.ExecuteScriptAsync(
                    "window.postMessage(" + jsonMsg + ", '*');");
            }
            catch (Exception ex)
            {
                ShowError($"Failed to load file: {ex.Message}");
            }
        }

        private void OnDpiChanged(object sender, DpiChangedEventArgs e)
        {
            if (_webView?.CoreWebView2 != null)
            {
                _webView.InvalidateVisual();
                _webView.UpdateLayout();
            }
        }

        private void ShowError(string message)
        {
            TxtStatus.Text = message;
            TxtStatus.Foreground = new SolidColorBrush(Colors.OrangeRed);
            TxtStatus.Visibility = Visibility.Visible;
        }

        private static Button CreateDownloadButton()
        {
            var button = new Button
            {
                Content = "WebView2 Runtime is required. Click to download.",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(20, 6, 20, 6)
            };
            button.Click += (s, e) =>
                Process.Start("https://go.microsoft.com/fwlink/p/?LinkId=2124703");
            return button;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_hostWindow != null)
                _hostWindow.DpiChanged -= OnDpiChanged;

            Dispose();
        }

        public void Dispose()
        {
            if (_webView != null)
            {
                if (_webView.CoreWebView2 != null)
                {
                    _webView.CoreWebView2.WebMessageReceived -= OnWebMessageReceived;
                    _webView.CoreWebView2.NavigationStarting -= OnNavigationStarting;
                }
                _webView.Dispose();
                _webView = null;
            }
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
        }
    }
}
